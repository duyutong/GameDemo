using System;
using System.Collections;
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
        wnd.maxSize = new Vector2(700, 200.1f);
        self = wnd;
        return wnd;
    }

    public void CreateGUI()
    {
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
        csharpPath = CSharpPathTextField.value;
        prefabPath = PrefabTextField.value;
        btPath = RuntimeTextField.value;
        isPrefab = PrefabToggle.value;
        isRuntime = RuntimToggle.value;
        uiName = Path.GetFileNameWithoutExtension(csharpPath);

        CreateBTAndPrefabFirst();
    }

    #region 第一步：创建行为树和预制体（无脚本）

    private void CreateBTAndPrefabFirst()
    {
        if (isRuntime) CreateBTAsset();
        if (isPrefab) CreatePrefabWithoutScript();
        CreateScriptAndWaitForCompile();
    }

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
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        AssetDatabase.CreateAsset(container, btPath);
        AssetDatabase.SaveAssets();
    }

    private void CreatePrefabWithoutScript()
    {
        string uiRootPath = "Assets/AddressableAssets/Prefabs/UI/UIRoot/UIRoot.prefab";

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
        Transform parent = uiRootInstance.transform.Find("Canvas/MainCanvas");
        panel.transform.SetParent(parent, false);
        panel.transform.Reset();

        RectTransform panelRt = panel.AddComponent<RectTransform>();
        SetFullStretch(panelRt);

        GameObject main = new GameObject("main");
        main.transform.SetParent(panel.transform, false);
        RectTransform mainRt = main.AddComponent<RectTransform>();
        SetFullStretch(mainRt);

        if (isRuntime && File.Exists(btPath))
        {
            var container = AssetDatabase.LoadAssetAtPath<BTContainer>(btPath);
            var btRuntime = panel.AddComponent<BTRuntimeComponent>();
            btRuntime.container = container;
        }

        string dir = Path.GetDirectoryName(prefabPath);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        PrefabUtility.SaveAsPrefabAsset(panel, prefabPath);
        GameObject.DestroyImmediate(uiRootInstance);
    }

    #endregion

    #region 第二步：生成脚本并等待编译挂载

    private void CreateScriptAndWaitForCompile()
    {
        if (File.Exists(csharpPath))
        {
            EditorUtility.DisplayDialog("提示", "CSharp文件已经存在！", "确定");
            AttachScriptToPrefab(); // 直接挂载
            return;
        }

        string scriptContent = ScriptTemplate
            .Replace("#SCRIPTNAME#", uiName)
            .Replace("#BTRuntime#", isRuntime ? BTRuntimeTemplate : string.Empty);

        // 确保目录存在
        string dir = Path.GetDirectoryName(csharpPath);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        File.WriteAllText(csharpPath, scriptContent);
        AssetDatabase.Refresh();
    }
    private void OnDisable()
    {
        TryAttachScriptWithRetry();
    }
    private void TryAttachScriptWithRetry()
    {
        EditorUtilityExtensions.CheckRes(PrefabPath, ".prefab", (_path) => 
        {
            string fileName = Path.GetFileName(_path);
            if (fileName.StartsWith("UIWindow_")) 
            {
                GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(_path.ToShortPath());
                if (prefabAsset == null) return;

                // 实例化预制体进行编辑
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefabAsset);
                if (instance == null) return;

                // 查找根节点：预制体根节点可能是实例本身，也可能是名为 uiName 的子物体
                string uiName = instance.name;
                Transform panelTransform = instance.transform;
                GameObject panel = instance;

                // 获取脚本类型
                Type scriptType = GetTypeByName(uiName);
                if (scriptType == null)
                {
                    GameObject.DestroyImmediate(instance);
                    return;
                }

                var btRuntime = panel.GetComponent<BTRuntimeComponent>();
                var existing = panel.GetComponent(scriptType);
                if (existing != null)
                {
                    if (btRuntime != null) existing.SetMemberValue("bTRuntimeComp", btRuntime);
                }
                else 
                {
                    // 添加新组件
                    var component = panel.AddComponent(scriptType);
                    if (btRuntime != null) component.SetMemberValue("bTRuntimeComp", btRuntime);
                }

                // 保存预制体并覆盖原文件
                PrefabUtility.SaveAsPrefabAssetAndConnect(instance, _path.ToShortPath(), InteractionMode.UserAction);
                GameObject.DestroyImmediate(instance);

                AssetDatabase.Refresh();
            }
        });
    }

    private void AttachScriptToPrefab()
    {
        if (!isPrefab) return;

        // 重新加载预制体资产
        GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefabAsset == null)
        {
            Debug.LogError($"预制体不存在：{prefabPath}");
            return;
        }

        // 实例化预制体进行编辑
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefabAsset);
        if (instance == null)
        {
            Debug.LogError($"实例化预制体失败：{prefabPath}");
            return;
        }

        // 查找根节点：预制体根节点可能是实例本身，也可能是名为 uiName 的子物体
        Transform panelTransform = instance.transform.Find(uiName);
        if (panelTransform == null)
        {
            // 如果找不到，尝试将实例本身作为根节点
            if (instance.name == uiName)
                panelTransform = instance.transform;
            else
            {
                Debug.LogError($"找不到名为 {uiName} 的根节点，预制体根节点名称：{instance.name}");
                GameObject.DestroyImmediate(instance);
                return;
            }
        }
        GameObject panel = panelTransform.gameObject;

        // 获取脚本类型
        Type scriptType = GetTypeByName(uiName);
        if (scriptType == null)
        {
            Debug.LogError($"无法获取脚本类型 {uiName}");
            GameObject.DestroyImmediate(instance);
            return;
        }

        // 移除可能已存在的旧组件（避免重复）
        var existing = panel.GetComponent(scriptType);
        if (existing != null) DestroyImmediate(existing);

        // 添加新组件
        var component = panel.AddComponent(scriptType);

        // 关联行为树组件
        if (isRuntime)
        {
            var btRuntime = panel.GetComponent<BTRuntimeComponent>();
            if (btRuntime != null)
            {
                var field = scriptType.GetField("bTRuntimeComp",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (field != null)
                    field.SetValue(component, btRuntime);
                else
                    Debug.LogWarning($"脚本 {uiName} 中没有 bTRuntimeComp 字段");
            }
        }

        // 保存预制体并覆盖原文件
        PrefabUtility.SaveAsPrefabAssetAndConnect(instance, prefabPath, InteractionMode.UserAction);
        GameObject.DestroyImmediate(instance);

        AssetDatabase.Refresh();
        Debug.Log($"成功将脚本 {uiName} 挂载到预制体 {prefabPath}");
    }

    #endregion

    #region 辅助方法

    private static void SetFullStretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private static Type GetTypeByName(string name)
    {
        // 优先使用 TypeCache 更快
        var types = TypeCache.GetTypesDerivedFrom<MonoBehaviour>();
        foreach (var t in types)
            if (t.Name == name) return t;

        // 回退到 AppDomain 查找
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => { try { return a.GetTypes(); } catch { return Array.Empty<Type>(); } })
            .FirstOrDefault(t => t.Name == name);
    }

    private void OnCSharpTextFieldChanged(ChangeEvent<string> evt)
    {
        string uiWinName = $"UIWindow_{evt.newValue}";
        string btName = uiWinName + ".asset";

        CSharpPathTextField.value = $"{CSharpPath}{uiWinName}/{uiWinName}.cs";
        PrefabTextField.value = $"{PrefabPath}{uiWinName}/{uiWinName}.prefab";
        RuntimeTextField.value = $"{BTPath}{uiWinName}/{btName}";
    }

    #endregion

    #region 模板常量

    private const string BTRuntimeTemplate = "public BTRuntimeComponent bTRuntimeComp;";
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
}";

    #endregion
}