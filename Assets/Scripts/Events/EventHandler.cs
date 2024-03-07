using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 事件委托者，游戏全局事件的监听以及委托创建
/// </summary>
/// <param name="inputX"></param>
/// <param name="inputY"></param>
/// <param name="isWalking"></param>
/// <param name="isRunning"></param>
/// <param name="isIdle"></param>
/// <param name="isCarrying"></param>
/// <param name="toolEffect"></param>
/// <param name="isUsingToolRight"></param>
/// <param name="isUsingToolLeft"></param>
/// <param name="isUsingToolUp"></param>
/// <param name="isUsingToolDown"></param>
/// <param name="isLiftingToolRight"></param>
/// <param name="isLiftingToolLeft"></param>
/// <param name="isLiftingToolUp"></param>
/// <param name="isLiftingToolDown"></param>
/// <param name="isSwingToolRight"></param>
/// <param name="isSwingToolLeft"></param>
/// <param name="isSwingToolUp"></param>
/// <param name="isSwingToolDown"></param>
/// <param name="isPickingRight"></param>
/// <param name="isPickingLeft"></param>
/// <param name="isPickingUp"></param>
/// <param name="isPickingDown"></param>
/// <param name="idleRight"></param>
/// <param name="idelLeft"></param>
/// <param name="idleUp"></param>
/// <param name="idleDown"></param>
/// 

#region 移动委托，与动画相互连接
public delegate void MovementDelegate(float inputX,float inputY,  bool isWalking,bool isRunning,bool isIdle,bool isCarrying,
    ToolEffect toolEffect,
    bool isUsingToolRight,bool isUsingToolLeft,bool isUsingToolUp,bool isUsingToolDown,
    bool isLiftingToolRight,bool isLiftingToolLeft,bool isLiftingToolUp,bool isLiftingToolDown,
    bool isSwingToolRight,bool isSwingToolLeft,bool isSwingToolUp,bool isSwingToolDown,
    bool isPickingRight,bool isPickingLeft,bool isPickingUp,bool isPickingDown,
    bool idleRight,bool idelLeft,bool idleUp,bool idleDown);

#endregion
public static class EventHandler
{
    #region 物品事件 Item Event
    public static event Action DropSelectedItemEvent;  //物品掉落事件委托，只是单纯的将物品掉落

    /// <summary>
    ///  当物品掉落事件触发时，所有监听委托的事件都会触发
    /// </summary>
    public static void CallDropSelectedItemEvent()
    {
        if (DropSelectedItemEvent != null)
            DropSelectedItemEvent();
    }

    public static event Action RemoveSelectedItemFromInventoryEvent; //物品从背包清除的时事件委托

    /// <summary>
    /// 当物物品从背包清除的时，所有监听该委托的事件都会触发 : 
    /// 1.种植植物
    /// 2.孵化鸡蛋
    /// </summary>
    public static void CallRemoveSelectedItemFromInventoryEvent()
    {
        if (RemoveSelectedItemFromInventoryEvent != null)
        {
            RemoveSelectedItemFromInventoryEvent();
        }
    }

    public static event Action InstantiateCropPrefabEvent; //生成谷物的预制体事件

    /// <summary>
    /// 当需要生成谷物的预制体的时候，所有监听该委托的事件都会触发 : 
    /// 1.在重新加载场景的时如果是第一次加载场景，就
    /// </summary>
    public static void CallInstantiateCropPrefabEvent()
    {
        if(InstantiateCropPrefabEvent != null)
        {
            InstantiateCropPrefabEvent();
        }
    }

    public static event Action<InventoryLocation,List<InventoryItem>> InventoryUpdateEvent; //更新背包物品的委托

    /// <summary>
    /// 当背包的物品信息发生更新的时候，所以监听该委托的事件就会触发
    /// </summary>
    /// <param name="inventoryLocation"> 枚举类型，背包的位置</param>
    /// <param name="inventoryLists"> InventoryList的集合链表</param>
    public static void CallInventoryUpdateEvent(InventoryLocation inventoryLocation,List<InventoryItem> inventoryLists)
    {
        if(InventoryUpdateEvent != null)
        {
            InventoryUpdateEvent(inventoryLocation, inventoryLists);
        }
    }
    #endregion

