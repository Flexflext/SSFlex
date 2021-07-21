using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LevelManager : MonoBehaviourPunCallbacks
{
    public static LevelManager Instance;

    private Dictionary<int, bool> playerDead = new Dictionary<int, bool>();

    private void Awake()
    {
        Instance = this;
    }


    public void UpdateDictionary(int _id, bool _lifebool)
    {
        photonView.RPC("RPC_UpdateDictionary", RpcTarget.All, _id, _lifebool);
    }

    [PunRPC]
    private void RPC_UpdateDictionary(int _id, bool _lifebool)
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

    public int CheckIfWon()
    {
        foreach (var player in playerDead)
        {
            if (player.Value)
            {
                return player.Key;
            }
        }

        return 0;
    }

}
