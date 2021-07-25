using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;

public enum Team
{
    Red,
    Blue,
    Yellow,
    Green,
}

public class PlayerGFXChange : MonoBehaviourPunCallbacks
{
    // Script von Felix
    // Purpose: Leben vom Spieler + VFX Play

    [SerializeField] private Team team;
    public Team CurrentTeam => team;

    [Header("Materials")]
    [SerializeField] private Material robotRed;
    [SerializeField] private Material robotBlue;
    [SerializeField] private Material robotYellow;
    [SerializeField] private Material robotGreen;

    private Material mCurrentMat;

    [Header("Refrences")]
    [SerializeField] private SkinnedMeshRenderer[] robots;

    [SerializeField]
    private PlayerShooting mPlayerShooting;

    private void Awake()
    {
        // Change the Player Gfx to the given Team Color (Material)
        ChangePlayerGfx(team);
    }


    /// <summary>
    /// Change the Player Gfx to the given Team Color (Material)
    /// </summary>
    /// <param name="_team"></param>
    public void ChangePlayerGfx(Team _team)
    {
        // Set CurrentMat
        switch (_team)
        {
            case Team.Red:
                mCurrentMat = robotRed;
                break;
            case Team.Blue:
                mCurrentMat = robotBlue;
                break;
            case Team.Yellow:
                mCurrentMat = robotYellow;
                break;
            case Team.Green:
                mCurrentMat = robotGreen;
                break;
            default:
                break;
        }

        // Chnage the Gfx to the Current Material
        ChangeGfx(mCurrentMat);

        // Check if photon view is mine to set Custom Proertie
        if (photonView.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("TeamIndex", _team);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    /// <summary>
    /// Event Function to CHnage the Custom Propertie on all Instances of the Game
    /// </summary>
    /// <param name="targetPlayer"></param>
    /// <param name="changedProps"></param>
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!photonView.IsMine && targetPlayer == photonView.Owner)
        {
            ChangePlayerGfx((Team)changedProps["TeamIndex"]);   
        }
    }

    /// <summary>
    /// Change all Player Materials to the currentMat
    /// </summary>
    /// <param name="_currentMat"></param>
    private void ChangeGfx(Material _currentMat)
    {
        foreach (SkinnedMeshRenderer renderer in robots)
        {
            renderer.material = _currentMat;
        }        
    }
}