    #region 时间季节事件Time Events
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameMinuteEvent; //当游戏分钟更新事件

    /// <summary>
    /// 当游戏分钟发生更新的时候，所以监听该委托的事件就会触发
    /// </summary>
    /// <param name="gameYear"></param>
    /// <param name="season"></param>
    /// <param name="gameDay"></param>
    /// <param name="gameOnWeek"></param>
    /// <param name="gameHour"></param>
    /// <param name="gameMinuter"></param>
    /// <param name="gameSecond"></param>
    public static void CallAdvanceGameMinuteEvent(int gameYear, Season season, int gameDay, string gameOnWeek, int gameHour, int gameMinuter, int gameSecond)
    {
        if (AdvanceGameMinuteEvent != null)
        {
            AdvanceGameMinuteEvent(gameYear, season, gameDay, gameOnWeek, gameHour, gameMinuter, gameSecond);
        }
    }

    public static event Action<int, Season, int, string, int, int, int> AdvanceGameHourEvent;//当游戏小时更新事件

    /// <summary>
    /// 当游戏小时发生更新的时候，所以监听该委托的事件就会触发
    /// </summary>
    /// <param name="gameYear"></param>
    /// <param name="season"></param>
    /// <param name="gameDay"></param>
    /// <param name="gameOnWeek"></param>
    /// <param name="gameHour"></param>
    /// <param name="gameMinuter"></param>
    /// <param name="gameSecond"></param>

    public static void CallAdvanceGameHourEvent(int gameYear, Season season, int gameDay, string gameOnWeek, int gameHour, int gameMinuter, int gameSecond)
    {
        if (AdvanceGameHourEvent != null)
        {
            AdvanceGameHourEvent(gameYear, season, gameDay, gameOnWeek, gameHour, gameMinuter, gameSecond);
        }
    }


    public static event Action<int, Season, int, string, int, int, int> AdvanceGameDayEvent;//当游戏小时更新事件
    /// <summary>
    ///  当游戏的日期发生更新的时候，所以监听该委托的事件就会触发
    /// </summary>
    /// <param name="gameYear"></param>
    /// <param name="season"></param>
    /// <param name="gameDay"></param>
    /// <param name="gameOnWeek"></param>
    /// <param name="gameHour"></param>
    /// <param name="gameMinuter"></param>
    /// <param name="gameSecond"></param>

    public static void CallAdvanceGameDayEvent(int gameYear,Season season,int gameDay,string gameOnWeek,int gameHour,int gameMinuter,int gameSecond)
    {
        if(AdvanceGameDayEvent!= null)
        {
            AdvanceGameDayEvent(gameYear, season, gameDay, gameOnWeek, gameHour, gameMinuter, gameSecond);
        }
    }

    public static event Action<int, Season, int, string, int, int, int> AdvanceGameSeansonEvent; //游戏季节发生更新

    /// <summary>
    ///  当游戏季节发生更新的时候，所以监听该委托的事件就会触发
    /// </summary>
    /// <param name="gameYear"></param>
    /// <param name="season"></param>
    /// <param name="gameDay"></param>
    /// <param name="gameOnWeek"></param>
    /// <param name="gameHour"></param>
    /// <param name="gameMinuter"></param>
    /// <param name="gameSecond"></param>

    public static void CallAdvanceGameSeansonEvent(int gameYear, Season season, int gameDay, string gameOnWeek, int gameHour, int gameMinuter, int gameSecond)
    {
        if (AdvanceGameSeansonEvent != null)
        {
            AdvanceGameSeansonEvent(gameYear, season, gameDay, gameOnWeek, gameHour, gameMinuter, gameSecond);
        }
    }

    public static event Action<int, Season, int, string, int, int, int> AdvanceGameYearEvent; //游戏年份发生更新

