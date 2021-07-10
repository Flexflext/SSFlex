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
    [SerializeField] private LayerMask groundStoneMask;
    [SerializeField] private LayerMask groundGravelMask;

    [Header("Testing Stats")]
    [SerializeField] private Vector3 moveDir;

    [SerializeField] private float currentPlayerSpeed;
    [SerializeField] private float playerWalkingSpeed;
    [SerializeField] private float playerRunSpeed;
    [SerializeField] private float playerSneakingSpeed;

    [SerializeField] private float jumpForce;

    [SerializeField] private bool isOnStone;
    [SerializeField] private bool isOnGravel;
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
    private bool isGroundStoned;
    private bool isGroundGraveled;
    private bool useGravity = true;

    private bool isStoneWalking;
    private bool isStoneRunning;
    private bool isGravelWalking;
    private bool isGravelRunning;




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

        // LayerMask Checks
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        isGroundStoned = Physics.CheckSphere(groundCheck.position, groundDistance, groundStoneMask);
        isGroundGraveled = Physics.CheckSphere(groundCheck.position, groundDistance, groundGravelMask);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        // Audio
        AudioMixing();
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

    #region Audio
    private void AudioMixing()
    {
        if (isGroundStoned && !isGroundGraveled)
        {
            isOnStone = true;

            if (isMoving && !isStoneRunning)
            {
                isStoneWalking = true;
                AudioManager.Instance.PlayRandom("StoneWalk", 3);
            }

            if (isRunning && !isStoneWalking)
            {
                isStoneRunning = true;
                AudioManager.Instance.PlayRandom("StoneRun", 4);
                AudioManager.Instance.Play("BreathingRun");
            }
        }
        else
        {
            isOnStone = false;
        }
        
        if (isGroundGraveled && !isGroundStoned)
        {
            isOnGravel = true;

            if (isMoving && !isGravelRunning)
            {
                isGravelWalking = true;
                AudioManager.Instance.PlayRandom("GravelWalk", 1);
            }

            if (isRunning && !isGravelWalking)
            {
                isGravelRunning = true;
                AudioManager.Instance.PlayRandom("GravelRun", 2);
                AudioManager.Instance.Play("BreathingRun");
            }
        }
        else
        {
            isOnGravel = false;
        }


        if (!isOnStone)
        {
            AudioManager.Instance.Stop("StoneWalk");
            AudioManager.Instance.Stop("StoneRun");
            AudioManager.Instance.Stop("BreathingRun");
        }
        if (!isOnGravel)
        {
            AudioManager.Instance.Stop("GravelWalk");
            AudioManager.Instance.Stop("GravelRun");
            AudioManager.Instance.Stop("BreathingRun");
        }
        if (!isMoving || isSneaking)
        {
            AudioManager.Instance.Stop("GravelWalk");
            AudioManager.Instance.Stop("GravelRun");
            AudioManager.Instance.Stop("StoneWalk");
            AudioManager.Instance.Stop("StoneRun");
            AudioManager.Instance.Stop("BreathingRun");
        }

    }
    #endregion 
}
