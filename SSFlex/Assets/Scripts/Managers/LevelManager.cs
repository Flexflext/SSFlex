using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LevelManager : MonoBehaviourPunCallbacks
{
    // Script von Felix
    // Purpose: Manager for the Game

    public static LevelManager Instance;

    private Dictionary<string, bool> playerDead;

    private void Awake()
    {

        playerDead = new Dictionary<string, bool>();
        Instance = this;
    }

    // Add to Dictionary if is Alive
    public void AddDictionary(string _id, bool _lifebool)
    {
        photonView.RPC("RPC_AddDictionary", RpcTarget.All, _id, _lifebool);
    }

    /// <summary>
    /// Update Dictionary witch RPC Call
    /// </summary>
    /// <param name="_id"></param>
    /// <param name="_lifebool"></param>
    public void UpdateDictionary(string _id, bool _lifebool)
    {
        photonView.RPC("RPC_UpdateDictionary", RpcTarget.All, _id, _lifebool);
    }

    /// <summary>
    /// Update the Dictionary wich new value
    /// </summary>
    /// <param name="_id"></param>
    /// <param name="_lifebool"></param>
    [PunRPC]
    private void RPC_UpdateDictionary(string _id, bool _lifebool)
    {
        if (playerDead.ContainsKey(_id))
        {
            playerDead[_id] = _lifebool;
        }
    }

    /// <summary>
    /// Add to Dictionary RPC Call witch id and bool
    /// </summary>
    /// <param name="_id"></param>
    /// <param name="_lifebool"></param>
    [PunRPC]
    private void RPC_AddDictionary(string _id, bool _lifebool)
    {
        playerDead.Add(_id, _lifebool);
    }

    /// <summary>
    /// Check if the Only one left alive in the Dictionary
    /// </summary>
    /// <returns></returns>
    public bool CheckIfTheOnlyOneAlive()
    {
        int peopleAlive = 0;

        // Chek how many alive 
        foreach (var player in playerDead)
        {
            if (player.Value)
            {
                peopleAlive++;
            }
        }

        // Retuirn true if the only one alive else return false
        if (peopleAlive < 2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Check if Won witch RPC Call
    /// </summary>
    public void CheckIfWon()
    {
        photonView.RPC("RPC_CheckIfWon", RpcTarget.All);
    }

    /// <summary>
    /// Check if Won RPC Call and opens the Menu accordinly
    /// </summary>
    [PunRPC]
    private void RPC_CheckIfWon()
    {
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
