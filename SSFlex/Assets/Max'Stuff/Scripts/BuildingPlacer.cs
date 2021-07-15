using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using AvailableBuildingDimensions;

public class BuildingPlacer : MonoBehaviourPunCallbacks
{
    public GameObject HitObj => mHitObj;

    public float MaxBuildHeight => mMaxBuildHeight;
    public bool IsClipped => mIsClipped;
    public bool IsInBuildMode => mIsInBuildMode;
    public bool IsInMineMode => mIsInMineMode;


    [Header("Max Build Range")]
    [SerializeField]
    private float mMaxBuildRange;

    // Max Height to build
    [SerializeField]
    private float mMaxBuildHeight;
    // The threshhold for up, down, left, right used to determined if the buildings has to clipped by checking the equivalent dot product
    [SerializeField]
    private float mClipThreshold_Side;
    [SerializeField]
    private float mClipThreshold_Up;
    [SerializeField]
    private float mThicknessAdj;

    [Header("Components")]
    [SerializeField]
    private GameObject mBuildPoint;
    [SerializeField]
    private Camera mMainCam;
    [SerializeField]
    private ResourceMiner mMiner;
    [SerializeField]
    private LayerMask mBuildLayer;


    [Header("Prefabs for Buildings")]
    [SerializeField]
    private GameObject mNormalWall;
    [SerializeField]
    private GameObject mNormalGate;
    [SerializeField]
    private GameObject mHalfWall;
    [SerializeField]
    private GameObject mNormalStairs;
    
    private GameObject mCurrentBuilding;
    private List<GameObject> mAllBuildings;

    [Header("Placeholder for Buildings")]
    [SerializeField]
    private GameObject mNormalWallPlaceholder;
    [SerializeField]
    private GameObject mNormalGatePlaceholder;
    [SerializeField]
    private GameObject mHalfWallPlaceholder;
    [SerializeField]
    private GameObject mNormalStairsPlaceholder;
    [SerializeField]
    private PlaceholderScript mPlaceholderScript;

    private GameObject mCurrentPlaceholder;
    private List<GameObject> mAllPlaceholder;

    private NormalBuildingInfo.EClipSideSlots mHitSlotToAdd_Side;
    private NormalBuildingInfo.EClipSideSlots mCurrentSlotToAdd_Side;

    private NormalBuildingInfo.EClipFaceSlots mHitSlotToAdd_Face;
    private NormalBuildingInfo.EClipFaceSlots mCurrentSlotToAdd_Face;

    private BuildingDimensions.EBuildingDimensions mHitObjDimension;
    private BuildingDimensions.EBuildingDimensions mCurrentDimension;
    private BuildingDimensions mAvailableDimensions;


    private NormalBuildingInfo mHitObjInfo;
    private Collider mCurrentCollider;
    private GameObject mHitObj;



    private float mRotFloat;
    private float mRelativeHitDot;

    private int mBuidlIdx = 1;
    private bool mIsInMineMode = true;

    private bool mIsInBuildMode;
    private bool mIsClipped;

    private float mDotProductSide;
    private float mDotProductUp;

    private float mResourceAmount;
    private void Start()
    {
        if (!photonView.IsMine)
            return;

        mAvailableDimensions = new BuildingDimensions();

        mAllBuildings = new List<GameObject>()
        {
            mNormalWall,
            mNormalGate,
            mHalfWall,
            mNormalStairs
        };
        mAllPlaceholder = new List<GameObject>()
        {
            mNormalWallPlaceholder,
            mNormalGatePlaceholder,
            mHalfWallPlaceholder,
            mNormalStairsPlaceholder
        };
        mCurrentPlaceholder = mNormalWallPlaceholder;
        mCurrentBuilding = mNormalWall;
        mPlaceholderScript = mCurrentPlaceholder.GetComponent<PlaceholderScript>();

        mCurrentCollider = mNormalWall.GetComponentInChildren<Collider>();
        mHitSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.none;
        mCurrentSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.none;

        mHitSlotToAdd_Face = NormalBuildingInfo.EClipFaceSlots.none;
        mCurrentSlotToAdd_Face = NormalBuildingInfo.EClipFaceSlots.none;
    }