    /// <summary>
    /// 当游戏年份发生更新的时候，所以监听该委托的事件就会触发
    /// </summary>
    /// <param name="gameYear"></param>
    /// <param name="season"></param>
    /// <param name="gameDay"></param>
    /// <param name="gameOnWeek"></param>
    /// <param name="gameHour"></param>
    /// <param name="gameMinuter"></param>
    /// <param name="gameSecond"></param>
    public static void CallAdvanceGameYearEvent(int gameYear, Season season, int gameDay, string gameOnWeek, int gameHour, int gameMinuter, int gameSecond)
    {
        if (AdvanceGameYearEvent != null)
        {
            AdvanceGameYearEvent(gameYear, season, gameDay, gameOnWeek, gameHour, gameMinuter, gameSecond);
        }
    }

    #endregion

    #region   移动事件MovmentEvent
    public static event MovementDelegate MovementEvent;

    //给publisher订阅委托的函数
    public static void CallMovementEvent(float inputX, float inputY, bool isWalking, bool isRunning, bool isIdle, bool isCarrying,
    ToolEffect toolEffect,
    bool isUsingToolRight, bool isUsingToolLeft, bool isUsingToolUp, bool isUsingToolDown,
    bool isLiftingToolRight, bool isLiftingToolLeft, bool isLiftingToolUp, bool isLiftingToolDown,
    bool isSwingToolRight, bool isSwingToolLeft, bool isSwingToolUp, bool isSwingToolDown,
    bool isPickingRight, bool isPickingLeft, bool isPickingUp, bool isPickingDown,
    bool idleRight, bool idelLeft, bool idleUp, bool idleDown)
    {
        if(MovementEvent != null)
        {
            MovementEvent(inputX, inputY, isWalking, isRunning, isIdle,isCarrying,
                toolEffect,
                isUsingToolRight,isUsingToolLeft, isUsingToolUp,isUsingToolDown,
                isLiftingToolRight,isLiftingToolLeft,isLiftingToolUp,isLiftingToolDown,
                isSwingToolRight,isSwingToolLeft,isSwingToolUp,isSwingToolDown,
                isPickingRight,isPickingLeft,isPickingUp,isPickingDown,
                idleRight,idelLeft,idleUp,idleDown);
        }
    }


    //场景加载事件
    public static event Action BeforeSceneUnloadFadeOutEvent; //场景卸载前淡出事件

    /// <summary>
    /// 当场景卸载前淡出触发的时候，所以监听该委托的事件就会触发
    /// </summary>
    public static void CallBeforeSceneUnloadFadeOutEvent()
    {
        if(BeforeSceneUnloadFadeOutEvent!= null)
        {
            BeforeSceneUnloadFadeOutEvent();
        }
    }

    public static event Action BeforeSceneUnloadEvent; //场景卸载前事件

    /// <summary>
    /// 当场景卸载前触发的时候，所以监听该委托的事件就会触发
    /// </summary>
    public static void CallBeforeSceneUnloadEvent()
    {
        if (BeforeSceneUnloadEvent != null)
        {
            BeforeSceneUnloadEvent();
        }
    }

    public static event Action AfterSceneLoadEvent;//场景加载后的事件

    /// <summary>
    /// 当场景加载后的时候，所以监听该委托的事件就会触发
    /// </summary>
    public static void CallAfterSceneLoadEvent()
    {
        if (AfterSceneLoadEvent != null)
        {
            AfterSceneLoadEvent();
        }
    }

    public static event Action AfterSceneLoadFadeInEvent;//场景加载淡入后的事件

    /// <summary>
    /// 当场景加载淡入后的时候，所以监听该委托的事件就会触发
    /// </summary>
    public static void CallAfterSceneLoadFadeInEvent()
    {
        if (AfterSceneLoadFadeInEvent != null)
        {
            AfterSceneLoadFadeInEvent();
        }
    }
#endregion

    #region 特效Effect事件

    public static event Action<Vector3, HarvestEffect> HarvestActionEvent; //当触发收获行为的事件

    /// <summary>
    /// 当场景触发收获行为的时候，所以监听该委托的事件就会触发
    /// </summary>
    /// <param name="effectPosition"></param>
    /// <param name="harvestEffect"></param>
    public static void CallHarvestActionEvent(Vector3 effectPosition,HarvestEffect harvestEffect)
    {
        if(HarvestActionEvent != null)
        {
            HarvestActionEvent(effectPosition,harvestEffect);
        }
    }
    #endregion
}
