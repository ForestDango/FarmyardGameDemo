using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 背包系统，挂载在同名游戏对象上
/// </summary>
[RequireComponent(typeof(GenerateGUID))]
public class InventoryManager : Singleton<InventoryManager>,ISaveable
{
    private UIInventoryBar inventoryBar;
    private Dictionary<int, ItemDetails> itemDetailsDictionary; //通过itemDetails上的itemCode(int类型)的值寻找对应的itemDetails
    private int[] selectedInvenetoryItem;

    public List<InventoryItem>[] inventoryLists; //一个存放所有InventoryItem的链表的数组
    [HideInInspector] public int[] inventoryListInCapcityInArray; //背包容量大小默认是12

    [SerializeField] private SO_ItemList itemLists; //SO_ItemList登记过所有Item


    public string _iSaveableUniqueID;
    public string ISaveableUniqueID { get => _iSaveableUniqueID; set =>_iSaveableUniqueID = value; }

    public GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get =>_gameObjectSave; set => _gameObjectSave = value; }

    protected override void Awake()
    {
        base.Awake();

        CreateInventoryLists();

        CreateItemDetailsDictionary();

        selectedInvenetoryItem = new int[(int)InventoryLocation.count];

        for (int i = 0; i < selectedInvenetoryItem.Length; i++)
        {
            selectedInvenetoryItem[i] = -1; //-1表示我们没有选中任何一个物品
        }

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void Start()
    {
        inventoryBar = FindObjectOfType<UIInventoryBar>();
    }

    private void OnEnable()
    {
        ISaveableRegister();
    }

    private void OnDisable()
    {
        ISaveableDeregister();
    }

    /// <summary>
    /// 初始化所有背包的List数据
    /// </summary>
    private void CreateInventoryLists()
    {
        inventoryLists = new List<InventoryItem>[(int)InventoryLocation.count];//我们有0号背包系统的链表和1号背包系统的链表

        for (int i = 0; i < (int)InventoryLocation.count; i++)
        {
            inventoryLists[i] = new List<InventoryItem>();
        }

        //出事背包容量列表
        inventoryListInCapcityInArray = new int[(int)InventoryLocation.count];

        //初始化玩家背包（0号背包）容量
        inventoryListInCapcityInArray[(int)InventoryLocation.player] = Settings.playerIntialInventoryCapcity;
    }
    /// <summary>
    /// 重载函数
    /// </summary>
    /// <param name="inventoryLocation"></param>
    /// <param name="item"></param>
    /// <param name="gameObjectToDelete"></param>
    public void AddItem(InventoryLocation inventoryLocation, Item item,GameObject gameObjectToDelete) 
    {
        AddItem(inventoryLocation, item);

        Destroy(gameObjectToDelete);
    }

    /// <summary>
    /// 将物品添加打到特定的InventoryLocation，并检查背包中是否有相同的物品
    /// </summary>
    /// <param name="inventoryLocation"></param>
    /// <param name="itemCode"></param>
    public void AddItem(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        int itemPositon = FindItemInventory(inventoryLocation, itemCode);

        if (itemPositon != -1)
        {
            AddItemAtPosition(inventoryList, itemCode, itemPositon);//如果找到物品就数量++
        }
        else
        {
            AddItemAtPosition(inventoryList, itemCode); //找不到就在空白处新添加物品
        }

        EventHandler.CallInventoryUpdateEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
    }


    /// <summary>
    /// 添加物品
    /// </summary>
    /// <param name="inventoryLocation"></param>
    /// <param name="item"></param>
    public void AddItem(InventoryLocation inventoryLocation,Item item)
    {
        int itemCode = item.ItemCode;
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        int itemPositon = FindItemInventory(inventoryLocation, itemCode);

        if(itemPositon != -1)
        {
            AddItemAtPosition(inventoryList, itemCode, itemPositon);//如果找到物品就数量++
        }
        else
        {
            AddItemAtPosition(inventoryList, itemCode); //找不到就在空白处新添加物品
        }

        EventHandler.CallInventoryUpdateEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
    }
    /// <summary>
    /// 交换物品顺序
    /// </summary>
    /// <param name="inventoryLocation"></param>
    /// <param name="fromSlotNumber"></param>
    /// <param name="toSlotNumber"></param>
    public void SwapInventoryItems(InventoryLocation inventoryLocation, int fromSlotNumber, int toSlotNumber)
    {
        if(fromSlotNumber < inventoryLists[(int)inventoryLocation].Count && toSlotNumber < inventoryLists[(int)inventoryLocation].Count &&
            fromSlotNumber != toSlotNumber  && toSlotNumber >= 0)
        {
            InventoryItem fromSlotItem = inventoryLists[(int)inventoryLocation][fromSlotNumber];
            InventoryItem toSlotItem = inventoryLists[(int)inventoryLocation][toSlotNumber];

            inventoryLists[(int)inventoryLocation][toSlotNumber] = fromSlotItem;
            inventoryLists[(int)inventoryLocation][fromSlotNumber] = toSlotItem;

            EventHandler.CallInventoryUpdateEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);

        }
    }

    /// <summary>
    /// 将物品添加到背包的位置
    /// </summary>
    /// <param name="inventoryList"></param>
    /// <param name="itemCode"></param>

    private void AddItemAtPosition(List<InventoryItem> inventoryList, int itemCode)
    {
        InventoryItem inventoryItem = new InventoryItem();

        inventoryItem.itemCode = itemCode;
        inventoryItem.itemQuantity = 1;

        inventoryList.Add(inventoryItem);
    }

    /// <summary>
    /// 对于背包中已经有的物品就加数量
    /// </summary>
    /// <param name="inventoryList"></param>
    /// <param name="itemCode"></param>
    /// <param name="itemPositon"></param>

    private void AddItemAtPosition(List<InventoryItem> inventoryList, int itemCode, int positon)
    {
        InventoryItem inventoryItem = new InventoryItem();

        int quantity = inventoryList[positon].itemQuantity + 1;
        inventoryItem.itemCode = itemCode;
        inventoryItem.itemQuantity = quantity;
        inventoryList[positon] = inventoryItem;

        Debug.ClearDeveloperConsole();
    }

    /// <summary>
    /// 移除背包的物品
    /// </summary>
    /// <param name="inventoryLocation"></param>
    /// <param name="itemCode"></param>
    internal void RemoveItem(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];
        int itemPosition = FindItemInventory(inventoryLocation, itemCode);

        if (itemPosition != -1)
        {
            RemoveItemAtPosition(inventoryList,itemCode,itemPosition);
        }
        EventHandler.CallInventoryUpdateEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
    }

    /// <summary>
    /// 移除制定位置的物品，如果数量大于0则只是减少数量，否则直接在inventoryList上移除该物品
    /// </summary>
    /// <param name="inventoryList"></param>
    /// <param name="itemCode"></param>
    /// <param name="position"></param>
    private void RemoveItemAtPosition(List<InventoryItem> inventoryList, int itemCode, int position)
    {
        InventoryItem inventoryItem = new InventoryItem();

        int quantity = inventoryList[position].itemQuantity - 1;

        if(quantity > 0)
        {
            inventoryItem.itemQuantity = quantity;
            inventoryItem.itemCode = itemCode;
            inventoryList[position] = inventoryItem;
        }
        else
        {
            inventoryList.RemoveAt(position);
        }
    }

    public string GetItemTypeDesciption(ItemType itemType)
    {
        string itemDescription;

        switch (itemType)
        {
            case ItemType.Breaking_tool:
                itemDescription = Settings.BreakingTool; 
                break;

            case ItemType.Chopping_tool:
                itemDescription = Settings.ChoppingTool;
                break;

            case ItemType.Hoeing_tool:
                itemDescription = Settings.HoeingTool;
                break;

            case ItemType.Reping_tool:
                itemDescription = Settings.ReapingTool;
                break;
            case ItemType.Watering_tool:
                itemDescription = Settings.WateringTool;
                break;
            case ItemType.Collecting_tool:
                itemDescription = Settings.CollectingTool;
                break;
            default:
                itemDescription = itemType.ToString();
                break;
        }

        return itemDescription;
    }

    /// <summary>
    /// 判断物品是否在背包里面，找不到就return -1，找到的话就返回发现物品的位置
    /// </summary>
    /// <param name="inventoryLocation"></param>
    /// <param name="itemCode"></param>
    /// <returns></returns>
    public int FindItemInventory(InventoryLocation inventoryLocation,int itemCode)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        for (int i = 0; i < inventoryList.Count; i++)
        {
            if(inventoryList[i].itemCode == itemCode)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// 初始化创建的字典
    /// </summary>

    private void CreateItemDetailsDictionary()
    {
        itemDetailsDictionary = new Dictionary<int, ItemDetails>();

        foreach (var itemDetails in itemLists.itemDetails)
        {
            itemDetailsDictionary.Add(itemDetails.itemCode,itemDetails);
        }
    }

    /// <summary>
    /// 在背包中找ItemDetails
    /// </summary>
    /// <param name="itemCode"></param>
    /// <returns></returns>
    public ItemDetails GetItemDetails(int itemCode)
    {
        ItemDetails itemDetails;

        if(itemDetailsDictionary.TryGetValue(itemCode,out itemDetails))
        {
            return itemDetails;
        }
        else
        {
            return null;
        }
    }

    public void SetSelectedInventoryItem(InventoryLocation inventoryLocation,int itemCode)
    {
        selectedInvenetoryItem[(int)inventoryLocation] = itemCode;
    }

    public ItemDetails GetSelectedInventoryItemDetails(InventoryLocation inventoryLocation)
    {
        int itemCode = GetSelectedInventoryItem(inventoryLocation);

        if(itemCode == -1)
        {
            return  null;
        }
        else
        {
            return GetItemDetails(itemCode);
        }
    }

    public void ClearSelectedInventoryItem(InventoryLocation inventoryLocation)
    {
        selectedInvenetoryItem[(int)inventoryLocation] = -1;
    }

    public int GetSelectedInventoryItem(InventoryLocation inventoryLocation)
    {
        return selectedInvenetoryItem[(int)inventoryLocation];
    }


    /// <summary>
    /// 吃到物品用的调试
    /// </summary>
    /// <param name="inventoryLists"></param>
    private void DebugPrintInventoryList(List<InventoryItem> inventoryLists)
    {
        foreach (var inventoyItem in inventoryLists)
        {
            Debug.Log("Item Description:" + InventoryManager.Instance.GetItemDetails(inventoyItem.itemCode).itemDescription +
                "       Item Quantity:" + inventoyItem.itemQuantity);
        }
    }
    #region 实现ISaveable接口的所有方法

    /// <summary>
    /// 登记可存储的数据
    /// </summary>
    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);  
    }

    /// <summary>
    /// 注销可存储的数据
    /// </summary>
    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public GameObjectSave ISaveableSave()
    {
        SceneSave sceneSave = new SceneSave();

        GameObjectSave.sceneData.Remove(Settings.persistentScene);

        sceneSave.listInventoryItemArray = inventoryLists;

        sceneSave.intArrayDicitionary = new Dictionary<string, int[]>();
        sceneSave.intArrayDicitionary.Add("inventoryListCapcityArray", inventoryListInCapcityInArray);

        GameObjectSave.sceneData.Add(Settings.persistentScene, sceneSave);

        return GameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if (gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            if (gameObjectSave.sceneData.TryGetValue(Settings.persistentScene, out SceneSave sceneSave))
            {
                if (sceneSave.listInventoryItemArray != null)
                {
                    inventoryLists = sceneSave.listInventoryItemArray;

                    for (int i = 0; i < (int)InventoryLocation.count; i++)
                    {
                        EventHandler.CallInventoryUpdateEvent((InventoryLocation)i, inventoryLists[i]);

                    }

                    PlayerController.Instance.ClearCarriedItem();
                    inventoryBar.ClearHighlightOnInventorySlots();
                }

                if(sceneSave.intArrayDicitionary != null && sceneSave.intArrayDicitionary.TryGetValue("inventoryListCapcityArray",out int[] inventoryCapityArray))
                {
                    inventoryListInCapcityInArray = inventoryCapityArray;
                }
            }
        }
    }

    public void ISaveableStoreScene(string sceneName)
    {
        
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        
    }

    #endregion
}
