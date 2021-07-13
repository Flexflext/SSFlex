using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    private PhotonView photonView;
    private Vector3 player1SpawnPoint = new Vector3(-90, 5, 0);
    private Vector3 player2SpawnPoint = new Vector3(90, 5, 0);
    private Vector3 player3SpawnPoint = new Vector3(0, 5, -90);
    private Vector3 player4SpawnPoint = new Vector3(0, 5, 90);

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
        // Instatntiates the PlayerController and sets its Spawnpoint.
        GameObject player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), player1SpawnPoint, Quaternion.identity);


        int teamNum = PlayerTeamManager.Instance.Subscribe(player.GetComponent<PlayerController>());

        switch (teamNum)
        {
            case 1:
                player.GetComponent<PlayerGFXChange>().ChangePlayerGfx(Team.Blue);
                player.transform.position = player2SpawnPoint;
                break;
            case 2:
                player.GetComponent<PlayerGFXChange>().ChangePlayerGfx(Team.Red);
                player.transform.position = player1SpawnPoint;
                break;
            case 3:
                player.GetComponent<PlayerGFXChange>().ChangePlayerGfx(Team.Yellow);
                player.transform.position = player3SpawnPoint;
                break;
            case 4:
                player.GetComponent<PlayerGFXChange>().ChangePlayerGfx(Team.Green);
                player.transform.position = player4SpawnPoint;
                break;
            default:
                break;
        }

        
    }
}
