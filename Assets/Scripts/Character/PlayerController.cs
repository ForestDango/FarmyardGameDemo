using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

/// <summary>
/// ��ҿ��ƽű�
/// </summary>
[RequireComponent(typeof(GenerateGUID))] //��Ҫ���GenerateGUID
public class PlayerController : Singleton<PlayerController>,ISaveable
{
    [SerializeField] private float movementBuffer = 0.71f; //ͬʱ�ƶ��ٶȻ���

    [SerializeField] private float currentSpeed;

    private bool playerToolDisabled = false;

    #region �ⲿ�����ű�
    private GridCursor gridCursor;

    private Cursor cursor;

    private Camera mainCamera;

    private Rigidbody2D rigi2D;

#pragma warning disable 414
    private Direction playerDirection;
#pragma warning restore 414

    #endregion

    #region �������ؽű�
    private AnimationOverrides animationOverrides;
    private List<CharacterAttribute> characterAttributesCustomisatetionList;

    [Tooltip("Should be populated in the prefab with the equipped item sprite renderer")]
    [SerializeField] private SpriteRenderer equippedItemSpriteRenderer = null;

    private CharacterAttribute armCharacterAttribute;
    private CharacterAttribute toolCharacterAttribute;
    #endregion



    #region �������XY�������
    private float xInput;
    private float yInput;

    #endregion 

    #region ��Ҷ�������ֵ�����Լ�ToolEffect
    private bool isCarrying = false;
    private bool isIdle;
    private bool isRunning;
    private bool isWalking;

    private bool isLiftingToolRight;
    private bool isLiftingToolLeft;
    private bool isLiftingToolUp;
    private bool isLiftingToolDown;

    private bool isUsingToolRight;
    private bool isUsingToolLeft;
    private bool isUsingToolUp;
    private bool isUsingToolDown;

    private bool isSwingToolRight;
    private bool isSwingToolLeft;
    private bool isSwingToolUp;
    private bool isSwingToolDown;

    private bool isPickingRight;
    private bool isPickingLeft;
    private bool isPickingUp;
    private bool isPickingDown;

    ToolEffect toolEffect = ToolEffect.none;
    #endregion

    #region ����


    private bool _playerInputDisabled = false;

    /// <summary>
    /// ��ֹ�����������
    /// </summary>
    public bool PlayerInputDisabled 
    {
        get => _playerInputDisabled;
        set => _playerInputDisabled = value;
    }

    public string _iSaveableUniqueID;

    /// <summary>
    /// ��Ϸ����Ķ�һ�޶�ID(GUID)
    /// </summary>
    public string ISaveableUniqueID { get => _iSaveableUniqueID; set => _iSaveableUniqueID = value; }

