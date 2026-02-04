using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class CompalexBlockItem : VisualElement
{
    public int index;
    public bool isSelected;
    public Action<int, bool> onSelectedChanged;

    private bool isEnumerable;
    private DropdownField typeDropdown;
    private TextField variableTextField;
    private Toggle IEnumerableToggle;
    public new class UxmlFactory : UxmlFactory<CompalexBlockItem, UxmlTraits> { }

    public CompalexBlockItem()
    {
        RegisterCallback<PointerDownEvent>(OnPointerDown); ;
    }
    public void Init()
    {
        isSelected = false;
        EnableInClassList("selected", isSelected);


        InitItemUI();
    }
    public void InitItemUI()
    {
        //搜索菜单
        SearchMenuWindowProvider menu = ScriptableObject.CreateInstance<SearchMenuWindowProvider>();
        menu.OnCreateSearchTreeAction = () =>
        {
            List<SearchTreeEntry> entries = new List<SearchTreeEntry>();
            entries.Add(new SearchTreeGroupEntry(new GUIContent("变量类型")));

            entries.Add(new SearchTreeGroupEntry(new GUIContent("常用值类型")) { level = 1 });
            List<SearchTreeEntry> values = menu.GetCommonValueTypeEntries(2);
            entries.AddRange(values);

            entries.Add(new SearchTreeGroupEntry(new GUIContent("自定义Model")) { level = 1 });
            List<SearchTreeEntry> models = menu.GetModelEntries(2);
            entries.AddRange(models);

            entries.Add(new SearchTreeGroupEntry(new GUIContent("枚举")) { level = 1 });
            List<SearchTreeEntry> enumNames = GetEnumNames();
            entries.AddRange(enumNames);

            return entries;
        };
        menu.onSelectEntryHandler += (entry, context) =>
        {
            if (string.IsNullOrEmpty(entry.name)) return false;
            typeDropdown.value = entry.name;
            return true;
        };
        typeDropdown = this.Q<DropdownField>("TypeDropdown");
        typeDropdown.choices = new List<string>();
        typeDropdown.value = "";
        typeDropdown.RegisterCallback<PointerDownEvent>((evt) =>
        {
            evt.StopPropagation();
            var mousePos = evt.position; // UI Toolkit里的界面坐标（相对EditorWindow）
            mousePos = GUIUtility.GUIToScreenPoint(mousePos); // 转换成屏幕坐标
            SearchWindow.Open(new SearchWindowContext(mousePos), menu);
        });

        variableTextField = this.Q<TextField>("VariableTextField");

        IEnumerableToggle = this.Q<Toggle>("IEnumerableToggle");
        IEnumerableToggle.SetValueWithoutNotify(false);
        IEnumerableToggle.RegisterValueChangedCallback((_value) =>
        {
            if (isEnumerable == _value.newValue) isEnumerable = _value.previousValue;
            else isEnumerable = _value.newValue;
            IEnumerableToggle.SetValueWithoutNotify(isEnumerable);
        });
    }
    private List<SearchTreeEntry> GetEnumNames() 
    {
        List<string> enumNames = EnumDefinitions.GetEnumNames();
        List<SearchTreeEntry> entries = new List<SearchTreeEntry>();
        foreach (string name in enumNames) 
        {
            entries.Add(new SearchTreeEntry(new GUIContent("  " + name)) { level = 2 });
        }
        return entries;
    }
    private void OnPointerDown(PointerDownEvent evt)
    {
        isSelected = true;
        SetSelected(isSelected);
    }
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        EnableInClassList("selected", isSelected);
        onSelectedChanged?.Invoke(index, isSelected);
    }
    public void SetSelectWithoutNotify(bool selected)
    {
        isSelected = selected;
        EnableInClassList("selected", isSelected);
    }
    public void SetBlockData(NetworkProtocolBlockData blockData) 
    {
        typeDropdown.SetValueWithoutNotify(blockData.typeName);
        variableTextField.SetValueWithoutNotify(blockData.variableName);
        IEnumerableToggle.SetValueWithoutNotify(blockData.isEnumerable);
        isEnumerable = blockData.isEnumerable;
    }
    public NetworkProtocolBlockData GetBlockData()
    {
        NetworkProtocolBlockData blockData = new NetworkProtocolBlockData();
        blockData.typeName = typeDropdown.value.Trim();
        blockData.variableName = variableTextField.value.Trim();
        blockData.isEnumerable = isEnumerable;
        return blockData;
    }
}