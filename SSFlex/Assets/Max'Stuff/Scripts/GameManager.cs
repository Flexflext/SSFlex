using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private float mFov;
    private float mMouseSensitivity;

    private OptionsManager mOptions;
    private GameObject mEscapeMenu;

    private Scene mCurrentScene;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;

        mCurrentScene = SceneManager.GetActiveScene();
    }


    private void Update()
    {
        GetOptions();

        if (mEscapeMenu == null && mCurrentScene != SceneManager.GetSceneByName("MainMenu") && mCurrentScene != SceneManager.GetSceneByName("Lobby"))
        {
            mEscapeMenu = GameObject.FindGameObjectWithTag("EscapeMenu");
            mEscapeMenu.SetActive(false);
            mOptions.gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Escape) && mEscapeMenu != null)
            ToggleEscapeMenu();
    }

    private void ToggleEscapeMenu()
    {
        if (!mEscapeMenu.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
            mEscapeMenu.SetActive(true);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            mEscapeMenu.SetActive(false);
        }

    }

    private void GetOptions()
    {
        if (mOptions == null)
            mOptions = FindObjectOfType<OptionsManager>();

        if(mOptions != null)
        {
            mFov = mOptions.Fov;
            mMouseSensitivity = mOptions.MouseSensitivity;
        }   
    }
}
