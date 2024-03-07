using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTestHarness : MonoBehaviour
{
    public float xInput;
    public float yInput;

    public bool isCarrying;
    public bool isIdle;
    public bool isRunning;
    public bool isWalking;

    public ToolEffect toolEffect;

    public bool isLiftingToolRight;
    public bool isLiftingToolLeft;
    public bool isLiftingToolUp;
    public bool isLiftingToolDown;

    public bool isUsingToolRight;
    public bool isUsingToolLeft;
    public bool isUsingToolUp;
    public bool isUsingToolDown;

    public bool isSwingToolRight;
    public bool isSwingToolLeft;
    public bool isSwingToolUp;
    public bool isSwingToolDown;

    public bool isPickingRight;
    public bool isPickingLeft;
    public bool isPickingUp;
    public bool isPickingDown;

    public bool idleUp;
    public bool idleDown;
    public bool idleLeft;
    public bool idleRight;

    private void Update()
    {
        EventHandler.CallMovementEvent(xInput, yInput, isWalking, isRunning, isIdle, isCarrying,
     toolEffect,
     isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
     isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
     isSwingToolRight, isSwingToolLeft, isSwingToolUp, isSwingToolDown,
     isPickingRight, isPickingLeft, isPickingUp, isPickingDown,
     idleUp, idleDown, idleLeft, idleRight);
    }
}
