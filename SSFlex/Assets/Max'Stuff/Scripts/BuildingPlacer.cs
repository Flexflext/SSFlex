using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    public float MaxBuildHeight => mMaxBuildHeight;
    public bool IsClipped => mIsClipped;
    public GameObject HitObj => mHitObj;

    [SerializeField]
    private float mMaxBuildRange;

    private bool mIsClipped;
    private bool mIsInBuildMode;
    private float PointClamp;

    private float mRotFloat;
    private float mRelativeHitDot;

    [SerializeField]
    private float mMaxBuildHeight;

    [SerializeField]
    private float mClipThreshold_Side;
    [SerializeField]
    private float mClipThreshold_Up;

    [SerializeField]
    private LayerMask mBuildLayer;

    [SerializeField]
    private RectTransform mCrosshair;

    [SerializeField]
    private GameObject mNormalWall;
    private GameObject mNormalWallClipPos;

    [SerializeField]
    private GameObject mBuildPoint;

    [SerializeField]
    private GameObject NormalWallPlaceholder;

    [SerializeField]
    private Camera mMainCam;

    [SerializeField]
    private PlaceholderScript mPlaceholderScript;

    private GameObject mHitObj;

    private BuildingInfo mHitObjInfo;

    private Collider mCurrentCollider;

    private BuildingInfo.EClipSlots mSlotToAdd;


    private float mDotProductSide;
    private float mDotProductUp;

    private void Start()
    {
        mCurrentCollider = mNormalWall.GetComponentInChildren<Collider>();
        mSlotToAdd = BuildingInfo.EClipSlots.none;
    }

    private void Update()
    {
        Vector3 hitPos = new Vector3();
       
        Vector3 crosPos = mMainCam.ScreenToWorldPoint(mCrosshair.transform.position);

        Vector3 dirVec = new Vector3();

        RaycastHit hit;
        if (Physics.Raycast(crosPos, mMainCam.transform.forward, out hit, 1000, mBuildLayer))
        {
            hitPos = hit.point;

           
            if (Vector3.Distance(transform.position, hitPos) <= mMaxBuildRange)
                mBuildPoint.transform.position = new Vector3(hitPos.x, hitPos.y, hitPos.z);

            if (hit.collider.gameObject.CompareTag("ClipTag"))
            {
                mHitObj = hit.collider.gameObject;
                
                if(mHitObjInfo == null || mHitObjInfo.gameObject != hit.collider.gameObject)
                    mHitObjInfo = mHitObj.GetComponent<BuildingInfo>();

                Vector3 stupidArtistHeightAdj = new Vector3(mHitObj.transform.position.x, mHitObj.transform.position.y + 2.075f,mHitObj.transform.position.z);

                dirVec = stupidArtistHeightAdj - hitPos;

                mDotProductSide = Vector3.Dot(mHitObj.transform.right, dirVec);
                mDotProductUp = Vector3.Dot(mHitObj.transform.up, dirVec);
                mIsClipped = true;
            }
            else
                mIsClipped = false;
        }
        else
        {
            mHitObj = null;
            mIsClipped = false;
        }

        if (Input.GetKeyDown(KeyCode.B))
            mIsInBuildMode = true;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if(mRotFloat <= 0)
                mRotFloat += 90f;
            else
                mRotFloat -= 90f;
        }

        if (mIsInBuildMode)
        {
            NormalWallPlaceholder.SetActive(true);

            if (mIsClipped)
            {
                GetClippedBuildingPos();
            }
            else if (!mIsClipped)
            {
                SetBuildingNormal(mBuildPoint.transform.position);
                mSlotToAdd = BuildingInfo.EClipSlots.none;
            }
            
        }
        else
            NormalWallPlaceholder.SetActive(false);
    }

    private void GetClippedBuildingPos()
    {
        Vector3 tmpDir = mHitObj.transform.position - transform.position;

        mRelativeHitDot = Vector3.Dot(mHitObj.transform.forward, tmpDir);

        if (mHitObj.transform.localEulerAngles.y >= 0 && mHitObj.transform.localEulerAngles.y <= 180)
            mRelativeHitDot = +mRelativeHitDot;
        else
            mRelativeHitDot = -mRelativeHitDot;

        //Debug.Log(mRelativeHitDot);

        //Vector3 hitObjSize = mHitObjCollider.bounds.extents;
        Vector3 hitObjSize = new Vector3(5.2f, 5.1f, 2.2f);
        Vector3 currentObjSize = mCurrentCollider.bounds.size ;
        Vector3 totalPlaceAdj = hitObjSize + currentObjSize;

        Vector3 posToSet = new Vector3();

        //Vector3 hitObjSize = mHitObjCollider.transform.localScale / 2;
        //Vector3 currentObjSize = mNormalWall.transform.localScale / 2;
        //Vector3 totalPlaceAdj = hitObjSize + currentObjSize;


        if (mDotProductSide >= mClipThreshold_Side)
        {
            if(mRotFloat == 0)
                posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(-totalPlaceAdj.x, 0, 0));
            else
            {
                if(mHitObj.transform.localEulerAngles.y >= 180)
                {
                    if (mRelativeHitDot < 0 && !mHitObjInfo.OccupiedSlots.Contains(BuildingInfo.EClipSlots.left))
                    {
                        posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(-totalPlaceAdj.x / 2, 0, -totalPlaceAdj.z));
                        mSlotToAdd = BuildingInfo.EClipSlots.left;
                        //Debug.Log("- links vorne");
                    }
                    if (mRelativeHitDot > 0 && !mHitObjInfo.OccupiedSlots.Contains(BuildingInfo.EClipSlots.right))
                    {
                        posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(-totalPlaceAdj.x / 2, 0, totalPlaceAdj.z));
                        mSlotToAdd = BuildingInfo.EClipSlots.right;
                        //Debug.Log("- rechts hinten");
                    }
                }
                else
                {
                    if (mRelativeHitDot < 0 && !mHitObjInfo.OccupiedSlots.Contains(BuildingInfo.EClipSlots.right))
                    {
                        posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(-totalPlaceAdj.x / 2, 0, totalPlaceAdj.z));
                        mSlotToAdd = BuildingInfo.EClipSlots.right;
                        //Debug.Log("+ recht hinter");
                    }
                    if (mRelativeHitDot > 0 && !mHitObjInfo.OccupiedSlots.Contains(BuildingInfo.EClipSlots.left))
                    {
                        posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(-totalPlaceAdj.x / 2, 0, -totalPlaceAdj.z));
                        mSlotToAdd = BuildingInfo.EClipSlots.left;
                        //Debug.Log("+ links vorne");
                    }
                }     
            }
        }

        if (mDotProductSide <= -mClipThreshold_Side )
        {
            if(mRotFloat == 0)
                posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(totalPlaceAdj.x, 0,0));
            else
            {
                if(mHitObj.transform.localEulerAngles.y >= 0 && mHitObj.transform.localEulerAngles.y < 180f)
                {
                    if (mRelativeHitDot < 0 && !mHitObjInfo.OccupiedSlots.Contains(BuildingInfo.EClipSlots.left))
                    {
                        posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(totalPlaceAdj.x / 2, 0, totalPlaceAdj.z));
                        mSlotToAdd = BuildingInfo.EClipSlots.left;
                        //Debug.Log("+ links hinter");
                    }
                    if (mRelativeHitDot > 0 && !mHitObjInfo.OccupiedSlots.Contains(BuildingInfo.EClipSlots.right))
                    {
                        posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(totalPlaceAdj.x / 2, 0, -totalPlaceAdj.z));
                        mSlotToAdd = BuildingInfo.EClipSlots.right;
                        //Debug.Log("+ rechts vorne");
                    }
                }    
                else
                {
                    if (mRelativeHitDot < 0 && !mHitObjInfo.OccupiedSlots.Contains(BuildingInfo.EClipSlots.right))
                    {
                        posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(totalPlaceAdj.x / 2, 0, -totalPlaceAdj.z));
                        mSlotToAdd = BuildingInfo.EClipSlots.right;
                        //Debug.Log("- rechts vorne");
                    }
                    if (mRelativeHitDot > 0 && !mHitObjInfo.OccupiedSlots.Contains(BuildingInfo.EClipSlots.left))
                    {
                        posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(totalPlaceAdj.x / 2, 0, totalPlaceAdj.z));
                        mSlotToAdd = BuildingInfo.EClipSlots.left;
                        //Debug.Log("- links hinter");
                    }
                }
            }
        }



        if(mDotProductSide < mClipThreshold_Side && mDotProductSide > -mClipThreshold_Side)
        {
            if (mDotProductUp >= mClipThreshold_Up && mHitObj.transform.position.y - hitObjSize.y / 2 > 0 && !mHitObjInfo.OccupiedSlots.Contains(BuildingInfo.EClipSlots.down))
            {
                posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(0, -totalPlaceAdj.y, 0));

                mSlotToAdd = BuildingInfo.EClipSlots.down;
            }

            if (mDotProductUp <= -mClipThreshold_Up && !mHitObjInfo.OccupiedSlots.Contains(BuildingInfo.EClipSlots.up))
            {
                posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(0, totalPlaceAdj.y, 0));

                mSlotToAdd = BuildingInfo.EClipSlots.up;
            }
        }

        SetClippedBuildingRotation();
        SetClippedBuildingPos(posToSet);

        //NormalWallPlaceholder.transform.rotation = new Quaternion(mHitObj.transform.rotation.x, mHitObj.transform.rotation.y / 2, mHitObj.transform.rotation.z, mHitObj.transform.rotation.w);

        //Debug.Log(mRotFloat);

        if (Input.GetKeyDown(KeyCode.Mouse0) && mPlaceholderScript.ValidPosition)
        {
            mHitObjInfo.AddClipSlot(mSlotToAdd);
            Instantiate(mNormalWall, NormalWallPlaceholder.transform.position, NormalWallPlaceholder.transform.rotation);
        }
    }

    private void SetClippedBuildingRotation( )
    {
        NormalWallPlaceholder.transform.eulerAngles = new Vector3(mHitObj.transform.eulerAngles.x, mHitObj.transform.eulerAngles.y + mRotFloat, mHitObj.transform.eulerAngles.z);
    }

    private void SetClippedBuildingPos(Vector3 _posToAdj)
    {
        NormalWallPlaceholder.transform.position = _posToAdj;
    }

    private void SetBuildingNormal(Vector3 _posToSet)
    {
        float posHeightY = 0;
        posHeightY = NormalWallPlaceholder.transform.localScale.y / 2;

        float posX = Mathf.Round(_posToSet.x);
        float posY = Mathf.Round(_posToSet.y);
        float posZ = Mathf.Round(_posToSet.z);
        Vector3 actualPos = new Vector3(posX, posY, posZ);

        NormalWallPlaceholder.transform.position = new Vector3(actualPos.x, actualPos.y + posHeightY -0.5f , actualPos.z);
        NormalWallPlaceholder.transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + mRotFloat, transform.eulerAngles.z);

        //NormalWallPlaceholder.transform.rotation = transform.rotation;

        if (Input.GetKeyDown(KeyCode.Mouse0) && mPlaceholderScript.ValidPosition)
            Instantiate(mNormalWall, NormalWallPlaceholder.transform.position, NormalWallPlaceholder.transform.rotation);
    }
}
