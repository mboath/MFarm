using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using System.Linq;
using System;

public class ItemEditor : EditorWindow
{
    private ItemDataList_SO dataBase;
    private List<ItemDetails> itemList = new List<ItemDetails>();
    private VisualTreeAsset itemRowTemplate;
    private ListView itemListView;
    private ScrollView itemDetailsSection;
    private ItemDetails activeItem;
    private VisualElement iconPreview;
    private Sprite defaultIcon;

    [MenuItem("M Farm/ItemEditor")]
    public static void ShowExample()
    {
        ItemEditor wnd = GetWindow<ItemEditor>();
        wnd.titleContent = new GUIContent("ItemEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        //VisualElement label = new Label("Hello World! From C#");
        //root.Add(label);

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UI Builder/ItemEditor.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        //取得模板
        itemRowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UI Builder/ItemRow Template.uxml");

        //取得默认图标
        defaultIcon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/M Studio/Art/Items/Icons/icon_M.png");

        //变量赋值
        itemListView = root.Q<VisualElement>("ItemList").Q<ListView>("ListView");
        itemDetailsSection = root.Q<ScrollView>("ItemDetails");
        iconPreview = itemDetailsSection.Q<VisualElement>("Icon");

        //获得按键
        root.Q<Button>("AddButton").clicked += OnAddItemClicked;
        root.Q<Button>("DeleteButton").clicked += OnDeleteItemClicked;
        
        //加载数据
        LoadDataBase();

        //生成ListView
        GenerateListView();
    }

    #region 按键事件
    private void OnAddItemClicked()
    {
        ItemDetails newItem = new ItemDetails();
        newItem.itemID = 1001 + itemList.Count;
        newItem.itemName = "NEW ITEM";
        itemList.Add(newItem);
        itemListView.Rebuild();
    }

    private void OnDeleteItemClicked()
    {
        itemList.Remove(activeItem);
        itemListView.Rebuild();
        itemDetailsSection.visible = false;
    }
    #endregion

    private void LoadDataBase()
    {
        var dataArray = AssetDatabase.FindAssets("ItemDataList_SO");

        if (dataArray.Length > 1)
        {
            var path = AssetDatabase.GUIDToAssetPath(dataArray[0]);
            dataBase = AssetDatabase.LoadAssetAtPath(path, typeof(ItemDataList_SO)) as ItemDataList_SO;
        }

        itemList = dataBase.ItemDetailsList;

        //如果不标记则无法保存数据
        EditorUtility.SetDirty(dataBase);
    }

    private void GenerateListView()
    {
        Func<VisualElement> makeItem = () => itemRowTemplate.CloneTree();

        Action<VisualElement, int> bindItem = (e, i) =>
        {
            if (i < itemList.Count)
            {
                if (itemList[i].itemIcon != null)
                    e.Q<VisualElement>("Icon").style.backgroundImage = itemList[i].itemIcon.texture;
                e.Q<Label>("Name").text = (itemList[i] == null ? "NO ITEM" : itemList[i].itemName);
            }
        };

        itemListView.fixedItemHeight = 50;  //根据需要高度调整数值
        itemListView.itemsSource = itemList;
        itemListView.makeItem = makeItem;
        itemListView.bindItem = bindItem;

        itemListView.onSelectionChange += OnListSelectionChange;

        itemDetailsSection.visible = false;  //右侧信息面板不可见
    }

    private void OnListSelectionChange(IEnumerable<object> selectedItem)
    {
        activeItem = (ItemDetails)selectedItem.First();
        GetItemDetails();
        itemDetailsSection.visible = true;  //右侧信息面板可见
    }

    private void GetItemDetails()
    {
        itemDetailsSection.MarkDirtyRepaint();

        //itemID
        itemDetailsSection.Q<IntegerField>("itemID").value = activeItem.itemID;
        itemDetailsSection.Q<IntegerField>("itemID").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemID = evt.newValue;
        });

        //itemName
        itemDetailsSection.Q<TextField>("itemName").value = activeItem.itemName;
        itemDetailsSection.Q<TextField>("itemName").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemName = evt.newValue;
            itemListView.Rebuild();
        });

        //itemType
        itemDetailsSection.Q<EnumField>("itemType").Init(activeItem.itemType);
        itemDetailsSection.Q<EnumField>("itemType").value = activeItem.itemType;
        itemDetailsSection.Q<EnumField>("itemType").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemType = (ItemType)evt.newValue;
        });

        //itemIcon
        itemDetailsSection.Q<ObjectField>("itemIcon").value = activeItem.itemIcon;
        iconPreview.style.backgroundImage = (activeItem.itemIcon == null ? defaultIcon.texture : activeItem.itemIcon.texture);
        itemDetailsSection.Q<ObjectField>("itemIcon").RegisterValueChangedCallback(evt =>
        {
            Sprite newIcon = evt.newValue as Sprite;
            activeItem.itemIcon = newIcon;
            iconPreview.style.backgroundImage = (newIcon == null ? defaultIcon.texture : newIcon.texture);
            itemListView.Rebuild();
        });

        //itemOnWorldSprite
        itemDetailsSection.Q<ObjectField>("itemOnWorldSprite").value = activeItem.itemOnWorldSprite;
        itemDetailsSection.Q<ObjectField>("itemOnWorldSprite").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemOnWorldSprite = evt.newValue as Sprite;
        });

        //itemDescription
        itemDetailsSection.Q<TextField>("itemDescription").value = activeItem.itemDescription;
        itemDetailsSection.Q<TextField>("itemDescription").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemDescription = evt.newValue;
        });

        //itemUseRadius
        itemDetailsSection.Q<IntegerField>("itemUseRadius").value = activeItem.itemUseRadius;
        itemDetailsSection.Q<IntegerField>("itemUseRadius").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemUseRadius = evt.newValue;
        });

        //canPickUp
        itemDetailsSection.Q<Toggle>("canPickUp").value = activeItem.canPickUp;
        itemDetailsSection.Q<Toggle>("canPickUp").RegisterValueChangedCallback(evt =>
        {
            activeItem.canPickUp = evt.newValue;
        });

        //canDrop
        itemDetailsSection.Q<Toggle>("canDrop").value = activeItem.canDrop;
        itemDetailsSection.Q<Toggle>("canDrop").RegisterValueChangedCallback(evt =>
        {
            activeItem.canDrop = evt.newValue;
        });

        //canCarry
        itemDetailsSection.Q<Toggle>("canCarry").value = activeItem.canCarry;
        itemDetailsSection.Q<Toggle>("canCarry").RegisterValueChangedCallback(evt =>
        {
            activeItem.canCarry = evt.newValue;
        });

        //itemPrice
        itemDetailsSection.Q<IntegerField>("itemPrice").value = activeItem.itemPrice;
        itemDetailsSection.Q<IntegerField>("itemPrice").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemPrice = evt.newValue;
        });

        //sellPercentage
        itemDetailsSection.Q<Slider>("sellPercentage").value = activeItem.sellPercentage;
        itemDetailsSection.Q<Slider>("sellPercentage").RegisterValueChangedCallback(evt =>
        {
            activeItem.sellPercentage = evt.newValue;
        });
    }
}