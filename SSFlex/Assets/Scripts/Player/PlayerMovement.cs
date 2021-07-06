using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // This script is responsible for: 
    // - Walking
    // - Running
    [SerializeField] private Vector3 moveDir;

    [SerializeField] private float currentPlayerSpeed;
    [SerializeField] private float playerWalkingSpeed;
    [SerializeField] private float playerRunSpeed;
    [SerializeField] private float playerSneakSpeed;

    private PlayerLook playerLook;
    private Rigidbody rb;

    // Invisible Stats
    
    public float dashSpeed;
    public float dashTime;

    private bool isMoving;
    private bool isRunning;
    private bool isDashing;
    private bool isSneaking;


    private void Awake()
    {
        playerLook = GetComponent<PlayerLook>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        currentPlayerSpeed = playerWalkingSpeed;
    }

    private void Update()
    {
        MoveDirection();

        PlayerRun();
        PlayerSneak();
        
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
            currentPlayerSpeed = playerWalkingSpeed;
            // Normal footsteps
        }
        else
        {
            isMoving = false;
        }
    }

    private void PlayerRun()
    {
        if (Input.GetKey(KeyCode.LeftShift) && isSneaking == false && isMoving == true)
        {
            currentPlayerSpeed = playerRunSpeed;
            isRunning = true;
            
            // Louder Footsteps
        }
        else
        {
            isRunning = false;
        }
    }

    private void PlayerSneak()
    {
        if (Input.GetKey(KeyCode.LeftControl) && isRunning == false && isMoving == true)
        {
            currentPlayerSpeed = playerSneakSpeed;
            isSneaking = true;

            // No footsteps
        }
        else
        {
            isSneaking = false;
        }
    }


    IEnumerator QuickDash()
    {
        isDashing = true;
        rb.AddForce(moveDir * dashSpeed, ForceMode.Impulse);
        yield return new WaitForSeconds(dashTime);
        isDashing = false;
        rb.velocity = Vector3.zero;
        
        // RotS DashSound
    }
}
