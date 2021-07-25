using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System;

public class MineableObject : MonoBehaviourPunCallbacks
{
    public bool Mined => mWasMined;
    public int Value => mResourceValue;

    [Header("The value of the object and the time it takes to mine it")]
    [SerializeField]
    private int mResourceValue;
    [SerializeField]
    private float mMaxMineDuration;
    private float mCurrentMineDuration;
    [SerializeField]
    private float mDissolveTime;
    [SerializeField]
    private float mDissolveMuli;
    [SerializeField]
    private float mCutoffHeight;


    [Header("Components")]
    [SerializeField]
    private Collider mCollider;
    [SerializeField]
    private MeshRenderer mMeshRenderer;
    [SerializeField]
    private AudioSource mAudio;
    [SerializeField]
    private AudioClip mSFX_BeingMined;

    [SerializeField]
    Material mDissolveMaterial;

    private ResourceMiner mMiner;

    private float mMinedAnimClipLenght;
    private float mMineSpeed;
    private bool mIsBeingMined;
    private bool mWasMined;

    private void Start()
    {
        mCurrentMineDuration = mMaxMineDuration;
    }

    private void Update()
    {

        if (mMiner != null && mMiner.Mining)
            mIsBeingMined = true;
        else
            mIsBeingMined = false;

        MineProgress();
    }

    private void MineProgress()
    {
        if (mIsBeingMined)
        {
            mCurrentMineDuration -= mMineSpeed * Time.deltaTime;

            PlayerHud.Instance.MiningProgress(mMaxMineDuration, mCurrentMineDuration);

            if(mAudio.clip != mSFX_BeingMined)
                mAudio.clip = mSFX_BeingMined;

            if (!mAudio.isPlaying)
                mAudio.Play();
        }
        else
            mAudio.Stop();
    }

    public int MineResource(float _mineSpeed, ResourceMiner _miner)
    {
        if(mMiner != _miner)
        {
            photonView.TransferOwnership(PhotonNetwork.LocalPlayer);

            mMiner = _miner;
            mMineSpeed = _mineSpeed;
            mCurrentMineDuration = mMaxMineDuration;
        }

        if (mCurrentMineDuration <= 0)
        {
            mWasMined = true;

            //if(photonView.IsMine)
            //_miner.AddResource(mResourceValue);
            mMeshRenderer.material = mDissolveMaterial;
            StartCoroutine("DissolveMaterial");

            StartCoroutine("ResourceHasBeenMined");
            return mResourceValue;
        }
        else
            return default;
    }

    private IEnumerator ResourceHasBeenMined()
    {
        if (mCollider.enabled)
            mCollider.enabled = false;


        yield return new WaitForSeconds(mDissolveTime);


        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }

        StopCoroutine(ResourceHasBeenMined());
    }

    private IEnumerator DissolveMaterial()
    {
        float value = -mCutoffHeight;

        while (value <= mCutoffHeight)
        {
            value += mDissolveMuli * Time.deltaTime;
            mDissolveMaterial.SetFloat("_cutoffHeight", value);

            yield return null;
        }
    }
}
