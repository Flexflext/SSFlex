using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    // This script is responsible for:
    // - Change of Speed
    // - Walking
    // - Running
    // - Sneaking
    // - Jumping
    // - Gravity
    // - Movement Animations
    // - Movement Audio

    #region Visible
    [Header("References")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;

    [Header("Testing Stats")]
    [SerializeField] private Vector3 moveDir;

    [SerializeField] private float currentPlayerSpeed;
    [SerializeField] private float playerWalkingSpeed;
    [SerializeField] private float playerRunSpeed;
    [SerializeField] private float playerSneakingSpeed;

    [SerializeField] private float jumpForce;
    #endregion

    #region Non-Visible
    // Non-visible References

    private PlayerShooting shooting;
    private Rigidbody rb;
    private Animator animator;

    // Non-visible Stats

    private float movementMultiplier = 1;
    public float MovementMultiplier { get { return movementMultiplier; } set { movementMultiplier = Mathf.Clamp01(value); } }

    private bool isMoving;
    private bool isRunning;
    private bool isSneaking;

    private float groundDistance = 0.4f;
    private bool isGrounded;
    private bool useGravity = true;
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        shooting = GetComponent<PlayerShooting>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        currentPlayerSpeed = playerWalkingSpeed;
    }

    private void Update()
    {
        // Movement
        MoveDirection();

        PlayerRun();
        PlayerSneak();

        // Jump
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector3(moveDir.x * currentPlayerSpeed * Time.fixedDeltaTime, rb.velocity.y, moveDir.z * currentPlayerSpeed * Time.fixedDeltaTime);

        rb.useGravity = false;
        if (useGravity)
        {
            rb.AddForce(Physics.gravity * (rb.mass * rb.mass));
        }
    }

    #region OnGround_MovementPatterns
    private void MoveDirection()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");

        moveDir = transform.forward * ver + transform.right * hor;
        moveDir.Normalize();


        if (moveDir.magnitude >= 0.1f)
        {
            isMoving = true;

            if (isRunning)
            {
                animator.SetFloat("Velocity", 1f);
            }
            else
            {
                animator.SetFloat("Velocity", 0.5f);
            }
            
        }
        else
        {
            isMoving = false;
            animator.SetFloat("Velocity", 0);
        }
    }

    private void PlayerRun()
    {
        if (Input.GetKey(KeyCode.LeftShift) && isMoving == true && !shooting.ImAiming)
        {
            isRunning = true;
            shooting.InterruptReload();
            currentPlayerSpeed = playerRunSpeed;
        }
        else
        {
            currentPlayerSpeed = playerWalkingSpeed * movementMultiplier;
            isRunning = false;
        }
    }


    private void PlayerSneak()
    {
        if (Input.GetKey(KeyCode.LeftControl) && isMoving == true)
        {
            currentPlayerSpeed = playerSneakingSpeed;
            isSneaking = true;

            // No footsteps
        }
        else
        {
            isSneaking = false;
        }
    }

    private void Jump()
    {
        rb.AddForce(new Vector3(0, jumpForce));
    }
    #endregion


}
