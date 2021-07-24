﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System;

public class MineableObject : MonoBehaviourPunCallbacks
{
    [Header("The value of the object and the time it takes to mine it")]
    [SerializeField]
    private int mResourceValue;
    [SerializeField]
    private float mMaxMineDuration;
    private float mCurrentMineDuration;
    [SerializeField]
    private float mDissolveTime;

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

        if (mWasMined)
            ResourceHasBeenMined();

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

    public void MineResource(float _mineSpeed, ResourceMiner _miner)
    {
        if(mMiner != _miner)
        {
            mMiner = _miner;
            mMineSpeed = _mineSpeed;
            mCurrentMineDuration = mMaxMineDuration;
        }

        if (mCurrentMineDuration <= 0)
        {
            mWasMined = true;

            _miner.AddResource(mResourceValue);
            StartCoroutine("ResourceHasBeenMined");
        }
    }


    private IEnumerator ResourceHasBeenMined()
    {
        if (mCollider.enabled)
            mCollider.enabled = false;

        mMeshRenderer.material = mDissolveMaterial;

        yield return new WaitForSeconds(mDissolveTime);

        //if (photonView.IsMine && PhotonNetwork.LocalPlayer == photonView.Owner)
        //    photonView.RPC("DestroyGO", RpcTarget.AllBufferedViaServer);

        if (photonView.IsMine)
        {
            //PhotonNetwork.RemoveRPCs(photonView);
            PhotonNetwork.Destroy(this.gameObject);
        }

        StopCoroutine(ResourceHasBeenMined());
    }

    [PunRPC]
    private void DestroyGO()
    {
        PhotonNetwork.Destroy(this.gameObject);
        PhotonNetwork.RemoveRPCs(photonView);
    }
}
