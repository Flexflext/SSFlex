using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadRobot : MonoBehaviourPunCallbacks
{

    [SerializeField] private Material redRobot;
    [SerializeField] private Material blueRobot;
    [SerializeField] private Material yellowRobot;
    [SerializeField] private Material greenRobot;

    [SerializeField] private SkinnedMeshRenderer meshRend;


    public void ChangeApperance(Team _team)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        int teamNum = 0;

        switch (_team)
        {
            case Team.Red:
                teamNum = 1;
                break;
            case Team.Blue:
                teamNum = 2;
                break;
            case Team.Yellow:
                teamNum = 3;
                break;
            case Team.Green:
                teamNum = 4;
                break;
            default:
                break;
        }

        photonView.RPC("RPC_ChnageApperance", RpcTarget.All, teamNum);

    }

    [PunRPC]
    private void RPC_ChnageApperance(int _teamnum)
    {
        switch (_teamnum)
        {
            case 1:
                meshRend.material = redRobot;
                break;
            case 2:
                meshRend.material = blueRobot;
                break;
            case 3:
                meshRend.material = yellowRobot;
                break;
            case 4:
                meshRend.material = greenRobot;
                break;
            default:
                break;
        }
    }
}
