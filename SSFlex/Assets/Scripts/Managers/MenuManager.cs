using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


/// <summary>
/// Written by Max
/// 
/// This Script Manages the different menus in the Main Menu Scene
/// </summary>
public class MenuManager : MonoBehaviour
{
    // Code: Haoke & Max
    // Responsible for all the menu methods.

    public static MenuManager Instance;

    [SerializeField]
    private float mRoomFullDisplayDuration;
    

    [SerializeField]
    private GameObject mMainMenu;
    [SerializeField]
    private GameObject mLoadingScreen;
    [SerializeField]
    private GameObject mOptionsMenu;
    [SerializeField]
    private GameObject mCreateRoomMenu;
    [SerializeField]
    private GameObject mRoomMenu;
    [SerializeField]
    private GameObject mErrorMenu;
    [SerializeField]
    private GameObject mFindRoomMenu;
    [SerializeField]
    private GameObject mNameMenu;

    [SerializeField]
    private TMP_InputField mPlayerNameInput;
    [SerializeField]
    private TMP_InputField mServerNameInput;
    [SerializeField]
    private TextMeshProUGUI mEnterRoomNameText;
    [SerializeField]
    private TextMeshProUGUI mEnterPlayerNameText;
    [SerializeField]
    private TextMeshProUGUI mText_RoomFull;

    [SerializeField]
    private string[] mEnterRoomNameMessages;
    [SerializeField]
    private string[] mEnterPlayerNameMessages;

    [SerializeField]
    private GameObject mMasterStartButton;
    [SerializeField]
    private GameObject mClientStartButton;

    private List<GameObject> mAllMenus;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }


    private void Start()
    {
        mAllMenus = new List<GameObject>()
        {
            mLoadingScreen,
            mOptionsMenu,
            mCreateRoomMenu,
            mRoomMenu,
            mErrorMenu,
            mFindRoomMenu,
            mNameMenu
        };
    }

    private void Update()
    {
        if (mRoomMenu.activeSelf)
            mMainMenu.SetActive(false);
    }


    #region Toggles the various Menus
    public void AdminMainMenu()
    {
        if (mMainMenu != null)
        {
            if (mMainMenu.activeSelf)
                mMainMenu.SetActive(false);
            else
                mMainMenu.SetActive(true);
        }

        CloseMenus(mMainMenu);
    }

    public void AdminLoadingMenu()
    {
        if (mLoadingScreen != null)
        {
            if (mLoadingScreen.activeSelf)
                mLoadingScreen.SetActive(false);
            else
                mLoadingScreen.SetActive(true);
        }

        CloseMenus(mLoadingScreen);
    }

    public void AdmitOptionsMenu()
    {
        if (mOptionsMenu != null)
        {
            if (mOptionsMenu.activeSelf)
                mOptionsMenu.SetActive(false);
            else
                mOptionsMenu.SetActive(true);
        }

        CloseMenus(mOptionsMenu);
    }

    public void AdminNameMenu()
    {
        if (mCreateRoomMenu.activeSelf)
            mCreateRoomMenu.SetActive(false);

        Debug.Log("DDDD");

        if (!mServerNameInput.isActiveAndEnabled || mServerNameInput.text.Length > 0)
        {
            if (mNameMenu != null)
            {
                if (mNameMenu.activeSelf)
                    mNameMenu.SetActive(false);
                else
                    mNameMenu.SetActive(true);

                Launcher.Instance.SetRoomName(mServerNameInput.text);

                mEnterRoomNameText.gameObject.SetActive(false);
            }
            CloseMenus(mNameMenu);
        }
        else
        {
            mEnterRoomNameText.gameObject.SetActive(true) ;
            int rndIdx = Random.Range(0, mEnterRoomNameMessages.Length);
            mEnterRoomNameText.text = mEnterRoomNameMessages[rndIdx];
        }
    }

    public void AdminCreateRoomMenu()
    {
        if (mCreateRoomMenu != null)
        {
            if (mCreateRoomMenu.activeSelf)
                mCreateRoomMenu.SetActive(false);
            else
                mCreateRoomMenu.SetActive(true);
        }

        CloseMenus(mCreateRoomMenu);
    }

    public void AdminRoomMenu()
    {
        if(mPlayerNameInput.text.Length > 0)
        {
            if (mRoomMenu != null)
            {
                if (mRoomMenu.activeSelf)
                    mRoomMenu.SetActive(false);
                else
                    mRoomMenu.SetActive(true);

                mEnterPlayerNameText.gameObject.SetActive(false);
            }
            CloseMenus(mRoomMenu);
        }
        else
        {
            mEnterPlayerNameText.gameObject.SetActive(true);
            int rndIdx = Random.Range(0, mEnterPlayerNameMessages.Length);
            mEnterPlayerNameText.text = mEnterPlayerNameMessages[rndIdx];
        }
    }

   
    public void AdminErrorMenu()
    {
        if (mErrorMenu != null)
        {
            if (mErrorMenu.activeSelf)
                mErrorMenu.SetActive(false);
            else
                mErrorMenu.SetActive(true);
        }

        CloseMenus(mErrorMenu);
    }

    public void AdminFindRoomMenu()
    {
        if (mPlayerNameInput.text.Length > 0)
        {
            if (mFindRoomMenu != null)
            {
                if (mFindRoomMenu.activeSelf)
                    mFindRoomMenu.SetActive(false);
                else
                    mFindRoomMenu.SetActive(true);

                

                mEnterPlayerNameText.gameObject.SetActive(false);
            }

            CloseMenus(mFindRoomMenu);
        }
        else
        {
            mEnterPlayerNameText.gameObject.SetActive(true);
            int rndIdx = Random.Range(0, mEnterPlayerNameMessages.Length);
            mEnterPlayerNameText.text = mEnterPlayerNameMessages[rndIdx];
        }
    }
    private void CloseMenus(GameObject _menuToCheck)
    {
        foreach (GameObject menu in mAllMenus)
        {
            if (menu != _menuToCheck && menu != null)
                menu.SetActive(false);
        }
    }

    public void AdminServerFullMenu()
    {
        StartCoroutine(RommFullDisplay());
    }
    #endregion

    /// <summary>
    /// Displays a message if the room is full and the player tries to enter
    /// </summary>
    private IEnumerator RommFullDisplay()
    {
        mText_RoomFull.gameObject.SetActive(true);
        new WaitForSeconds(mRoomFullDisplayDuration);
        mText_RoomFull.gameObject.SetActive(false);

        StopCoroutine(RommFullDisplay());
        return null;
    }

    /// <summary>
    /// Creates a room from the launcher
    /// </summary>
    public void CreateRoomFromLauncher()
    {
        if(mPlayerNameInput.text.Length > 0)
            Launcher.Instance.CreateRoom();
    }


    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Destroyes the Instances for the Game and Room Manager so they dont double in the main menu
    /// </summary>
    public void LoadMainMenu()
    {
        Destroy(GameManager.Instance.gameObject);
        Destroy(RoomManager.Instance.gameObject);
        PhotonNetwork.LoadLevel(0);
    }
}
