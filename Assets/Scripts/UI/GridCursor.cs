using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ����Grid��ָ�룬���ص�UIPanel�ϣ�����GridCursor��Cursor
/// </summary>
public class GridCursor : MonoBehaviour
{
    private Canvas canvas; //canvas���
    private Grid grid; //�ҵ������е�Grid���
    private Camera mainCamera; //����������

    [SerializeField] private Image cursorImage = null; //Image���
    [SerializeField] private RectTransform cursorRectTransform = null; //cursor��GUI�����λ��
    [SerializeField] private Sprite greenCursor = null; //��ɫָ��ľ���ͼ
    [SerializeField] private Sprite redCursor = null; //��ɫָ��ľ���ͼ
    [SerializeField] private SO_CropDetailsList so_CropDetailsList; //CropDetailsList�����ݼ���
    [SerializeField] private SO_EggDetailsList so_EggDetailsList; // EggDetailsList�����ݼ���
    #region ����Properties
    private bool _cursorPositionIsValid = false;

    /// <summary>
    /// ָ���Ƿ����
    /// </summary>
    public bool CursorPositionIsValid
    {
        get =>  _cursorPositionIsValid;
        set => _cursorPositionIsValid = value;
    }

    private int _itemUseGridRadius = 0;

    /// <summary>
    /// Item��Ʒ����ʹ��Grid�뾶
    /// </summary>
    public int ItemUseGridRaudius
    {
        get => _itemUseGridRadius;
        set => _itemUseGridRadius = value;
    }

    private ItemType _selectedItemType;

    /// <summary>
    /// ѡ�����Ʒ����
    /// </summary>
    public ItemType SelectedItemType
    {
        get => _selectedItemType;
        set => _selectedItemType = value;
    }

    private bool _cursorIsEnabled = false;

    /// <summary>
    /// ָ���Ƿ����
    /// </summary>
    public bool CursorIsEnabled
    {
        get => _cursorIsEnabled;
        set => _cursorIsEnabled = value;
    }
    #endregion
    private void Start()
    {
        mainCamera = Camera.main;
        canvas = GetComponentInParent<Canvas>();
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += AfterSceneLoaded; //�����¼�AfterSceneLoadEvent
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoaded;
    }

    private void Update()
    {
        if (CursorIsEnabled)
            DisplayCursor();
    }

    /// <summary>
    /// ÿ֡���ã�ָ���λ���Լ�����ָ�����
    /// </summary>
    /// <returns></returns>
    private Vector3Int DisplayCursor()
    {
        if(grid != null)
        {
            Vector3Int gridPosition = GetGridPositionForCurosr();

            Vector3Int playerPosition = GetGridPositionForPlayer();

            SetCursorValidity(gridPosition ,playerPosition);

            cursorRectTransform.position = GetRectTransformForCursor(gridPosition);

            return gridPosition;
        }
        else
        {
            return Vector3Int.zero;
        }
    }

    private Vector3 GetRectTransformForCursor(Vector3Int gridPosition)
    {
        Vector3 gridWorldPostion = grid.CellToWorld(gridPosition);
        Vector2 gridScreenPosition = mainCamera.WorldToScreenPoint(gridWorldPostion);

        return RectTransformUtility.PixelAdjustPoint(gridScreenPosition, cursorRectTransform, canvas);
    }

    /// <summary>
    /// ��ָ������Ϊ����
    /// </summary>
    /// <param name="cursorGridPosition"></param>
    /// <param name="playerGridPosition"></param>
    private void SetCursorValidity(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        SetCursorToValid();

        if(Mathf.Abs(cursorGridPosition.x - playerGridPosition.x) > ItemUseGridRaudius ||
            Mathf.Abs(cursorGridPosition.y - playerGridPosition.y) > ItemUseGridRaudius)
        {
            SetCursorToInValid();
            return;

        }

        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);

        if(itemDetails == null)
        {
            SetCursorToInValid();
            return;
        }

        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropetyDetails(cursorGridPosition.x, cursorGridPosition.y);

