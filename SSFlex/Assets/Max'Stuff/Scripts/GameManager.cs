using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool PreparationPhase => mPreparationPhase;
    public PrimaryWeapon StartWeapon => mStartWeapon;

    [Header("The lenght of the Preparation Phase")]
    [SerializeField]
    private float mMaxPreparationTime;
    private float mPreparationTimer;

    private OptionsManager mOptions;
    [SerializeField]
    private GameObject mEscapeMenu;


    private float mFov;
    private float mMouseSensitivity;
    private bool mPreparationPhase = true;

    private Scene mCurrentScene;

    private PrimaryWeapon mStartWeapon;

    private void Awake()
    {
        if (Instance == null)     
            Instance = this;
        else
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);


        mPreparationTimer = mMaxPreparationTime;
    }



    private void Update()
    {
        if(mPreparationPhase)
            CountPrepPhase();

        if(mEscapeMenu == null)
        {
            mCurrentScene = SceneManager.GetActiveScene();
            if (mCurrentScene.buildIndex > 1)
            {
                mEscapeMenu = GameObject.FindGameObjectWithTag("EscapeMenu");
                mEscapeMenu.SetActive(false);
            }
        }



        GetOptions();

        if (Input.GetKeyDown(KeyCode.Escape) && mEscapeMenu != null)
            ToggleEscapeMenu();
    }

    public void SetStartWeapon(PrimaryWeapon _startWeapon)
    {
        mStartWeapon = _startWeapon;
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
