using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // This script is responsible for: 
    // - Walking
    // - Running
    [SerializeField] private Vector3 moveDir;

    [SerializeField] private float playerWalkingSpeed;
    [SerializeField] private float playerRunSpeed;
    [SerializeField] private float currentPlayerSpeed;

    private float movementMultiplier = 1;
    public float MovementMultiplier { get { return movementMultiplier; } set { movementMultiplier = Mathf.Clamp01(value); } }

    private PlayerLook playerLook;
    private PlayerShooting shooting;
    private Rigidbody rb;
    private Animator animator;

    // Invisible Stats
    
    public float dashSpeed;
    public float dashTime;

    private bool isMoving;
    private bool isRunning;
    private bool isDashing;


    private void Awake()
    {
        playerLook = GetComponent<PlayerLook>();
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
        MoveDirection();

        PlayerRun();

        if (Input.GetKeyDown(KeyCode.E) && isDashing == false)
        {
            StartCoroutine(QuickDash());
        }
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector3(moveDir.x * currentPlayerSpeed * Time.fixedDeltaTime, rb.velocity.y, moveDir.z * currentPlayerSpeed * Time.fixedDeltaTime);
    }

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


    IEnumerator QuickDash()
    {
        rb.AddForce(moveDir * dashSpeed, ForceMode.Impulse);
        yield return new WaitForSeconds(dashTime);
        rb.velocity = Vector3.zero;
    }
}
