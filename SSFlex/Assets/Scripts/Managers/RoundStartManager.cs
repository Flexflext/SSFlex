using Photon.Pun;
using TMPro;
using UnityEngine;


/// <summary>
/// Written by Max
/// 
/// This Script Manages the when the round starts
/// </summary>
public class RoundStartManager : MonoBehaviour
{
    public static RoundStartManager Instance;

    public bool PreparationPhase => mPreparationPhase;

    [Header("The lenght of the Preparation Phase")]
    [SerializeField]
    private float mMaxPreparationTime;
    private float mPreparationTimer;

    private bool mPreparationPhase = true;

    [SerializeField]
    private GameObject mPreparationWalls;

    [SerializeField]
    private PhotonView mPhotonView;

    [SerializeField]
    private TextMeshProUGUI mTimer;
    [SerializeField]
    private GameObject mKillCount;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);
    }

    private void Start()
    {
        mPreparationTimer = mMaxPreparationTime;
        AudioManager.Instance.Stop("MainTheme");
        AudioManager.Instance.Play("Ambient");

    }

    /// <summary>
    /// Counts down till it hits zero and deactivates the walls trozgh an RPC call
    /// </summary>
    private void Update()
    {
        mPreparationTimer -= Time.deltaTime;

        mTimer.text = (int)mPreparationTimer + " sec"; 

        if (mPreparationTimer < 1)
        {
            mPhotonView.RPC("DeactivateWalls", RpcTarget.AllBufferedViaServer);
        }
    }

    /// <summary>
    /// RPC call to deactivate the walls for everybody
    /// </summary>
    [PunRPC]
    private void DeactivateWalls()
    {
        mPreparationPhase = false;
        mPreparationWalls.SetActive(false);
        mTimer.gameObject.SetActive(false);
        mKillCount.SetActive(true);
    }
}
