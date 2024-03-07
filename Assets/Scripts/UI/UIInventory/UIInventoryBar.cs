using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 挂载到同名游戏对象上，管理12个UIInventorySlot
/// </summary>
public class UIInventoryBar : MonoBehaviour
{
    [SerializeField] private Sprite blank16Sprite = null;
    [SerializeField] private UIInventorySlot[] inventorySlots = null;
    public GameObject inventoryBarDragItem; //拖动的时候生成的DragItem
    [HideInInspector] public GameObject inventoryTextBoxGameObject;

    private RectTransform rectTransform;

    private bool isInventoryBarPositionBottom = true;  //判断背包条是否在屏幕下方

    /// <summary>
    /// 判断InventoryBar是否在屏幕下方
    /// </summary>
    public bool IsInventoryBarPositionBottom
    {
        get
        {
            return isInventoryBarPositionBottom;
        }
        set
        {
            isInventoryBarPositionBottom = value;
        }
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        SwitchInventoryBarPosition();
    }

    private void OnEnable()
    {
        EventHandler.InventoryUpdateEvent += InventoryUpdate; //登记InventoryUpdateEvent
    }

    private void OnDisable()
    {
        EventHandler.InventoryUpdateEvent -= InventoryUpdate;
    }

    /// <summary>
    /// 当背包信息发生更新的时候，就先清空再重新添加物品
    /// </summary>
    /// <param name="inventoryLocation"></param>
    /// <param name="inventoryList"></param>
    private void InventoryUpdate(InventoryLocation inventoryLocation, List<InventoryItem> inventoryList)
    {
        if(inventoryLocation == InventoryLocation.player)
        {
            CLearInventorySlots(); //先清空InventroyBar里面所有的InvnetorSlot
            if(inventoryList.Count >0 && inventorySlots.Length > 0) //再遍历一遍inventoryList和inventorySlots重新添加回bar里
            {
                for (int i = 0; i < inventorySlots.Length; i++)
                {
                    if (i < inventoryList.Count)
                    {
                        int itemCode = inventoryList[i].itemCode;

                        ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);

                        if (itemDetails != null)
                        {
                            inventorySlots[i].inventorySlotImage.sprite = itemDetails.itemSprite;
                            inventorySlots[i].textMeshProUGUI.text = inventoryList[i].itemQuantity.ToString();
                            inventorySlots[i].itemDetails = itemDetails;
                            inventorySlots[i].itemQuantity = inventoryList[i].itemQuantity;
                            SetHighlightedInvenotrySlot(i);
                        }
                    }
                    else
                    {
                        break;
                    }

                }
            }
        }
    }

    /// <summary>
    /// 清除当前拖拽的物品
    /// </summary>
    public void ClearCurrentlyDraggedItem()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].ClearSelectedItem();
        }
    }

    /// <summary>
    /// 销毁当前拖拽的物品，在结束拖拽的的方法调用
    /// </summary>
    public void DesotryCurrentlyDraggedItem()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if(inventorySlots[i].draggedItem != null)
            {
                Destroy(inventorySlots[i].draggedItem);
            }
        }
    }
    /// <summary>
    /// 清除所有背包格子的物品
    /// </summary>

    private void CLearInventorySlots()
    {
        if(inventorySlots.Length > 0)
        {
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                inventorySlots[i].inventorySlotImage.sprite = blank16Sprite;
                inventorySlots[i].textMeshProUGUI.text = "";
                inventorySlots[i].itemDetails = null;
                inventorySlots[i].itemQuantity = 0;
                SetHighlightedInvenotrySlot(i);
            }
        }
    }

    /// <summary>
    /// 切换InventoryBar在屏幕中的位置
    /// </summary>
    private void SwitchInventoryBarPosition()
    {
        Vector3 playerViewportPosition = PlayerController.Instance.GetPlayerVierportPosition();

        if (playerViewportPosition.y > 0.3f && !IsInventoryBarPositionBottom)
        {
            rectTransform.pivot = new Vector2(0.5f, 0f);
            rectTransform.anchorMin = new Vector2(0.5f, 0f);
            rectTransform.anchorMax = new Vector2(0.5f, 0f);
            rectTransform.anchoredPosition = new Vector2(0f, 2.5f);

            isInventoryBarPositionBottom = true;
        }

        else if(playerViewportPosition.y <= 0.3f && IsInventoryBarPositionBottom)
        {

            rectTransform.pivot = new Vector2(0.5f, 1f);
            rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rectTransform.anchoredPosition = new Vector2(0f, -2.5f);

            isInventoryBarPositionBottom = false;            
        }
    }
    #region 选择高光 Inventory Highlight
    public void ClearHighlightOnInventorySlots()
    {
        if(inventorySlots.Length > 0)
        {
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (inventorySlots[i].isSelected)
                {
                    inventorySlots[i].isSelected = false;
                    inventorySlots[i].inventorySlotHighlight.color = new Color(0f, 0f, 0f, 0f);
                    //update inventory
                    InventoryManager.Instance.ClearSelectedInventoryItem(InventoryLocation.player);
                }
            }
        }
    }

    public void SetHightedInventorySlot()
    {
       if(inventorySlots.Length > 0)
        {
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                SetHighlightedInvenotrySlot(i);
            }
        }
    }

    private void SetHighlightedInvenotrySlot(int itemPosition)
    {
        if(inventorySlots.Length > 0 && inventorySlots[itemPosition].itemDetails != null)
        {
            if (inventorySlots[itemPosition].isSelected)
            {
                inventorySlots[itemPosition].inventorySlotHighlight.color = new Color(1f, 1f, 1f, 1f);

                InventoryManager.Instance.SetSelectedInventoryItem(InventoryLocation.player, inventorySlots[itemPosition].itemDetails.itemCode);
            }
        }
    }

    #endregion
}
