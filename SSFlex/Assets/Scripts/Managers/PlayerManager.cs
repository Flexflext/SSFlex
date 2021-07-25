using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    // Code: Haoke
    // Purpose: Spawns players with their player controllers at their desiganted locations.

    private PhotonView photonView;
    private Vector3 player1SpawnPoint_Red = new Vector3(-110, 3, 0);
    private Vector3 player2SpawnPoint_Blue = new Vector3(110, 3, 0);
    private Vector3 player3SpawnPoint_Yellow = new Vector3(0, 3, -110);
    private Vector3 player4SpawnPoint_Green = new Vector3(0, 3, 110);

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
        player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), player1SpawnPoint_Red, Quaternion.identity);

        switch (team)
        {
            case Team.Blue:
                player.GetComponent<PlayerGFXChange>().ChangePlayerGfx(Team.Blue);
                player.transform.position = player2SpawnPoint_Blue;
                break;
            case Team.Red:
                player.GetComponent<PlayerGFXChange>().ChangePlayerGfx(Team.Red);
                player.transform.position = player1SpawnPoint_Red;
                break;
            case Team.Yellow:
                player.GetComponent<PlayerGFXChange>().ChangePlayerGfx(Team.Yellow);
                player.transform.position = player3SpawnPoint_Yellow;
                break;
            case Team.Green:
                player.GetComponent<PlayerGFXChange>().ChangePlayerGfx(Team.Green);
                player.transform.position = player4SpawnPoint_Green;
                break;
            default:
                break;
        }
    }
}
