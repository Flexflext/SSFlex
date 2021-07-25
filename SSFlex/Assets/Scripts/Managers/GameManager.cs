using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;


/// <summary>
/// Written by Max
/// 
/// Manages some Game Infos
/// </summary>
public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;

    public System.Action OnFovChange;
    public System.Action OnMouseSensChange;

    public int MaxPlayer => mMaxPlayer;
    // Gets and sets mouse Sensitivity and FOV
    public float Fov { get{ return mFov; } set { mFov = value; } }
    public float MouseSensitivity { get { return mMouseSensitivity; } set { mMouseSensitivity = value; } }
    public EWeaponsAndUtensils StartWeapon => mStartWeapon;

    [SerializeField]
    private int mMaxPlayer;

    private OptionsManager mOptions;

    private float mFov = 100f;
    private float mMouseSensitivity = 0.2f;

    private EWeaponsAndUtensils mStartWeapon;

    private void Awake()
    {
        if (Instance == null)     
            Instance = this;
        else
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);     
    }

    private void Update()
    {
        GetOptions();
    }

    public void SetStartWeapon(EWeaponsAndUtensils _startWeapon)
    {
        mStartWeapon = _startWeapon;
    }

    /// <summary>
    /// Gets the chosen oprion of the player and safes them
    /// </summary>
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
