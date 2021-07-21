using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;

    public int MaxPlayer => mMaxPlayer;
    public float Fov => mFov;
    public float MouseSensitivity => mMouseSensitivity;
    public EWeaponsAndUtensils StartWeapon => mStartWeapon;

    [SerializeField]
    private int mMaxPlayer;

    private OptionsManager mOptions;

    private float mFov;
    private float mMouseSensitivity;

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

    [PunRPC]
    public void SetStartWeapon(EWeaponsAndUtensils _startWeapon)
    {
        mStartWeapon = _startWeapon;
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
