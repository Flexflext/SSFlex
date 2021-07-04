using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    [SerializeField]
    private float mMaxBuildRange;

    private bool mIsClipped;
    private bool mIsInBuildMode;
    private float PointClamp;

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

    private GameObject mHitObj;

    public GameObject DebugWall;

    private void Update()
    {
        Vector3 hitPos = new Vector3();
       
        Vector3 crosPos = mMainCam.ScreenToWorldPoint(mCrosshair.transform.position);

        Vector3 dirVec = new Vector3();

        float dotSide = 0;
        float dotUp = 0;

        RaycastHit hit;
        if (Physics.Raycast(crosPos, mMainCam.transform.forward, out hit, 1000, mBuildLayer))
        {
            hitPos = hit.point;

            if (Vector3.Distance(transform.position, hitPos) <= mMaxBuildRange)
                mBuildPoint.transform.position = new Vector3(hitPos.x, hitPos.y, hitPos.z);

            if (hit.collider.gameObject.CompareTag("ClipTag"))
            {
                mHitObj = hit.collider.gameObject;
                dirVec = mHitObj.transform.position - hitPos;

                dotSide = Vector3.Dot(mHitObj.transform.right, dirVec);
                dotUp = Vector3.Dot(mHitObj.transform.up, dirVec);
                mIsClipped = true;


                if (Input.GetKeyDown(KeyCode.Mouse1))
                    Destroy(mHitObj);
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

        //Debug.Log(dotSide + " Side");
        //Debug.Log(dotUp + " Up");

        Debug.Log(DebugWall.transform.localPosition);

        if (mIsInBuildMode)
        {
            NormalWallPlaceholder.SetActive(true);

            if (mIsClipped)
            {
                SetClippedBuilding(dotSide, dotUp);
            }
            else if (!mIsClipped)
            {
                SetBuildingNormal(mBuildPoint.transform.position);
            }
            
        }
        else
            NormalWallPlaceholder.SetActive(false);
    }

    private void SetClippedBuilding(float _sideClip, float _upClip)
    {
        Vector3 clipPos = new Vector3();

        Vector3 hitObjSize = mHitObj.transform.localScale / 2;
        Vector3 currentObjSize = mNormalWall.transform.localScale / 2;
        Vector3 totalPlaceAdj = hitObjSize + currentObjSize;


        if (_sideClip >= mClipThreshold_Side)
        {
            clipPos = mHitObj.transform.position - totalPlaceAdj;
            clipPos.y = mHitObj.transform.position.y;

            NormalWallPlaceholder.transform.position = clipPos;
        }

        if (_sideClip <= -mClipThreshold_Side)
        {
            clipPos = mHitObj.transform.position + totalPlaceAdj;
            clipPos.y = mHitObj.transform.position.y;

            NormalWallPlaceholder.transform.position = clipPos;
        }

        NormalWallPlaceholder.transform.eulerAngles = mHitObj.transform.eulerAngles;

        if (Input.GetKeyDown(KeyCode.Mouse0))
            Instantiate(mNormalWall, NormalWallPlaceholder.transform.position, NormalWallPlaceholder.transform.rotation);
    }


    private void SetBuildingNormal(Vector3 _posToSet)
    {
        float posY = 0;
        posY = NormalWallPlaceholder.transform.localScale.y / 2;

        NormalWallPlaceholder.transform.position = new Vector3(_posToSet.x, _posToSet.y + posY, _posToSet.z);
        NormalWallPlaceholder.transform.rotation = transform.rotation;

        if (Input.GetKeyDown(KeyCode.Mouse0))
            Instantiate(mNormalWall, NormalWallPlaceholder.transform.position, NormalWallPlaceholder.transform.rotation);
    }

}
