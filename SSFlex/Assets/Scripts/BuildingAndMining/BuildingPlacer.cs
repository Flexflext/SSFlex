using AvailableBuildingDimensions;
using Photon.Pun;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


/// <summary>
/// Written by Max
/// 
/// This class is responsible for the building behaviour
/// 
/// Gives two option
/// 1. Placing a building normal
/// 2. Clipping a building on an already existing one
/// </summary>
public class BuildingPlacer : MonoBehaviourPunCallbacks
{
    public GameObject HitObj => mHitObj;

    public float MaxBuildHeight => mMaxBuildHeight;
    public bool IsClipped => mIsClipped;
    public bool IsInBuildMode => mIsInBuildMode;
    public bool IsInMineMode => mIsInMineMode;

    // The Max range for the player to build
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
    // float to add up on a face position
    [SerializeField]
    private float mThicknessAdj;

    [Header("Components")]
    // Build point is the position on which the player would place his building
    [SerializeField]
    private GameObject mBuildPoint;
    [SerializeField]
    private Camera mMainCam;
    [SerializeField]
    private ResourceMiner mMiner;
    // The Layer on which the player is allowed to build
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
    
    // The currently choosen building that ought to be build
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

    // The currently choosen building Placeholder of the building that ought to be build
    private GameObject mCurrentPlaceholder;
    private List<GameObject> mAllPlaceholder;

    // The clip slots around the building, including the buildings sides and face 
    private NormalBuildingInfo.EClipSideSlots mHitSlotToAdd_Side;
    private NormalBuildingInfo.EClipSideSlots mCurrentSlotToAdd_Side;

    private NormalBuildingInfo.EClipFaceSlots mHitSlotToAdd_Face;
    private NormalBuildingInfo.EClipFaceSlots mCurrentSlotToAdd_Face;

    // The size of the building the player is looking at
    private BuildingDimensions.EBuildingDimensions mHitObjDimension;
    // The size of the building the player is building
    private BuildingDimensions.EBuildingDimensions mCurrentDimension;
    // The dimensions of all buildings
    private BuildingDimensions mAvailableDimensions;

    // Building informations about the building the player is looking at
    private NormalBuildingInfo mHitObjInfo;
    private Collider mCurrentCollider;
    private GameObject mHitObj;

    // The rotation of the placeholder
    private float mRotFloat;
    // The dot product between the player and the Building he is looking at, used to determine the rotation direction of the building that ought to be build
    private float mRelativeHitDot;

    // Indexer to toggle through the builing List
    private int mBuidlIdx = 1;
    // Bool to check if the player is in mine mode
    private bool mIsInMineMode = true;

    // Bool to check if the player is in build mode
    private bool mIsInBuildMode;
    private bool mIsClipped;

    // The dot products to check where the clipped building has to be placed
    private float mDotProductSide;
    private float mDotProductUp;

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
        mCurrentDimension = mCurrentPlaceholder.GetComponent<PlaceholderScript>().CurrentDimension;

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

