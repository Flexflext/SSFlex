using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

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
    private string[] mEnterRoomNameMessages;
    [SerializeField]
    private string[] mEnterPlayerNameMessages;

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
            mMainMenu,
            mLoadingScreen,
            mOptionsMenu,
            mCreateRoomMenu,
            mRoomMenu,
            mErrorMenu,
            mFindRoomMenu,
            mNameMenu
        };
    }


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
    }

    public void AdminNameMenu()
    {
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

    //public void SetRoomNameInLauncher()
    //{
    //    if(mServerNameInput)
    //}

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
            Debug.Log(mPlayerNameInput.text.Length);

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

    public void CreateRoomFromLauncher()
    {
        if(mPlayerNameInput.text.Length > 0)
            Launcher.Instance.CreateRoom();
    }

    private void CloseMenus(GameObject _menuToCheck)
    {
        foreach (GameObject menu in mAllMenus)
        {
            if (menu != _menuToCheck)
                menu.SetActive(false);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadMainMenu()
    {
        Destroy(RoomManager.Instance.gameObject);
        PhotonNetwork.LoadLevel(0);
    }
}
