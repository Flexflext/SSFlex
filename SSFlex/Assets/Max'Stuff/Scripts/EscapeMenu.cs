using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EscapeMenu : MonoBehaviour
{
    public static EscapeMenu Instance;


    [SerializeField]
    private GameObject mEscapeMenuContent;

    [SerializeField]
    private GameObject mPlayerHud;
    [SerializeField]
    private GameObject mOptionsMenu;

    public System.Action OnToggle;

    private void Awake()
    {
        Instance = this;

        ToggleEscapeMenu();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ToggleEscapeMenu();
    }

    private void ToggleEscapeMenu()
    {
        if (OnToggle != null)
        {
            OnToggle.Invoke();
        }

        if (mEscapeMenuContent.activeSelf)
        {
            mPlayerHud.SetActive(true);
            mEscapeMenuContent.SetActive(false);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            mPlayerHud.SetActive(false);
            mEscapeMenuContent.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void ToggleOptionsMenu()
    {  

        if (mOptionsMenu.activeSelf)
        {
            mOptionsMenu.SetActive(false);           
        }
        else
        {
            mOptionsMenu.SetActive(true);  
        }
            
    }

    public void ResumeGame()
    {
        ToggleEscapeMenu();
    }

    public void LoadMainMenu()
    {
        Destroy(RoomManager.Instance.gameObject);
        PhotonNetwork.LoadLevel(0);
    }
}
