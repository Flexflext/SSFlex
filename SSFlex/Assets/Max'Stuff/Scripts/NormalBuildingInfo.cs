using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NormalBuildingInfo : MonoBehaviourPunCallbacks
{
    public enum EClipSideSlots
    {
        up,
        right,
        down,
        left,
        none
    }

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
    private EClipFaceSlots mDefaultFaceSlot;
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

    private Color mDamagedColour;

    private List<EClipSideSlots> mOccupiedSideSlots;
    private List<EClipFaceSlots> mOccupiedFaceSlots;

    private float mRFloat;
    private bool mIsFirstFloor;

    private void Update()
    {
        //Debug.DrawRay(mMeshRenderer.bounds.center, transform.right * (mMeshRenderer.bounds.extents.x + mExtendMod));
    }

    private void Awake()
    {
        mOccupiedSideSlots = new List<EClipSideSlots>();
        mOccupiedFaceSlots = new List<EClipFaceSlots>();
    }
    private void Start()
    {
        GetNeighboursSide();
        GetNeighboursUp();


        mBuildParticle.Play();

        AddClipSlotSide(mDefaultSideSlot);
        AddClipSlotFace(mDefaultFaceSlot);
    }


    public void AddClipSlotSide(EClipSideSlots _slotToAdd)
    {
        mOccupiedSideSlots.Add(_slotToAdd);
    }

    public void AddClipSlotFace(EClipFaceSlots _slotToAdd)
    {
        mOccupiedFaceSlots.Add(_slotToAdd);
    }

    public void SetFirstFloor()
    {
        mIsFirstFloor = true;
    }

    public void RemoveClipSlotSide(EClipSideSlots _slotToRemove)
    {
        mOccupiedSideSlots.Remove(_slotToRemove);
    }

    public void RemoveClipSlotFront(EClipFaceSlots _slotToRemove)
    {
        mOccupiedFaceSlots.Remove(_slotToRemove);
    }

    public void TakeDamage(float _damage)
    {
        mHealth -= _damage;

        mRFloat += mColourDamageValue;
        mDamagedColour = new Color(mRFloat, 0, 0, 50);
        mDamagedColour = Color.HSVToRGB(0, mColourStartValue + mRFloat, 1);
        mDamagedParticle.Play();

        mMeshRenderer.material.color = mDamagedColour;

        if (mHealth <= 0)
        {
            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(this.gameObject);
                PhotonNetwork.RemoveRPCs(photonView);
            }
        }
    }

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
}