        if(gridPropertyDetails != null)
        {
            switch (itemDetails.itemType)
            {
                case ItemType.Seed:
                    if (!IsCursorValidToSeed(gridPropertyDetails))
                    {
                        SetCursorToInValid();
                        return;
                    }
                    break;

                case ItemType.Commodity:
                    if (!IsCursorValidToCommidity(gridPropertyDetails))
                    {
                        SetCursorToInValid();
                        return;
                    }
                    break;

                case ItemType.Eggs:
                    if (!IsCursorValidToEggs(gridPropertyDetails))
                    {
                        SetCursorToInValid();
                        return;
                    }
                    break;

                case ItemType.Watering_tool:
                case ItemType.Breaking_tool:
                case ItemType.Chopping_tool:
                case ItemType.Hoeing_tool:
                case ItemType.Reping_tool:
                case ItemType.Collecting_tool:
                    if (!IsCursorValidForTool(gridPropertyDetails,itemDetails))
                    {
                        SetCursorToInValid();
                        return;
                    }
                    break;


                case ItemType.None:
                    break;

                case ItemType.Count:
                    break;

                default:
                    break;

            }
        }
        else
        {
            SetCursorToInValid();
            return;
        }

    }

    /// <summary>
    /// ָ���Ƿ��Tool���͵���Ʒ����
    /// </summary>
    /// <param name="gridPropertyDetails"></param>
    /// <param name="itemDetails"></param>
    /// <returns></returns>
    private bool IsCursorValidForTool(GridPropertyDetails gridPropertyDetails,ItemDetails itemDetails)
    {
        switch (itemDetails.itemType)
        {
            case ItemType.Hoeing_tool:
                if(gridPropertyDetails.isDiggable && gridPropertyDetails.daysSinceDug == -1)
                {
                    Vector3 cursorWorldPostiin = new Vector3(GetWorldPositionForCursor().x + 1f, GetWorldPositionForCursor().y +1f, 0f);

                    List<Item> itemList = new List<Item>();

                    HelperMethod.GetComponenetsAtBoxLocation<Item>(out itemList,cursorWorldPostiin,Settings.cursorSize,0f);

                    bool foundRepable = false;

                    foreach (Item item in itemList)
                    {
                        if(InventoryManager.Instance.GetItemDetails(item.ItemCode).itemType == ItemType.Reapable_scenery)
                        {
                            foundRepable = true;
                            break;
                        }
                    }

                    if(foundRepable)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }

            case ItemType.Watering_tool:
                if(gridPropertyDetails.daysSinceDug > -1 && gridPropertyDetails.daysSinceWatered == -1)
                {
                    Debug.Log("����true");
                    return true;
                }
                else
                {
                    return false;
                }

            case ItemType.Breaking_tool:
            case ItemType.Chopping_tool:
            case ItemType.Collecting_tool:

                if(gridPropertyDetails.seedItemCode != -1)
                {
                    CropDetails cropDetails = so_CropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);

                    if(cropDetails != null)
                    {
                        if(gridPropertyDetails.growthDays >= cropDetails.growthDays[cropDetails.growthDays.Length - 1])
                        {
                            if (cropDetails.CanUseToolToHarvestCrop(itemDetails.itemCode))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                else if(gridPropertyDetails.eggItemCode != -1)
                {
                    EggDetails eggDetails = so_EggDetailsList.GetEggDetails(gridPropertyDetails.seedItemCode);

                    if (eggDetails != null)
                    {
                        Debug.Log("eggDetails != null");
                        if (gridPropertyDetails.growthDays >= eggDetails.growthDays[eggDetails.growthDays.Length - 1])
                        {
                            Debug.Log("���յ������Ѿ�����");
                            if (eggDetails.CanUseToolToHarvsetEggs(itemDetails.itemCode))
                            {
                                Debug.Log("�����ջ�С���� ");
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }


                return false;

            default:
                return false;
        }
    }

    private Vector3 GetWorldPositionForCursor()
    {
        return grid.CellToWorld(GetGridPositionForCurosr());
    }
    /// <summary>
    /// �ж�ָ���Ƿ��Commidity���Ϳ���
    /// </summary>
    /// <param name="gridPropertyDetails"></param>
    /// <returns></returns>
    private bool IsCursorValidToCommidity(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItem;
    }
    /// <summary>
    /// �ж�ָ���Ƿ��Seed���Ϳ���
    /// </summary>
    /// <param name="gridPropertyDetails"></param>
    /// <returns></returns>
    private bool IsCursorValidToSeed(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItem;
    }

    /// <summary>
    /// �ж�ָ���Ƿ��Eggs���Ϳ���
    /// </summary>
    /// <param name="gridPropertyDetails"></param>
    /// <returns></returns>
    private bool IsCursorValidToEggs(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItem;
    }

    /// <summary>
    /// ��ָ������Ϊ����
    /// </summary>
    private void SetCursorToValid()
    {
        cursorImage.sprite = greenCursor;
        CursorPositionIsValid = true;
    }
    /// <summary>
    /// ��ָ������Ϊ������
    /// </summary>

    private void SetCursorToInValid()
    {
        cursorImage.sprite = redCursor;
        CursorPositionIsValid = false;
    }

    /// <summary>
    /// �õ���ҵ�Gridλ��
    /// </summary>
    /// <returns></returns>
    public Vector3Int GetGridPositionForPlayer()
    {
        return grid.WorldToCell(PlayerController.Instance.transform.position);
    }

    /// <summary>
    /// �õ�ָ���Gridλ��
    /// </summary>
    /// <returns></returns>
    public Vector3Int GetGridPositionForCurosr()
    {
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));

        return grid.WorldToCell(worldPosition);
    }

    /// <summary>
    /// ������������ɺ�
    /// </summary>
    private void AfterSceneLoaded()
    {
        grid = GameObject.FindObjectOfType<Grid>();
    }

    /// <summary>
    /// ����ָ��
    /// </summary>
    public void EnableCursor()
    {
        cursorImage.color = new Color(1f, 1f, 1f, 1f);

        CursorIsEnabled = true;
    }

    /// <summary>
    /// ����ָ��
    /// </summary>
    public void DisableCursor()
    {
        cursorImage.color = Color.clear;

        CursorIsEnabled = false;
    }

}
