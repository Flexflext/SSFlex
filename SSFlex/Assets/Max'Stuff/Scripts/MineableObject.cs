using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class MineableObject : MonoBehaviourPunCallbacks
{
    [Header("The value of the object and the time it takes to mine it")]
    [SerializeField]
    private float mResourceValue;
    [SerializeField]
    private float mMaxMineDuration;
    private float mCurrentMineDuration;


    [Header("Components")]
    [SerializeField]
    private Collider mCollider;
    [SerializeField]
    private Animator mAnimator;
    [SerializeField]
    private AnimationClip mWasMindedClip;


    [Header("Particle System")]
    [SerializeField]
    private ParticleSystem mBeingMined;


    private ResourceMiner mMiner;

    private float mMinedAnimClipLenght;
    private float mMineSpeed;
    private bool mIsBeingMined;
    private bool mWasMined;

    private void Start()
    {
        mCurrentMineDuration = mMaxMineDuration;
        mMinedAnimClipLenght = mWasMindedClip.length;
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

        Debug.Log(mMiner);
    }

    private void MineProgress()
    {
        if (mIsBeingMined)
        {
            mCurrentMineDuration -= mMineSpeed * Time.deltaTime;

            if (!mBeingMined.isPlaying)
                mBeingMined.Play();

            mAnimator.SetBool("BeingMined", true);
        }
        else if (!mIsBeingMined)
        {
            if (mBeingMined.isPlaying)
                mBeingMined.Stop();

            mAnimator.SetBool("BeingMined", false);
        }
    }

    public float MineResource(float _mineSpeed, ResourceMiner _miner)
    {
        if(mMiner != _miner)
        {
            mMiner = _miner;
            mMineSpeed = _mineSpeed;
            mCurrentMineDuration = mMaxMineDuration;
        }

        if (mCurrentMineDuration <= 0)
        {
            mAnimator.SetBool("WasMined", true);            
            mWasMined = true;

            StartCoroutine("ResourceHasBeenMined");
            return mResourceValue;
        }
        else
            return default;
    }


    private void ResourceHasBeenMined()
    {
        if (mCollider.enabled)
            mCollider.enabled = false;

        if (mMinedAnimClipLenght <= 0)
            Destroy(gameObject);
        else
            mMinedAnimClipLenght -= Time.deltaTime;
    }
}
