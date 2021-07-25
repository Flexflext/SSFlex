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
    [SerializeField] private Team team;
    public Team CurrentTeam => team;

    [Header("Materials")]
    [SerializeField] private Material robotRed;
    [SerializeField] private Material robotBlue;
    [SerializeField] private Material robotYellow;
    [SerializeField] private Material robotGreen;

    private List<Material> mAllMats;

    private Material mCurrentMat;

    [Header("Refrences")]
    [SerializeField] private SkinnedMeshRenderer[] robots;

    [SerializeField]
    private PlayerShooting mPlayerShooting;

    private void Awake()
    {
        mAllMats = new List<Material>()
        {
            robotRed,
            robotBlue,
            robotYellow,
            robotGreen
        };

        ChangePlayerGfx(team);
    }


    public void ChangePlayerGfx(Team _team)
    {
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

        ChangeGfx(mCurrentMat);

        if (photonView.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("TeamIndex", _team);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!photonView.IsMine && targetPlayer == photonView.Owner)
        {
            ChangePlayerGfx((Team)changedProps["TeamIndex"]);   
        }
    }

    private void ChangeGfx(Material _currentMat)
    {
        foreach (SkinnedMeshRenderer renderer in robots)
        {
            renderer.material = _currentMat;
        }        
    }
}
