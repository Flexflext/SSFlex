using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Written by Max
/// 
/// This script hold all data of a build building
/// </summary>
public class NormalBuildingInfo : MonoBehaviourPunCallbacks
{
    /// <summary>
    /// The side slots of the building
    /// </summary>
    public enum EClipSideSlots
    {
        up,
        right,
        down,
        left,
        none
    }

    /// <summary>
    /// The face slots of the building
    /// </summary>
    public enum EClipFaceSlots
    {
        front,
        behind,
        none
    }

    public List<EClipSideSlots> OccupiedSideSlots => mOccupiedSideSlots;
    public List<EClipFaceSlots> OccupiedFaceSlots => mOccupiedFaceSlots;

    public AvailableBuildingDimensions.BuildingDimensions.EBuildingDimensions CurrentDimension => mCurrentDimension;

    public bool IsFirstFloor => mIsFirstFloor;


    [Header("Healt, Damage colour change value and clolour start value upon being damaged")]
    [SerializeField]
    private float mHealth;
    [SerializeField]
    private float mColourDamageValue;
    [SerializeField]
    private float mColourStartValue;
    // used to extend the reach of the raycast to find a neighbour
    [SerializeField]
    private float mExtendMod;

    [Header("The default occupied slots of the Building")]
    [SerializeField]
    private EClipSideSlots mDefaultSideSlot;
    [SerializeField]
    private EClipFaceSlots mDefaultFaceSlot_First;
    [SerializeField]
    private EClipFaceSlots mDefaultFaceSlot_Second;
    [Header("The dimension that provides the actual scale of the Building")]
    [SerializeField]
    private AvailableBuildingDimensions.BuildingDimensions.EBuildingDimensions mCurrentDimension;

    [Header("Components")]
    [SerializeField]
    private MeshRenderer mMeshRenderer;
    [SerializeField]
    private LayerMask mBuildLayer;

    [Header("Particle Systems")]
    [SerializeField]
    private ParticleSystem mDamagedParticle;
    [SerializeField]
    private ParticleSystem mBuildParticle;

    [SerializeField]
    private AudioSource mAudio;
    [SerializeField]
    private AudioClip mSFX_BuildAudio;

    private Color mDamagedColour;

    private List<EClipSideSlots> mOccupiedSideSlots;
    private List<EClipFaceSlots> mOccupiedFaceSlots;

    private float mRFloat;
    private bool mIsFirstFloor;

    private void Awake()
    {
        mOccupiedSideSlots = new List<EClipSideSlots>();
        mOccupiedFaceSlots = new List<EClipFaceSlots>();
    }

    /// <summary>
    /// Sets the default face and side Slots
    /// </summary>
    private void Start()
    {
        GetNeighboursSide();
        GetNeighboursUp();


        mBuildParticle.Play();
        mAudio.clip = mSFX_BuildAudio;
        mAudio.Play();

        AddClipSlotSide(mDefaultSideSlot);
        AddClipSlotFace(mDefaultFaceSlot_First);
        AddClipSlotFace(mDefaultFaceSlot_Second);
    }

    /// <summary>
    /// Adds a clipped building to the given side slot
    /// </summary>
    public void AddClipSlotSide(EClipSideSlots _slotToAdd)
    {
        mOccupiedSideSlots.Add(_slotToAdd);
    }

    /// <summary>
    /// Adds a clipped building to the given face slot
    /// </summary>
    public void AddClipSlotFace(EClipFaceSlots _slotToAdd)
    {
        mOccupiedFaceSlots.Add(_slotToAdd);
    }

    /// <summary>
    /// Sets the buildig to be on ground Level
    /// </summary>
    public void SetFirstFloor()
    {
        mIsFirstFloor = true;
    }

    /// <summary>
    /// Removes a clipped building to the given face slot
    /// </summary>
    public void RemoveClipSlotSide(EClipSideSlots _slotToRemove)
    {
        mOccupiedSideSlots.Remove(_slotToRemove);
    }

    /// <summary>
    /// Removes a clipped building to the given face slot
    /// </summary>
    public void RemoveClipSlotFront(EClipFaceSlots _slotToRemove)
    {
        mOccupiedFaceSlots.Remove(_slotToRemove);
    }

