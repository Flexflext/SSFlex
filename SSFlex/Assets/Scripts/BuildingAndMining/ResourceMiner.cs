using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class ResourceMiner : MonoBehaviourPunCallbacks
{
    public int ResourceAmount => mCurrentResourceAmount;
    public bool Mining => mIsMining;

    [Header("Max mining distance and speed")]
    [SerializeField]
    private float mMineSpeed;
    [SerializeField]
    private float mMaxMineDistance;

    [Header("Components")]
    [SerializeField]
    private BuildingPlacer mBuildPlacer;
    [SerializeField]
    private Camera mMainCam;
    [SerializeField]
    private Animator mAnimator;
    [SerializeField]
    private LayerMask mResourceLayer;
    [SerializeField]
    private AudioSource mAudio;

    [SerializeField]
    private ParticleSystem mMiningParticle;
    [SerializeField]
    private AudioClip mSFX_Mining;

    private MineableObject mHitObj;

    private bool mIsMining;
    private int mCurrentResourceAmount = 100;

    private void Update()
    {
        if (mBuildPlacer.IsInMineMode && Input.GetKey(KeyCode.Mouse0))
        {
            mMiningParticle.Play();

            if (mAudio.clip != mSFX_Mining)
                mAudio.clip = mSFX_Mining;

            if(!mAudio.isPlaying)
                mAudio.Play();

            mAnimator.SetBool("Farm", true);

            LookForResource();
        }
        else
        {
            mMiningParticle.Stop();
            mAnimator.SetBool("Farm", false);
            mAudio.Stop();
            mIsMining = false;
        }


        PlayerHud.Instance.SetCurrentResourceAmount(mCurrentResourceAmount);
    }

    private void LookForResource()
    {
        if (mMainCam == null)
            return;

        

        RaycastHit hit;

        if (Physics.Raycast(mMainCam.transform.position, mMainCam.transform.forward, out hit, 1000, mResourceLayer))
        {
            Debug.Log("Looking for Resources");
            if (mHitObj == null)
                mHitObj = hit.collider.gameObject.GetComponent<MineableObject>();

            if (mHitObj != null)
            {
                mIsMining = true;
                mHitObj.MineResource(mMineSpeed, this);

                if (photonView.IsMine && mHitObj.Mined)
                    mCurrentResourceAmount += mHitObj.Value;
            }
            else
                mIsMining = false;
        }
        else
        {
            mIsMining = false;
            mHitObj = null;
        }
    }

    public void SubtractResource(int _toSubtract)
    {
        mCurrentResourceAmount -= _toSubtract;
    }

    public void AddResource(int _value)
    {
        photonView.RPC("RPCAddResource", RpcTarget.All, _value);
    }

    [PunRPC]
    private void RPCAddResource(int _toAdd)
    {
        if (!photonView.IsMine)
            return;

        mCurrentResourceAmount += _toAdd;
    }
}
