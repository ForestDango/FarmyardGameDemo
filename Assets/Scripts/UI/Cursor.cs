using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��ָͨ�룬���ص�UIPanel�ϣ�����GridCursor��Cursor
/// </summary>
public class Cursor : MonoBehaviour
{
    private Canvas canvas;
    private Camera mainCamera;

    [SerializeField] private Image cursorImage = null; //CursorͼƬ
    [SerializeField] private RectTransform cursorRectTransform = null; //cursor��GUI�����λ��
    [SerializeField] private Sprite greenCursorSprite = null; //��ɫָ��ľ���ͼ
    [SerializeField] private Sprite transparentCursorSprite = null; //��ɫָ��ľ���ͼ
    [SerializeField] private GridCursor gridCursor = null; //��һ��ָ���ͼƬ

    #region ����Properties
    private bool _cursorIsEnabled;

    /// <summary>
    /// ���ԣ�ָ���Ƿ�������״̬
    /// </summary>
    public bool CursorIsEnabled
    {
        get => _cursorIsEnabled;
        set => _cursorIsEnabled = value;
    }

    private bool _cursorPositionIsValid;

    /// <summary>
    /// ���ԣ�ָ�����ڵ�λ���Ƿ����
    /// </summary>
    public bool CursorPositionIsValid
    {
        get => _cursorPositionIsValid;
        set => _cursorPositionIsValid = value;

    }

    private ItemType _selectedItemType;

    /// <summary>
    /// ���ԣ�ѡ�����Ʒ����
    /// </summary>
    public ItemType SelectedItemType
    {
        get => _selectedItemType;
        set => _selectedItemType = value;
    }


    private float _itemUseRadius = 0f;

    /// <summary>
    /// ���ԣ�Itemʹ�����Ƶİ뾶
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
    /// ÿ֡�����ã�����ָ���Ƿ�����Լ�ָ�����GUI��λ��
    /// </summary>
    private void DisplayCursor()
    {
        Vector3 cursorWorldPosition = GetWolrdPositionForCursor();

        SetCursorValidity(cursorWorldPosition, PlayerController.Instance.GetPlayerCenterPosition());

        cursorRectTransform.position = GetRectTransformPositionForCursor();
    }

    /// <summary>
    /// �ж�ָ���Ƿ�Ϊ����״̬
    /// </summary>
    /// <param name="cursorPosition"></param>
    /// <param name="playerPosition"></param>
    private void SetCursorValidity(Vector3 cursorPosition, Vector3 playerPosition)
    {
        SetCursorToValid(); //������Ϊ����

        if(cursorPosition.x > (playerPosition.x + ItemUseRaudis /2f) && cursorPosition.y > (playerPosition.y + ItemUseRaudis /2f) ||
            cursorPosition.x < (playerPosition.x - ItemUseRaudis / 2f) && cursorPosition.y > (playerPosition.y + ItemUseRaudis / 2f) ||
            cursorPosition.x < (playerPosition.x - ItemUseRaudis / 2f) && cursorPosition.y < (playerPosition.y - ItemUseRaudis / 2f) ||
            cursorPosition.x > (playerPosition.x + ItemUseRaudis / 2f) && cursorPosition.y < (playerPosition.y - ItemUseRaudis / 2f)
            )
        {
            SetCursorToInvalid();
            return; 
        }

        // �������֮һ������ָ������Ϊ������
        if(Mathf.Abs(cursorPosition.x - playerPosition.x) > ItemUseRaudis ||
            Mathf.Abs(cursorPosition.y - playerPosition.y) > ItemUseRaudis)
        {
            SetCursorToInvalid();
            return;
        }

        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);

        //����Ҳ��������е�itemDetails
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
    /// ����ʹ��ReapingTool��ָ�룬�ж���ײ���������Ƿ��ж�Ӧ��temList�Լ���ЩitemList���Ƿ�����ΪReapable_Scenery
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
    #region ��������public Methods
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
