using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using BTBaseNode = BehaviorTreeBaseNode;

public class CustomNodeEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset visualTreeAsset = default;

    //NodeView
    private CustomNodeView nodeView;

    //ToolBar
    private TextField nameTextField;
    private Button searchBtn;
    private DropdownField nodeTypeField;
    private Button createBtn;
    private List<string> nodeTypeList = new List<string>();

    //PortSetting
    private VisualElement settingView;
    private ScrollView scrollView;
    private Button addPortBtn;
    private Button savePortBtn;
    private DefaultNode currNode;

    [MenuItem("BehaviourTreeEditor/Open NodeEditor")]
    public static void OpenWindow()
    {
        CustomNodeEditor wnd = GetWindow<CustomNodeEditor>();
        wnd.titleContent = new GUIContent("CustomNodeEditor");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/BehaviorTree/Editor/UIBuilder/CustomNodeEditor.uxml");
        visualTreeAsset.CloneTree(root);

        InitNodeTypeList();

        nodeView = root.Q<CustomNodeView>();
        nodeView.onSelectAction = OnSelectAction;
        nodeView.onUnselectAction = OnUnselectAction;

        nameTextField = root.Q<TextField>("nameTextField");
        nameTextField.RegisterValueChangedCallback((_value) =>
        {
            if (currNode != null)
            {
                currNode.title = _value.newValue;
                currNode.RefreshExpandedState();
            }
        });

        nodeTypeField = root.Q<DropdownField>("nodeTypeField");
        nodeTypeField.choices = nodeTypeList;
        nodeTypeField.value = nodeTypeList[0];
        nodeTypeField.RegisterValueChangedCallback((_value) =>
        {
            if (currNode != null)
            {
                currNode.nodeType = _value.newValue;
                currNode.RefreshExpandedState();
            }
        });

        searchBtn = root.Q<Button>("searchBtn");
        searchBtn.RegisterCallback<PointerUpEvent>(OnClickSearchBtn);

        createBtn = root.Q<Button>("createBtn");
        createBtn.clicked += OnClickCreateBtn;

        addPortBtn = root.Q<Button>("add");
        addPortBtn.clicked += OnClickAddPortBtn;

        savePortBtn = root.Q<Button>("save");
        savePortBtn.clicked += OnClickSaveBtn;

        scrollView = root.Q<ScrollView>("scrollView");
        settingView = root.Q<VisualElement>("settingView");
        settingView.style.display = DisplayStyle.None;
    }
    private void InitNodeTypeList()
    {
        nodeTypeList.Clear();
        string path = Application.dataPath.Replace("\\", "/") + "/BehaviorTree/Editor/Node/Base";
        DirectoryInfo directory = Directory.CreateDirectory(path);
        FileInfo[] fileInfos = directory.GetFiles();
        foreach (FileInfo info in fileInfos)
        {
            if (info.Extension.ToLower() != ".cs") continue;
            nodeTypeList.Add(info.Name.Split(".")[0]);
        }
    }
    private void OnClickSaveBtn()
    {
        GraphSaveUtility.GenNodeToCSharp(currNode);
        GraphSaveUtility.GenStateToCSharp(currNode);
    }
    private void OnClickSearchBtn(PointerUpEvent evt) 
    {
        SearchMenuWindowProvider menu = ScriptableObject.CreateInstance<SearchMenuWindowProvider>();
        menu.OnCreateSearchTreeAction = () =>
        {
            List<SearchTreeEntry> entries = new List<SearchTreeEntry>();
            entries.Add(new SearchTreeGroupEntry(new GUIContent("创建新节点")));

            entries.Add(new SearchTreeGroupEntry(new GUIContent("装饰器")) { level = 1 });
            List<SearchTreeEntry> decorators = GetEntries<DecoratorNode>(2);
            entries.AddRange(decorators);

            entries.Add(new SearchTreeGroupEntry(new GUIContent("触发器")) { level = 1 });
            List<SearchTreeEntry> triggers = GetEntries<TriggerNode>(2);
            entries.AddRange(triggers);

            entries.Add(new SearchTreeGroupEntry(new GUIContent("行为节点")) { level = 1 });
            List<SearchTreeEntry> states = GetEntries<BehaviorNode>(2);
            entries.AddRange(states);

            return entries;
        };
        menu.onSelectEntryHandler += (entry, context) =>
        {
            Type type = Type.GetType(entry.name);
            nameTextField.value = entry.name.Split("_")[1];
            return CreatNode(type);
        };

        evt.StopPropagation();
        var mousePos = evt.position;
        mousePos = GUIUtility.GUIToScreenPoint(mousePos);
        SearchWindow.Open(new SearchWindowContext(mousePos), menu);
    }
    private bool CreatNode(Type type, Vector2 pos = default)
    {
        Type nodeType = Type.GetType(type.FullName);
        BTBaseNode node = (BTBaseNode)Activator.CreateInstance(nodeType);
        string nodeName = nameTextField.text;
        string nodeTypeName = nodeType.BaseType.Name;
        nodeTypeField.value = nodeTypeName;
        currNode = nodeView.CreatNode(nodeName, nodeTypeName);

        var ports = node.inputContainer.Query<Port>().ToList();
        ports.AddRange(node.outputContainer.Query<Port>().ToList());
        
        List<BTNodePortSetting> settings = new();
        foreach (var port in ports) 
        {
            BTNodePortSetting setting = new BTNodePortSetting();
            setting.node = currNode;
            setting.portName = port.portName;
            setting.direction = port.direction;
            setting.capacity = port.capacity;
            setting.portType = setting.GetEPortTypeByType(port.portType);

            settings.Add(setting);
        }
        currNode.AddPortForNode(settings);

        return true;
    }

    public List<SearchTreeEntry> GetEntries<T>(int level)
    {
        List<SearchTreeEntry> entries = new List<SearchTreeEntry>();
        var nodes = GetClassList(typeof(T));
        foreach (var _node in nodes)
        {
            if (!_node.IsSubclassOf(typeof(T))) continue;
            Type type = Type.GetType(_node.FullName);
            entries.Add(new SearchTreeEntry(new GUIContent("  " + _node.FullName)) { level = level, userData = type });
        }
        return entries;
    }
    /// <summary>
    /// 获取指定类型的类列表
    /// </summary>
    /// <param name="type">基础类型</param>
    /// <returns>类列表</returns>
    private List<Type> GetClassList(Type type)
    {
        var q = type.Assembly.GetTypes()
             .Where(x => !x.IsAbstract)
             .Where(x => !x.IsGenericTypeDefinition)
             .Where(x => type.IsAssignableFrom(x));
        return q.ToList();
    }
    private void OnClickAddPortBtn()
    {
        ProtSettingView settingView = new ProtSettingView();
        BTNodePortSetting portSetting = new BTNodePortSetting();
        portSetting.node = currNode;
        settingView.ShowProtSetting(portSetting, true);
        settingView.onDelPort = OnDeletePort;
        scrollView.Add(settingView);
    }

    private void OnClickCreateBtn()
    {
        string nodeType = nodeTypeField.value;
        string nodeName = nameTextField.text;
        if (string.IsNullOrEmpty(nodeName)) return;
        nodeView.CreatNode(nodeName, nodeType);
    }
    private void OnSelectAction(BehaviorTreeBaseNode _node)
    {
        DefaultNode node = _node as DefaultNode;
        currNode = node;
        if (node == null) return;

        nodeTypeField.value = node.nodeType;
        nameTextField.value = _node.title;

        scrollView.contentContainer.Clear();
        //遍历node的所有接口
        List<Port> ports = node.inputContainer.Query<Port>().ToList();
        ports.AddRange(node.outputContainer.Query<Port>().ToList());

        foreach (Port port in ports)
        {
            ProtSettingView settingView = new ProtSettingView();
            BTNodePortSetting portSetting = new BTNodePortSetting();
            portSetting.node = _node;
            portSetting.portName = port.portName;
            portSetting.direction = port.direction;
            portSetting.capacity = port.capacity;
            portSetting.portType = portSetting.GetEPortTypeByType(port.portType);

            settingView.ShowProtSetting(portSetting);
            settingView.onDelPort = OnDeletePort;
            scrollView.Add(settingView);
        }

        settingView.style.display = DisplayStyle.Flex;
    }
    private void OnDeletePort(ProtSettingView view)
    {
        scrollView.contentContainer.Remove(view);
    }
    private void OnUnselectAction()
    {
        currNode = null;
        settingView.style.display = DisplayStyle.None;
    }
}