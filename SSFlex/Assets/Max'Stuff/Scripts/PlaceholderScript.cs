using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AvailableBuildingDimensions;

public class PlaceholderScript : MonoBehaviour
{
    public BuildingDimensions.EBuildingDimensions CurrentDimension => mCurrentDimension;
    public bool ValidPosition => mValidPosition;
    public int BuildingValue => mBuildingValue;
    public string ObjName => mObjName;

    [Header("Building name and Cost")]
    [SerializeField]
    private string mObjName;
    [SerializeField]
    private int mBuildingValue;

    [Header("Components")]
    [SerializeField]
    private MeshRenderer mMeshRenderer;
    [SerializeField]
    private BuildingPlacer mBuildPlacer;
    [SerializeField]
    private ResourceMiner mMiner;
    [SerializeField]
    private LayerMask mBuildLayer;

    [Header("Valid and Unvalid Mats for the Object")]
    [SerializeField]
    private Material mValidMaterial;
    [SerializeField]
    private Material mUnvalidMaterial;

    [Header("The Dimensions of the Buildings Collider")]
    [SerializeField]
    private BuildingDimensions.EBuildingDimensions mCurrentDimension;
    private BuildingDimensions mAvailableDimensions;

    // Position Validation bool
    private bool mValidPosition = true;
    private int mResourceAmount;

    private void Start()
    {
        mAvailableDimensions = new BuildingDimensions();
    }

    private void Update()
    {
        if(!mValidPosition)
            mMeshRenderer.material = mUnvalidMaterial;
        else if(mMeshRenderer.material != mValidMaterial)
            mMeshRenderer.material = mValidMaterial;

        mResourceAmount = mMiner.ResourceAmount;


        if (transform.position.y > mBuildPlacer.MaxBuildHeight)
            mValidPosition = false;
        else if (!mBuildPlacer.IsClipped && transform.position.y > mAvailableDimensions.mBuildingDimensions[(int)mCurrentDimension].y / 2)
            mValidPosition = false;
        else if (mResourceAmount > mBuildingValue)
        {
            Collider[] overlapColl = Physics.OverlapBox(transform.position, mAvailableDimensions.mBuildingDimensions[(int)mCurrentDimension] / 2.85f, transform.rotation);


            for (int i = 0; i < overlapColl.Length; i++)
            {
                if (overlapColl[i].gameObject.CompareTag("ClipTag") && overlapColl[i].gameObject != this.gameObject && mBuildPlacer.HitObj != null && overlapColl[i].gameObject != mBuildPlacer.HitObj)
                {
                    Debug.Log("DD"); ;
                    mValidPosition = false;
                    return;
                }
                else if (overlapColl[i].gameObject.CompareTag("ClipTag") && overlapColl[i].gameObject != this.gameObject && mBuildPlacer.HitObj == null)
                {
                    Debug.Log("DD"); ;

                    mValidPosition = false;
                    return;
                }
                else if (overlapColl[i].gameObject.CompareTag("Blocker"))
                {
                    Debug.Log("DD"); ;

                    mValidPosition = false;
                    return;
                }
                else if (overlapColl[i].gameObject.CompareTag("Player"))
                {
                    Debug.Log("DD"); ;

                    mValidPosition = false;
                    return;
                }
                else
                    mValidPosition = true;
            }
        }
        else
            mValidPosition = false;
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
