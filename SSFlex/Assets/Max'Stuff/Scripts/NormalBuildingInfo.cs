using System.Collections.Generic;
using UnityEngine;

public class NormalBuildingInfo : MonoBehaviour
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
    public BuildingDimensions.EBuildingDimensions CurrentDimension => mCurrentDimension;
    public bool IsFirstFloor => mIsFirstFloor;


    private List<EClipSideSlots> mOccupiedSideSlots;

    private List<EClipFaceSlots> mOccupiedFaceSlots;

    [SerializeField]
    private float mHealth;

    [SerializeField]
    private EClipSideSlots mDefaultSideSlot;
    [SerializeField]
    private EClipFaceSlots mDefaultFaceSlot;

    [SerializeField]
    private BuildingDimensions.EBuildingDimensions mCurrentDimension;

    private bool mIsFirstFloor;

    [SerializeField]
    private LayerMask mBuildLayer;

    [SerializeField]
    private float mExtendMod;

    [SerializeField]
    private MeshRenderer mMeshRenderer;

    [SerializeField]
    private float mColourDamageValue;

    [SerializeField]
    private ParticleSystem mDamagedParticle;
    [SerializeField]
    private ParticleSystem mBuildParticle;

    private Color mDamagedColour;
    private float mRFloat;

    [SerializeField]
    private float mColourStartValue;

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
        if (mOccupiedSideSlots.Contains(EClipSideSlots.up))
            GetNeighboursSide();
        //if (mOccupiedSideSlots.Contains(EClipSideSlots.down))
        //    GetNeighboursUp();

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
    }

    private void GetNeighboursSide()
    {
        RaycastHit hitRight;
        RaycastHit hitLeft;

        //Debug.Log("Edgecase");

        if (Physics.Raycast(mMeshRenderer.bounds.center, transform.right, out hitRight, mMeshRenderer.bounds.extents.x + mExtendMod, mBuildLayer))
        {
            //Debug.Log("Hit right");
            mOccupiedSideSlots.Add(EClipSideSlots.right);
            hitRight.collider.gameObject.GetComponent<NormalBuildingInfo>().AddClipSlotSide(EClipSideSlots.left);
        }
        if (Physics.Raycast(mMeshRenderer.bounds.center, -transform.right, out hitLeft, mMeshRenderer.bounds.extents.x + mExtendMod, mBuildLayer))
        {
            //Debug.Log("Hit left");
            mOccupiedSideSlots.Add(EClipSideSlots.left);
            hitLeft.collider.gameObject.GetComponent<NormalBuildingInfo>().AddClipSlotSide(EClipSideSlots.right);
        }
    }

    private void GetNeighboursUp()
    {
        RaycastHit hitUp;
        RaycastHit hitDown;

        //Debug.Log("Edgecase");

        if (Physics.Raycast(mMeshRenderer.bounds.center, transform.up, out hitUp, mMeshRenderer.bounds.extents.x + mExtendMod, mBuildLayer))
        {
            //Debug.Log("Hit up");
            mOccupiedSideSlots.Add(EClipSideSlots.up);
            hitUp.collider.gameObject.GetComponent<NormalBuildingInfo>().AddClipSlotSide(EClipSideSlots.down);
        }
        if (Physics.Raycast(mMeshRenderer.bounds.center, -transform.up, out hitDown, mMeshRenderer.bounds.extents.x + mExtendMod, mBuildLayer))
        {
            //Debug.Log("Hit down");
            mOccupiedSideSlots.Add(EClipSideSlots.down);
            hitDown.collider.gameObject.GetComponent<NormalBuildingInfo>().AddClipSlotSide(EClipSideSlots.up);
        }
    }
}
