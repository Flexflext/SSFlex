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

    private Team team;
    GameObject player;

    private Dictionary<int, Team> playerIdTeam;

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

    public void ChangeTeam(Team _team)
    {
        team = _team;
    }

    private void CreateController()
    {
        // Instatntiates the PlayerController and sets its Spawnpoint.
        player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), player1SpawnPoint, Quaternion.identity);

        switch (team)
        {
            case Team.Blue:
                player.GetComponent<PlayerGFXChange>().ChangePlayerGfx(Team.Blue);
                player.transform.position = player2SpawnPoint;
                break;
            case Team.Red:
                player.GetComponent<PlayerGFXChange>().ChangePlayerGfx(Team.Red);
                player.transform.position = player1SpawnPoint;
                break;
            case Team.Yellow:
                player.GetComponent<PlayerGFXChange>().ChangePlayerGfx(Team.Yellow);
                player.transform.position = player3SpawnPoint;
                break;
            case Team.Green:
                player.GetComponent<PlayerGFXChange>().ChangePlayerGfx(Team.Green);
                player.transform.position = player4SpawnPoint;
                break;
            default:
                break;
        }
    }
}
