using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DeadRobot : MonoBehaviour
{

    [SerializeField] private Material redRobot;
    [SerializeField] private Material blueRobot;
    [SerializeField] private Material yellowRobot;
    [SerializeField] private Material greenRobot;

    private SkinnedMeshRenderer meshRend;

    private void Start()
    {
        meshRend = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    public void ChangeApperance(Team _team)
    {
        switch (_team)
        {
            case Team.Red:
                meshRend.material = redRobot;
                break;
            case Team.Blue:
                meshRend.material = blueRobot;
                break;
            case Team.Yellow:
                meshRend.material = yellowRobot;
                break;
            case Team.Green:
                meshRend.material = greenRobot;
                break;
            default:
                break;
        }
    }
}
