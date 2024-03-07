using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(AStar))]
public class AStarTest : MonoBehaviour
{
    [SerializeField] private NPCPath npcPath;
    [SerializeField] private bool moveNpc = false;
    [SerializeField] private SceneName sceneName;
    [SerializeField] private Vector2Int finishPosition;
    [SerializeField] private AnimationClip idleDownAnimationClip;
    [SerializeField] private AnimationClip eventAnimationClip;
    private NPCMovement npcMovement;

    private void Start()
    {
        npcMovement = npcPath.GetComponent<NPCMovement>();
        npcMovement.npcFacingDirectionAtDestination = Direction.down;
        npcMovement.npcTargetAnimationClip = idleDownAnimationClip;
    }

    private void Update()
    {
        if (moveNpc)
        {
            moveNpc = false;

            NPCScheduleEvent npcScheduleEvent = new NPCScheduleEvent(0, 0, 0, 0,Weather.none, Season.none, sceneName, 
                new GridCoordinate(finishPosition.x, finishPosition.y), eventAnimationClip);

            npcPath.BuildPath(npcScheduleEvent);
        }
    }
    //private AStar aStar;
    //[SerializeField] private Vector2Int startPosition;
    //[SerializeField] private Vector2Int finishPosition;
    //[SerializeField] private Tilemap tileMapToDisplayPathOn = null;
    //[SerializeField] private TileBase tileToUseToDisplayPath = null;
    //[SerializeField] private bool displayeStartAndFinish = false;
    //[SerializeField] private bool displayPath = false;

    //private Stack<NPCMovementStep> npcMovementSteps;

    //private void Awake()
    //{
    //    aStar = GetComponent<AStar>();

    //    npcMovementSteps = new Stack<NPCMovementStep>();
    //}


    //private void Update()
    //{
    //    if(startPosition != null && finishPosition != null && tileToUseToDisplayPath != null && tileToUseToDisplayPath != null)
    //    {
    //        if(displayeStartAndFinish)
    //        {
    //            tileMapToDisplayPathOn.SetTile(new Vector3Int(startPosition.x, startPosition.y,0),tileToUseToDisplayPath);
    //            tileMapToDisplayPathOn.SetTile(new Vector3Int(finishPosition.x, finishPosition.y, 0), tileToUseToDisplayPath);
    //        }
    //        else
    //        {
    //            tileMapToDisplayPathOn.SetTile(new Vector3Int(startPosition.x, startPosition.y, 0), null);
    //            tileMapToDisplayPathOn.SetTile(new Vector3Int(finishPosition.x, finishPosition.y, 0), null);
    //        }

    //        if (displayPath)
    //        {
    //            Enum.TryParse<SceneName>(SceneManager.GetActiveScene().name, out SceneName sceneName);

    //            aStar.BuildPath(sceneName, startPosition, finishPosition, npcMovementSteps);

    //            foreach (NPCMovementStep npcMovementStep in npcMovementSteps)
    //            {
    //                tileMapToDisplayPathOn.SetTile(new Vector3Int(npcMovementStep.gridCoordinate.x, npcMovementStep.gridCoordinate.y, 0), tileToUseToDisplayPath);
    //            }
    //        }
    //        else
    //        {
    //            if(npcMovementSteps.Count > 0)
    //            {
    //                foreach (NPCMovementStep nPCMovementStep in npcMovementSteps) 
    //                {
    //                    tileMapToDisplayPathOn.SetTile(new Vector3Int(nPCMovementStep.gridCoordinate.x, nPCMovementStep.gridCoordinate.y, 0), null);
    //                }

    //                npcMovementSteps.Clear();
    //            }
    //        }
    //    }
    //}
}
