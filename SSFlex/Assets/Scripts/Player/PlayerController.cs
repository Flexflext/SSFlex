using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    // This script is responsible for:
    // - Jumping
    // - Gravity
    // - (Potentially sneaking and sliding)



    [Header("References")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;

    [Header("Stats")]
    [SerializeField] private float jumpForce;

    // Invisible references
    private Rigidbody rb;

    // Invisible stats
    private float groundDistance = 0.4f;

    private bool isGrounded;
    private bool useGravity = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Responsible for:
    // - Groundcheck
    // - Keybinding: Jump
    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    // Responsible for realistic gravity physics.
    void FixedUpdate()
    {
        rb.useGravity = false;
        if (useGravity)
        {
            rb.AddForce(Physics.gravity * (rb.mass * rb.mass));
        }
    }

    private void Jump()
    {
        rb.AddForce(new Vector3(0, jumpForce));
    }
}
