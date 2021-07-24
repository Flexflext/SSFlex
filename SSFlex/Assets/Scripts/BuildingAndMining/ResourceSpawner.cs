using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> mResourceSpawnPoints;

    [SerializeField]
    private string[] mResources;

    public void SpawnResources()
    {
        for (int i = 0; i < mResourceSpawnPoints.Count; i++)
        {
            string resourceToSet = "";
            Vector3 rotation = new Vector3();
            rotation.y = Random.Range(-180f, 180f);

            int rndResource = Random.Range(0, 2);

            if (rndResource == 0)
                resourceToSet = mResources[0];
            else
                resourceToSet = mResources[1];

            GameObject resource = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", resourceToSet), mResourceSpawnPoints[i].transform.position, Quaternion.Euler(rotation));
            resource.transform.SetParent(mResourceSpawnPoints[i].transform);
        }
    }
}
