using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using UnityEngine.UIElements;

public class UIStarter : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset visualTreeAsset = default;
    public static UIStarter self;

    private TextField CSharpTextField;
    private TextField CSharpPathTextField;

    private Toggle PrefabToggle;
    private TextField PrefabTextField;

    private Toggle RuntimToggle;
    private TextField RuntimeTextField;

    private Button BtnConfirm;

    private const string CSharpPath = "Assets/Scripts/UIWindowComponent/";
    private const string PrefabPath = "Assets/AddressableAssets/Prefabs/UI/";
    private const string BTPath = "Assets/BehaviorTree/BT/";

    private enum FlowState
    {
        None,
        WaitingCompile,
        WaitingDomainReload,
        Ready
    }

    private FlowState state = FlowState.None;
    private string csharpPath;
    private string prefabPath;
    private string btPath;
    private string uiName;
    private bool isPrefab;
    private bool isRuntime;

    [MenuItem("Tools/UI/UIStarter")]
    [MenuItem("Assets/UI/UIStarter")]
    public static UIStarter ShowExample()
    {
        UIStarter wnd = GetWindow<UIStarter>("UIStarter");
        wnd.minSize = new Vector2(512, 200);
        wnd.maxSize = new Vector2(700, (float)200.1);
        self = wnd;
        return wnd;
    }
    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UIStarter/Editor/UIBuilder/UIStarter.uxml");
        visualTreeAsset.CloneTree(root);

        CSharpTextField = root.Q<TextField>("CSharpTextField");
        CSharpTextField.RegisterCallback<ChangeEvent<string>>(OnCSharpTextFieldChanged);

        CSharpPathTextField = root.Q<TextField>("CSharpPathTextField");

        PrefabToggle = root.Q<Toggle>("PrefabToggle");
        PrefabToggle.RegisterCallback<ChangeEvent<bool>>((evt) => { PrefabTextField.SetEnabled(evt.newValue); });

        PrefabTextField = root.Q<TextField>("PrefabTextField");

        RuntimToggle = root.Q<Toggle>("RuntimToggle");
        RuntimToggle.RegisterCallback<ChangeEvent<bool>>((evt) => { RuntimeTextField.SetEnabled(evt.newValue); });
        RuntimeTextField = root.Q<TextField>("RuntimeTextField");

        BtnConfirm = root.Q<Button>("BtnConfirm");
        BtnConfirm.clicked += OnBtnConfirmClick;
    }

    private void OnBtnConfirmClick()
    {
        CreateCSharp();
    }
    #region Step1 Script

    private void CreateCSharp()
    {
        csharpPath = CSharpPathTextField.value;
        prefabPath = PrefabTextField.value;
        btPath = RuntimeTextField.value;

        isPrefab = PrefabToggle.value;
        isRuntime = RuntimToggle.value;

        uiName = Path.GetFileNameWithoutExtension(csharpPath);

        if (File.Exists(csharpPath))
        {
            EditorUtility.DisplayDialog("提示", "CSharp文件已经存在！", "确定");
            EnterNextStep();
            return;
        }

        string scriptContent =
            ScriptTemplate.Replace("#SCRIPTNAME#", uiName)
                          .Replace("#BTRuntime#", isRuntime ? BTRuntimeTemplate : string.Empty);

        EditorUtilityExtensions.SaveCSFile(csharpPath, scriptContent);

        AssetDatabase.Refresh();

        state = FlowState.WaitingCompile;

        EditorApplication.update -= Tick;
        EditorApplication.update += Tick;
    }

    #endregion

    #region Flow Tick

    private void Tick()
    {
        if (state == FlowState.WaitingCompile)
        {
            if (!EditorApplication.isCompiling) state = FlowState.WaitingDomainReload;
        }
        if (state == FlowState.WaitingDomainReload)
        {
            // 等 Unity 完全 reload 完程序集（关键修复点）
            if (TypeCache.GetTypesDerivedFrom<MonoBehaviour>().Count > 0) state = FlowState.Ready;
            return;
        }

        if (state == FlowState.Ready)
        {
            state = FlowState.None;
            EditorApplication.update -= Tick;

            CreateBTAsset();
            CreatePrefabAndBindScript();
        }
    }

    #endregion

    #region BT Asset

    private void CreateBTAsset()
    {
        if (!isRuntime) return;

        if (File.Exists(btPath))
        {
            EditorUtility.DisplayDialog("提示", "行为树文件已经存在！", "确定");
            return;
        }

        var container = ScriptableObject.CreateInstance<BTContainer>();
        container.edgeDatas.Clear();
        container.nodeDatas.Clear();

        string dir = Path.GetDirectoryName(btPath);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        AssetDatabase.CreateAsset(container, btPath);
        AssetDatabase.SaveAssets();
    }

    #endregion

    #region Prefab

    private void CreatePrefabAndBindScript()
    {
        if (!isPrefab) return;

        string uiRootPath = "Assets/AddressableAssets/Prefabs/UI/UIRoot/UIRoot.prefab";

        var type = GetTypeByName(uiName);

        if (type == null)
        {
            Debug.LogError("找不到类型: " + uiName);
            return;
        }

        if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
        {
            EditorUtility.DisplayDialog("提示", "预制体文件已经存在！", "确定");
            return;
        }

        GameObject uiRootPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(uiRootPath);
        if (uiRootPrefab == null)
        {
            Debug.LogError("UIRoot路径错误");
            return;
        }

        GameObject uiRootInstance = (GameObject)PrefabUtility.InstantiatePrefab(uiRootPrefab);

        GameObject panel = new GameObject(uiName);
        panel.transform.SetParent(uiRootInstance.transform, false);

        RectTransform panelRt = panel.AddComponent<RectTransform>();
        SetFullStretch(panelRt);

        GameObject main = new GameObject("main");
        main.transform.SetParent(panel.transform, false);

        RectTransform mainRt = main.AddComponent<RectTransform>();
        SetFullStretch(mainRt);

        var component = panel.AddComponent(type);

        if (isRuntime && File.Exists(btPath))
        {
            var container = AssetDatabase.LoadAssetAtPath<BTContainer>(btPath);

            var btRuntime = panel.AddComponent<BTRuntimeComponent>();
            btRuntime.container = container;

            component.SetMemberValue("bTRuntimeComp", btRuntime);
        }

        string dir = Path.GetDirectoryName(prefabPath);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        PrefabUtility.SaveAsPrefabAsset(panel, prefabPath);

        GameObject.DestroyImmediate(uiRootInstance);
    }

    #endregion

    #region Utils

    private static void SetFullStretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private static Type GetTypeByName(string name)
    {
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a =>
            {
                try { return a.GetTypes(); }
                catch { return Array.Empty<Type>(); }
            })
            .FirstOrDefault(t => t.Name == name);
    }

    private void EnterNextStep()
    {
        CreateBTAsset();
        CreatePrefabAndBindScript();
    }

    #endregion
    private void OnCSharpTextFieldChanged(ChangeEvent<string> evt)
    {
        string uiWinName = $"UIWindow_{evt.newValue}";
        string btName = uiWinName + ".asset";

        string csharpPath = $"{CSharpPath}{uiWinName}/{uiWinName}.cs";
        string prefabPath = $"{PrefabPath}{uiWinName}/{uiWinName}.prefab";
        string btPath = $"{BTPath}{uiWinName}/{btName}";

        CSharpPathTextField.value = csharpPath;
        PrefabTextField.value = prefabPath;
        RuntimeTextField.value = btPath;
    }
    #region ScriptTemplate
    private const string BTRuntimeTemplate = @"public BTRuntimeComponent bTRuntimeComp;";
    private const string ScriptTemplate =
@"using System;
using UnityEngine;

public class #SCRIPTNAME# : UIWindowComponentBase
{
    #BTRuntime#
    protected override void OnOpen()
    {
        base.OnOpen();
    }
}
";
    #endregion
}