    /// <summary>
    /// RPC call from the player upon shooting at the wall
    /// 
    /// 1. The player calles the Take Damage
    /// Now the colour value of the buildings material hat to be updated.
    /// 2. Creates Hashtable with 4 floats, RGBA and takes the values of the Damaged colour
    /// 3. Updates the custom property wth the hashtables value
    /// 4. Calls the MateChange method for all clients with the given value
    /// 5. Destroyes the GO ofer the Network if the health falls under or on zero and removes his RPC
    /// </summary>
    [PunRPC]
    public void TakeDamage(float _damage)
    {
        mHealth -= _damage;

        mRFloat += mColourDamageValue;
        mDamagedColour = new Color(mRFloat, 0, 0, 50);
        mDamagedColour = Color.HSVToRGB(0, mColourStartValue + mRFloat, 1);
        mDamagedParticle.Play();
        

        if (photonView.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("colourR", mDamagedColour.r);
            hash.Add("colourG", mDamagedColour.g);
            hash.Add("colourB", mDamagedColour.b);
            hash.Add("colourA", mDamagedColour.a);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }

        photonView.RPC("MateChange", RpcTarget.AllBufferedViaServer, mDamagedColour.r, mDamagedColour.g, mDamagedColour.b, mDamagedColour.a);


        if (mHealth <= 0)
        {
            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(this.gameObject);
                PhotonNetwork.RemoveRPCs(photonView);
            }
        }
    }

    /// <summary>
    /// RPC call to let all players see the Colour change in the material
    /// </summary>
    [PunRPC]
    private void MateChange(float _r, float _g, float _b,float _a)
    {
        mMeshRenderer.material.color = new Color(_r,_g,_b,_a);
    }

    /// <summary>
    /// Property Update for the Colour value
    /// </summary>
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!photonView.IsMine && targetPlayer == photonView.Owner)
        {
            MateChange((float)changedProps["colourR"], (float)changedProps["colourG"], (float)changedProps["colourB"], (float)changedProps["colourA"]);
        }
    }

    #region raycasts to check for neighbouring buildings
    private void GetNeighboursSide()
    {
        RaycastHit hitRight;
        RaycastHit hitLeft;


        if (Physics.Raycast(mMeshRenderer.bounds.center, transform.right, out hitRight, mMeshRenderer.bounds.extents.x + mExtendMod, mBuildLayer))
        {
            if (hitRight.collider.CompareTag("ClipTag"))
            {
                mOccupiedSideSlots.Add(EClipSideSlots.right);
                hitRight.collider.gameObject.GetComponent<NormalBuildingInfo>().AddClipSlotSide(EClipSideSlots.left);
            }
            
        }
        if (Physics.Raycast(mMeshRenderer.bounds.center, -transform.right, out hitLeft, mMeshRenderer.bounds.extents.x + mExtendMod, mBuildLayer))
        {
            if (hitLeft.collider.CompareTag("ClipTag"))
            {
                mOccupiedSideSlots.Add(EClipSideSlots.left);
                hitLeft.collider.gameObject.GetComponent<NormalBuildingInfo>().AddClipSlotSide(EClipSideSlots.right);
            }
            
        }
    }

    private void GetNeighboursUp()
    {
        RaycastHit hitUp;
        RaycastHit hitDown;


        if (Physics.Raycast(mMeshRenderer.bounds.center, transform.up, out hitUp, mMeshRenderer.bounds.extents.x + mExtendMod, mBuildLayer))
        {
            if (hitUp.collider.CompareTag("ClipTag"))
            {
                mOccupiedSideSlots.Add(EClipSideSlots.up);
                hitUp.collider.gameObject.GetComponent<NormalBuildingInfo>().AddClipSlotSide(EClipSideSlots.down);
            }
            
        }

        if (Physics.Raycast(mMeshRenderer.bounds.center, -transform.up, out hitDown, mMeshRenderer.bounds.extents.x + mExtendMod, mBuildLayer))
        {
            if (hitDown.collider.CompareTag("ClipTag"))
            {
                mOccupiedSideSlots.Add(EClipSideSlots.down);
                hitDown.collider.gameObject.GetComponent<NormalBuildingInfo>().AddClipSlotSide(EClipSideSlots.up);
            }          
        }
    }
    #endregion
}
