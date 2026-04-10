using System;
using System.IO;
using UnityEditor;
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
        string csharpPath = CSharpPathTextField.value;
        string prefabPath = PrefabTextField.value;
        string btPath = RuntimeTextField.value;

        bool isPrefab = PrefabToggle.value;
        bool isRuntime = RuntimToggle.value;

        bool isExists = File.Exists(csharpPath);
        if (isExists) { EditorUtility.DisplayDialog("提示","文件已经存在！","确定"); return; }

        string uiWinName = Path.GetFileNameWithoutExtension(csharpPath);
        string scriptContent = ScriptTemplate.Replace("#SCRIPTNAME#", uiWinName).Replace("#BTRuntime#", isRuntime ? BTRuntimeTemplate : string.Empty);
        EditorUtilityExtensions.SaveCSFile(csharpPath, scriptContent);
    }

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
