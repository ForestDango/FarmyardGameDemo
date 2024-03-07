using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System;

/// <summary>
/// 背包格子，挂载给同名的预制体，同时继承事件系统的拖拽和点击接口
/// </summary>
public class UIInventorySlot : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
{
    private Canvas parentCanvas; //父Canvas
    private Camera mainCamera;  //主摄像机
    public GameObject draggedItem; //拖拽的Item
    private GridCursor gridCursor; //Grid的指针
    private Cursor cursor; //普通的指针
    private Transform itemParent; //父Item

    [SerializeField] private float TextboxDistance = 25f; //格子的间距
    public Image inventorySlotHighlight;
    public Image inventorySlotImage;
    public TextMeshProUGUI textMeshProUGUI;

    [SerializeField] private UIInventoryBar inventoryBar = null;
    [HideInInspector] public ItemDetails itemDetails;
    [SerializeField] private GameObject InventoryTextboxPrefab = null;
    [SerializeField] private GameObject itemPrefab = null;
    [HideInInspector] public int itemQuantity;
    [SerializeField] private int slotNumber = 0;
    [HideInInspector] public bool isSelected = false;

    private void Awake()
    {
        parentCanvas = transform.GetComponentInParent<Canvas>();
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SceneLoaded; //登记AfterSceneLoadEvent事件
        EventHandler.DropSelectedItemEvent += DropSelectedItemMousePosition; //登记DropSelectedItemEvent事件
        EventHandler.RemoveSelectedItemFromInventoryEvent += RemoveSelectedItemFormInventory; //登记RemoveSelectedItemFromInventoryEvent事件
    }

   

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SceneLoaded;
        EventHandler.DropSelectedItemEvent -= DropSelectedItemMousePosition;
        EventHandler.RemoveSelectedItemFromInventoryEvent -= RemoveSelectedItemFormInventory;
    }

    //将选择的物品移除背包
    private void RemoveSelectedItemFormInventory()
    {
        if(itemDetails != null && isSelected)
        {
            int itemCode = itemDetails.itemCode;

            InventoryManager.Instance.RemoveItem(InventoryLocation.player, itemCode);

            if(InventoryManager.Instance.FindItemInventory(InventoryLocation.player,itemCode) == -1)
            {
                ClearSelectedItem();
            }
        }
    }

    /// <summary>
    /// 场景加载完成后
    /// </summary>
    public void SceneLoaded()
    {
        itemParent = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        gridCursor = FindObjectOfType<GridCursor>();
        cursor = FindObjectOfType<Cursor>();
    }

    /// <summary>
    /// 将指针设置为空
    /// </summary>
    private void ClearCursor()
    {
        cursor.DisableCursor();

        cursor.SelectedItemType = ItemType.None;

        gridCursor.DisableCursor();
        
        gridCursor.SelectedItemType = ItemType.None;
    }
    #region  IDrag接口
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(itemDetails != null)
        {
            PlayerController.Instance.DisablePlayerInputAndResetMovement();

            draggedItem = Instantiate(inventoryBar.inventoryBarDragItem, inventoryBar.transform);

            Image draggedItemImage = draggedItem.GetComponentInChildren<Image>();
            draggedItemImage.sprite = inventorySlotImage.sprite;
            Debug.Log("draggedItemImage.sprite" + draggedItemImage.sprite);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(draggedItem != null)
        {
            draggedItem.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(draggedItem != null)
        {
            Destroy(draggedItem);

            if(eventData.pointerCurrentRaycast.gameObject != null && eventData.pointerCurrentRaycast.gameObject.GetComponent<UIInventorySlot>() != null)
            {
                int toSlotNumber = eventData.pointerCurrentRaycast.gameObject.GetComponent<UIInventorySlot>().slotNumber;

                InventoryManager.Instance.SwapInventoryItems(InventoryLocation.player,slotNumber,toSlotNumber);

                DestoryInventoryTextbox();

                ClearSelectedItem();
            }
            else
            {
                if (itemDetails.canBeDropped)
                {
                    DropSelectedItemMousePosition();
                }
            }

            PlayerController.Instance.EnablePlayerInput();
        }
    }
    #endregion
    private void DropSelectedItemMousePosition()
    {
        if(itemDetails != null && isSelected)
        {
            if (gridCursor.CursorPositionIsValid)
            {
                Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));

                GameObject itemGameobject = Instantiate(itemPrefab, new Vector3(worldPosition.x , worldPosition.y, worldPosition.z), Quaternion.identity, itemParent);
                Item item = itemGameobject.GetComponent<Item>();
                item.ItemCode = itemDetails.itemCode;

                InventoryManager.Instance.RemoveItem(InventoryLocation.player, item.ItemCode);

                if (InventoryManager.Instance.FindItemInventory(InventoryLocation.player, item.ItemCode) == -1)
                {
                    ClearSelectedItem();
                }
            }
        }
    }
    #region IPointer接口
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemQuantity != 0)
        {
            inventoryBar.inventoryTextBoxGameObject = Instantiate(InventoryTextboxPrefab, transform.position, Quaternion.identity);
            inventoryBar.inventoryTextBoxGameObject.transform.SetParent(parentCanvas.transform, false);

            UIInventoryTextBox inventoryTextBox = inventoryBar.inventoryTextBoxGameObject.GetComponent<UIInventoryTextBox>();

            string itemTypeDescrption = InventoryManager.Instance.GetItemTypeDesciption(itemDetails.itemType);

            inventoryTextBox.SetTextboxText(itemDetails.itemDescription, itemTypeDescrption, "", itemDetails.itemDescription, "", "");

            if (inventoryBar.IsInventoryBarPositionBottom)
            {
                inventoryBar.inventoryTextBoxGameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);
                inventoryBar.inventoryTextBoxGameObject.transform.position = new Vector3(transform.position.x, transform.position.y + TextboxDistance, transform.position.z);
            }
            else
            {
                inventoryBar.inventoryTextBoxGameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
                inventoryBar.inventoryTextBoxGameObject.transform.position = new Vector3(transform.position.x, transform.position.y - TextboxDistance, transform.position.z);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DestoryInventoryTextbox();
    }

    private void DestoryInventoryTextbox()
    {
        if (inventoryBar.inventoryTextBoxGameObject != null)
            Destroy(inventoryBar.inventoryTextBoxGameObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if (isSelected)
            {
                ClearSelectedItem();
            }
            else
            {
                if(itemQuantity > 0)
                {
                    SetSelectedItem();
                }
            }
        }
    }
    #endregion
    private void SetSelectedItem()
    {
        inventoryBar.ClearHighlightOnInventorySlots();

        isSelected = true;

        inventoryBar.SetHightedInventorySlot();

        gridCursor.ItemUseGridRaudius = itemDetails.itemUseGridRadius;
        cursor.ItemUseRaudis = itemDetails.itemUseRadius;

        if(itemDetails.itemUseGridRadius > 0)
        {
            gridCursor.EnableCursor();
        }
        else
        {
            gridCursor.DisableCursor();
        }

        if (itemDetails.itemUseRadius > 0f)
        {
            cursor.EnableCursor();
        }
        else
        {
            cursor.DisableCursor();
        }

        gridCursor.SelectedItemType = itemDetails.itemType;
        cursor.SelectedItemType = itemDetails.itemType;

        InventoryManager.Instance.SetSelectedInventoryItem(InventoryLocation.player,itemDetails.itemCode);

        if (itemDetails.canBeCarried)
        {
            //如果canBeCarryied就覆盖之前的动画
            PlayerController.Instance.ShowCarriedItem(itemDetails.itemCode);
        }
        else
        {
            PlayerController.Instance.ClearCarriedItem();
        }
    }

    public void ClearSelectedItem()
    {
        ClearCursor();

        inventoryBar.ClearHighlightOnInventorySlots();

        isSelected = false;

        InventoryManager.Instance.ClearSelectedInventoryItem(InventoryLocation.player);

        PlayerController.Instance.ClearCarriedItem();
    }
}
