using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceMiner : MonoBehaviour
{
    public int ResourceAmount => mCurrentResourceAmount;
    public bool Mining => mIsMining;

    [Header("Max mining distance and speed")]
    [SerializeField]
    private float mMineSpeed;
    [SerializeField]
    private float mMaxMineDistance;

    [Header("Components")]
    [SerializeField]
    private BuildingPlacer mBuildPlacer;
    [SerializeField]
    private Camera mMainCam;
    [SerializeField]
    private LayerMask mResourceLayer;


    private MineableObject mHitObj;

    private bool mIsMining;
    private int mCurrentResourceAmount = 100;

    private void Update()
    {
        if (mBuildPlacer.IsInMineMode && Input.GetKey(KeyCode.Mouse0))
            LookForResource();
        else
            mIsMining = false;
    }

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
                    mCurrentResourceAmount += mHitObj.MineResource(mMineSpeed, this);
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

    public void SubtractResource(int _toSubtract)
    {
        mCurrentResourceAmount -= _toSubtract;
    }
}
