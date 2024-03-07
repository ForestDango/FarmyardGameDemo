
/// <summary>
/// ����λ��
/// </summary>
public enum InventoryLocation
{
    player,
    chest,
    count,
}

/// <summary>
/// ʹ�ù��ߵ��Ӿ�Ч��
/// </summary>
public enum ToolEffect
{
    none,
    watering,
}

/// <summary>
/// �ջ���Ʒ���Ӿ�Ч��
/// </summary>
public enum HarvestEffect
{
    decidousLeavesFalling,
    pineConesFalling,
    choppingTreeTrunk,
    breakingStone,
    reaping,
    breakingEggs,
    none,
}
/// <summary>
/// ��ҳ���
/// </summary>

public enum Direction
{
    up,
    down,
    right,
    left,
    none,
}

/// <summary>
/// ��Ʒ�����
/// </summary>

public enum ItemType
{
    Seed,
    Commodity,
    Watering_tool,
    Hoeing_tool,
    Chopping_tool,
    Breaking_tool,
    Reping_tool,
    Collecting_tool,
    Reapable_scenery,
    Furinture,
    Eggs,
    None,
    Count,
}
/// <summary>
/// ��Ϸ����
/// </summary>
public enum Season
{
    Spring,
    Summer,
    Autumn,
    Winter,
    none,
    count,
}

/// <summary>
/// ��ͬ���ֵ���Ч
/// </summary>
public enum SoundName
{
    none = 0,
    effectFootstepSoftGround = 10,
    effectFootstepHardGround = 20,
    effectAxe= 30,
    effectPickaxe = 40,
    effectScythe = 50,
    effectHoe = 60,
    effectWateringCan = 70,
    effectBasket = 80,
    effectPickUpSound = 90,
    effectRustle = 100,
    effectTreeFalling = 110,
    effectPlantingSound = 120,
    effectPluck = 130,
    effectStoneShatter = 140,
    effectWoodSplinters = 150,
    effectHatchEggs = 160,
    ambientCountryside1 = 1000,
    ambientCountryside2 = 1010,
    ambientInDoors = 1020,
    musicCalm3 = 2000,
    musicCalm1 = 2010

}

/// <summary>
/// ��Ҷ�������
/// </summary>
public enum AnimationName
{
    idleDown,
    idleUp,
    idleRight,
    idleLeft,
    walkUp,
    walkDown,
    walkRight,
    walkLeft,
    runUp,
    runDown,
    runRight,
    runLeft,
    useToolUp,
    useToolDown,
    useToolRight,
    useToolLeft,
    swingToolUp,
    swingToolDown,
    swingToolRight,
    swingToolLeft,
    liftToolUp,
    liftToolDown,
    liftToolRight,
    liftToolLeft,
    holdToolUp,
    holdToolDown,
    holdToolRight,
    holdToolLeft,
    pickDown,
    pickUp,
    pickRight,
    pickLeft,
    count,
}

/// <summary>
/// ��Ϸ����
/// </summary>
public enum Weather
{
    dry,
    raining,
    snowing,
    none,
    count,
}

/// <summary>
/// ��Ҷ������岿λ
/// </summary>
public enum CharacterPartAnimator
{
    body,
    arms,
    hair,
    tool,
    hat,
    count,
}

/// <summary>
/// ��Ҷ���������ɫ
/// </summary>
public enum ParVariantColor
{
    none,
    count,
}

/// <summary>
/// ��Ҷ�������ʹ�õĹ���
/// </summary>
public enum ParVariantType
{
    none,
    carry,
    hoe,
    pickaxe,
    axe,
    scythe,
    wateringCan,
    count,
}

/// <summary>
/// Grid��Boolֵ����
/// </summary>
public enum GridBoolProperty
{
    diggable,
    canDropItem,
    canPlaneFurniture,
    canHatchEggs,
    isPath,
    isNPCObstacle,
}

/// <summary>
/// ��������
/// </summary>
public enum SceneName
{
    Scene1_Farm,
    Scene2_Field,
    Scene3_Cabin,

}

/// <summary>
/// NPC����
/// </summary>
public enum Facing
{
    none,
    front,
    back,
    right,
}
