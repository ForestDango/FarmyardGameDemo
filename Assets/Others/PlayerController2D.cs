using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2D : MonoBehaviour
{ 
    Camera mainCamera;

    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float runSpeed = 8f;

    private float horizontal;
    private float vertical;
    private Vector2 moveVector;

    private float lastHorizontal;
    private float lastVertical;
    public Vector2 lastMotionVector;
    private bool isMoving;
    private bool isRunning;

    Rigidbody2D rigi2D;
    Animator animator;

    private bool _playerInputDisabled = false;

    public bool PlayerInputDisabled
    {
        get => _playerInputDisabled;
        set => _playerInputDisabled = value;
    }

    private  void Awake()
    {
        mainCamera = Camera.main;
        rigi2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isRunning = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRunning = false;
        }
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        moveVector.x = horizontal;
        moveVector.y = vertical;

        animator.SetFloat("horizontal", horizontal);
        animator.SetFloat("vertical", vertical);

        isMoving = horizontal != 0 || vertical != 0;
        animator.SetBool("isMoving", isMoving);

        if (horizontal != 0 || vertical != 0)
        {
            lastMotionVector = new Vector2(horizontal, vertical).normalized;

            animator.SetFloat("lastHorizontal", horizontal);
            animator.SetFloat("lastVertical", vertical);
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void OnDisable()
    {
        rigi2D.velocity = Vector2.zero;
    }

    private void Move()
    {
        rigi2D.velocity = moveVector * (isRunning ? runSpeed : moveSpeed);
    }

    public Vector3 GetPlayerVierportPosition()
    {
        return mainCamera.WorldToViewportPoint(transform.position);
    }
}
