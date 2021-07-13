using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerTeamManager : MonoBehaviourPunCallbacks
{
    public static PlayerTeamManager Instance;

    public List<PlayerController> playerList = new List<PlayerController>();

    private void Awake()
    {
        Instance = this;
    }

    public int Subscribe(PlayerController _controller)
    {
        photonView.RPC("RPC_Subscribe", RpcTarget.AllViaServer, _controller);
        return playerList.Count;
    }

    [PunRPC]
    private void RPC_Subscribe(PlayerController _controller)
    {

        Debug.Log("HUHU");
        if (!playerList.Contains(_controller))
        {
            playerList.Add(_controller);
        }
    }

    
    public int UnSubscribe(PlayerController _controller)
    {
        int pos = playerList.Count;

        photonView.RPC("RPC_UnSubscribe", RpcTarget.All, _controller);

        return pos;
    }

    [PunRPC]
    private void RPC_UnSubscribe(PlayerController _controller)
    {
        if (playerList.Contains(_controller))
        {
            playerList.Remove(_controller);
        }
    }
}