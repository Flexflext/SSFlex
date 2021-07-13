using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            mFindRoomMenu
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

        CloseMenu(mMainMenu);
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

        CloseMenu(mLoadingScreen);
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

    public void AdminCreateRoomMenu()
    {
        if (mCreateRoomMenu != null)
        {
            if (mCreateRoomMenu.activeSelf)
                mCreateRoomMenu.SetActive(false);
            else
                mCreateRoomMenu.SetActive(true);
        }

        CloseMenu(mCreateRoomMenu);
    }

    public void AdminRoomMenu()
    {
        if (mRoomMenu != null)
        {
            if (mRoomMenu.activeSelf)
                mRoomMenu.SetActive(false);
            else
                mRoomMenu.SetActive(true);
        }

        CloseMenu(mRoomMenu);
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

        CloseMenu(mErrorMenu);
    }

    public void AdminFindRoomMenu()
    {
        if (mFindRoomMenu != null)
        {
            if (mFindRoomMenu.activeSelf)
                mFindRoomMenu.SetActive(false);
            else
                mFindRoomMenu.SetActive(true);
        }

        CloseMenu(mFindRoomMenu);
    }

    private void CloseMenu(GameObject _menuToCheck)
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
}
