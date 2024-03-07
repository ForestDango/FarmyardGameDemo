using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 动画参数控制器，挂载在Player对象上
/// </summary>

public class AniamtionParameterControls : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        EventHandler.MovementEvent += SetAnimationParmater; //订阅EventHandler中的MovementEvent事件，当Call方法调用的时候执行SetAnimationParmater方法
    }

    private void OnDisable()
    {
        EventHandler.MovementEvent -= SetAnimationParmater;
    }

    /// <summary>
    /// 设置动画参数
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
    private void SetAnimationParmater(float inputX, float inputY, bool isWalking, bool isRunning, bool isIdle, bool isCarrying,
    ToolEffect toolEffect,
    bool isUsingToolRight, bool isUsingToolLeft, bool isUsingToolUp, bool isUsingToolDown,
    bool isLiftingToolRight, bool isLiftingToolLeft, bool isLiftingToolUp, bool isLiftingToolDown,
    bool isSwingToolRight, bool isSwingToolLeft, bool isSwingToolUp, bool isSwingToolDown,
    bool isPickingRight, bool isPickingLeft, bool isPickingUp, bool isPickingDown,
    bool idleRight, bool idelLeft, bool idleUp, bool idleDown)
    {
        animator.SetFloat(Settings.inputX, inputX);
        animator.SetFloat(Settings.inputY, inputY);
        animator.SetBool(Settings.isWalking, isWalking);
        animator.SetBool(Settings.isRunning, isRunning);

        //TODO:处理Idle,
        //TODO:处理IsCarrying


        animator.SetInteger(Settings.toolEffect, (int)toolEffect);

        if (isUsingToolRight)
            animator.SetTrigger(Settings.isUsingToolRight);
        if (isUsingToolLeft)
            animator.SetTrigger(Settings.isUsingToolLeft);
        if (isUsingToolUp)
            animator.SetTrigger(Settings.isUsingToolUp);
        if (isUsingToolDown)
            animator.SetTrigger(Settings.isUsingToolDown);
        if (isLiftingToolRight)
            animator.SetTrigger(Settings.isLiftingToolRight);
        if (isLiftingToolLeft)
            animator.SetTrigger(Settings.isLiftingToolLeft);
        if (isLiftingToolUp)
            animator.SetTrigger(Settings.isLiftingToolUp);
        if (isLiftingToolDown)
            animator.SetTrigger(Settings.isLiftingToolDown);
        if (isSwingToolRight)
            animator.SetTrigger(Settings.isSwingToolRight);
        if (isSwingToolLeft)
            animator.SetTrigger(Settings.isSwingToolLeft);
        if (isSwingToolUp)
            animator.SetTrigger(Settings.isSwingToolUp);
        if (isSwingToolDown)
            animator.SetTrigger(Settings.isSwingToolDown);
        if (isPickingRight)
            animator.SetTrigger(Settings.isPickingRight);
        if (isPickingLeft)
            animator.SetTrigger(Settings.isPickingLeft);
        if (isPickingUp)
            animator.SetTrigger(Settings.isPickingUp);
        if (isPickingDown)
            animator.SetTrigger(Settings.isPickingDown);

        if (idleRight)
            animator.SetTrigger(Settings.idleRight);
        if (idelLeft)
            animator.SetTrigger(Settings.idleLeft);
        if (idleUp)
            animator.SetTrigger(Settings.idleUp);
        if (idleDown)
            animator.SetTrigger(Settings.idleDown);
    }

    /// <summary>
    /// 动画器事件，在玩家移动的时候调用
    /// </summary>
    private void AnimationEventPlayFootstepSound()
    {
        AudioManager.Instance.PlaySound(SoundName.effectFootstepHardGround);
    }
}
