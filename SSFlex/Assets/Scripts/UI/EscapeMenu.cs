using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// Written by Max
/// 
/// THis Script Manages the Escape Menu in the Main Scene
/// </summary>
public class EscapeMenu : MonoBehaviourPunCallbacks
{
    public static EscapeMenu Instance;

    // The content of the Escape Menu
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
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ToggleEscapeMenu();
    }

    /// <summary>
    /// Toggle the Escape Menu 
    /// </summary>
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

    /// <summary>
    /// Toggles the Options Menu
    /// </summary>
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

    /// <summary>
    /// Load the Player back into the Main Menu
    /// 
    /// 1. Starts Coroutine to initialize the Disconnect
    /// </summary>
    public void LoadMainMenu()
    {
        StartCoroutine(Disconnect());

        LevelManager.Instance.UpdateDictionary(PhotonNetwork.NetworkingClient.UserId, false);

        if (LevelManager.Instance.CheckIfTheOnlyOneAlive())
        {
            LevelManager.Instance.CheckIfWon();
        }
    }

    /// <summary>
    /// Coroutine to Disconnect the Player from the Server and load him back into the main menu
    /// 
    /// 1. Tries to Disconnect from the Server
    /// 2. If the player is disconnected the RoomManager and GameManager instances will be destroyed to negate a photonViewID doubling
    /// 3. Load the Main Menu
    /// </summary>
    private IEnumerator Disconnect()
    {
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
            yield return null;

        Destroy(RoomManager.Instance.gameObject);
        Destroy(GameManager.Instance.gameObject);
        PhotonNetwork.LoadLevel(0);
    }

    /// <summary>
    /// 
    /// </summary>
    public void RestartRound()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

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
