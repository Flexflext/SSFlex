using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class PreparationCounter : MonoBehaviour
{
    public static PreparationCounter Instance;

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
    }

    private void Update()
    {
        mPreparationTimer -= Time.deltaTime;

        mTimer.text = (int)mPreparationTimer + " sec"; 

        if (mPreparationTimer < 1)
        {
            mPhotonView.RPC("DeactivateWalls", RpcTarget.AllBufferedViaServer);
        }
    }

    [PunRPC]
    private void DeactivateWalls()
    {
        mPreparationPhase = false;
        mPreparationWalls.SetActive(false);
        mTimer.gameObject.SetActive(false);
        mKillCount.SetActive(true);
    }
}
