using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;
using UnityEditor.Experimental.GraphView;
using System.IO;
using System.Reflection;
using Network.API;

public class NetworkCodeGenerator : EditorWindow
{
    [SerializeField]
    public VisualTreeAsset visualTreeAsset = default;
    public static NetworkCodeGenerator self;

    private VisualTreeAsset httpEventTemplate;
    private VisualTreeAsset webSocketEventTemplate;
    private VisualTreeAsset currEventTemplateitem;
    
    private ScrollView eventScrollView;
    private ProtocolEditor modelEditor;
    private DropdownField connectionType;
    private TextField protocolName;

    private Button openClientScriptButton;
    private Button openServerScriptButton;
    private Button protoSearchButton;
    private Button addEventButton;
    private Button removeButton;
    private Button searchButton;
    private Button applayButton;
    private Button clearButton;

    private List<EventVisualElement> tempEventDatas = new List<EventVisualElement>();
    private Dictionary<string, List<NetworkProtocolBlockData>> tempModelDataDic = new Dictionary<string, List<NetworkProtocolBlockData>>();

    public static NetworkCodeGenerator OpenWindow()
    {
        NetworkCodeGenerator wnd = CreateWindow<NetworkCodeGenerator>("NetworkCodeGenerator");
        wnd.maxSize = new Vector2(1200, 420);
        wnd.minSize = new Vector2(1200, 360);
        self = wnd;
        return wnd;
    }
    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Network/Editor/UIBuilder/NetworkCodeGenerator/NetworkCodeGenerator.uxml");
        visualTreeAsset.CloneTree(root);

        connectionType = root.Q<DropdownField>("ConnectionType");
        connectionType.RegisterValueChangedCallback(OnConnectionTypeValueChanged);
        connectionType.choices = new List<string>() { "Http", "WebSocket" };
        connectionType.SetValueWithoutNotify("Http");

        protocolName = root.Q<TextField>("ProtocolName");
        protocolName.RegisterCallback<FocusOutEvent>(OnProtocolNameValueChanged);

        openClientScriptButton =root.Q<Button>("OpenClientScript");
        openClientScriptButton.clicked += OnOpenClientScriptButtonClick;

        openServerScriptButton = root.Q<Button>("OpenServerScript");
        openServerScriptButton.clicked += OnOpenServerScriptButtonClick;

        Button saveButton = root.Q<Button>("saveButton");
        saveButton.clicked += OnSaveButtonClick;

        //------Eventiew------
        httpEventTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Network/Editor/UIBuilder/HttpEventVisualElement/HttpEventVisualElement.uxml");
        webSocketEventTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Network/Editor/UIBuilder/WebSocketEventVisualElement/WebSocketEventVisualElement.uxml");
        currEventTemplateitem = httpEventTemplate;

        eventScrollView = root.Q<ScrollView>("EventScrollView");

        addEventButton = root.Q<Button>("AddEventButton");
        addEventButton.clicked += OnAddEventButtonClick;

        protoSearchButton = root.Q<Button>("ProtoSearchButton");
        protoSearchButton.RegisterCallback<PointerUpEvent>(OnProtoSearchButtonClick);

        //------Modelview------
        VisualElement modelVisualElement = root.Q<VisualElement>("ModelEditor");
        modelEditor = modelVisualElement.Q<ProtocolEditor>();
        if (modelEditor != null)
        {
            modelEditor.InitUI();
            modelEditor.SetTitle("Model");
        }

        applayButton = root.Q<Button>("ModelApplyButton");
        applayButton.clicked += OnApplyButtonClick;

        clearButton = root.Q<Button>("ModelClearButton");
        clearButton.clicked += OnClearButtonClick;

        removeButton = root.Q<Button>("ModelRemoveButton");
        removeButton.clicked += OnRemoveButtonClick;

