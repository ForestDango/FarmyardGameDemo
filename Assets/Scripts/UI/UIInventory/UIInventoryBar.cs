using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ص�ͬ����Ϸ�����ϣ�����12��UIInventorySlot
/// </summary>
public class UIInventoryBar : MonoBehaviour
{
    [SerializeField] private Sprite blank16Sprite = null;
    [SerializeField] private UIInventorySlot[] inventorySlots = null;
    public GameObject inventoryBarDragItem; //�϶���ʱ�����ɵ�DragItem
    [HideInInspector] public GameObject inventoryTextBoxGameObject;

    private RectTransform rectTransform;

    private bool isInventoryBarPositionBottom = true;  //�жϱ������Ƿ�����Ļ�·�

    /// <summary>
    /// �ж�InventoryBar�Ƿ�����Ļ�·�
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
        EventHandler.InventoryUpdateEvent += InventoryUpdate; //�Ǽ�InventoryUpdateEvent
    }

    private void OnDisable()
    {
        EventHandler.InventoryUpdateEvent -= InventoryUpdate;
    }

    /// <summary>
    /// ��������Ϣ�������µ�ʱ�򣬾�����������������Ʒ
    /// </summary>
    /// <param name="inventoryLocation"></param>
    /// <param name="inventoryList"></param>
    private void InventoryUpdate(InventoryLocation inventoryLocation, List<InventoryItem> inventoryList)
    {
        if(inventoryLocation == InventoryLocation.player)
        {
            CLearInventorySlots(); //�����InventroyBar�������е�InvnetorSlot
            if(inventoryList.Count >0 && inventorySlots.Length > 0) //�ٱ���һ��inventoryList��inventorySlots������ӻ�bar��
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
    /// �����ǰ��ק����Ʒ
    /// </summary>
    public void ClearCurrentlyDraggedItem()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].ClearSelectedItem();
        }
    }

    /// <summary>
    /// ���ٵ�ǰ��ק����Ʒ���ڽ�����ק�ĵķ�������
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
    /// ������б������ӵ���Ʒ
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
    /// �л�InventoryBar����Ļ�е�λ��
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
    #region ѡ��߹� Inventory Highlight
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
