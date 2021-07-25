using Photon.Pun;
using System.Collections;
using UnityEngine;


/// <summary>
/// Written by Max
/// 
/// This script manages the behaviour of the mineable resources
/// </summary>
public class MineableObject : MonoBehaviourPunCallbacks
{
    public bool Mined => mWasMined;
    public int Value => mResourceValue;

    // The different informations of the Objects
    [Header("The value of the object and the time it takes to mine it")]
    // The amount of Resources the player gets out of it
    [SerializeField]
    private int mResourceValue;
    // The time the player needs to mine it
    [SerializeField]
    private float mMaxMineDuration;
    private float mCurrentMineDuration;
    // Different values for the shader material to which the Object is switching upon being mined
    [Header("Shader mods")]
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

    // The material to which the Renderer switches upon being destroyed
    [SerializeField]
    Material mDissolveMaterial;

    private ResourceMiner mMiner;

    // The speed by which the Resource gets mined
    private float mMineSpeed;
    // Checks to look if the Object is being mined
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

    /// <summary>
    /// This methods controlls the mining progress as well as the GUI
    /// 
    /// 1. Checks if the Object is being minde
    /// 2. Counts down the mining duration by the speed
    /// 3. Updates the Hud with the progress
    /// 4. Plays Auido
    /// </summary>
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

    /// <summary>
    /// This method controlls the mining behaviour
    /// 
    /// 1. Gets called from the ResourceMiner bythe player upon being mined
    /// 2. Transfers the Objects Owner to the player who is mining it
    /// 3. Sets the necessary variables
    /// 4. If the Mining Duration hits zero or below, the Material will be changed to the Shader material
    /// 5. Start a Coroutune for the dissolve effect and one for the Objects Destruction
    /// 6. returns the Resource value
    /// </summary>
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
            mMeshRenderer.material = mDissolveMaterial;

            StartCoroutine("DissolveMaterial");
            StartCoroutine("ResourceHasBeenMined");

            return mResourceValue;
        }
        else
            return default;
    }

    /// <summary>
    /// Disables the collider and Destroyes the Object after the given time
    /// </summary>
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

    /// <summary>
    /// Counts down the time value of the shader
    /// </summary>
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