        searchButton = root.Q<Button>("SearchButton");
        searchButton.RegisterCallback<PointerUpEvent>(OnSearchButtonClick);
    }

    private void OnOpenServerScriptButtonClick()
    {
        string serviceFullPath = NetworkPathConfig.GetServerServiceFullPath();
        string serverRoot = NetworkPathConfig.GetServerRootFullPath();
        string protocolName = this.protocolName.value;
        string scriptFullPath = Path.Combine(serviceFullPath, $"{protocolName}Service.cs");
        string slnFullPath = string.Empty;
        EditorUtilityExtensions.CheckRes(serverRoot, ".sln", (_path) => slnFullPath = _path);
        ExternalScriptOpener.OpenWithChoice(scriptFullPath, slnFullPath);
    }

    private void OnOpenClientScriptButtonClick()
    {
        string protocolName = this.protocolName.value;
        string apiFullPath = NetworkPathConfig.GetClientApiFullPath();
        string scriptPath = Path.Combine(apiFullPath, $"{protocolName}Api.cs");
        ExternalScriptOpener.OpenByFullPath(scriptPath);
    }

    private void OnProtocolNameValueChanged(FocusOutEvent evt)
    {
        string newValue = protocolName.value;
        string protoName_uc;
        EditorUtilityExtensions.ToCamelAndPascal(newValue, out _, out protoName_uc);
        Type type = EditorUtilityExtensions.GetType($"Network.API.{protoName_uc.Trim()}Api");
        if (type == null) return;

        protocolName.SetValueWithoutNotify(protoName_uc);
        if (type.IsSubclassOf(typeof(HttpMessageApi)))
        {
            string path = Path.Combine(NetworkPathConfig.GetClientApiFullPath(), $"{type.Name}.cs");
            LoadHttpProtoToWindow(path);
            connectionType.value = connectionType.choices[0];
        }
        else if (type.IsSubclassOf(typeof(WebSocketMessageApi)))
        {
            LoadWebSocketProtoToWindow(protoName_uc);
            connectionType.value = connectionType.choices[1];
        }
    }

    private void OnProtoSearchButtonClick(PointerUpEvent evt)
    {
        SearchMenuWindowProvider menu = ScriptableObject.CreateInstance<SearchMenuWindowProvider>();
        menu.OnCreateSearchTreeAction = () =>
        {
            List<SearchTreeEntry> entries = new List<SearchTreeEntry>();
            entries.Add(new SearchTreeGroupEntry(new GUIContent("Portocol")));

            entries.Add(new SearchTreeGroupEntry(new GUIContent("Http")) { level = 1 });
            List<SearchTreeEntry> httpProto = menu.GetProtocolEntries<HttpMessageApi>(2);
            entries.AddRange(httpProto);

            entries.Add(new SearchTreeGroupEntry(new GUIContent("Websocket")) { level = 1 });
            List<SearchTreeEntry> webProto = menu.GetProtocolEntries<WebSocketMessageApi>(2);
            entries.AddRange(webProto);

            return entries;
        };

        menu.onSelectEntryHandler += (entry, context) =>
        {
            if (string.IsNullOrEmpty(entry.name)) return false;
            string protoName = entry.name.Replace("Api", "").Trim();
            protocolName.value = protoName;
            Type type = EditorUtilityExtensions.GetType($"Network.API.{entry.name.Trim()}");
            if (type.IsSubclassOf(typeof(HttpMessageApi)))
            {
                LoadHttpProtoToWindow(entry.userData);
                connectionType.value = connectionType.choices[0];
            }
            else if (type.IsSubclassOf(typeof(WebSocketMessageApi)))
            {
                LoadWebSocketProtoToWindow(protoName);
                connectionType.value = connectionType.choices[1];
            }
            return true;
        };

        evt.StopPropagation();
        var mousePos = evt.position;
        mousePos = GUIUtility.GUIToScreenPoint(mousePos);
        SearchWindow.Open(new SearchWindowContext(mousePos), menu);
    }

    private void LoadWebSocketProtoToWindow(string protoName)
    {
        List<NetworkProtocolEventSaveData> eventDatas = ApiMethodParser.GetWebSocketProto(protoName);
        eventScrollView.contentContainer.Clear();
        tempEventDatas.Clear();
        foreach (var data in eventDatas)
        {
            VisualElement item = webSocketEventTemplate.CloneTree();
            eventScrollView.contentContainer.Add(item);
            EventVisualElement eventData = item.Q<EventVisualElement>();
            eventData.Init();
            eventData.SetProtocolDataToWindow(data);
            tempEventDatas.Add(eventData);
        }
    }

    private void LoadHttpProtoToWindow(object pathObj)
    {
        List<NetworkProtocolEventSaveData> eventDatas = ApiMethodParser.GetHttpProto(pathObj.ToString());
        eventScrollView.contentContainer.Clear();
        tempEventDatas.Clear();
        foreach (var data in eventDatas)
        {
            VisualElement item = httpEventTemplate.CloneTree();
            eventScrollView.contentContainer.Add(item);
            EventVisualElement eventData = item.Q<EventVisualElement>();
            eventData.Init();
            eventData.SetProtocolDataToWindow(data);
            tempEventDatas.Add(eventData);
        }
    }

    private void OnConnectionTypeValueChanged(ChangeEvent<string> evt)
    {
        bool isWebSocket = evt.newValue == "WebSocket";
        bool isHttp = evt.newValue == "Http";

        if (isHttp) currEventTemplateitem = httpEventTemplate;
        if (isWebSocket) currEventTemplateitem = webSocketEventTemplate;
    }

    public List<string> GetTempModelNames()
    {
        List<string> modelNames = new List<string>();
        foreach (KeyValuePair<string, List<NetworkProtocolBlockData>> keyValuePair in tempModelDataDic)
        {
            modelNames.Add(keyValuePair.Key);
        }
        return modelNames;
    }
    private void OnSearchButtonClick(PointerUpEvent evt)
    {
        SearchMenuWindowProvider menu = ScriptableObject.CreateInstance<SearchMenuWindowProvider>();
        menu.OnCreateSearchTreeAction = () =>
        {
            List<SearchTreeEntry> entries = new List<SearchTreeEntry>();
            entries.Add(new SearchTreeGroupEntry(new GUIContent("Models")));

            entries.Add(new SearchTreeGroupEntry(new GUIContent("已完成Models")) { level = 1 });
            List<SearchTreeEntry> exitModels = new List<SearchTreeEntry>();
            EditorUtilityExtensions.CheckRes(NetworkPathConfig.GetClientCommModelFullPath(), ".cs", (_path) =>
            {
                string modelName = Path.GetFileNameWithoutExtension(_path);
                exitModels.Add(new SearchTreeEntry(new GUIContent("  " + modelName)) { level = 2 });
            });
            entries.AddRange(exitModels);

            entries.Add(new SearchTreeGroupEntry(new GUIContent("编辑中的Models")) { level = 1 });
            List<SearchTreeEntry> tempModels = new List<SearchTreeEntry>();
            foreach (KeyValuePair<string, List<NetworkProtocolBlockData>> keyValuePair in tempModelDataDic)
            {
                string modelName = keyValuePair.Key;
                tempModels.Add(new SearchTreeEntry(new GUIContent("  " + modelName)) { level = 2 });
            }
            entries.AddRange(tempModels);
            return entries;
        };

        menu.onSelectEntryHandler += (entry, context) =>
        {
            if (string.IsNullOrEmpty(entry.name)) return false;
            rootVisualElement.Q<TextField>("ModelTextField").value = entry.name.TrimStart();
            LoadModelToWindow(entry.name.TrimStart());
            return true;
        };

        evt.StopPropagation();
        var mousePos = evt.position;
        mousePos = GUIUtility.GUIToScreenPoint(mousePos);
        SearchWindow.Open(new SearchWindowContext(mousePos), menu);
    }
    private void LoadModelToWindow(string modelName)
    {
        List<NetworkProtocolBlockData> blockDatas = new List<NetworkProtocolBlockData>();
        if (tempModelDataDic.ContainsKey(modelName))
        {
            blockDatas = tempModelDataDic[modelName];
        }
        else
        {

            Type modelType = EditorUtilityExtensions.GetType($"Network.Models.Common.{modelName}");
            if (modelType == null) return;
            PropertyInfo[] properties = modelType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                Type propertyType;
                bool isEnumerable = EditorUtilityExtensions.TryGetListElementType(property, out propertyType);
                NetworkProtocolBlockData blockData = new NetworkProtocolBlockData();
                blockData.typeName = isEnumerable ? CommonValueTypes.GetShortNameByType(propertyType) : CommonValueTypes.GetShortNameByType(property.PropertyType);
                blockData.variableName = property.Name;
                blockData.isEnumerable = isEnumerable;
                blockDatas.Add(blockData);
            }
        }
        modelEditor.SetProtocolDataToWindow(blockDatas);
    }
    private void OnSaveButtonClick()
    {
        GenerateDataSet();
    }
    private void OnAddEventButtonClick()
    {
        if (tempEventDatas == null) tempEventDatas = new List<EventVisualElement>();

        VisualElement item = currEventTemplateitem.CloneTree();
        eventScrollView.contentContainer.Add(item);
        EventVisualElement eventData = item.Q<EventVisualElement>();
        eventData.Init();
        tempEventDatas.Add(eventData);
    }
    private void OnClearButtonClick()
    {
        modelEditor.ClearAll();
    }
    private void OnRemoveButtonClick()
    {
        //删除tempModelDataDic中对应Model
        string modelName = rootVisualElement.Q<TextField>("ModelTextField").value;
        if (string.IsNullOrEmpty(modelName)) return;
        if (!tempModelDataDic.ContainsKey(modelName)) return;
        tempModelDataDic.Remove(modelName);

        modelEditor.ClearAll();
    }
    private void OnApplyButtonClick()
    {
        List<NetworkProtocolBlockData> blockDatas = modelEditor.GetProtocolData();
        string modelName = rootVisualElement.Q<TextField>("ModelTextField").value;
        if (string.IsNullOrEmpty(modelName)) return;

        bool isStar = modelName.StartsWith("*");
        modelName = isStar ? modelName : $"*{modelName}";
        rootVisualElement.Q<TextField>("ModelTextField").SetValueWithoutNotify(modelName);
        tempModelDataDic[modelName] = blockDatas;
    }

    /// <summary>
    /// 构建数据结构，为代码生成做准备
    /// </summary>
    private void GenerateDataSet()
    {
        string connectionType = this.connectionType.value;
        string protocolName = this.protocolName.value;
        List<NetworkProtocolEventSaveData> eventSaveDatas = new List<NetworkProtocolEventSaveData>();
        foreach (EventVisualElement eventVisual in tempEventDatas)
        {
            if (!eventVisual.isAdd) continue;
            eventSaveDatas.Add(eventVisual.GetEventSaveData());
        }
        NetworkProtocolSaveData protocolSaveData = new NetworkProtocolSaveData();
        protocolSaveData.eventData = eventSaveDatas;
        protocolSaveData.protoclName = protocolName;
        protocolSaveData.connectionType = connectionType;
        // 复制枚举容器
        NetworkProtocolSaveUtility.CopyEnumDefinitionsToServer();
        if (connectionType == "Http")
        {
            //----Http Protocol ----
            NetworkProtocolSaveUtility.GenServer_HttpProtocol(protocolSaveData);
            NetworkProtocolSaveUtility.GenClient_HttpProtocol(protocolSaveData);
        }
        else if (connectionType == "WebSocket")
        {
            //----WebSocket Protocol ----
            NetworkProtocolSaveUtility.GenClient_WebSocketMessageApi(protocolName);
            NetworkProtocolSaveUtility.GenServer_WebSocketHandler(protocolName, protocolSaveData);
            NetworkProtocolSaveUtility.RefreshNetworkEventPaths_WebSocket(protocolName, protocolSaveData);
            NetworkProtocolSaveUtility.RefreshApiManager_WebSocket(protocolName);
        }
        //----ModelSave----
        if (tempModelDataDic != null && tempModelDataDic.Count > 0)
        {
            Dictionary<string, List<NetworkProtocolBlockData>> saveDic = new Dictionary<string, List<NetworkProtocolBlockData>>();
            foreach (KeyValuePair<string, List<NetworkProtocolBlockData>> keyValuePair in tempModelDataDic)
            {
                string modelName = keyValuePair.Key;
                bool isStar = modelName.StartsWith("*");
                modelName = isStar ? modelName.Substring(1) : modelName;
                saveDic[modelName] = keyValuePair.Value;
            }
            NetworkProtocolSaveUtility.GenServer_Model(saveDic);
            NetworkProtocolSaveUtility.GenClient_Model(saveDic);
            tempModelDataDic.Clear();
        }

        Debug.Log("Network Code Generate Completed!");
        AssetDatabase.Refresh();
    }
}