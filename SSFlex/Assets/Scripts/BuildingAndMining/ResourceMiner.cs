using Photon.Pun;
using UnityEngine;


/// <summary>
/// Written by Max
/// 
/// This script manages the Player behaviour while mining
/// </summary>
public class ResourceMiner : MonoBehaviourPunCallbacks
{
    public int ResourceAmount => mCurrentResourceAmount;
    public bool Mining => mIsMining;

    [Header("Max mining distance and speed")]
    [SerializeField]
    private float mMineSpeed;
    [SerializeField]
    private float mMaxMineDistance;
    [SerializeField]
    private int mCurrentResourceAmount;

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

    /// <summary>
    /// Sets the start resource amount
    /// </summary>
    private void Start()
    {
        if (photonView.IsMine)
            PlayerHud.Instance.SetCurrentResourceAmount(mCurrentResourceAmount);
    }

    /// <summary>
    /// Starts the ANimations ans Sound if the player is Mining
    /// </summary>
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
    }

    /// <summary>
    /// Resposible for trying to detect resources
    /// 
    /// 1. Cast a Raycst to check if the player is hitting something on the Resource layer
    /// 2. If so, takes its MineableObject Components
    /// 3. Calls MineResource on the HitObject if the return value is above zero and adds the given resources onto the player
    /// 4. Updates the GUI
    /// </summary>
    private void LookForResource()
    {
        if (mMainCam == null)
            return;

        RaycastHit hit;

        if (Physics.Raycast(mMainCam.transform.position, mMainCam.transform.forward, out hit, 1000, mResourceLayer))
        {
            if (mHitObj == null)
                mHitObj = hit.collider.gameObject.GetComponent<MineableObject>();

            if (mHitObj != null)
            {
                mIsMining = true;

                if (mHitObj.MineResource(mMineSpeed, this) > 0)
                {
                    mCurrentResourceAmount += mHitObj.MineResource(mMineSpeed, this);

                    if (photonView.IsMine)
                        PlayerHud.Instance.SetCurrentResourceAmount(mCurrentResourceAmount);
                }
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

    /// <summary>
    /// 1. Subtracts Resources upon building
    /// 2. Updates the GUI
    /// </summary>
    public void SubtractResource(int _toSubtract)
    {
        mCurrentResourceAmount -= _toSubtract;

        if (photonView.IsMine)
            PlayerHud.Instance.SetCurrentResourceAmount(mCurrentResourceAmount);
    }
}
