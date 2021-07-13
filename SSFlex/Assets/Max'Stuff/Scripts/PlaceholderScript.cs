using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceholderScript : MonoBehaviour
{
    public BuildingDimensions.EBuildingDimensions CurrentDimension => mCurrentDimension;
    public bool ValidPosition => mValidPosition;

    private bool mValidPosition = true;

    [SerializeField]
    private MeshRenderer mMeshRenderer;
    [SerializeField]
    private BuildingPlacer mBuildPlacer;

    [SerializeField]
    private Material mValidMaterial;
    [SerializeField]
    private Material mUnvalidMaterial;

    [SerializeField]
    private LayerMask mBuildLayer;

    [SerializeField]
    private BuildingDimensions.EBuildingDimensions mCurrentDimension;

    private void Update()
    {
        if(!mValidPosition)
            mMeshRenderer.material = mUnvalidMaterial;
        else if(mMeshRenderer.material != mValidMaterial)
            mMeshRenderer.material = mValidMaterial;

        //if (transform.position.y >= mBuildPlacer.MaxBuildHeight)
        //    mValidPosition = false;
        
        //if (!mBuildPlacer.IsClipped && transform.position.y > transform.position.y + transform.localScale.y / 2)
        //    mValidPosition = false;

        Collider[] overlapColl = Physics.OverlapBox(transform.position, BuildingDimensions.Instance.mBuildingDimensions[(int)mCurrentDimension] / 2, transform.rotation);

        int unvalid = 0;

        for (int i = 0; i < overlapColl.Length; i++)
        {
            if (overlapColl[i].gameObject.CompareTag("ClipTag") && overlapColl[i].gameObject != this.gameObject && mBuildPlacer.HitObj != null && overlapColl[i].gameObject != mBuildPlacer.HitObj)
            {
                mValidPosition = false;
                Debug.LogWarning("DDDDDDDDDDDDDDD");
                return;
            }
            else if (overlapColl[i].gameObject.CompareTag("Blocker"))
            {
                mValidPosition = false;
                return;
            }
            else if (overlapColl[i].gameObject.CompareTag("Player"))
            {
                mValidPosition = false;
                return;
            }
            else
                mValidPosition = true;

        }

    }

    

    //private void OnTriggerEnter(Collider other)
    //{
    //    if(!mBuildPlacer.IsClipped && other.CompareTag("Ground") || other.CompareTag("ClipTag"))
    //        mValidPosition = false;
    //    else if (mBuildPlacer.IsClipped && other.CompareTag("Ground") || other.CompareTag("ClipTag"))
    //        mValidPosition = true;
    //    else
    //        mValidPosition = false;
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if(other.CompareTag("ClipTag"))
    //        mValidPosition = true;
    //}
}