    private void Update()
    {
        if (!photonView.IsMine)
            return;


        if (GameManager.Instance.PreparationPhase)
        {
            ManageBuildMode();
            ManageBuildingClip();
            ChangeModeType();
            ChangeBuildType();
        }
        else if (mIsInBuildMode && !GameManager.Instance.PreparationPhase)
            mIsInBuildMode = false;
    }

    private void ManageBuildMode()
    {
        if (mIsInBuildMode)
        {
            mCurrentPlaceholder.SetActive(true);

            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                if (mRotFloat <= 0)
                    mRotFloat += 90f;
                else
                    mRotFloat -= 90f;
            }

            if (mIsClipped)
            {
                GetClippedBuildingPos_Side();
            }
            else if (!mIsClipped)
            {
                SetBuildingNormal(mBuildPoint.transform.position);
                mHitSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.none;
                mCurrentSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.none;
            }

        }
        else
            mCurrentPlaceholder.SetActive(false);
    }

    private void ManageBuildingClip()
    {
        Vector3 hitPos = new Vector3();

        Vector3 dirVec = new Vector3();

        RaycastHit hit;

        if (Physics.Raycast(mMainCam.transform.position, mMainCam.transform.forward, out hit, 1000, mBuildLayer))
        {
            hitPos = hit.point;

            if (Vector3.Distance(transform.position, hitPos) <= mMaxBuildRange)
                mBuildPoint.transform.position = new Vector3(hitPos.x, hitPos.y, hitPos.z);

            if (hit.collider.gameObject.CompareTag("ClipTag"))
            {
                mHitObj = hit.collider.gameObject;

                if (mHitObjInfo == null || mHitObjInfo.gameObject != hit.collider.gameObject)
                    mHitObjInfo = mHitObj.GetComponent<NormalBuildingInfo>();

                Vector3 stupidArtistHeightAdj = new Vector3(mHitObj.transform.position.x, mHitObj.transform.position.y + 2.075f, mHitObj.transform.position.z);

                dirVec = stupidArtistHeightAdj - hitPos;

                mDotProductSide = Vector3.Dot(mHitObj.transform.right, dirVec);
                mDotProductUp = Vector3.Dot(mHitObj.transform.up, dirVec);
                mIsClipped = true;
            }
            else
            {
                mHitObj = null;
                mIsClipped = false;
            }
        }
        else
        {
            mHitObj = null;
            mIsClipped = false;
        }
    }

    private void GetClippedBuildingPos_Side()
    {
        Vector3 tmpDir = mHitObj.transform.position - transform.position;

        mRelativeHitDot = Vector3.Dot(mHitObj.transform.forward, tmpDir);

        if (mHitObj.transform.localEulerAngles.y >= 0 && mHitObj.transform.localEulerAngles.y <= 180)
            mRelativeHitDot = +mRelativeHitDot;
        else
            mRelativeHitDot = -mRelativeHitDot;

        //Debug.Log(mRelativeHitDot);

        //Collider tmpColl = mHitObj.GetComponent<Collider>();
        //Debug.Log(tmpColl.bounds.size);
        //Vector3 hitObjSize = tmpColl.bounds.size;

        mHitObjDimension = mHitObjInfo.CurrentDimension;
        mCurrentDimension = mCurrentPlaceholder.GetComponent<PlaceholderScript>().CurrentDimension;

        Vector3 hitObjSize = mAvailableDimensions.mBuildingDimensions[(int)mHitObjDimension] / 2;
        Vector3 currentObjSize = mAvailableDimensions.mBuildingDimensions[(int)mCurrentDimension] / 2;

        float hitX = hitObjSize.x * HitObj.transform.localScale.x;
        float hitY = hitObjSize.y * HitObj.transform.localScale.y;
        float hitZ = hitObjSize.z * HitObj.transform.localScale.z;
        hitObjSize = new Vector3(hitX, hitY, hitZ);

        float currentX = hitObjSize.x * mCurrentBuilding.transform.localScale.x;
        float currentY = hitObjSize.y * mCurrentBuilding.transform.localScale.y;
        float currentZ = hitObjSize.z * mCurrentBuilding.transform.localScale.z;
        currentObjSize = new Vector3(currentX, currentY, currentZ);

        Vector3 totalPlaceAdj = hitObjSize + currentObjSize;

        Vector3 posToSet = new Vector3();


        if (mDotProductSide < mClipThreshold_Side && mDotProductSide > -mClipThreshold_Side && mDotProductUp < mClipThreshold_Up && mDotProductUp > -mClipThreshold_Up)
        {
            if (mHitObj.transform.localEulerAngles.y <= 180)
            {
                if (mRelativeHitDot <= 0 && !mHitObjInfo.OccupiedFaceSlots.Contains(NormalBuildingInfo.EClipFaceSlots.front))
                {
                    posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(0, 0, totalPlaceAdj.z / mThicknessAdj));
                    mHitSlotToAdd_Face = NormalBuildingInfo.EClipFaceSlots.front;
                    mCurrentSlotToAdd_Face = NormalBuildingInfo.EClipFaceSlots.behind;

                }
                else if (mRelativeHitDot > 0 && !mHitObjInfo.OccupiedFaceSlots.Contains(NormalBuildingInfo.EClipFaceSlots.behind))
                {
                    posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(0, 0, -totalPlaceAdj.z / mThicknessAdj));
                    mHitSlotToAdd_Face = NormalBuildingInfo.EClipFaceSlots.behind;
                    mCurrentSlotToAdd_Face = NormalBuildingInfo.EClipFaceSlots.front;
                }
                else
                {
                    mHitSlotToAdd_Face = NormalBuildingInfo.EClipFaceSlots.none;
                    mCurrentSlotToAdd_Face = NormalBuildingInfo.EClipFaceSlots.none;
                }
            }
            else
            {
                if (mRelativeHitDot <= 0 && !mHitObjInfo.OccupiedFaceSlots.Contains(NormalBuildingInfo.EClipFaceSlots.behind))
                {
                    posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(0, 0, -totalPlaceAdj.z / mThicknessAdj));
                    mHitSlotToAdd_Face = NormalBuildingInfo.EClipFaceSlots.behind;
                    mCurrentSlotToAdd_Face = NormalBuildingInfo.EClipFaceSlots.front;
                }
                else if(mRelativeHitDot > 0 && !mHitObjInfo.OccupiedFaceSlots.Contains(NormalBuildingInfo.EClipFaceSlots.front))
                {
                    posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(0, 0, totalPlaceAdj.z / mThicknessAdj));
                    mHitSlotToAdd_Face = NormalBuildingInfo.EClipFaceSlots.front;
                    mCurrentSlotToAdd_Face = NormalBuildingInfo.EClipFaceSlots.behind;
                }
                else
                {
                    mHitSlotToAdd_Face = NormalBuildingInfo.EClipFaceSlots.none;
                    mCurrentSlotToAdd_Face = NormalBuildingInfo.EClipFaceSlots.none;
                }
            }
        }
        else
        {
            mHitSlotToAdd_Face = NormalBuildingInfo.EClipFaceSlots.none;
            mCurrentSlotToAdd_Face = NormalBuildingInfo.EClipFaceSlots.none;
        }

        if(mDotProductSide >= mClipThreshold_Side || mDotProductSide <= -mClipThreshold_Side || mDotProductSide < mClipThreshold_Side && mDotProductSide > -mClipThreshold_Side)
        {
            if (mDotProductSide >= mClipThreshold_Side)
            {
                if (mRotFloat == 0)
                {
                    if (!mHitObjInfo.OccupiedSideSlots.Contains(NormalBuildingInfo.EClipSideSlots.left))
                    {
                        posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(-totalPlaceAdj.x, 0, 0));
                        //mHitSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.left;
                        //mCurrentSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.right;


                    }
                    //else
                    //{
                    //    posToSet = default;
                    //    mHitSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.none;
                    //    mCurrentSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.none;
                    //    //Debug.Log("No valid Clip Pos");
                    //}

                }
                else if (mRotFloat != 0)
                {
                    if (mHitObj.transform.localEulerAngles.y >= 180)
                    {
                        if (mRelativeHitDot < 0 && !mHitObjInfo.OccupiedSideSlots.Contains(NormalBuildingInfo.EClipSideSlots.left))
                        {
                            posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(-totalPlaceAdj.x / 2, 0, -totalPlaceAdj.z));
                            //mHitSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.left;
                            //mCurrentSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.right;
                            //Debug.Log("- links vorne");
                        }
                        if (mRelativeHitDot > 0 && !mHitObjInfo.OccupiedSideSlots.Contains(NormalBuildingInfo.EClipSideSlots.right))
                        {
                            posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(-totalPlaceAdj.x / 2, 0, totalPlaceAdj.z));
                            //mHitSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.right;
                            //mCurrentSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.left;
                            //Debug.Log("- rechts hinten");
                        }
                    }
                    else
                    {
                        if (mRelativeHitDot < 0 && !mHitObjInfo.OccupiedSideSlots.Contains(NormalBuildingInfo.EClipSideSlots.right))
                        {
                            posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(-totalPlaceAdj.x / 2, 0, totalPlaceAdj.z));
                            //mHitSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.right;
                            //mCurrentSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.left;
                            //Debug.Log("+ recht hinter");
                        }
                        if (mRelativeHitDot > 0 && !mHitObjInfo.OccupiedSideSlots.Contains(NormalBuildingInfo.EClipSideSlots.left))
                        {
                            posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(-totalPlaceAdj.x / 2, 0, -totalPlaceAdj.z));
                            //mHitSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.left;
                            //mCurrentSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.right;
                            //Debug.Log("+ links vorne");
                        }
                    }
                }
                //else
                //{
                //    posToSet = default;
                //    mHitSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.none;
                //    mCurrentSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.none;
                //    //Debug.Log("No valid Clip Pos");
                //}
            }

            if (mDotProductSide <= -mClipThreshold_Side)
            {
                if (mRotFloat == 0)
                {
                    if (!mHitObjInfo.OccupiedSideSlots.Contains(NormalBuildingInfo.EClipSideSlots.right))
                    {
                        posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(totalPlaceAdj.x, 0, 0));
                        //mHitSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.right;
                        //mCurrentSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.left;
                    }
                    //else
                    //{
                    //    posToSet = default;
                    //    mHitSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.none;
                    //    mCurrentSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.none;
                    //    //Debug.Log("No valid Clip Pos");
                    //}
                }
                else if (mRotFloat != 0)
                {
                    if (mHitObj.transform.localEulerAngles.y >= 0 && mHitObj.transform.localEulerAngles.y < 180f)
                    {
                        if (mRelativeHitDot < 0 && !mHitObjInfo.OccupiedSideSlots.Contains(NormalBuildingInfo.EClipSideSlots.left))
                        {
                            posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(totalPlaceAdj.x / 2, 0, totalPlaceAdj.z));
                            //mHitSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.left;
                            //mCurrentSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.right;
                            //Debug.Log("+ links hinter");
                        }
                        if (mRelativeHitDot > 0 && !mHitObjInfo.OccupiedSideSlots.Contains(NormalBuildingInfo.EClipSideSlots.right))
                        {
                            posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(totalPlaceAdj.x / 2, 0, -totalPlaceAdj.z));
                            //mHitSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.right;
                            //mCurrentSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.left;
                            //Debug.Log("+ rechts vorne");
                        }
                    }
                    else
                    {
                        if (mRelativeHitDot < 0 && !mHitObjInfo.OccupiedSideSlots.Contains(NormalBuildingInfo.EClipSideSlots.right))
                        {
                            posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(totalPlaceAdj.x / 2, 0, -totalPlaceAdj.z));
                            //mHitSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.right;
                            //mCurrentSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.left;
                            //Debug.Log("- rechts vorne");
                        }
                        if (mRelativeHitDot > 0 && !mHitObjInfo.OccupiedSideSlots.Contains(NormalBuildingInfo.EClipSideSlots.left))
                        {
                            posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(totalPlaceAdj.x / 2, 0, totalPlaceAdj.z));
                            //mHitSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.left;
                            //mCurrentSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.right;
                            //Debug.Log("- links hinter");
                        }
                    }
                }
                //else
                //{
                //    posToSet = default;
                //    mHitSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.none;
                //    mCurrentSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.none;
                //    //Debug.Log("No valid Clip Pos");
                //}
            }

            if (mDotProductSide < mClipThreshold_Side && mDotProductSide > -mClipThreshold_Side)
            {
                if (mDotProductUp >= mClipThreshold_Up && mHitObj.transform.position.y - hitObjSize.y / 2 > 0 && !mHitObjInfo.OccupiedSideSlots.Contains(NormalBuildingInfo.EClipSideSlots.down) && mHitObjInfo.IsFirstFloor)
                {
                    posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(0, -totalPlaceAdj.y, 0));

                    //mHitSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.down;
                    //mCurrentSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.up;
                }

                if (mDotProductUp <= -mClipThreshold_Up && !mHitObjInfo.OccupiedSideSlots.Contains(NormalBuildingInfo.EClipSideSlots.up))
                {
                    posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(0, totalPlaceAdj.y, 0));

                    //mHitSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.up;
                    //mCurrentSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.down;
                }
            }
        }
        else
        {
            //mHitSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.none;
            //mCurrentSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.none;
            Debug.Log("No valid Clip Pos");
        }
 

        SetClippedBuildingRotation();
        SetClippedBuildingPos(posToSet);

        //NormalWallPlaceholder.transform.rotation = new Quaternion(mHitObj.transform.rotation.x, mHitObj.transform.rotation.y / 2, mHitObj.transform.rotation.z, mHitObj.transform.rotation.w);

        //Debug.Log(mRotFloat);

        if (Input.GetKeyDown(KeyCode.Mouse0) && mPlaceholderScript.ValidPosition)
        {
            //if(mHitSlotToAdd_Side != NormalBuildingInfo.EClipSideSlots.none)
                //mHitObjInfo.AddClipSlotSide(mHitSlotToAdd_Side);
            //if(mHitSlotToAdd_Face != NormalBuildingInfo.EClipFaceSlots.none)
                //mHitObjInfo.AddClipSlotFace(mHitSlotToAdd_Face);

            string objToBuild = mPlaceholderScript.ObjName;

            GameObject currentBuilding = Instantiate(mCurrentBuilding, mCurrentPlaceholder.transform.position, mCurrentPlaceholder.transform.rotation);

            //GameObject currentBuilding = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", objToBuild), mCurrentPlaceholder.transform.position, mCurrentPlaceholder.transform.rotation);
            NormalBuildingInfo currenBuildingInfo = currentBuilding.GetComponent<NormalBuildingInfo>();

            mMiner.SubtractResource(mPlaceholderScript.BuildingValue);

            if (currentBuilding.transform.position.y /*+ mAvailableDimensions.mBuildingDimensions[(int)mCurrentDimension].y / 2*/ > mCurrentPlaceholder.transform.position.y)
            {
                currenBuildingInfo.SetFirstFloor();
            }

            //if (mCurrentSlotToAdd_Side != NormalBuildingInfo.EClipSideSlots.none)
                //currenBuildingInfo.AddClipSlotSide(mCurrentSlotToAdd_Side);

            if (mCurrentSlotToAdd_Face != NormalBuildingInfo.EClipFaceSlots.none)
                currenBuildingInfo.AddClipSlotFace(mCurrentSlotToAdd_Face);

            //if (mHitSlotToAdd_Side == NormalBuildingInfo.EClipSideSlots.up && mCurrentSlotToAdd_Side == NormalBuildingInfo.EClipSideSlots.down || mHitObjInfo.IsFirstFloor)
            //{
            //    if (mHitSlotToAdd_Side != NormalBuildingInfo.EClipSideSlots.down)
            //        currenBuildingInfo.SetFirstFloor();
            //}
        }
    }


    private void SetClippedBuildingRotation( )
    {
        mCurrentPlaceholder.transform.eulerAngles = new Vector3(mHitObj.transform.eulerAngles.x, mHitObj.transform.eulerAngles.y + mRotFloat, mHitObj.transform.eulerAngles.z);
    }

    private void SetClippedBuildingPos(Vector3 _posToAdj)
    {
        if (_posToAdj != Vector3.zero)
            mCurrentPlaceholder.transform.position = _posToAdj;
        else
            SetBuildingNormal(mBuildPoint.transform.position);
    }

    private void SetBuildingNormal(Vector3 _posToSet)
    {
        float posHeightY = 0;
        posHeightY = mBuildPoint.transform.position.y;

        float posX = Mathf.Round(_posToSet.x);
        float posY = Mathf.Round(_posToSet.y);
        float posZ = Mathf.Round(_posToSet.z);
        Vector3 actualPos = new Vector3(posX, posY, posZ);

        mCurrentPlaceholder.transform.position = new Vector3(actualPos.x, actualPos.y, actualPos.z);
        mCurrentPlaceholder.transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + mRotFloat, transform.eulerAngles.z);

        if (Input.GetKeyDown(KeyCode.Mouse0) && mPlaceholderScript.ValidPosition)
        {
            GameObject currentBuilding = Instantiate(mCurrentBuilding, mCurrentPlaceholder.transform.position, mCurrentPlaceholder.transform.rotation);

            string objToBuild = mPlaceholderScript.ObjName;

            //GameObject currentBuilding = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", objToBuild), mCurrentPlaceholder.transform.position, mCurrentPlaceholder.transform.rotation);
            currentBuilding.GetComponent<NormalBuildingInfo>().AddClipSlotSide(NormalBuildingInfo.EClipSideSlots.down);
            //Debug.Log(mPlaceholderScript.BuildingValue);
            mMiner.SubtractResource(mPlaceholderScript.BuildingValue);
        }
    }

    private void ChangeModeType()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (!mIsInBuildMode && mIsInMineMode)
            {
                mIsInMineMode = false;
                mIsInBuildMode = true;
            }
            else
            {
                mIsInMineMode = true;
                mIsInBuildMode = false;
            }
        }
          
    }

    private void ChangeBuildType()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TogglePlaceholder(mAllPlaceholder[mBuidlIdx]);
            ToggleBuilding(mAllBuildings[mBuidlIdx]);

            mBuidlIdx++;
            if (mBuidlIdx >= mAllBuildings.Count)
                mBuidlIdx = 0;
        }
    }

    private void TogglePlaceholder(GameObject _currentPlaceholder)
    {
        foreach (GameObject placeholder in mAllPlaceholder)
        {
            if (placeholder == _currentPlaceholder)
            {
                placeholder.SetActive(true);
                mCurrentPlaceholder = placeholder;
                mPlaceholderScript = placeholder.GetComponent<PlaceholderScript>();
            }
            else
                placeholder.SetActive(false);
        }
    }

    private void ToggleBuilding(GameObject _currentBuilding)
    {
        foreach (GameObject building in mAllBuildings)
        {
            if (building == _currentBuilding)
            {
                mCurrentBuilding = building;
            }
        }
    }
}
