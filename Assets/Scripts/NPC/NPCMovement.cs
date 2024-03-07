using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NPCPath))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class NPCMovement : MonoBehaviour
{
    public SceneName npcTargetScene;
    [HideInInspector] public SceneName npcCurrentScene;
    [HideInInspector] public Vector3Int npcCurrentGridPosition;
    [HideInInspector] public Vector3Int npcTargetGridPosition;
    [HideInInspector] public Vector3 npcTargetWorldPosition;
    public Direction npcFacingDirectionAtDestination;

    private SceneName npcPreviousMovementStepScene;
    private Vector3Int npcNextGridPosition;
    private Vector3 npcNextWorldPosition;

    [Header("NPC Movement")]
    public float npcNormalSpeed = 2f;

    [SerializeField] private float npcMinSpeed = 1f;
    [SerializeField] private float npcMaxSpeed = 3f;
    private bool npcIsMoving = false;

    [HideInInspector] public AnimationClip npcTargetAnimationClip;

    [Header("NPC Animation")]
    [SerializeField] private AnimationClip blankAnimation;

    private Grid grid;
    private Animator animator;
    private SpriteRenderer sr;
    private WaitForFixedUpdate waitForFixedUpdate;
    private Rigidbody2D rigi2D;
    private BoxCollider2D boxCollider2D;
    private AnimatorOverrideController animatorOverrideController;
    private NPCPath npcPath;
    private int lastMoveAnimationParameter;
    private bool npcInitialise = false;
    [HideInInspector] public bool npcActiveInScene = true;

    private bool sceneLoaded = false;

    private Coroutine moveToGridPostionCoroutine;

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += AfterSceneLoaded;
        EventHandler.BeforeSceneUnloadEvent += BeforeSceneUnloaded;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoaded;
        EventHandler.BeforeSceneUnloadEvent -= BeforeSceneUnloaded;
    }
    private void Awake()
    {
        rigi2D = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        npcPath = GetComponent<NPCPath>();
        animator = GetComponent<Animator>();

        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;

        npcTargetScene = npcCurrentScene;
        npcTargetGridPosition = npcCurrentGridPosition;
        npcTargetWorldPosition = transform.position;
    }

    private void Start()
    {
        waitForFixedUpdate = new WaitForFixedUpdate();
        SetIdleAnimation();
    }

    private void FixedUpdate()
    {
        if (sceneLoaded)
        {
            if (!npcIsMoving)
            {
                npcCurrentGridPosition = GetGridPosition(transform.position);
                npcNextGridPosition = npcCurrentGridPosition;

                if(npcPath.npcMovementStepStack.Count > 0)
                {
                    NPCMovementStep npcMovementStep = npcPath.npcMovementStepStack.Peek();

                    npcCurrentScene = npcMovementStep.sceneName;

                    if(npcCurrentScene != npcPreviousMovementStepScene)
                    {
                        npcCurrentGridPosition = (Vector3Int)npcMovementStep.gridCoordinate;
                        npcNextGridPosition = npcCurrentGridPosition;
                        transform.position = GetWorldPosition(npcNextGridPosition);
                        npcPreviousMovementStepScene = npcCurrentScene;
                        npcPath.UpdateTimeOnPath();
                    }

                    if(npcCurrentScene.ToString() == SceneManager.GetActiveScene().name)
                    {
                        SetNPCActiveInScene();
                        npcMovementStep = npcPath.npcMovementStepStack.Pop();
                        npcNextGridPosition = (Vector3Int)npcMovementStep.gridCoordinate;
                        TimeSpan npcMovementStepTime = new TimeSpan(npcMovementStep.hour, npcMovementStep.minute, npcMovementStep.second);
                        MoveToGridPosition(npcNextGridPosition, npcMovementStepTime, TimeManager.Instance.GetGameTime());
                    }

                    else
                    {
                        SetNPCInActiveInScene();

                        npcCurrentGridPosition = (Vector3Int)npcMovementStep.gridCoordinate;
                        npcNextGridPosition = npcCurrentGridPosition;
                        transform.position = GetWorldPosition(npcNextGridPosition);

                        TimeSpan npcMovementTimeSpan = new TimeSpan(npcMovementStep.hour, npcMovementStep.minute, npcMovementStep.second);

                        TimeSpan gameTime = TimeManager.Instance.GetGameTime();

                        if(npcMovementTimeSpan < gameTime)
                        {
                            npcMovementStep = npcPath.npcMovementStepStack.Pop();

                            npcCurrentGridPosition = (Vector3Int)npcMovementStep.gridCoordinate;
                            npcNextGridPosition = npcCurrentGridPosition;
                            transform.position = GetWorldPosition(npcNextGridPosition);
                        }
                    }
                }
                else
                {
                    ResetMoveAnimation();
                    SetNPCFacingDirection();
                    SetNPCEventAnimation();
                }
            }
        }
    }

    private void SetNPCEventAnimation()
    {
        if(npcTargetAnimationClip != null)
        {
            ResetIdleAnimation();
            animatorOverrideController[blankAnimation] = npcTargetAnimationClip;
            animator.SetBool(Settings.eventAnimation, true);
        }
        else
        {
            animatorOverrideController[blankAnimation] = blankAnimation;
            animator.SetBool(Settings.eventAnimation, false);
        }
    }

    public void ClearNPCEventAnimation()
    {
        animatorOverrideController[blankAnimation] = blankAnimation;
        animator.SetBool(Settings.eventAnimation, false);

        transform.rotation = Quaternion.identity;
    }

    private void SetNPCFacingDirection()
    {
        ResetIdleAnimation();

        switch (npcFacingDirectionAtDestination)
        {
            case Direction.up:
                animator.SetBool(Settings.idleUp, true);
                break;
            case Direction.down:
                animator.SetBool(Settings.idleDown, true);
                break;
            case Direction.left:
                animator.SetBool(Settings.idleLeft, true);
                break;
            case Direction.right:
                animator.SetBool(Settings.idleRight, true);
                break;

            default:
                break;
        }
    }

    private void ResetMoveAnimation()
    {
        animator.SetBool(Settings.walkDown,false);
        animator.SetBool(Settings.walkUp, false);
        animator.SetBool(Settings.walkLeft, false);
        animator.SetBool(Settings.walkRight, false);
    }

    private void ResetIdleAnimation()
    {
        animator.SetBool(Settings.idleDown, false);
        animator.SetBool(Settings.idleUp, false);
        animator.SetBool(Settings.idleLeft, false);
        animator.SetBool(Settings.idleRight, false);
    }

    private void MoveToGridPosition(Vector3Int gridPosition, TimeSpan npcMovementStepTime, TimeSpan gameTime)
    {
        moveToGridPostionCoroutine =  StartCoroutine(MoveToGridPositionCoroutine(gridPosition,npcMovementStepTime,gameTime));
    }

    private IEnumerator MoveToGridPositionCoroutine(Vector3Int gridPosition, TimeSpan npcMovementStepTime, TimeSpan gameTime)
    {
        npcIsMoving = true;
        SetMoveAnimation(gridPosition);

        npcNextWorldPosition = GetWorldPosition(gridPosition);

        if(npcMovementStepTime > gameTime)
        {
            float timeToMove = (float)(npcMovementStepTime.TotalSeconds - gameTime.TotalSeconds);
            float npcCaculatedSpeed = Mathf.Max(npcMinSpeed, Vector3.Distance(transform.position, npcNextWorldPosition)) / timeToMove / Settings.secondTimePerGameTimeSecond;

            //float npcCaculatedSpeed = Vector3.Distance(transform.position, npcNextWorldPosition) / timeToMove / Settings.secondTimePerGameTimeSecond;
            if (npcCaculatedSpeed <= npcMaxSpeed)
            {
                while(Vector3.Distance(transform.position,npcNextWorldPosition) > Settings.pixelSize)
                {
                    Vector3 unitVector = Vector3.Normalize(npcNextWorldPosition - transform.position);
                    Vector2 move = new Vector2(unitVector.x * npcCaculatedSpeed * Time.fixedDeltaTime, unitVector.y * npcCaculatedSpeed * Time.fixedDeltaTime);

                    rigi2D.MovePosition(rigi2D.position + move);

                    yield return waitForFixedUpdate;
                }
            }
        }

        rigi2D.position = npcNextWorldPosition;
        npcCurrentGridPosition = gridPosition;
        npcNextGridPosition = npcCurrentGridPosition;
        npcIsMoving = false;
    }

    private void SetMoveAnimation(Vector3Int gridPosition)
    {
        ResetIdleAnimation();
        ResetMoveAnimation();

        Vector3 toWorldPositon = GetWorldPosition(gridPosition);
        Vector3 directionVector = toWorldPositon - transform.position;

        if(Mathf.Abs(directionVector.x) >= Mathf.Abs(directionVector.y))
        {
            if(directionVector.x > 0)
            {
                animator.SetBool(Settings.walkRight, true);
            }
            else
            {
                animator.SetBool(Settings.walkLeft, true);
            }
        }
        else
        {
            if(directionVector.y > 0)
            {
                animator.SetBool(Settings.walkUp, true);
            }
            else
            {
                animator.SetBool(Settings.walkDown, true);
            }
        }
    }

    public void CancelNPCMovement()
    {
        npcPath.ClearPath();
        npcNextGridPosition = Vector3Int.zero;
        npcNextWorldPosition = Vector3Int.zero;
        npcIsMoving = false;

        if(moveToGridPostionCoroutine != null)
        {
            StopCoroutine(moveToGridPostionCoroutine);
        }

        ResetMoveAnimation();

        ClearNPCEventAnimation();
        npcTargetAnimationClip = null;

        ResetIdleAnimation();
        SetIdleAnimation();
    }

    public Vector3 GetWorldPosition(Vector3Int gridPosition)
    {
        Vector3 worldPosition = grid.CellToWorld(gridPosition);

        return new Vector3(worldPosition.x + Settings.gridCellSize / 2f, worldPosition.y + Settings.gridCellSize / 2f, worldPosition.z);
    }   

    public void SetNPCActiveInScene()
    {
        sr.enabled = true;
        boxCollider2D.enabled = true;
        npcActiveInScene = true;
    }

    public void SetNPCInActiveInScene()
    {
        sr.enabled = false;
        boxCollider2D.enabled = false;
        npcActiveInScene = false;
    }

    private Vector3Int GetGridPosition(Vector3 worldPosition)
    {
        if (grid != null)
        {
            return grid.WorldToCell(worldPosition);
        }
        else
        {
            return Vector3Int.zero;
        }
    }

    private void AfterSceneLoaded()
    {
        grid = FindObjectOfType<Grid>();

        if (!npcInitialise)
        {
            InitialiseNPC();
            npcInitialise = true;
        }

        sceneLoaded = true;
    }

    private void InitialiseNPC()
    {
        if(npcCurrentScene.ToString() == SceneManager.GetActiveScene().name)
        {
            SetNPCActiveInScene();
        }
        else
        {
            SetNPCInActiveInScene();
        }

        npcCurrentGridPosition = GetGridPosition(transform.position);

        npcNextGridPosition = npcCurrentGridPosition;
        npcTargetGridPosition = npcCurrentGridPosition;
        npcTargetWorldPosition = GetWorldPosition(npcTargetGridPosition);
        npcNextWorldPosition = GetWorldPosition(npcCurrentGridPosition);
    }

    private void BeforeSceneUnloaded()
    {
        sceneLoaded = false;
    }

    private void SetIdleAnimation()
    {
        animator.SetBool(Settings.idleDown, true);
    }
    public void SetScheduleEventDetails(NPCScheduleEvent npcScheduleEvent)
    {
        npcTargetScene = npcScheduleEvent.toSceneName;
        npcTargetGridPosition = (Vector3Int)npcScheduleEvent.toGridCoordinate;
        npcTargetWorldPosition = GetWorldPosition(npcTargetGridPosition);
        npcFacingDirectionAtDestination = npcScheduleEvent.npFacingDirectionAtDestination;
        npcTargetAnimationClip = npcScheduleEvent.animationAtDestination;
        ClearNPCEventAnimation();

    }
}
