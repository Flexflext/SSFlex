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

    private int mBuidlIdx = 1;

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
    private GameObject mBuildPoint;

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

    [SerializeField]
    private GameObject mNormalWallPlaceholder;
    [SerializeField]
    private GameObject mNormalGatePlaceholder;
    [SerializeField]
    private GameObject mHalfWallPlaceholder;
    [SerializeField]
    private GameObject mNormalStairsPlaceholder;

    private GameObject mCurrentPlaceholder;
    private List<GameObject> mAllPlaceholder;

    [SerializeField]
    private Camera mMainCam;

    [SerializeField]
    private PlaceholderScript mPlaceholderScript;

    private GameObject mHitObj;

    private NormalBuildingInfo mHitObjInfo;

    private Collider mCurrentCollider;

    private NormalBuildingInfo.EClipSideSlots mHitSlotToAdd_Side;
    private NormalBuildingInfo.EClipSideSlots mCurrentSlotToAdd_Side;

    private NormalBuildingInfo.EClipFaceSlots mHitSlotToAdd_Face;
    private NormalBuildingInfo.EClipFaceSlots mCurrentSlotToAdd_Face;

    private BuildingDimensions.EBuildingDimensions mHitObjDimension;
    private BuildingDimensions.EBuildingDimensions mCurrentDimension;

    private float mDotProductSide;
    private float mDotProductUp;

    private void Start()
    {
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

        mCurrentCollider = mNormalWall.GetComponentInChildren<Collider>();
        mHitSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.none;
        mCurrentSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.none;

        mHitSlotToAdd_Face = NormalBuildingInfo.EClipFaceSlots.none;
        mCurrentSlotToAdd_Face = NormalBuildingInfo.EClipFaceSlots.none;
    }

    private void Update()
    {
        Debug.Log(mIsClipped);

        ChangeBuildType();

        ManageBuildingClip();

        ManageBuildMode();
    }

    private void ManageBuildMode()
    {
        if (mIsInBuildMode)
        {
            mCurrentPlaceholder.SetActive(true);

            if (Input.GetKeyDown(KeyCode.Q))
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

        //Debug.Log(mIsClipped);
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

        Vector3 hitObjSize = BuildingDimensions.Instance.mBuildingDimensions[(int)mHitObjDimension] / 2;
        Vector3 currentObjSize = BuildingDimensions.Instance.mBuildingDimensions[(int)mCurrentDimension] / 2;
        Vector3 totalPlaceAdj = hitObjSize + currentObjSize;

        Vector3 posToSet = new Vector3();

        //Vector3 hitObjSize = mHitObjCollider.transform.localScale / 2;
        //Vector3 currentObjSize = mNormalWall.transform.localScale / 2;
        //Vector3 totalPlaceAdj = hitObjSize + currentObjSize;


        if (mDotProductSide < mClipThreshold_Side && mDotProductSide > -mClipThreshold_Side && mDotProductUp < mClipThreshold_Up && mDotProductUp > -mClipThreshold_Up)
        {
            if (mHitObj.transform.localEulerAngles.y <= 180)
            {
                if (mRelativeHitDot <= 0 && !mHitObjInfo.OccupiedFaceSlots.Contains(NormalBuildingInfo.EClipFaceSlots.front))
                {
                    posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(0, 0, totalPlaceAdj.z / 2));
                    mHitSlotToAdd_Face = NormalBuildingInfo.EClipFaceSlots.front;
                    mCurrentSlotToAdd_Face = NormalBuildingInfo.EClipFaceSlots.behind;
                }
                else if (mRelativeHitDot > 0 && !mHitObjInfo.OccupiedFaceSlots.Contains(NormalBuildingInfo.EClipFaceSlots.behind))
                {
                    posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(0, 0, -totalPlaceAdj.z / 2));
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
                    posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(0, 0, -totalPlaceAdj.z / 2));
                    mHitSlotToAdd_Face = NormalBuildingInfo.EClipFaceSlots.behind;
                    mCurrentSlotToAdd_Face = NormalBuildingInfo.EClipFaceSlots.front;
                }
                else if(mRelativeHitDot > 0 && !mHitObjInfo.OccupiedFaceSlots.Contains(NormalBuildingInfo.EClipFaceSlots.front))
                {
                    posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(0, 0, totalPlaceAdj.z / 2));
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
                        mHitSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.left;
                        mCurrentSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.right;


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
                            mHitSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.left;
                            mCurrentSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.right;
                            //Debug.Log("- links vorne");
                        }
                        if (mRelativeHitDot > 0 && !mHitObjInfo.OccupiedSideSlots.Contains(NormalBuildingInfo.EClipSideSlots.right))
                        {
                            posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(-totalPlaceAdj.x / 2, 0, totalPlaceAdj.z));
                            mHitSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.right;
                            mCurrentSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.left;
                            //Debug.Log("- rechts hinten");
                        }
                    }
                    else
                    {
                        if (mRelativeHitDot < 0 && !mHitObjInfo.OccupiedSideSlots.Contains(NormalBuildingInfo.EClipSideSlots.right))
                        {
                            posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(-totalPlaceAdj.x / 2, 0, totalPlaceAdj.z));
                            mHitSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.right;
                            mCurrentSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.left;
                            //Debug.Log("+ recht hinter");
                        }
                        if (mRelativeHitDot > 0 && !mHitObjInfo.OccupiedSideSlots.Contains(NormalBuildingInfo.EClipSideSlots.left))
                        {
                            posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(-totalPlaceAdj.x / 2, 0, -totalPlaceAdj.z));
                            mHitSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.left;
                            mCurrentSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.right;
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
                        mHitSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.right;
                        mCurrentSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.left;
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
                            mHitSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.left;
                            mCurrentSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.right;
                            //Debug.Log("+ links hinter");
                        }
                        if (mRelativeHitDot > 0 && !mHitObjInfo.OccupiedSideSlots.Contains(NormalBuildingInfo.EClipSideSlots.right))
                        {
                            posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(totalPlaceAdj.x / 2, 0, -totalPlaceAdj.z));
                            mHitSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.right;
                            mCurrentSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.left;
                            //Debug.Log("+ rechts vorne");
                        }
                    }
                    else
                    {
                        if (mRelativeHitDot < 0 && !mHitObjInfo.OccupiedSideSlots.Contains(NormalBuildingInfo.EClipSideSlots.right))
                        {
                            posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(totalPlaceAdj.x / 2, 0, -totalPlaceAdj.z));
                            mHitSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.right;
                            mCurrentSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.left;
                            //Debug.Log("- rechts vorne");
                        }
                        if (mRelativeHitDot > 0 && !mHitObjInfo.OccupiedSideSlots.Contains(NormalBuildingInfo.EClipSideSlots.left))
                        {
                            posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(totalPlaceAdj.x / 2, 0, totalPlaceAdj.z));
                            mHitSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.left;
                            mCurrentSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.right;
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

                    mHitSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.up;
                    mCurrentSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.down;
                }

                if (mDotProductUp <= -mClipThreshold_Up && !mHitObjInfo.OccupiedSideSlots.Contains(NormalBuildingInfo.EClipSideSlots.up))
                {
                    posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(0, totalPlaceAdj.y, 0));

                    mHitSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.down;
                    mCurrentSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.up;
                }
            }
        }
        else
        {
            mHitSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.none;
            mCurrentSlotToAdd_Side = NormalBuildingInfo.EClipSideSlots.none;
            Debug.Log("No valid Clip Pos");
        }





        

        SetClippedBuildingRotation();
        SetClippedBuildingPos(posToSet);

        //NormalWallPlaceholder.transform.rotation = new Quaternion(mHitObj.transform.rotation.x, mHitObj.transform.rotation.y / 2, mHitObj.transform.rotation.z, mHitObj.transform.rotation.w);

        //Debug.Log(mRotFloat);

        if (Input.GetKeyDown(KeyCode.Mouse0) && mPlaceholderScript.ValidPosition)
        {
            if(mHitSlotToAdd_Side != NormalBuildingInfo.EClipSideSlots.none)
                mHitObjInfo.AddClipSlotSide(mHitSlotToAdd_Side);
            if(mHitSlotToAdd_Face != NormalBuildingInfo.EClipFaceSlots.none)
                mHitObjInfo.AddClipSlotFace(mHitSlotToAdd_Face);

            GameObject currentBuilding = Instantiate(mCurrentBuilding, mCurrentPlaceholder.transform.position, mCurrentPlaceholder.transform.rotation);
            NormalBuildingInfo currenBuildingInfo = currentBuilding.GetComponent<NormalBuildingInfo>();

            if(mCurrentSlotToAdd_Side != NormalBuildingInfo.EClipSideSlots.none)
                currenBuildingInfo.AddClipSlotSide(mCurrentSlotToAdd_Side);

            if (mCurrentSlotToAdd_Face != NormalBuildingInfo.EClipFaceSlots.none)
                currenBuildingInfo.AddClipSlotFace(mCurrentSlotToAdd_Face);

            if (mHitSlotToAdd_Side == NormalBuildingInfo.EClipSideSlots.up && mCurrentSlotToAdd_Side == NormalBuildingInfo.EClipSideSlots.down || mHitObjInfo.IsFirstFloor)
            {
                if(mHitSlotToAdd_Side != NormalBuildingInfo.EClipSideSlots.down)
                    currenBuildingInfo.SetFirstFloor();
            }
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
        posHeightY = mCurrentPlaceholder.transform.localScale.y / 2;

        float posX = Mathf.Round(_posToSet.x);
        float posY = Mathf.Round(_posToSet.y);
        float posZ = Mathf.Round(_posToSet.z);
        Vector3 actualPos = new Vector3(posX, posY, posZ);

        mCurrentPlaceholder.transform.position = new Vector3(actualPos.x, 0 , actualPos.z);
        mCurrentPlaceholder.transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + mRotFloat, transform.eulerAngles.z);

        if (Input.GetKeyDown(KeyCode.Mouse0) && mPlaceholderScript.ValidPosition)
        {
            GameObject currentBuilding = Instantiate(mCurrentBuilding, mCurrentPlaceholder.transform.position, mCurrentPlaceholder.transform.rotation);
            currentBuilding.GetComponent<NormalBuildingInfo>().AddClipSlotSide(NormalBuildingInfo.EClipSideSlots.down);
        }
    }

    private void ChangeBuildType()
    {
        if (Input.GetKeyDown(KeyCode.B))
            mIsInBuildMode = true;

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
