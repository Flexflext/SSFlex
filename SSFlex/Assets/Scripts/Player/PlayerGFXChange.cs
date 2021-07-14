﻿using System.Collections;
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

    [Header("Materials")]
    [SerializeField] private Material robotRed;
    [SerializeField] private Material robotBlue;
    [SerializeField] private Material robotYellow;
    [SerializeField] private Material robotGreen;

    [Header("Refrences")]
    [SerializeField] private SkinnedMeshRenderer[] robots;

    private void Awake()
    {
        ChangePlayerGfx(team);
    }


    public void ChangePlayerGfx(Team _team)
    {
        switch (_team)
        {
            case Team.Red:
                ChangeGfx(robotRed);
                break;
            case Team.Blue:
                ChangeGfx(robotBlue);
                break;
            case Team.Yellow:
                ChangeGfx(robotYellow);
                break;
            case Team.Green:
                ChangeGfx(robotGreen);
                break;
            default:
                break;
        }

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


    private void ChangeGfx(Material _mat)
    {
        foreach (SkinnedMeshRenderer renderer in robots)
        {
            renderer.material = _mat;
        }
    }
}
