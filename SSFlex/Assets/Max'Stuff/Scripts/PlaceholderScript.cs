using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceholderScript : MonoBehaviour
{
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



    private void Update()
    {
        if(!mValidPosition)
            mMeshRenderer.material = mUnvalidMaterial;
        else if(mMeshRenderer.material != mValidMaterial)
            mMeshRenderer.material = mValidMaterial;

        if (transform.position.y >= mBuildPlacer.MaxBuildHeight)
            mValidPosition = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground") || other.CompareTag("ClipTag"))
            mValidPosition = true;
        else
            mValidPosition = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("ClipTag"))
            mValidPosition = true;
    }
}
