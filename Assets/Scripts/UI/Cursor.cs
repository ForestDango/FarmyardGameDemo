using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 普通指针，挂载到UIPanel上，控制GridCursor和Cursor
/// </summary>
public class Cursor : MonoBehaviour
{
    private Canvas canvas;
    private Camera mainCamera;

    [SerializeField] private Image cursorImage = null; //Cursor图片
    [SerializeField] private RectTransform cursorRectTransform = null; //cursor在GUI界面的位置
    [SerializeField] private Sprite greenCursorSprite = null; //绿色指针的精灵图
    [SerializeField] private Sprite transparentCursorSprite = null; //红色指针的精灵图
    [SerializeField] private GridCursor gridCursor = null; //另一个指针的图片

    #region 属性Properties
    private bool _cursorIsEnabled;

    /// <summary>
    /// 属性，指针是否是启用状态
    /// </summary>
    public bool CursorIsEnabled
    {
        get => _cursorIsEnabled;
        set => _cursorIsEnabled = value;
    }

    private bool _cursorPositionIsValid;

    /// <summary>
    /// 属性，指针所在的位置是否可用
    /// </summary>
    public bool CursorPositionIsValid
    {
        get => _cursorPositionIsValid;
        set => _cursorPositionIsValid = value;

    }

    private ItemType _selectedItemType;

    /// <summary>
    /// 属性，选择的物品类型
    /// </summary>
    public ItemType SelectedItemType
    {
        get => _selectedItemType;
        set => _selectedItemType = value;
    }


    private float _itemUseRadius = 0f;

    /// <summary>
    /// 属性，Item使用限制的半径
    /// </summary>
    public float ItemUseRaudis
    {
        get => _itemUseRadius;
        set => _itemUseRadius = value;
    }
    #endregion

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (CursorIsEnabled)
        {
            DisplayCursor();
        }
    }
    /// <summary>
    /// 每帧都调用，设置指针是否可用以及指针的在GUI的位置
    /// </summary>
    private void DisplayCursor()
    {
        Vector3 cursorWorldPosition = GetWolrdPositionForCursor();

        SetCursorValidity(cursorWorldPosition, PlayerController.Instance.GetPlayerCenterPosition());

        cursorRectTransform.position = GetRectTransformPositionForCursor();
    }

    /// <summary>
    /// 判断指针是否为可用状态
    /// </summary>
    /// <param name="cursorPosition"></param>
    /// <param name="playerPosition"></param>
    private void SetCursorValidity(Vector3 cursorPosition, Vector3 playerPosition)
    {
        SetCursorToValid(); //先设置为可用

        if(cursorPosition.x > (playerPosition.x + ItemUseRaudis /2f) && cursorPosition.y > (playerPosition.y + ItemUseRaudis /2f) ||
            cursorPosition.x < (playerPosition.x - ItemUseRaudis / 2f) && cursorPosition.y > (playerPosition.y + ItemUseRaudis / 2f) ||
            cursorPosition.x < (playerPosition.x - ItemUseRaudis / 2f) && cursorPosition.y < (playerPosition.y - ItemUseRaudis / 2f) ||
            cursorPosition.x > (playerPosition.x + ItemUseRaudis / 2f) && cursorPosition.y < (playerPosition.y - ItemUseRaudis / 2f)
            )
        {
            SetCursorToInvalid();
            return; 
        }

        // 如果其中之一大于则将指针设置为不可用
        if(Mathf.Abs(cursorPosition.x - playerPosition.x) > ItemUseRaudis ||
            Mathf.Abs(cursorPosition.y - playerPosition.y) > ItemUseRaudis)
        {
            SetCursorToInvalid();
            return;
        }

        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);

        //如果找不到背包中的itemDetails
        if(itemDetails == null)
        {
            SetCursorToInvalid();
            return;
        }

        switch (itemDetails.itemType)
        {
            case ItemType.Watering_tool:
            case ItemType.Breaking_tool:
            case ItemType.Chopping_tool:
            case ItemType.Hoeing_tool:
            case ItemType.Reping_tool:
            case ItemType.Collecting_tool:
                if (!SetCursorValidityTool(cursorPosition, playerPosition, itemDetails))
                {
                    SetCursorToInvalid();
                    return;
                }
                break;

            case ItemType.Count:
                break;
            case ItemType.None:
                break;
            default:
                break;
        }
    }

    private bool SetCursorValidityTool(Vector3 cursorPosition, Vector3 playerPosition, ItemDetails itemDetails)
    {
        switch (itemDetails.itemType)
        {
            case ItemType.Reping_tool:
                return SetCursorValidityReapingTool(cursorPosition, playerPosition, itemDetails);
                

            default:
                return false;
        }
    }

    /// <summary>
    /// 对于使用ReapingTool的指针，判断碰撞器方向上是否有对应的temList以及这些itemList上是否类型为Reapable_Scenery
    /// </summary>
    /// <param name="cursorPosition"></param>
    /// <param name="playerPosition"></param>
    /// <param name="itemDetails"></param>
    /// <returns></returns>

    private bool SetCursorValidityReapingTool(Vector3 cursorPosition, Vector3 playerPosition, ItemDetails itemDetails)
    {
        List<Item> itemList = new List<Item>();

        if(HelperMethod.GetComponenetAtCursorLocation<Item>(out itemList, cursorPosition))
        {
            if(itemList.Count != 0)
            {
                foreach (Item item in itemList)
                {
                    if(InventoryManager.Instance.GetItemDetails(item.ItemCode).itemType == ItemType.Reapable_scenery)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private void SetCursorToValid()
    {
        cursorImage.sprite = greenCursorSprite;
        CursorPositionIsValid = true;

        gridCursor.DisableCursor();
    }

    private void SetCursorToInvalid()
    {
        cursorImage.sprite = transparentCursorSprite;
        CursorPositionIsValid = false;

        gridCursor.EnableCursor();
    }
    #region 公共方法public Methods
    private Vector2 GetRectTransformPositionForCursor()
    {
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        return RectTransformUtility.PixelAdjustPoint(screenPosition, cursorRectTransform, canvas);
    }

    public Vector3 GetWolrdPositionForCursor()
    {
        Vector3 screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);

        return worldPosition;
    }

    public void DisableCursor()
    {
        cursorImage.color = new Color(1f, 1f, 1f, 0f);
        CursorIsEnabled = false;

    }

    public void EnableCursor()
    {
        cursorImage.color = new Color(1f, 1f, 1f, 1f);
        CursorIsEnabled = true;
    }
    #endregion
}
