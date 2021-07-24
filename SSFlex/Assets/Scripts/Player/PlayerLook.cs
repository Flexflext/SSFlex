using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerLook : MonoBehaviourPunCallbacks
{
    // Script von Felix
    // Purpose: Script for the Player to Look Around (Cam Control)


    [Header("Stats")]
    [SerializeField] private float sensX = 100f;
    [SerializeField] private float sensY = 100f;
    public float AdsMultiplier { get { return currentAdsSensMultiplier; } set { currentAdsSensMultiplier = Mathf.Clamp01(value); } }

    [Header("Refs")]
    public Transform cam;
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform GFXHolder;


    private float mouseX;
    private float mouseY;

    private float currentAdsSensMultiplier = 1f;

    private float xRotation = 0f;
    private float yRotation = 0f; 

    private bool canLook = true;


    private void Start()
    {
        // Sub to Toggle Event 
        EscapeMenu.Instance.OnToggle += ChangeCanLook;

        // Check that that the Script is Only Run if the Owner
        if (!photonView.IsMine)
        {
            return;
        }

        yRotation = cam.transform.rotation.y;
        xRotation = cam.transform.rotation.x;


        //Changes the Mouse Sens to the Game Managers
        ChangeMouseSens();
        // Add Event Listener to GameManager Event
        GameManager.Instance.OnMouseSensChange += ChangeMouseSens;

        //Lock the mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Check that that the Script is Only Run if the Owner
        if (!photonView.IsMine)
        {
            return;
        }

        //Check that timeScale is above 0
        if (!canLook)
        {
            return;
        }

        //Get mouse input
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        //Check how to rotate multiplier by Delta time and ads Multiplier + sens
        yRotation += mouseX * sensX * Time.deltaTime * currentAdsSensMultiplier * Screen.height;
        xRotation -= mouseY * sensY * Time.deltaTime * currentAdsSensMultiplier * Screen.width;

        //Clmap xRot between -90 and 90 deg
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // AddRotation to cams + GFX and orientation
        //cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, Quaternion.Euler(xRotation, yRotation, 0), Time.deltaTime);
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        GFXHolder.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    /// <summary>
    /// Add Recoil to x and y rotation
    /// </summary>
    /// <param name="_xrecoil"></param>
    /// <param name="_yrecoil"></param>
    public void AddRecoil(float _xrecoil, float _yrecoil)
    {
        // Check that that the Script is Only Run if the Owner
        if (!photonView.IsMine)
        {
            return;
        }

        xRotation -= _xrecoil;
        yRotation += _yrecoil;
    }

    /// <summary>
    /// Change Mouse Sens to GamemNagewr Sens
    /// </summary>
    private void ChangeMouseSens()
    {
        sensX = GameManager.Instance.MouseSensitivity;
        sensY = GameManager.Instance.MouseSensitivity;
    }

    /// <summary>
    /// Change the bool if the Player Can Look Around
    /// </summary>
    private void ChangeCanLook()
    {
        Debug.Log("Hier");
        canLook = !canLook;
    }

    
    private void OnDestroy()
    {
        // Unsub from Toggle Event
        GameManager.Instance.OnMouseSensChange -= ChangeMouseSens;
    }

}
