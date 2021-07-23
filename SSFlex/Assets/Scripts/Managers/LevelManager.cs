using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LevelManager : MonoBehaviourPunCallbacks
{
    public static LevelManager Instance;

    private Dictionary<string, bool> playerDead = new Dictionary<string, bool>();

    private void Awake()
    {
        Instance = this;
    }


    public void AddDictionary(string _id, bool _lifebool)
    {
        photonView.RPC("RPC_AddDictionary", RpcTarget.All, _id, _lifebool);
    }

    public void UpdateDictionary(string _id, bool _lifebool)
    {
        photonView.RPC("RPC_UpdateDictionary", RpcTarget.All, _id, _lifebool);
    }

    [PunRPC]
    private void RPC_UpdateDictionary(string _id, bool _lifebool)
    {
        if (playerDead.ContainsKey(_id))
        {
            playerDead[_id] = _lifebool;
        }
    }

    [PunRPC]
    private void RPC_AddDictionary(string _id, bool _lifebool)
    {
        playerDead.Add(_id, _lifebool);
    }

    public bool CheckIfTheOnlyOneAlive()
    {
        int peopleAlive = 0;

        foreach (var player in playerDead)
        {
            if (player.Value)
            {
                peopleAlive++;
            }
        }

        if (peopleAlive < 2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void CheckIfWon()
    {
        photonView.RPC("RPC_CheckIfWon", RpcTarget.All);
    }

    public void StartTime()
    {
        photonView.RPC("RPC_StartTime", RpcTarget.All);
    }

    [PunRPC]
    private void RPC_StartTime()
    {
        Time.timeScale = 1;
    }


    [PunRPC]
    private void RPC_CheckIfWon()
    {
        Time.timeScale = 0;

        foreach (var player in playerDead)
        {
            if (player.Key == PhotonNetwork.NetworkingClient.UserId)
            {
                if (player.Value)
                {
                    //Open the Menu when Won
                    EscapeMenu.Instance.OpenEndMenu("WUHU U Did it", PhotonNetwork.NetworkingClient.NickName);
                }
                else
                {

                    //Open the Menu when dead
                    EscapeMenu.Instance.OpenEndMenu("Git Gud", "Not U");
                }
            }
        }
    }

}
