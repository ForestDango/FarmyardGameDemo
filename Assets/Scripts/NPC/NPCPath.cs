using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPCMovement))]
public class NPCPath : MonoBehaviour
{
    public Stack<NPCMovementStep> npcMovementStepStack;
    private NPCMovement npcMovement;

    private void Awake()
    {
        npcMovement = GetComponent<NPCMovement>();
        npcMovementStepStack = new Stack<NPCMovementStep>();
    }
    
    public void ClearPath()
    {
        npcMovementStepStack.Clear();
    }

    public void BuildPath(NPCScheduleEvent npcScheduleEvent)
    {
        ClearPath();

        if(npcScheduleEvent.toSceneName == npcMovement.npcCurrentScene)
        {
            Vector2Int npcCurrentGridPositon = (Vector2Int)npcMovement.npcCurrentGridPosition;

            Vector2Int npcTargetGridPosition = (Vector2Int)npcScheduleEvent.toGridCoordinate;

            NPCManager.Instance.BuilPath(npcScheduleEvent.toSceneName, npcCurrentGridPositon, npcTargetGridPosition, npcMovementStepStack);            
        }
        else if(npcScheduleEvent.toSceneName != npcMovement.npcCurrentScene)
        {
            SceneRoute sceneRoute;
            sceneRoute = NPCManager.Instance.GetSceneRoute(npcMovement.npcCurrentScene.ToString(), npcScheduleEvent.toSceneName.ToString());

            if(sceneRoute != null)
            {
                for (int i = sceneRoute.scenePathList.Count - 1; i > 0; i--)
                {
                    int toGridX, toGridY, fromGirdX, fromGridY;
                    ScenePath scenePath = sceneRoute.scenePathList[i];

                    if(scenePath.toGridCell.x >= Settings.maxGridWidth || scenePath.toGridCell.y >= Settings.maxGridHeight)
                    {
                        toGridX = npcScheduleEvent.toGridCoordinate.x;
                        toGridY = npcScheduleEvent.toGridCoordinate.y;
                    }
                    else
                    {
                        toGridX = scenePath.toGridCell.x;
                        toGridY = scenePath.toGridCell.y;
                    }

                    if (scenePath.fromGridCell.x >= Settings.maxGridWidth || scenePath.fromGridCell.y >= Settings.maxGridHeight)
                    {
                        fromGirdX = npcMovement.npcCurrentGridPosition.x;
                        fromGridY = npcMovement.npcCurrentGridPosition.y;
                    }
                    else
                    {
                        fromGirdX = scenePath.fromGridCell.x;
                        fromGridY = scenePath.fromGridCell.y;
                    }
                    Vector2Int fromGridPosition = new Vector2Int(fromGirdX, fromGridY);
                    Vector2Int toGridPostion = new Vector2Int(toGridX, toGridY);

                    NPCManager.Instance.BuilPath(scenePath.sceneName, fromGridPosition, toGridPostion, npcMovementStepStack);
                }
            }
        }

        if (npcMovementStepStack.Count > 1)
        {
            UpdateTimeOnPath();
            npcMovementStepStack.Pop(); //discard starting step

            npcMovement.SetScheduleEventDetails(npcScheduleEvent);
        }
    }

    public void UpdateTimeOnPath()
    {
        TimeSpan currentGameTime = TimeManager.Instance.GetGameTime();

        NPCMovementStep previousNpcMovementStep = null;

        foreach (NPCMovementStep npcMovementStep in npcMovementStepStack)
        {
            if (previousNpcMovementStep == null)
                previousNpcMovementStep = npcMovementStep;

            npcMovementStep.hour = currentGameTime.Hours;
            npcMovementStep.minute = currentGameTime.Minutes;
            npcMovementStep.second = currentGameTime.Seconds;

            TimeSpan movementTimeStep;

            if (MovementIsDiagonal(npcMovementStep, previousNpcMovementStep))
            {
                movementTimeStep = new TimeSpan(0, 0, (int)(Settings.gridCellDiagnolSize / Settings.secondTimePerGameTimeSecond / npcMovement.npcNormalSpeed));
            }
            else
            {
                movementTimeStep = new TimeSpan(0, 0, (int)(Settings.gridCellSize / Settings.secondTimePerGameTimeSecond / npcMovement.npcNormalSpeed));
            }

            currentGameTime = currentGameTime.Add(movementTimeStep);

            previousNpcMovementStep = npcMovementStep;
        }
    }

    private bool MovementIsDiagonal(NPCMovementStep npcMovementStep, NPCMovementStep previousNpcMovementStep)
    {
        if((npcMovementStep.gridCoordinate.x != previousNpcMovementStep.gridCoordinate.x ) && 
            (npcMovementStep.gridCoordinate.y != previousNpcMovementStep.gridCoordinate.y))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
