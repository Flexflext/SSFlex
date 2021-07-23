using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EscapeMenu : MonoBehaviourPunCallbacks
{
    public static EscapeMenu Instance;


    [SerializeField]
    private GameObject mEscapeMenuContent;

    [SerializeField]
    private GameObject mPlayerHud;
    [SerializeField]
    private GameObject mOptionsMenu;
    [SerializeField] private GameObject endMenu;
    [SerializeField] private GameObject startAgainButton;
    [SerializeField] private GameObject waitForAdminText;
    [SerializeField] private TMPro.TMP_Text endText;
    [SerializeField] private TMPro.TMP_Text winnerText;

    public System.Action OnToggle;

    private bool roundEnded;

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

            if (roundEnded)
            {
                endMenu.SetActive(true);
            }

            if (!endMenu.activeSelf)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            
        }
        else
        {
            mPlayerHud.SetActive(false);
            mEscapeMenuContent.SetActive(true);

            if (roundEnded)
            {
                endMenu.SetActive(false);
            }
            

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
        PhotonNetwork.LeaveRoom();
        LevelManager.Instance.StartTime();

        LevelManager.Instance.UpdateDictionary(PhotonNetwork.NetworkingClient.UserId, false);

        if (LevelManager.Instance.CheckIfTheOnlyOneAlive())
        {
            LevelManager.Instance.CheckIfWon();
        }

        Destroy(RoomManager.Instance.gameObject);
        PhotonNetwork.LoadLevel(0);
    }

    public void RestartRound()
    {
        LevelManager.Instance.StartTime();
        PhotonNetwork.LoadLevel(1);
    }


    public void OpenEndMenu(string _message, string _winner)
    {
        if (OnToggle != null)
        {
            OnToggle.Invoke();
        }

        roundEnded = true;

        endMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        endText.text = _message;
        winnerText.text = $"Winner: {_winner}";

        if (PhotonNetwork.NetworkingClient.LocalPlayer.IsMasterClient)
        {
            startAgainButton.SetActive(true);
        }
        else
        {
            waitForAdminText.SetActive(true);
        }
    }
}
