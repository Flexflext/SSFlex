using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool PreparationPhase => mPreparationPhase;

    [Header("The lenght of the Preparation Phase")]
    [SerializeField]
    private float mMaxPreparationTime;
    private float mPreparationTimer;

    private OptionsManager mOptions;
    private GameObject mEscapeMenu;

    private Scene mCurrentScene;

    private float mFov;
    private float mMouseSensitivity;
    private bool mPreparationPhase = true;

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
        mPreparationTimer = mMaxPreparationTime;
    }


    private void Update()
    {
        if(mPreparationPhase)
            CountPrepPhase();

        GetOptions();

        //if (mEscapeMenu == null && mCurrentScene != SceneManager.GetSceneByName("MainMenu") || mCurrentScene != SceneManager.GetSceneByName("Lobby"))
        //{
        //    mEscapeMenu = GameObject.FindGameObjectWithTag("EscapeMenu");
        //    mEscapeMenu.SetActive(false);
        //    mOptions.gameObject.SetActive(false);
        //}

        if (Input.GetKeyDown(KeyCode.Escape) && mEscapeMenu != null)
            ToggleEscapeMenu();
    }

    private void CountPrepPhase()
    {
        mPreparationTimer -= Time.deltaTime;

        if (mPreparationTimer <= 0)
            mPreparationPhase = false;
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