    public GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get => _gameObjectSave; set => _gameObjectSave = value; }

    #endregion

    #region ������Iemeurator��WaitForSeconds
    private WaitForSeconds afterUseToolAnimationPause;
    private WaitForSeconds useToolAnimationPause;
    private WaitForSeconds afterLiftToolAnimationPause;
    private WaitForSeconds liftToolAnimationPause;
    private WaitForSeconds pickAnimationPause;
    private WaitForSeconds afterPickAnimationpPause;
    #endregion

    protected override void Awake()
    {
        base.Awake();
        mainCamera = Camera.main;
        animationOverrides = GetComponentInChildren<AnimationOverrides>();
        armCharacterAttribute = new CharacterAttribute(CharacterPartAnimator.arms, ParVariantColor.none, ParVariantType.none);
        toolCharacterAttribute = new CharacterAttribute(CharacterPartAnimator.tool, ParVariantColor.none, ParVariantType.hoe);
        characterAttributesCustomisatetionList = new List<CharacterAttribute>();
        rigi2D = GetComponent<Rigidbody2D>();

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void OnEnable()
    {
        ISaveableRegister();
        EventHandler.BeforeSceneUnloadFadeOutEvent += DisablePlayerInputAndResetMovement;
        EventHandler.AfterSceneLoadFadeInEvent += EnablePlayerInput;
    }

    private void OnDisable()
    {
        ISaveableDeregister();
        EventHandler.BeforeSceneUnloadFadeOutEvent -= DisablePlayerInputAndResetMovement;
        EventHandler.AfterSceneLoadFadeInEvent -= EnablePlayerInput;
    }

    private void Start()
    {
        gridCursor = FindObjectOfType<GridCursor>();
        cursor = FindObjectOfType<Cursor>();
        useToolAnimationPause = new WaitForSeconds(Settings.useToolAnimationPause);
        afterUseToolAnimationPause = new WaitForSeconds(Settings.afterUseToolAnimationPause);
        liftToolAnimationPause = new WaitForSeconds(Settings.liftToolAnimationPause);
        afterLiftToolAnimationPause = new WaitForSeconds(Settings.afterLiftToolAnimationPause);
        pickAnimationPause = new WaitForSeconds(Settings.pickAnimationPause);
        afterPickAnimationpPause = new WaitForSeconds(Settings.afterPickAnimationPause);

    }

    private void Update()
    {
        if (PlayerInputDisabled)
            return;

        ResetAnimationParameter();
        PlayerMovementInput();
        PlayerWalkSwitchInput();

        PlayerClickInput();

        PlayerTestInput();

        //�����¼����ĺ���CallMovenmentEvent
        EventHandler.CallMovementEvent(xInput, yInput, isWalking, isRunning, isIdle, isCarrying,
     toolEffect,
     isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
     isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
     isSwingToolRight, isSwingToolLeft, isSwingToolUp, isSwingToolDown,
     isPickingRight, isPickingLeft, isPickingUp, isPickingDown,
     false, false, false, false);
    }

    private void FixedUpdate()
    {
        PlayerMovement();
    }

    private void PlayerTestInput()
    {
        if (Input.GetKey(KeyCode.T))
        {
            TimeManager.Instance.TestAdvanceGameMinute();
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            TimeManager.Instance.TestAdvanceGameDay();
        }
    }

    private void PlayerMovement()
    {
        Vector2 move = new Vector2(xInput * currentSpeed * Time.deltaTime, yInput * currentSpeed * Time.deltaTime);
        rigi2D.MovePosition(rigi2D.position + move);
    }

    /// <summary>
    /// �������������ϵİ��������Լ��ƶ���ص�bool�����ж�
    /// </summary>
    private void PlayerMovementInput()
    {
        if (_playerInputDisabled)
            return;

        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        //��ֹxy�����ϰ������쵼��������ٵ���
        if (xInput != 0 && yInput != 0)
        {
            xInput *= movementBuffer;
            yInput *= movementBuffer;
        }

        if (xInput != 0 || yInput != 0)
        {
            isWalking = false;
            isRunning = true;
            isIdle = false;
            currentSpeed = Settings.runSpeed;

            if (xInput < 0)
                playerDirection = Direction.left;
            else if (xInput > 0)
                playerDirection = Direction.right;
            else if (yInput > 0)
                playerDirection = Direction.up;
            else if (yInput < 0)
                playerDirection = Direction.down;

        }

        else if(xInput == 0 && yInput == 0)
        {
            isWalking = false;
            isRunning = false;
            isIdle = true;

        }       
    }

    /// <summary>
    /// ͨ��Shift���л�bool
    /// </summary>
    private void PlayerWalkSwitchInput()
    {
        if (_playerInputDisabled)
            return;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            isRunning = false;
            isWalking = true;
            isIdle = false;
            currentSpeed = Settings.walkSpeed;
        }

        else if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            isRunning = true;
            isWalking = false;
            isIdle = false;
            currentSpeed = Settings.runSpeed;
        }
    }

    /// <summary>
    /// �����������������ƶ����������ڳ����տ�ʼ����֮ǰ
    /// </summary>
    public void DisablePlayerInputAndResetMovement()
    {
        DisablePlayerInput();
        ResetMovement();

        EventHandler.CallMovementEvent(xInput, yInput, isWalking, isRunning, isIdle, isCarrying,
     toolEffect,
     isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
     isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
     isSwingToolRight, isSwingToolLeft, isSwingToolUp, isSwingToolDown,
     isPickingRight, isPickingLeft, isPickingUp, isPickingDown,
     false, false, false, false);
    }

    /// <summary>
    /// �����ƶ�����
    /// </summary>
    private void ResetMovement()
    {
        xInput = 0;
        yInput = 0;
        isRunning = false;
        isWalking = false;
        isIdle = true;
    }

    /// <summary>
    /// ��ÿ֡�����ö�������
    /// </summary>
    private void ResetAnimationParameter()
    {

        isCarrying = false;
        isIdle = false;
        isRunning = false;
        isWalking = false;

        isLiftingToolRight = false;
        isLiftingToolLeft = false;
        isLiftingToolUp = false;
        isLiftingToolDown = false;

        isUsingToolRight = false;
        isUsingToolLeft = false;
        isUsingToolUp = false;
        isUsingToolDown = false;

        isSwingToolRight = false;
        isSwingToolLeft = false;
        isSwingToolUp = false;
        isSwingToolDown = false;

        isPickingRight = false;
        isPickingLeft = false;
        isPickingUp = false;
        isPickingDown = false;

        toolEffect = ToolEffect.none;
    }
    
    /// <summary>
    /// ��ҵ�����룬��Update�����е���
    /// </summary>
    private void PlayerClickInput()
    {
        if (!playerToolDisabled)
        {
            if (Input.GetMouseButtonDown(0)) //������������
            {

                if (gridCursor.CursorIsEnabled || cursor.CursorIsEnabled)
                {
                    Vector3Int cursorGridPosition = gridCursor.GetGridPositionForCurosr();

                    Vector3Int playerGridPosition = gridCursor.GetGridPositionForPlayer();

                    ProcessPlayerClickInput(cursorGridPosition,playerGridPosition);
                }
            }
        }
    }
    /// <summary>
    /// �������������������ָ��ֻҪ���õ�ʱ��ķ���
    /// ��Update������ÿ֡����
    /// </summary>
    /// <param name="cursorGridPosition"></param>
    /// <param name="playerGridPosition"></param>

    private void ProcessPlayerClickInput(Vector3Int cursorGridPosition,Vector3Int playerGridPosition)
    {
        ResetMovement();

        Vector3Int playerDirection = GetPlayerClickPosition(cursorGridPosition, playerGridPosition);

        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropetyDetails(cursorGridPosition.x, cursorGridPosition.y);

        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);

        if(itemDetails != null)
        {
            switch (itemDetails.itemType)
            {
                case ItemType.Seed:
                    if (Input.GetMouseButtonDown(0))
                    {
                        ProcessPlayerClickInputSeed(gridPropertyDetails,itemDetails);
                    }
                    break;
                case ItemType.Commodity:
                    if (Input.GetMouseButtonDown(0))
                    {
                        ProcessPlayerClickInputCommodity(itemDetails);
                    }
                    break;

                case ItemType.Eggs:
                    if (Input.GetMouseButtonDown(0))
                    {
                        ProcessPlayerClickInputEggs(gridPropertyDetails,itemDetails);
                    }
                    break;
                case ItemType.Watering_tool:
                case ItemType.Chopping_tool:
                case ItemType.Hoeing_tool:
                case ItemType.Reping_tool:
                case ItemType.Breaking_tool:
                case ItemType.Collecting_tool:
                    ProcessPlayerClickInputTool(gridPropertyDetails, itemDetails, playerDirection);
                    break;

                case ItemType.None:
                    break;
                case ItemType.Count:
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// ������ҹ��ߵ���¼�
    /// </summary>
    /// <param name="gridPropertyDetails"></param>
    /// <param name="itemDetails"></param>
    /// <param name="playerDirection"></param>
    private void ProcessPlayerClickInputTool(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerDirection)
    {
        switch (itemDetails.itemType)
        {
            case ItemType.Hoeing_tool:
                if (gridCursor.CursorPositionIsValid)
                {
                    HoeGroundAtCursor(gridPropertyDetails, playerDirection);
                }
                break;

            case ItemType.Watering_tool:
                if (gridCursor.CursorPositionIsValid)
                {
                    WaterGroundAtCursor(gridPropertyDetails, playerDirection);
                }
                break;

            case ItemType.Collecting_tool:
                if (gridCursor.CursorPositionIsValid)
                {
                    CollectInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);
                }
                break;

            case ItemType.Reping_tool:
                if (cursor.CursorPositionIsValid)
                {
                    playerDirection = GetPlayerDirection(cursor.GetWolrdPositionForCursor(),GetPlayerCenterPosition());
                    ReapInPlayerPositionAtCursor(itemDetails, playerDirection);
                    
                }
                break;

            case ItemType.Chopping_tool:
                if (gridCursor.CursorPositionIsValid)
                {
                    ChopInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);
                }
                break;

            case ItemType.Breaking_tool:
                if (gridCursor.CursorPositionIsValid)
                {
                    BreakInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);
                }
                break;

            default:
                break;
        }
    }

    private void BreakInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerDirection)
    {
        AudioManager.Instance.PlaySound(SoundName.effectPickaxe);

        StartCoroutine(BreakInPlayerDirectionCoroutine(gridPropertyDetails, itemDetails, playerDirection));
    }

    private void ChopInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerDirection)
    {
        AudioManager.Instance.PlaySound(SoundName.effectAxe);

        StartCoroutine(ChopInPlayerDirectionCoroutine(gridPropertyDetails, itemDetails, playerDirection));
    }

    private void CollectInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerDirection)
    {
        AudioManager.Instance.PlaySound(SoundName.effectBasket);

        StartCoroutine(CollectInPlayerDirectionCoroutine(gridPropertyDetails, itemDetails, playerDirection));
    }

    private void ReapInPlayerPositionAtCursor(ItemDetails itemDetails, Vector3Int playerDirection)
    {
        StartCoroutine(ReapInPlayerPositionAtCursorCoroutine(itemDetails, playerDirection));
    }

    

    private Vector3Int GetPlayerDirection(Vector3 cursorPosition, Vector3 playerPosition)
    {
       if(cursorPosition.x > playerPosition.x 
            && cursorPosition.y < (playerPosition.y + cursor.ItemUseRaudis / 2f) 
            && cursorPosition.y > (playerPosition.y - cursor.ItemUseRaudis /2f))
        {
            return Vector3Int.right;
        }

       else if (cursorPosition.x < playerPosition.x 
            && cursorPosition.y < (playerPosition.y + cursor.ItemUseRaudis /2f)
            && cursorPosition.y > (playerPosition.y - cursor.ItemUseRaudis / 2f))
        {
            return Vector3Int.left;
        }

       else if(cursorPosition.y > playerPosition.y)
        {
            return Vector3Int.up;
        }
       else
        {
            return Vector3Int.down;
        }
    }

    private void WaterGroundAtCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        AudioManager.Instance.PlaySound(SoundName.effectWateringCan);

        StartCoroutine(WaterGroundAtCursorCoroutine(gridPropertyDetails, playerDirection));
    }

    private void HoeGroundAtCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        AudioManager.Instance.PlaySound(SoundName.effectHoe);

        StartCoroutine(HoeGroundAtCursorCoroutine(gridPropertyDetails, playerDirection));
    }

    private IEnumerator WaterGroundAtCursorCoroutine(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        PlayerInputDisabled = true;
        playerToolDisabled = true;

        toolCharacterAttribute.parVariantType = ParVariantType.wateringCan;
        characterAttributesCustomisatetionList.Clear();
        characterAttributesCustomisatetionList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomsationParameters(characterAttributesCustomisatetionList);

        toolEffect = ToolEffect.watering;

        if (playerDirection == Vector3Int.right)
        {
            isLiftingToolRight = true;
        }
        else if (playerDirection == Vector3Int.left)
        {
            isLiftingToolLeft = true;
        }
        else if (playerDirection == Vector3Int.up)
        {
            isLiftingToolUp = true;
        }
        else if (playerDirection == Vector3Int.down)
        {
            isLiftingToolDown = true;
        }

        yield return liftToolAnimationPause;

        if (gridPropertyDetails.daysSinceWatered == -1)
        {
            gridPropertyDetails.daysSinceWatered = 0;
        }

        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        GridPropertiesManager.Instance.DisplayWaterGround(gridPropertyDetails);

        yield return afterLiftToolAnimationPause;

        PlayerInputDisabled = false;
        playerToolDisabled = false;
    }

    private IEnumerator ReapInPlayerPositionAtCursorCoroutine(ItemDetails itemDetails, Vector3Int playerDirection)
    {
        PlayerInputDisabled = true;
        playerToolDisabled = true;

        toolCharacterAttribute.parVariantType = ParVariantType.scythe;
        characterAttributesCustomisatetionList.Clear();
        characterAttributesCustomisatetionList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomsationParameters(characterAttributesCustomisatetionList);

        UseToolInPlayerDirection(itemDetails, playerDirection);


        yield return useToolAnimationPause;

        PlayerInputDisabled = false;
        playerToolDisabled = false;
    }

    private IEnumerator CollectInPlayerDirectionCoroutine(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerDirection)
    {
        PlayerInputDisabled = true;
        playerToolDisabled = true;

        ProcessCropWithEquippedItemInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);

        yield return pickAnimationPause;
        yield return afterPickAnimationpPause;

        PlayerInputDisabled = false;
        playerToolDisabled = false;
    }

    private IEnumerator ChopInPlayerDirectionCoroutine(GridPropertyDetails gridPropertyDetails, ItemDetails equippeditemDetails, Vector3Int playerDirection)
    {
        PlayerInputDisabled = true;
        playerToolDisabled = true;

        toolCharacterAttribute.parVariantType = ParVariantType.axe;
        characterAttributesCustomisatetionList.Clear();
        characterAttributesCustomisatetionList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomsationParameters(characterAttributesCustomisatetionList);

        ProcessCropWithEquippedItemInPlayerDirection(gridPropertyDetails, equippeditemDetails, playerDirection);

        yield return useToolAnimationPause;

        yield return afterUseToolAnimationPause;

        PlayerInputDisabled = false;
        playerToolDisabled = false;
    }

    private IEnumerator BreakInPlayerDirectionCoroutine(GridPropertyDetails gridPropertyDetails,ItemDetails equippeditemDetails, Vector3Int playerDirection)
    {
        PlayerInputDisabled = true;
        playerToolDisabled = true;

        toolCharacterAttribute.parVariantType = ParVariantType.pickaxe;
        characterAttributesCustomisatetionList.Clear();
        characterAttributesCustomisatetionList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomsationParameters(characterAttributesCustomisatetionList);

        ProcessCropWithEquippedItemInPlayerDirection(gridPropertyDetails, equippeditemDetails, playerDirection);

        yield return useToolAnimationPause;

        yield return afterUseToolAnimationPause;

        PlayerInputDisabled = false;
        playerToolDisabled = false;
    }

    private void ProcessCropWithEquippedItemInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        switch (equippedItemDetails.itemType)
        {
            case ItemType.Chopping_tool:
            case ItemType.Breaking_tool:
                if (playerDirection == Vector3Int.right)
                {
                    isUsingToolRight = true;
                }
                else if (playerDirection == Vector3Int.left)
                {
                    isUsingToolLeft = true;
                }
                else if (playerDirection == Vector3Int.up)
                {
                    isUsingToolUp = true;
                }
                else if (playerDirection == Vector3Int.down)
                {
                    isUsingToolDown = true;
                }

                break;

            case ItemType.Collecting_tool:
                if(playerDirection == Vector3Int.right)
                {
                    isPickingRight = true;
                }
                else if (playerDirection == Vector3Int.left)
                {
                    isPickingLeft = true;
                }
                else if (playerDirection == Vector3Int.up)
                {
                    isPickingUp = true;
                }
                else if (playerDirection == Vector3Int.down)
                {
                    isPickingDown = true;
                }
                break;

            case ItemType.None:
                break;

            default:
                break;
        }

        Crops crop = GridPropertiesManager.Instance.GetCropObjectAtGridPosition(gridPropertyDetails);
        Eggs egg = GridPropertiesManager.Instance.GetEggsObjectAtGridPosition(gridPropertyDetails);

        if(crop != null)
        {
            switch (equippedItemDetails.itemType)
            {
                case ItemType.Chopping_tool:
                case ItemType.Breaking_tool:
                    crop.ProcessActionTool(equippedItemDetails, isUsingToolRight, isUsingToolLeft, isUsingToolDown, isUsingToolUp);
                    break;

                case ItemType.Collecting_tool:
                    crop.ProcessActionTool(equippedItemDetails,isPickingRight,isPickingLeft,isPickingDown,isPickingUp);
                    break;
            }
        }
        else if(egg  != null)
        {
            Debug.Log("egg != null");
            switch (equippedItemDetails.itemType)
            {
                case ItemType.Collecting_tool:
                    egg.ProcessActionTool(equippedItemDetails, isPickingRight, isPickingLeft, isPickingDown, isPickingUp);
                    break;
            }
        }
    }

    private void UseToolInPlayerDirection(ItemDetails equippeditemDetails, Vector3Int playerDirection)
    {
        if (Input.GetMouseButton(0))
        {
            switch (equippeditemDetails.itemType)
            {
                case ItemType.Reping_tool:
                    if(playerDirection == Vector3Int.right)
                    {
                        isSwingToolRight = true;
                    }
                    else if (playerDirection == Vector3Int.left)
                    {
                        isSwingToolLeft = true;
                    }
                    else if (playerDirection == Vector3Int.up)
                    {
                        isSwingToolUp = true;
                    }
                    else if (playerDirection == Vector3Int.down)
                    {
                        isSwingToolDown = true;
                    }
                    break;
            }

            Vector2 point = new Vector2(GetPlayerCenterPosition().x + (playerDirection.x * (equippeditemDetails.itemUseRadius / 2f)),
                GetPlayerCenterPosition().y + (playerDirection.y * (equippeditemDetails.itemUseRadius / 2f)));

            Vector2 size = new Vector2(equippeditemDetails.itemUseRadius, equippeditemDetails.itemUseRadius);

            Item[] itemArray = HelperMethod.GetComponentAtBoxLocationNonAlloc<Item>(Settings.maxCollidersToTestPreReapSwing, point, size, 0f);

            int reapableItemCount = 0;

            for (int i = itemArray.Length - 1; i >= 0 ; i--)
            {
                if(itemArray[i] != null)
                {
                    if(InventoryManager.Instance.GetItemDetails(itemArray[i].ItemCode).itemType == ItemType.Reapable_scenery)
                    {

                        Vector3 effectPosition = new Vector3(itemArray[i].transform.position.x, itemArray[i].transform.position.y + Settings.gridCellSize / 2f,
                            itemArray[i].transform.position.z);

                        EventHandler.CallHarvestActionEvent(effectPosition, HarvestEffect.reaping);

                        AudioManager.Instance.PlaySound(SoundName.effectScythe);

                        Destroy(itemArray[i].gameObject);

                        reapableItemCount++;
                        if (reapableItemCount >= Settings.maxTargetsComponenetToDestoryPreReapSwing)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }

    private IEnumerator HoeGroundAtCursorCoroutine(GridPropertyDetails gridPropertyDetails,Vector3Int playerDirection)
    {
        PlayerInputDisabled = true;
        playerToolDisabled = true;

        toolCharacterAttribute.parVariantType = ParVariantType.hoe;
        characterAttributesCustomisatetionList.Clear();
        characterAttributesCustomisatetionList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomsationParameters(characterAttributesCustomisatetionList);

        if(playerDirection == Vector3Int.right)
        {
            isUsingToolRight = true;
        }
        else if (playerDirection == Vector3Int.left)
        {
            isUsingToolLeft = true;
        }
        else if (playerDirection == Vector3Int.up)
        {
            isUsingToolUp = true;
        }
        else if (playerDirection == Vector3Int.down)
        {
            isUsingToolDown = true;
        }

        yield return useToolAnimationPause;

        if(gridPropertyDetails.daysSinceDug == -1)
        {
            gridPropertyDetails.daysSinceDug = 0;
        }

        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        GridPropertiesManager.Instance.DisplayDugGround(gridPropertyDetails);

        yield return afterUseToolAnimationPause;

        PlayerInputDisabled = false;
        playerToolDisabled = false;
    }

    private Vector3Int GetPlayerClickPosition(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        if(cursorGridPosition.x > playerGridPosition.x)
        {
            return Vector3Int.right;
        }
        else if(cursorGridPosition.x < playerGridPosition.x)
        {
            return Vector3Int.left;
        }
        else if(cursorGridPosition.y > playerGridPosition.y)
        {
            return Vector3Int.up;
        }
        else
        {
            return Vector3Int.down;
        }
    }

    private void ProcessPlayerClickInputSeed(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        if(itemDetails.canBeDropped && gridCursor.CursorPositionIsValid &&  gridPropertyDetails.daysSinceDug > -1 && gridPropertyDetails.seedItemCode == -1)
        {
            PlantSeedAtCursor(gridPropertyDetails,itemDetails);
        }
       else if(itemDetails.canBeDropped && gridCursor.CursorPositionIsValid)
        {
            EventHandler.CallDropSelectedItemEvent();
        }
    }

    /// <summary>
    /// ������ҵ������������
    /// </summary>
    /// <param name="gridPropertyDetails"></param>
    /// <param name="itemDetails"></param>
    private void ProcessPlayerClickInputEggs(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        //��ͬʱ����ü�����itemDetails���Ա��ӣ�gridָ��Ϊ�����ã��ø���gridPropertyDetailsΪ���Է��������Լ��ø���gridPropertyDetailsû�м����ڷ�����eggItemCodeΪ-1��ʱ��
        if (itemDetails.canBeDropped && gridCursor.CursorPositionIsValid && gridPropertyDetails.canHatchEggs  && gridPropertyDetails.eggItemCode == -1 && gridPropertyDetails.daysSinceHatched == -1)
        {
            Debug.Log("ִ�з���HatchEggsAtCursor");
           HatchEggsAtCursor(gridPropertyDetails, itemDetails);
        }
        //����ͬʱ����ü�����itemDetails���Ա��ӣ�gridָ��Ϊ�����ã��ø��ӿ������Ӽ���
        else if (itemDetails.canBeDropped && gridCursor.CursorPositionIsValid)
        {
            Debug.Log("ִ�з���CallDropSelectedItemEvent");
            Debug.Log("gridPropertyDetails.canHatchEggs = " + gridPropertyDetails.canHatchEggs);
            Debug.Log(" gridPropertyDetails.eggItemCode" + gridPropertyDetails.eggItemCode);
            EventHandler.CallDropSelectedItemEvent();
        }
    }

    private void HatchEggsAtCursor(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        if(GridPropertiesManager.Instance.GetEggDetails(itemDetails.itemCode) != null)
        {
            Debug.Log("�ҵ�itemDetails");
            gridPropertyDetails.eggItemCode = itemDetails.itemCode; //���ø����ϵ�gridPropertyDetails.eggItemCode��itemDetails.itemCode��ֵ
            gridPropertyDetails.growthDays = 0; //���øø����ϵ���������

            GridPropertiesManager.Instance.DisplayHatchedEggs(gridPropertyDetails);

            EventHandler.CallRemoveSelectedItemFromInventoryEvent(); //���÷������������ѡ�е���ƷҲ���Ǽ���
            
            AudioManager.Instance.PlaySound(SoundName.effectHatchEggs); //���ż�������������
        }
    }

    private void PlantSeedAtCursor(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        if (GridPropertiesManager.Instance.GetCropDetails(itemDetails.itemCode) != null)
        {
            gridPropertyDetails.seedItemCode = itemDetails.itemCode; //���ø����ϵ�gridPropertyDetails.eggItemCode��itemDetails.itemCode��ֵ
            gridPropertyDetails.growthDays = 0; //���øø����ϵ���������

            GridPropertiesManager.Instance.DisplayPlantedCrop(gridPropertyDetails);

            EventHandler.CallRemoveSelectedItemFromInventoryEvent();//���÷������������ѡ�е���ƷҲ����Crop����

            AudioManager.Instance.PlaySound(SoundName.effectPlantingSound);//������ֲ���������
        }
    }

    private void ProcessPlayerClickInputCommodity(ItemDetails itemDetails)
    {
        if (itemDetails.canBeDropped && gridCursor.CursorPositionIsValid)
        {
            EventHandler.CallDropSelectedItemEvent();
        }
    }

    public void SetPlayerDirection(Direction playerDirection)
    {
        switch (playerDirection)
        {
            case Direction.up:
                EventHandler.CallMovementEvent(0f, 0f, false, false, false, false, ToolEffect.none, false, false, false, false, false, 
                    false, false, false, false, false, false, false, false, false, false, false, false, false, true, false);
                break;
            case Direction.down:
                EventHandler.CallMovementEvent(0f, 0f, false, false, false, false, ToolEffect.none, false, false, false, false, false,
                    false, false, false, false, false, false, false, false, false, false, false, false, false, false, true);
                break;
            case Direction.left:
                EventHandler.CallMovementEvent(0f, 0f, false, false, false, false, ToolEffect.none, false, false, false, false, false,
                    false, false, false, false, false, false, false, false, false, false, false, false, true, false, false);
                break;
            case Direction.right:
                EventHandler.CallMovementEvent(0f, 0f, false, false, false, false, ToolEffect.none, false, false, false, false, false,
                    false, false, false, false, false, false, false, false, false, false, false, true, false, false, false);
                break;
            default:
                EventHandler.CallMovementEvent(0f, 0f, false, false, false, false, ToolEffect.none, false, false, false, false, false,
                    false, false, false, false, false, false, false, false, false, false, false, false, false, false, true);
                break;
        }
    }

    public Vector3 GetPlayerViewportPosition()
    {
        return mainCamera.WorldToViewportPoint(transform.position);
    }

    public Vector3 GetPlayerCenterPosition()
    {
        return new Vector3(transform.position.x, transform.position.y + Settings.playerCenterOffset, transform.position.z);
    }
    #region ��������Public Methods
    /// <summary>
    /// �����������
    /// </summary>
    public void EnablePlayerInput()
    {
        PlayerInputDisabled = false;
    }
    /// <summary>
    /// ��ֹ�������
    /// </summary>
    public void DisablePlayerInput()
    {
        PlayerInputDisabled = true;
    }

    public void ClearCarriedItem()
    {
        equippedItemSpriteRenderer.sprite = null;
        equippedItemSpriteRenderer.color = new Color(1f, 1f, 1f, 0f);


        armCharacterAttribute.parVariantType = ParVariantType.none;
        characterAttributesCustomisatetionList.Clear();
        characterAttributesCustomisatetionList.Add(armCharacterAttribute);
        animationOverrides.ApplyCharacterCustomsationParameters(characterAttributesCustomisatetionList);

        isCarrying = false;
    }

    public void ShowCarriedItem(int itemCode)
    {
        ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);
        if (itemDetails != null)
        {
            equippedItemSpriteRenderer.sprite = itemDetails.itemSprite;
            equippedItemSpriteRenderer.color = new Color(1f, 1f, 1f, 1f);


            armCharacterAttribute.parVariantType = ParVariantType.carry;
            characterAttributesCustomisatetionList.Clear();
            characterAttributesCustomisatetionList.Add(armCharacterAttribute);
            animationOverrides.ApplyCharacterCustomsationParameters(characterAttributesCustomisatetionList);

            isCarrying = true;
        }
    }

    public Vector3 GetPlayerVierportPosition()
    {
        return mainCamera.WorldToViewportPoint(transform.position);
    }

    #endregion


    #region ʵ��Isaveable�ӿڵķ���

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public GameObjectSave ISaveableSave()
    {
        GameObjectSave.sceneData.Remove(Settings.persistentScene);
        SceneSave sceneSave = new SceneSave();
        sceneSave.vector3Dictionary = new Dictionary<string, Vector3Serializable>();
        sceneSave.stringDictionary = new Dictionary<string, string>();

        Vector3Serializable vector3Serializable = new Vector3Serializable(transform.position.x, transform.position.y, transform.position.z);
        sceneSave.vector3Dictionary.Add("playerPosition", vector3Serializable);

        sceneSave.stringDictionary.Add("currentScene", SceneManager.GetActiveScene().name);

        sceneSave.stringDictionary.Add("playerDirection", playerDirection.ToString());

        GameObjectSave.sceneData.Add(Settings.persistentScene, sceneSave);

        return GameObjectSave;

    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if(gameSave.gameObjectData.TryGetValue(ISaveableUniqueID,out GameObjectSave gameObjectSave))
        {
            if(gameObjectSave.sceneData.TryGetValue(Settings.persistentScene,out SceneSave sceneSave))
            {
                if(sceneSave.vector3Dictionary != null && sceneSave.vector3Dictionary.TryGetValue("playerPosition",out Vector3Serializable playerPosition))
                {
                    transform.position = new Vector3(playerPosition.x,playerPosition.y,playerPosition.z);
                }

                if(sceneSave.stringDictionary != null)
                {
                    if(sceneSave.stringDictionary.TryGetValue("currentScene",out string currentScene))
                    {
                        SceneControllerManager.Instance.FadeAndLoadScene(currentScene, transform.position);
                    }

                    if(sceneSave.stringDictionary.TryGetValue("playerDirection",out string playerDir))
                    {
                        bool playerDirFound = Enum.TryParse<Direction>(playerDir, out Direction direction);

                        if (playerDirFound)
                        {
                            playerDirection = direction;
                            SetPlayerDirection(playerDirection);
                        }
                       
                    }
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
