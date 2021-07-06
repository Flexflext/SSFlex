using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    // Script for the Player to Look Around (Cam Control)

    [Header("Stats")]
    [SerializeField] private float sensX = 100f;
    [SerializeField] private float sensY = 100f;
    public float AdsMultiplier { get { return currentAdsSensMultiplier; } set { currentAdsSensMultiplier = Mathf.Clamp01(value); } }

    [Header("Refs")]
    [SerializeField] private Transform cam;
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform GFXHolder;

    private Rigidbody rb;

    private float mouseX;
    private float mouseY;

    private float currentAdsSensMultiplier = 1f;

    private float xRotation = 0f;
    private float yRotation = 0f;


    private void Start()
    {

        yRotation = cam.transform.rotation.y;
        xRotation = cam.transform.rotation.x;


        //Changes the Mouse Sens to the Game Managers
        //ChangeMouseSens();
        // Add Event Listener to GameManager Event
        //GameManager.Instance.OnMouseSensChange += ChangeMouseSens;

        //Lock the mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        //Check that timeScale is above 0
        if (Time.timeScale == 0)
        {
            return;
        }

        //Get mouse input
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        //Check how to rotate multiplier by Delta time and ads Multiplier + sens
        yRotation += mouseX * sensX * Time.deltaTime * currentAdsSensMultiplier;
        xRotation -= mouseY * sensY * Time.deltaTime * currentAdsSensMultiplier;

        //Clmap xRot between -90 and 90 deg
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // AddRotation to cams + GFX and orientation
        cam.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        GFXHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    /// <summary>
    /// Add Recoil to x and y rotation
    /// </summary>
    /// <param name="_xrecoil"></param>
    /// <param name="_yrecoil"></param>
    public void AddRecoil(float _xrecoil, float _yrecoil)
    {
        xRotation -= _xrecoil;
        yRotation += _yrecoil;
    }

    /// <summary>
    /// Change Mouse Sens to GamemNagewr Sens
    /// </summary>
    private void ChangeMouseSens()
    {
        //sensX = GameManager.Instance.MouseSens;
        //sensY = GameManager.Instance.MouseSens;
    }
}
