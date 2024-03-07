using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 带有Grid的指针，挂载到UIPanel上，控制GridCursor和Cursor
/// </summary>
public class GridCursor : MonoBehaviour
{
    private Canvas canvas; //canvas组件
    private Grid grid; //找到场景中的Grid组件
    private Camera mainCamera; //主摄像机组件

    [SerializeField] private Image cursorImage = null; //Image组件
    [SerializeField] private RectTransform cursorRectTransform = null; //cursor在GUI界面的位置
    [SerializeField] private Sprite greenCursor = null; //绿色指针的精灵图
    [SerializeField] private Sprite redCursor = null; //红色指针的精灵图
    [SerializeField] private SO_CropDetailsList so_CropDetailsList; //CropDetailsList的数据集合
    [SerializeField] private SO_EggDetailsList so_EggDetailsList; // EggDetailsList的数据集合
    #region 属性Properties
    private bool _cursorPositionIsValid = false;

    /// <summary>
    /// 指针是否可用
    /// </summary>
    public bool CursorPositionIsValid
    {
        get =>  _cursorPositionIsValid;
        set => _cursorPositionIsValid = value;
    }

    private int _itemUseGridRadius = 0;

    /// <summary>
    /// Item物品限制使用Grid半径
    /// </summary>
    public int ItemUseGridRaudius
    {
        get => _itemUseGridRadius;
        set => _itemUseGridRadius = value;
    }

    private ItemType _selectedItemType;

    /// <summary>
    /// 选择的物品类型
    /// </summary>
    public ItemType SelectedItemType
    {
        get => _selectedItemType;
        set => _selectedItemType = value;
    }

    private bool _cursorIsEnabled = false;

    /// <summary>
    /// 指针是否可用
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
        EventHandler.AfterSceneLoadEvent += AfterSceneLoaded; //订阅事件AfterSceneLoadEvent
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
    /// 每帧调用，指针的位置以及设置指针可用
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
    /// 将指针设置为可用
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
    /// 指针是否对Tool类型的物品可用
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
                    Debug.Log("返回true");
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
                            Debug.Log("丰收的日期已经到了");
                            if (eggDetails.CanUseToolToHarvsetEggs(itemDetails.itemCode))
                            {
                                Debug.Log("可以收获小鸡了 ");
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
    /// 判断指针是否对Commidity类型可用
    /// </summary>
    /// <param name="gridPropertyDetails"></param>
    /// <returns></returns>
    private bool IsCursorValidToCommidity(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItem;
    }
    /// <summary>
    /// 判断指针是否对Seed类型可用
    /// </summary>
    /// <param name="gridPropertyDetails"></param>
    /// <returns></returns>
    private bool IsCursorValidToSeed(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItem;
    }

    /// <summary>
    /// 判断指针是否对Eggs类型可用
    /// </summary>
    /// <param name="gridPropertyDetails"></param>
    /// <returns></returns>
    private bool IsCursorValidToEggs(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItem;
    }

    /// <summary>
    /// 将指针设置为可用
    /// </summary>
    private void SetCursorToValid()
    {
        cursorImage.sprite = greenCursor;
        CursorPositionIsValid = true;
    }
    /// <summary>
    /// 将指针设置为不可用
    /// </summary>

    private void SetCursorToInValid()
    {
        cursorImage.sprite = redCursor;
        CursorPositionIsValid = false;
    }

    /// <summary>
    /// 得到玩家的Grid位置
    /// </summary>
    /// <returns></returns>
    public Vector3Int GetGridPositionForPlayer()
    {
        return grid.WorldToCell(PlayerController.Instance.transform.position);
    }

    /// <summary>
    /// 得到指针的Grid位置
    /// </summary>
    /// <returns></returns>
    public Vector3Int GetGridPositionForCurosr()
    {
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));

        return grid.WorldToCell(worldPosition);
    }

    /// <summary>
    /// 当场景加载完成后
    /// </summary>
    private void AfterSceneLoaded()
    {
        grid = GameObject.FindObjectOfType<Grid>();
    }

    /// <summary>
    /// 启用指针
    /// </summary>
    public void EnableCursor()
    {
        cursorImage.color = new Color(1f, 1f, 1f, 1f);

        CursorIsEnabled = true;
    }

    /// <summary>
    /// 禁用指针
    /// </summary>
    public void DisableCursor()
    {
        cursorImage.color = Color.clear;

        CursorIsEnabled = false;
    }

}
