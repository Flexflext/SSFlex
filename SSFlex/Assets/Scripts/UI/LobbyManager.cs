using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    //private string playerName;

    //private void Update()
    //{
    //    playerRed.text = playerName;
    //}

    public void ChangeTeam(int _teamnum)
    {
        switch (_teamnum)
        {
            case 1:
                RoomManager.Instance.ChangeTeam(Team.Red);
                if (photonView.IsMine)
                {
                    photonView.RPC("DisplayTeam", RpcTarget.All,PhotonNetwork.LocalPlayer.NickName);
                }
                break;
            case 2:
                RoomManager.Instance.ChangeTeam(Team.Blue);
                if (photonView.IsMine)
                {
                    photonView.RPC("DisplayTeam", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName);
                }
                break;
            case 3:
                RoomManager.Instance.ChangeTeam(Team.Yellow);
                if (photonView.IsMine)
                {
                    photonView.RPC("DisplayTeam", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName);
                }
                break;
            case 4:
                RoomManager.Instance.ChangeTeam(Team.Green);
                if (photonView.IsMine)
                {
                    photonView.RPC("DisplayTeam", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName);
                }
                break;

            default:
                break;
        }
    }



    [PunRPC]
    private void DisplayTeam(string _name)
    {
        playerRed.text = _name;
        selectRedButton.SetActive(false);
    }


}
