using UnityEngine.UIElements;
[UxmlElement]
public partial class HttpEventVisualElement : EventVisualElement
{
    //public new class UxmlFactory : UxmlFactory<HttpEventVisualElement, UxmlTraits> { }
    private TextField patternTextField;
    private ProtocolEditor requestProtocolEditor;
    private ProtocolEditor responseProtocolEditor;
    private VisualElement requestProto;
    private VisualElement responseProto;
    private Button addButton;
    private Button removeButton;
    public override void Init()
    {
        addButton = this.Q<Button>("AddButton");
        addButton.clicked += OnAddButtonClick;
        isAdd = true;

        removeButton = this.Q<Button>("RemoveButton");
        removeButton.clicked += OnRemoveButtonClick;
        addButton.SetEnabled(false);

        requestProto = this.Q<VisualElement>("RequestProtocolEditor");
        requestProtocolEditor = requestProto.Q<ProtocolEditor>();
        requestProtocolEditor.InitUI();
        requestProtocolEditor.SetTitle("Request");

        responseProto = this.Q<VisualElement>("ResponseProtocolEditor");
        responseProtocolEditor = responseProto.Q<ProtocolEditor>();
        responseProtocolEditor.InitUI();
        responseProtocolEditor.SetTitle("Response");

        patternTextField = this.Q<TextField>("PatternTextField");
        patternTextField.RegisterCallback<FocusOutEvent>(OnPatternTextFieldFocusOut);
    }

    private void OnPatternTextFieldFocusOut(FocusOutEvent evt)
    {
        string currText = patternTextField.value;
        if (string.IsNullOrEmpty(currText)) return;
        if (currText.StartsWith("/")) return;
        patternTextField.SetValueWithoutNotify("/" + currText);
    }

    private void OnRemoveButtonClick()
    {
        isAdd = false;
        addButton.SetEnabled(true);
        removeButton.SetEnabled(false);
        requestProto.SetEnabled(false);
        responseProto.SetEnabled(false);
        patternTextField.SetEnabled(false);
    }

    private void OnAddButtonClick()
    {
        isAdd = true;
        addButton.SetEnabled(false);
        removeButton.SetEnabled(true);
        requestProto.SetEnabled(true);
        responseProto.SetEnabled(true);
        patternTextField.SetEnabled(true);
    }

    public override NetworkProtocolEventSaveData GetEventSaveData()
    {
        NetworkProtocolEventSaveData result = new NetworkProtocolEventSaveData();
        result.pattern = patternTextField.value;
        result.request = requestProtocolEditor.GetProtocolData();
        result.response = responseProtocolEditor.GetProtocolData();
        return result;
    }
    public override void SetProtocolDataToWindow(NetworkProtocolEventSaveData eventSaveData) 
    {
        patternTextField.value = eventSaveData.pattern;
        requestProtocolEditor.SetProtocolDataToWindow(eventSaveData.request);
        responseProtocolEditor.SetProtocolDataToWindow(eventSaveData.response);
    }
}
