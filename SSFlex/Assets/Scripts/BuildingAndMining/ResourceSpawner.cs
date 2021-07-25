using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ResourceSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private string[] mResources;

    [SerializeField]
    private int mAmountOfSpawns;

    private GameObject mResource;

    [SerializeField]
    private Collider mCollider;


    public void SpawnResource()
    {
        for (int i = 0; i < mAmountOfSpawns; i++)
        {
            string resourceToSet = "";
            Vector3 rotation = new Vector3();
            rotation.y = Random.Range(-180f, 180f);

            float rndPosX = Random.Range(transform.position.x - mCollider.bounds.extents.x, transform.position.x + mCollider.bounds.extents.x);
            float rndPosZ = Random.Range(transform.position.z - mCollider.bounds.extents.z, transform.position.z + mCollider.bounds.extents.z);
            Vector3 spawnPos = new Vector3(rndPosX, -1, rndPosZ);

            int rndResource = Random.Range(0, 2);

            if (rndResource == 0)
                resourceToSet = mResources[0];
            else
                resourceToSet = mResources[1];

            mResource = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", resourceToSet), spawnPos, Quaternion.Euler(rotation));
        }      
    }
}
