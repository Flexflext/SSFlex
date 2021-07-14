using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Select Name Text")]
    [SerializeField] private TMP_Text playerRed;
    [SerializeField] private TMP_Text playerBlue;
    [SerializeField] private TMP_Text playerYellow;
    [SerializeField] private TMP_Text playerGreen;

    [Header("Button GameObejcts")]
    [SerializeField] private GameObject selectRedButton;
    [SerializeField] private GameObject selectBlueButton;
    [SerializeField] private GameObject selectYellowButton;
    [SerializeField] private GameObject selectGreenButton;

    [SerializeField] private GameObject startGameButton;

    [SerializeField] private Dictionary<int, Team> playerIdTeam = new Dictionary<int, Team>();


    private Team team;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.SetActive(true);
        }
    }

    public void ChangeTeam(int _teamnum)
    {
        switch (_teamnum)
        {
            case 1:
                team = Team.Red;
                selectBlueButton.SetActive(false);
                selectYellowButton.SetActive(false);
                selectGreenButton.SetActive(false);
                break;
            case 2:
                team = Team.Blue;                
                selectRedButton.SetActive(false);
                selectYellowButton.SetActive(false);
                selectGreenButton.SetActive(false);
                break;
            case 3:
                team = Team.Yellow;
                selectBlueButton.SetActive(false);
                selectRedButton.SetActive(false);
                selectGreenButton.SetActive(false);
                break;
            case 4:
                team = Team.Green;
                selectBlueButton.SetActive(false);
                selectRedButton.SetActive(false);
                selectYellowButton.SetActive(false);
                break;

            default:
                break;
        }

        RoomManager.Instance.ChangeTeam(team);
        photonView.RPC("DisplayTeam", RpcTarget.AllBufferedViaServer, PhotonNetwork.LocalPlayer.NickName, _teamnum, PhotonNetwork.LocalPlayer.ActorNumber);
    }


    public void ReturnMenu()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.AdminLoadingMenu();
    }

    public void StartGame()
    {
        
        PhotonNetwork.LoadLevel(2);
    }

    // If the master of the room has left but there is still one client in this client will become the master and gets the startgame button.
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.AdminMainMenu();
    }


    #region RPC Calls

    [PunRPC]
    private void DisplayTeam(string _name, int _team, int _id)
    {

        switch (_team)
        {
            case 1:
                playerRed.text = _name;
                selectRedButton.SetActive(false);
                playerIdTeam.Add(_id, Team.Red);
                break;
            case 2:
                playerBlue.text = _name;
                selectBlueButton.SetActive(false);
                playerIdTeam.Add(_id, Team.Blue);
                break;
            case 3:
                playerYellow.text = _name;
                selectYellowButton.SetActive(false);
                playerIdTeam.Add(_id, Team.Yellow);
                break;
            case 4:
                playerGreen.text = _name;
                selectGreenButton.SetActive(false);
                playerIdTeam.Add(_id, Team.Green);
                break;
            default:
                break;
        }
    }

    #endregion
}