        if (RoundStartManager.Instance.PreparationPhase)
        {
            ManageBuildMode();
            ManageBuildingClip();
            ChangeModeType();
            ChangeBuildType();
        }
        else if (mIsInBuildMode && !RoundStartManager.Instance.PreparationPhase)
        {
            mIsInBuildMode = false;
            mCurrentPlaceholder.SetActive(false);
        }
    }

    /// <summary>
    /// Manages the rotation of the building as well as which behaviour has to be executed, clipped or normal
    /// </summary>
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

    /// <summary>
    /// Method to check if the building that ought to be build has to clip or not
    /// 
    /// 1. Sets the position of the build point on the hit position
    /// 2. Checks if the player is looking at an building on which he can clip the one he wishes to build
    /// 3. Calculates the Dot product for side and up clip to determine on which side the building has to clip
    /// </summary>
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

    /// <summary>
    /// Behaviour for the clipped building
    /// 
    /// 1.  Calculates the Direction vector to the Building 
    /// 2.  Calculates the dotproduct to hit hit object
    /// 3.  Sets the RelativeHitDot either minus or plus in accordance to the Hit object rotation
    /// 4.  Gets the dimensions of the Hit object
    /// 5.  Adds both, the Hit object and the to be build objects Dimensions, together
    /// 6.  Checks on which position the building should clip on by checking the Dot product and Object rotation
    /// 
    ///     The Buildings all have Slots on each side and its front aswell as its behind
    ///    
    /// 7.  The face slots get added upon building, the side slots get added from the building itself
    /// 8.  If a slot is found, the position on which the building ought to be placed is calculated by adding the Transformed Direction
    ///     of the combined scale of both buildings to the position of the Hit object
    /// 10. If the building that the player wants to build NOT a normal wall and on the upper side of the Hit object, the build height gets adjusted
    /// 11. Calls SetClippedBuildingRotation to set the buildings rotation and SetClippedBuildingPos with the calculated position
    /// 12. If the player hits the left mouse button, the choosen building will be Instantiated at the position of the placeholder
    /// 13. Sets the building is on ground level if it is
    /// </summary>
    private void GetClippedBuildingPos_Side()
    {

        Vector3 dirToHitObj = mHitObj.transform.position - transform.position;

        mRelativeHitDot = Vector3.Dot(mHitObj.transform.forward, dirToHitObj);

        if (mHitObj.transform.localEulerAngles.y >= 0 && mHitObj.transform.localEulerAngles.y <= 180)
            mRelativeHitDot = +mRelativeHitDot;
        else
            mRelativeHitDot = -mRelativeHitDot;

        mHitObjDimension = mHitObjInfo.CurrentDimension;  

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

        if (mDotProductSide >= mClipThreshold_Side || mDotProductSide <= -mClipThreshold_Side || mDotProductSide < mClipThreshold_Side && mDotProductSide > -mClipThreshold_Side)
        {
            if (mDotProductSide >= mClipThreshold_Side)
            {
                if (mRotFloat == 0)
                {
                    if (!mHitObjInfo.OccupiedSideSlots.Contains(NormalBuildingInfo.EClipSideSlots.left))
                        posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(-totalPlaceAdj.x, 0, 0));
                }
                else if (mRotFloat != 0)
                {
                    if (mHitObj.transform.localEulerAngles.y >= 180)
                    {
                        if (mRelativeHitDot < 0 && !mHitObjInfo.OccupiedSideSlots.Contains(NormalBuildingInfo.EClipSideSlots.left))
                            posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(-totalPlaceAdj.x / 2, 0, -totalPlaceAdj.z));

                        if (mRelativeHitDot > 0 && !mHitObjInfo.OccupiedSideSlots.Contains(NormalBuildingInfo.EClipSideSlots.right))
                            posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(-totalPlaceAdj.x / 2, 0, totalPlaceAdj.z));
                    }
                    else
                    {
                        if (mRelativeHitDot < 0 && !mHitObjInfo.OccupiedSideSlots.Contains(NormalBuildingInfo.EClipSideSlots.right))
                            posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(-totalPlaceAdj.x / 2, 0, totalPlaceAdj.z));

                        if (mRelativeHitDot > 0 && !mHitObjInfo.OccupiedSideSlots.Contains(NormalBuildingInfo.EClipSideSlots.left))
                            posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(-totalPlaceAdj.x / 2, 0, -totalPlaceAdj.z));
                    }
                }
            }
            

            if (mDotProductSide <= -mClipThreshold_Side)
            {
                if (mRotFloat == 0)
                {
                    if (!mHitObjInfo.OccupiedSideSlots.Contains(NormalBuildingInfo.EClipSideSlots.right))
                        posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(totalPlaceAdj.x, 0, 0));
                }
                else if (mRotFloat != 0)
                {
                    if (mHitObj.transform.localEulerAngles.y >= 0 && mHitObj.transform.localEulerAngles.y < 180f)
                    {
                        if (mRelativeHitDot < 0 && !mHitObjInfo.OccupiedSideSlots.Contains(NormalBuildingInfo.EClipSideSlots.left))
                            posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(totalPlaceAdj.x / 2, 0, totalPlaceAdj.z));

                        if (mRelativeHitDot > 0 && !mHitObjInfo.OccupiedSideSlots.Contains(NormalBuildingInfo.EClipSideSlots.right))
                            posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(totalPlaceAdj.x / 2, 0, -totalPlaceAdj.z));
                    }
                    else
                    {
                        if (mRelativeHitDot < 0 && !mHitObjInfo.OccupiedSideSlots.Contains(NormalBuildingInfo.EClipSideSlots.right))
                            posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(totalPlaceAdj.x / 2, 0, -totalPlaceAdj.z));

                        if (mRelativeHitDot > 0 && !mHitObjInfo.OccupiedSideSlots.Contains(NormalBuildingInfo.EClipSideSlots.left))
                            posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(totalPlaceAdj.x / 2, 0, totalPlaceAdj.z));
                    }
                }
            }

            if (mDotProductSide < mClipThreshold_Side && mDotProductSide > -mClipThreshold_Side)
            {
                if (mDotProductUp >= mClipThreshold_Up && mHitObj.transform.position.y - hitObjSize.y / 2 > 0 && !mHitObjInfo.OccupiedSideSlots.Contains(NormalBuildingInfo.EClipSideSlots.down) && mHitObjInfo.IsFirstFloor)
                    posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(0, -totalPlaceAdj.y, 0));

                if (mDotProductUp <= -mClipThreshold_Up && !mHitObjInfo.OccupiedSideSlots.Contains(NormalBuildingInfo.EClipSideSlots.up))
                    posToSet = mHitObj.transform.position + mHitObj.transform.TransformDirection(new Vector3(0, totalPlaceAdj.y, 0));
            }
        }
        

        if (mCurrentDimension != BuildingDimensions.EBuildingDimensions.normalWall && mDotProductSide < mClipThreshold_Side && mDotProductSide > -mClipThreshold_Side)
            posToSet.y -= mAvailableDimensions.mHeightAdjDimensions[(int)mCurrentDimension];

        if (posToSet != Vector3.zero)
        {
            SetClippedBuildingRotation();
            SetClippedBuildingPos(posToSet);
        }
        else
            SetBuildingNormal(mBuildPoint.transform.position);


        if (Input.GetKeyDown(KeyCode.Mouse0) && mPlaceholderScript.ValidPosition)
        {
            string objToBuild = mPlaceholderScript.ObjName;

            GameObject currentBuilding = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", objToBuild), mCurrentPlaceholder.transform.position, mCurrentPlaceholder.transform.rotation);
            NormalBuildingInfo currenBuildingInfo = currentBuilding.GetComponent<NormalBuildingInfo>();

            mMiner.SubtractResource(mPlaceholderScript.BuildingValue);

            if (currentBuilding.transform.position.y > mCurrentPlaceholder.transform.position.y)
                currenBuildingInfo.SetFirstFloor();
        }
    }

    /// <summary>
    /// Sets the rotation of the clipped building as the rotation of the hit object
    /// </summary>
    private void SetClippedBuildingRotation( )
    {
        mCurrentPlaceholder.transform.eulerAngles = new Vector3(mHitObj.transform.eulerAngles.x, mHitObj.transform.eulerAngles.y + mRotFloat, mHitObj.transform.eulerAngles.z);
    }

    /// <summary>
    /// Sets the position of the placeholder to the calculated position
    /// </summary>
    /// <param name="_posToAdj"></param>
    private void SetClippedBuildingPos(Vector3 _posToAdj)
    {
        mCurrentPlaceholder.transform.position = _posToAdj;
    }

    /// <summary>
    /// The Normal building behaviour
    /// 
    /// 1. Rounds the position of the building point
    /// 2. Sets the position and rotation of the placeholder 
    /// 3. If the player hits the left mouse button, the choosen building will be Instantiated at the position of the placeholder
    /// </summary>
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
            string objToBuild = mPlaceholderScript.ObjName;

            GameObject currentBuilding = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", objToBuild), mCurrentPlaceholder.transform.position, mCurrentPlaceholder.transform.rotation);
            currentBuilding.GetComponent<NormalBuildingInfo>().AddClipSlotSide(NormalBuildingInfo.EClipSideSlots.down);
            mMiner.SubtractResource(mPlaceholderScript.BuildingValue);
        }
    }

    /// <summary>
    /// Toggles between the build and farm mode
    /// </summary>
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

    /// <summary>
    /// Toggles between the different buildings and their respective placeholder
    /// </summary>
    private void ChangeBuildType()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TogglePlaceholder(mAllPlaceholder[mBuidlIdx]);
            ToggleBuilding(mAllBuildings[mBuidlIdx]);

            mCurrentDimension = mCurrentPlaceholder.GetComponent<PlaceholderScript>().CurrentDimension;

            mBuidlIdx++;
            if (mBuidlIdx >= mAllBuildings.Count)
                mBuidlIdx = 0;
        }
    }

    /// <summary>
    /// Toggles the Placeholder of the buildings
    /// </summary>
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

    /// <summary>
    /// Toggle the buildings
    /// </summary>
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
