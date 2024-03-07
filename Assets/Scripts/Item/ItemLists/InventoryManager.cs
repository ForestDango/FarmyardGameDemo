using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����ϵͳ��������ͬ����Ϸ������
/// </summary>
[RequireComponent(typeof(GenerateGUID))]
public class InventoryManager : Singleton<InventoryManager>,ISaveable
{
    private UIInventoryBar inventoryBar;
    private Dictionary<int, ItemDetails> itemDetailsDictionary; //ͨ��itemDetails�ϵ�itemCode(int����)��ֵѰ�Ҷ�Ӧ��itemDetails
    private int[] selectedInvenetoryItem;

    public List<InventoryItem>[] inventoryLists; //һ���������InventoryItem�����������
    [HideInInspector] public int[] inventoryListInCapcityInArray; //����������СĬ����12

    [SerializeField] private SO_ItemList itemLists; //SO_ItemList�Ǽǹ�����Item


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
            selectedInvenetoryItem[i] = -1; //-1��ʾ����û��ѡ���κ�һ����Ʒ
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
    /// ��ʼ�����б�����List����
    /// </summary>
    private void CreateInventoryLists()
    {
        inventoryLists = new List<InventoryItem>[(int)InventoryLocation.count];//������0�ű���ϵͳ�������1�ű���ϵͳ������

        for (int i = 0; i < (int)InventoryLocation.count; i++)
        {
            inventoryLists[i] = new List<InventoryItem>();
        }

        //���±��������б�
        inventoryListInCapcityInArray = new int[(int)InventoryLocation.count];

        //��ʼ����ұ�����0�ű���������
        inventoryListInCapcityInArray[(int)InventoryLocation.player] = Settings.playerIntialInventoryCapcity;
    }
    /// <summary>
    /// ���غ���
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
    /// ����Ʒ��Ӵ��ض���InventoryLocation������鱳�����Ƿ�����ͬ����Ʒ
    /// </summary>
    /// <param name="inventoryLocation"></param>
    /// <param name="itemCode"></param>
    public void AddItem(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        int itemPositon = FindItemInventory(inventoryLocation, itemCode);

        if (itemPositon != -1)
        {
            AddItemAtPosition(inventoryList, itemCode, itemPositon);//����ҵ���Ʒ������++
        }
        else
        {
            AddItemAtPosition(inventoryList, itemCode); //�Ҳ������ڿհ״��������Ʒ
        }

        EventHandler.CallInventoryUpdateEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
    }


    /// <summary>
    /// �����Ʒ
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
            AddItemAtPosition(inventoryList, itemCode, itemPositon);//����ҵ���Ʒ������++
        }
        else
        {
            AddItemAtPosition(inventoryList, itemCode); //�Ҳ������ڿհ״��������Ʒ
        }

        EventHandler.CallInventoryUpdateEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
    }
    /// <summary>
    /// ������Ʒ˳��
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
    /// ����Ʒ��ӵ�������λ��
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
    /// ���ڱ������Ѿ��е���Ʒ�ͼ�����
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
    /// �Ƴ���������Ʒ
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
    /// �Ƴ��ƶ�λ�õ���Ʒ�������������0��ֻ�Ǽ�������������ֱ����inventoryList���Ƴ�����Ʒ
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
    /// �ж���Ʒ�Ƿ��ڱ������棬�Ҳ�����return -1���ҵ��Ļ��ͷ��ط�����Ʒ��λ��
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
    /// ��ʼ���������ֵ�
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
    /// �ڱ�������ItemDetails
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
    /// �Ե���Ʒ�õĵ���
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
    #region ʵ��ISaveable�ӿڵ����з���

    /// <summary>
    /// �Ǽǿɴ洢������
    /// </summary>
    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);  
    }

    /// <summary>
    /// ע���ɴ洢������
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
