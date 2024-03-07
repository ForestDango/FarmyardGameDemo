using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// �¼�ί���ߣ���Ϸȫ���¼��ļ����Լ�ί�д���
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

#region �ƶ�ί�У��붯���໥����
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
    #region ��Ʒ�¼� Item Event
    public static event Action DropSelectedItemEvent;  //��Ʒ�����¼�ί�У�ֻ�ǵ����Ľ���Ʒ����

    /// <summary>
    ///  ����Ʒ�����¼�����ʱ�����м���ί�е��¼����ᴥ��
    /// </summary>
    public static void CallDropSelectedItemEvent()
    {
        if (DropSelectedItemEvent != null)
            DropSelectedItemEvent();
    }

    public static event Action RemoveSelectedItemFromInventoryEvent; //��Ʒ�ӱ��������ʱ�¼�ί��

    /// <summary>
    /// ������Ʒ�ӱ��������ʱ�����м�����ί�е��¼����ᴥ�� : 
    /// 1.��ֲֲ��
    /// 2.��������
    /// </summary>
    public static void CallRemoveSelectedItemFromInventoryEvent()
    {
        if (RemoveSelectedItemFromInventoryEvent != null)
        {
            RemoveSelectedItemFromInventoryEvent();
        }
    }

    public static event Action InstantiateCropPrefabEvent; //���ɹ����Ԥ�����¼�

    /// <summary>
    /// ����Ҫ���ɹ����Ԥ�����ʱ�����м�����ί�е��¼����ᴥ�� : 
    /// 1.�����¼��س�����ʱ����ǵ�һ�μ��س�������
    /// </summary>
    public static void CallInstantiateCropPrefabEvent()
    {
        if(InstantiateCropPrefabEvent != null)
        {
            InstantiateCropPrefabEvent();
        }
    }

    public static event Action<InventoryLocation,List<InventoryItem>> InventoryUpdateEvent; //���±�����Ʒ��ί��

    /// <summary>
    /// ����������Ʒ��Ϣ�������µ�ʱ�����Լ�����ί�е��¼��ͻᴥ��
    /// </summary>
    /// <param name="inventoryLocation"> ö�����ͣ�������λ��</param>
    /// <param name="inventoryLists"> InventoryList�ļ�������</param>
    public static void CallInventoryUpdateEvent(InventoryLocation inventoryLocation,List<InventoryItem> inventoryLists)
    {
        if(InventoryUpdateEvent != null)
        {
            InventoryUpdateEvent(inventoryLocation, inventoryLists);
        }
    }
    #endregion

    #region ʱ�伾���¼�Time Events
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameMinuteEvent; //����Ϸ���Ӹ����¼�

    /// <summary>
    /// ����Ϸ���ӷ������µ�ʱ�����Լ�����ί�е��¼��ͻᴥ��
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

    public static event Action<int, Season, int, string, int, int, int> AdvanceGameHourEvent;//����ϷСʱ�����¼�

    /// <summary>
    /// ����ϷСʱ�������µ�ʱ�����Լ�����ί�е��¼��ͻᴥ��
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


    public static event Action<int, Season, int, string, int, int, int> AdvanceGameDayEvent;//����ϷСʱ�����¼�
    /// <summary>
    ///  ����Ϸ�����ڷ������µ�ʱ�����Լ�����ί�е��¼��ͻᴥ��
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

    public static event Action<int, Season, int, string, int, int, int> AdvanceGameSeansonEvent; //��Ϸ���ڷ�������

    /// <summary>
    ///  ����Ϸ���ڷ������µ�ʱ�����Լ�����ί�е��¼��ͻᴥ��
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

    public static event Action<int, Season, int, string, int, int, int> AdvanceGameYearEvent; //��Ϸ��ݷ�������

    /// <summary>
    /// ����Ϸ��ݷ������µ�ʱ�����Լ�����ί�е��¼��ͻᴥ��
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

    #region   �ƶ��¼�MovmentEvent
    public static event MovementDelegate MovementEvent;

    //��publisher����ί�еĺ���
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


    //���������¼�
    public static event Action BeforeSceneUnloadFadeOutEvent; //����ж��ǰ�����¼�

    /// <summary>
    /// ������ж��ǰ����������ʱ�����Լ�����ί�е��¼��ͻᴥ��
    /// </summary>
    public static void CallBeforeSceneUnloadFadeOutEvent()
    {
        if(BeforeSceneUnloadFadeOutEvent!= null)
        {
            BeforeSceneUnloadFadeOutEvent();
        }
    }

    public static event Action BeforeSceneUnloadEvent; //����ж��ǰ�¼�

    /// <summary>
    /// ������ж��ǰ������ʱ�����Լ�����ί�е��¼��ͻᴥ��
    /// </summary>
    public static void CallBeforeSceneUnloadEvent()
    {
        if (BeforeSceneUnloadEvent != null)
        {
            BeforeSceneUnloadEvent();
        }
    }

    public static event Action AfterSceneLoadEvent;//�������غ���¼�

    /// <summary>
    /// ���������غ��ʱ�����Լ�����ί�е��¼��ͻᴥ��
    /// </summary>
    public static void CallAfterSceneLoadEvent()
    {
        if (AfterSceneLoadEvent != null)
        {
            AfterSceneLoadEvent();
        }
    }

    public static event Action AfterSceneLoadFadeInEvent;//�������ص������¼�

    /// <summary>
    /// ���������ص�����ʱ�����Լ�����ί�е��¼��ͻᴥ��
    /// </summary>
    public static void CallAfterSceneLoadFadeInEvent()
    {
        if (AfterSceneLoadFadeInEvent != null)
        {
            AfterSceneLoadFadeInEvent();
        }
    }
#endregion

    #region ��ЧEffect�¼�

    public static event Action<Vector3, HarvestEffect> HarvestActionEvent; //�������ջ���Ϊ���¼�

    /// <summary>
    /// �����������ջ���Ϊ��ʱ�����Լ�����ί�е��¼��ͻᴥ��
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
