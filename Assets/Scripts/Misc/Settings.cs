using UnityEngine;

public static  class Settings 
{
    #region 场景加载
    public const string persistentScene = "PersistenceScene";

    //当被遮挡以后就用调用这些参数来剔除遮挡
    public const float targetFadeAlpha = 0.45f;
    public const float fadeInTime = 0.15f;
    public const float fadeOutTime = 0.25f;

    #endregion

    #region Grid
    public const float gridCellSize = 1f;
    public const float gridCellDiagnolSize = 1.41f;

    public const float playerCenterOffset = 0.875f;

    public const int maxGridWidth = 99999;
    public const int maxGridHeight = 99999;

    #endregion

    #region 移动速度MovementSpeed
    public const float walkSpeed = 5.33f;
    public const float runSpeed = 8.67f;
    public const float swimSpeed = 2.33f;
    #endregion

    #region 动画暂停Animation Pause
    public static float useToolAnimationPause = 0.25f;
    public static float afterUseToolAnimationPause = 0.2f;
    public static float liftToolAnimationPause = 0.4f;
    public static float afterLiftToolAnimationPause = 0.4f;
    public static float pickAnimationPause = 1f;
    public static float afterPickAnimationPause = 0.2f;
    #endregion

    public static float pixelSize = 0.0625f;


    public static Vector2 cursorSize = Vector2.one;

    public static int playerIntialInventoryCapcity = 24;
    public static int playerMaximumInventoryCapcity = 48;

    #region NPC Animation
    public static int walkUp;
    public static int walkDown;
    public static int walkLeft;
    public static int walkRight;
    public static int eventAnimation;
    #endregion
 
    #region Player Animation
    public static int inputX;
    public static int inputY;
    public static int isWalking;
    public static int isRunning;
    public static int toolEffect;
    public static int isUsingToolRight;
    public static int isUsingToolLeft;
    public static int isUsingToolUp;
    public static int isUsingToolDown;
    public static int isLiftingToolRight;
    public static int isLiftingToolLeft;
    public static int isLiftingToolUp;
    public static int isLiftingToolDown;
    public static int isSwingToolUp;
    public static int isSwingToolDown;
    public static int isSwingToolLeft;
    public static int isSwingToolRight;
    public static int isPickingUp;
    public static int isPickingDown;
    public static int isPickingLeft;
    public static int isPickingRight;
    #endregion

    //分享型动画参数Idle用Trigger来触发
    public static int idleUp;
    public static int idleDown;
    public static int idleLeft;
    public static int idleRight;

    public const string HoeingTool = "Hoe";
    public const string ChoppingTool = "Axe";
    public const string BreakingTool = "Pickaxe";
    public const string ReapingTool = "Scythe";
    public const string WateringTool = "Watering Can";
    public const string CollectingTool = "Basket";

    public const float secondTimePerGameTimeSecond = 0.012f; //游戏中的一秒相当于现实的0.012f秒

    public const int maxCollidersToTestPreReapSwing = 15;
    public const int maxTargetsComponenetToDestoryPreReapSwing = 2;
    static Settings()
    {
        //NPC
        walkDown = Animator.StringToHash("walkDown");
        walkUp = Animator.StringToHash("walkUp");
        walkLeft = Animator.StringToHash("walkLeft");
        walkRight = Animator.StringToHash("walkRight");
        eventAnimation = Animator.StringToHash("eventAnimation");

        //Player
        inputX = Animator.StringToHash("xInput");
        inputY = Animator.StringToHash("yInput");
        isWalking = Animator.StringToHash("isWalking");
        isRunning = Animator.StringToHash("isRunning");
        toolEffect = Animator.StringToHash("toolEffect");
        isUsingToolLeft = Animator.StringToHash("isUsingToolLeft");
        isUsingToolRight = Animator.StringToHash("isUsingToolRight");
        isUsingToolUp = Animator.StringToHash("isUsingToolUp");
        isUsingToolDown = Animator.StringToHash("isUsingToolDown");
        isLiftingToolRight = Animator.StringToHash("isLiftingToolRight");
        isLiftingToolLeft = Animator.StringToHash("isLiftingToolLeft");
        isLiftingToolUp = Animator.StringToHash("isLiftingToolUp");
        isLiftingToolDown = Animator.StringToHash("isLiftingToolDown");
        isSwingToolUp = Animator.StringToHash("isSwingToolUp");
        isSwingToolDown = Animator.StringToHash("isSwingToolDown");
        isSwingToolLeft = Animator.StringToHash("isSwingToolLeft");
        isSwingToolRight = Animator.StringToHash("isSwingToolRight");
        isPickingUp = Animator.StringToHash("isPickingUp");
        isPickingDown = Animator.StringToHash("isPickingDown");
        isPickingLeft = Animator.StringToHash("isPickingLeft");
        isPickingRight = Animator.StringToHash("isPickingRight");

        idleUp = Animator.StringToHash("idleUp");
        idleDown = Animator.StringToHash("idleDown");
        idleLeft = Animator.StringToHash("idleLeft");
        idleRight = Animator.StringToHash("idleRight");
    }
}
