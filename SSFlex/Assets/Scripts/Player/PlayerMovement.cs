using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // This script is responsible for: 
    // - Walking
    // - Running



    // Invisible Stats
    private float playerMovementSpeed = 20f;

    private bool isMoving;
    private bool isRunning;
    
    void FixedUpdate()
    {
        PlayerSmoothMovement();
        PlayerRun();
    }

    private void PlayerSmoothMovement()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(hor, 0f, ver).normalized * playerMovementSpeed * Time.deltaTime;
        transform.Translate(direction, Space.Self);

        if (direction.magnitude >= 0.1f)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }

    private void PlayerRun()
    {
        if (Input.GetKey(KeyCode.LeftShift) && isMoving == true)
        {
            isRunning = true;
            playerMovementSpeed = 20f;
        }
        else
        {
            isRunning = false;
            playerMovementSpeed = 10f;
        }
    }
}
