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
                RoomManager.Instance.ChangeTeam(Team.Red);
                selectBlueButton.SetActive(false);
                selectYellowButton.SetActive(false);
                selectGreenButton.SetActive(false);
                photonView.RPC("DisplayTeamRed", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName);
                break;
            case 2:
                RoomManager.Instance.ChangeTeam(Team.Blue);
                selectRedButton.SetActive(false);
                selectYellowButton.SetActive(false);
                selectGreenButton.SetActive(false);
                photonView.RPC("DisplayTeamBlue", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName);
                break;
            case 3:
                RoomManager.Instance.ChangeTeam(Team.Yellow);
                selectBlueButton.SetActive(false);
                selectRedButton.SetActive(false);
                selectGreenButton.SetActive(false);
                photonView.RPC("DisplayTeamYellow", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName);
                break;
            case 4:
                RoomManager.Instance.ChangeTeam(Team.Green);
                selectBlueButton.SetActive(false);
                selectRedButton.SetActive(false);
                selectYellowButton.SetActive(false);
                photonView.RPC("DisplayTeamGreen", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName);
                break;

            default:
                break;
        }
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
    private void DisplayTeamRed(string _name)
    {
        playerRed.text = _name;
        selectRedButton.SetActive(false);
    }

    [PunRPC]
    private void DisplayTeamBlue(string _name)
    {
        playerBlue.text = _name;
        selectBlueButton.SetActive(false);
    }

    [PunRPC]
    private void DisplayTeamYellow(string _name)
    {
        playerYellow.text = _name;
        selectYellowButton.SetActive(false);
    }

    [PunRPC]
    private void DisplayTeamGreen(string _name)
    {
        playerGreen.text = _name;
        selectGreenButton.SetActive(false);
    }

    #endregion
}
