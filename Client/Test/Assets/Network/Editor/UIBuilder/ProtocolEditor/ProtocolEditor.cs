using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class ProtocolEditor : VisualElement
{
    private ScrollView requestScrollView;
    private VisualTreeAsset blockItemTemplate;
    private Label emptyLabel;
    private int selectedIndex = -1;
    private List<CompalexBlockItem> compalexBlockItems = new List<CompalexBlockItem>();
    public new class UxmlFactory : UxmlFactory<ProtocolEditor, UxmlTraits> { }
    public void InitUI()
    {
        blockItemTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Network/Editor/UIBuilder/CompalexBlockItem/CompalexBlockItem.uxml");

        requestScrollView = this.Q<ScrollView>("RequestScrollView");
        requestScrollView.contentContainer.Clear();
        emptyLabel = new Label("List is Empty");
        emptyLabel.AddToClassList("empty-label");
        requestScrollView.contentContainer.Add(emptyLabel);

        Button addButton = this.Q<Button>("AddButton");
        Button removeButton = this.Q<Button>("RemoveButton");

        addButton.clicked += OnAddButtonClick;
        removeButton.clicked += OnRemoveButtonClick;
    }
    public void ClearAll()
    {
        for (int i = requestScrollView.contentContainer.childCount; i > 1; i--)
        {
            int index = i - 1;
            requestScrollView.contentContainer.RemoveAt(index);
        }
        compalexBlockItems.Clear();
        selectedIndex = -1;
        emptyLabel.style.display = DisplayStyle.Flex;
    }
    private void OnRemoveButtonClick()
    {
        if (selectedIndex < 0) return;
        requestScrollView.contentContainer.RemoveAt(selectedIndex + 1);//因为有一个emptyLabel占位，所以要加1
        compalexBlockItems.RemoveAt(selectedIndex);
        foreach (CompalexBlockItem block in compalexBlockItems)
        {
            block.index = compalexBlockItems.IndexOf(block);
        }
        if (compalexBlockItems.Count > 0) selectedIndex = compalexBlockItems.Count - 1;
        else selectedIndex = -1;

        emptyLabel.style.display = compalexBlockItems.Count > 0 ? DisplayStyle.None : DisplayStyle.Flex;
    }

    private void OnAddButtonClick()
    {
        VisualElement item = blockItemTemplate.CloneTree();
        requestScrollView.contentContainer.Add(item);
        CompalexBlockItem block = item.Q<CompalexBlockItem>();
        block.Init();
        block.onSelectedChanged = OnBlockItemSelectChanged;
        compalexBlockItems.Add(block);
        block.index = compalexBlockItems.IndexOf(block);

        selectedIndex = block.index;
        block.SetSelected(true);

        emptyLabel.style.display = compalexBlockItems.Count > 0 ? DisplayStyle.None : DisplayStyle.Flex;
    }

    private void OnBlockItemSelectChanged(int index, bool isOn)
    {
        if (isOn)
        {
            selectedIndex = index;
            foreach (CompalexBlockItem block in compalexBlockItems)
            {
                if (block.index != index) block.SetSelected(false);
            }
        }
    }

    public void SetTitle(string title)
    {
        this.Q<Label>("TitleLabel").text = title;
    }
    public void SetProtocolDataToWindow(List<NetworkProtocolBlockData> blockDatas) 
    {
        ClearAll();
        foreach (NetworkProtocolBlockData blockData in blockDatas) 
        {
            VisualElement item = blockItemTemplate.CloneTree();
            requestScrollView.contentContainer.Add(item);
            CompalexBlockItem block = item.Q<CompalexBlockItem>();
            block.Init();
            block.SetBlockData(blockData);
            block.onSelectedChanged = OnBlockItemSelectChanged;
            compalexBlockItems.Add(block);
            block.index = compalexBlockItems.IndexOf(block);

            selectedIndex = block.index;
            block.SetSelected(true);
        }
        
        emptyLabel.style.display = compalexBlockItems.Count > 0 ? DisplayStyle.None : DisplayStyle.Flex;
    }
    public List<NetworkProtocolBlockData> GetProtocolData()
    {
        List<NetworkProtocolBlockData> result = new List<NetworkProtocolBlockData>();
        foreach (CompalexBlockItem block in compalexBlockItems)
        {
            NetworkProtocolBlockData blockData = block.GetBlockData();
            result.Add(blockData);
        }

        return result;
    }
}
