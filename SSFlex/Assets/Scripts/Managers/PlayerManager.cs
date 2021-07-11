using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    private PhotonView photonView;
    private Vector3 player1SpawnPoint = new Vector3(-90, 5, 0);

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (photonView.IsMine)
        {
            CreateController();
        }
    }

    private void CreateController()
    {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), player1SpawnPoint, Quaternion.identity);
    }
}
