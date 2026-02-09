using UnityEngine.UIElements;
[UxmlElement]
public partial class WebSocketEventVisualElement : EventVisualElement
{
    //public new class UxmlFactory : UxmlFactory<WebSocketEventVisualElement, UxmlTraits> { }
    private TextField patternTextField;
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
        patternTextField.SetEnabled(false);
    }
    private void OnAddButtonClick()
    {
        isAdd = true;
        addButton.SetEnabled(false);
        removeButton.SetEnabled(true);
        patternTextField.SetEnabled(true);
    }
    public override NetworkProtocolEventSaveData GetEventSaveData()
    {
        NetworkProtocolEventSaveData result = new NetworkProtocolEventSaveData();
        result.pattern = patternTextField.value;
        return result;
    }

    public override void SetProtocolDataToWindow(NetworkProtocolEventSaveData eventSaveData)
    {
        patternTextField.value = eventSaveData.pattern;
    }
}
