using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    Red,
    Blue,
    Yellow,
    Green,
}

public class PlayerGFXChange : MonoBehaviour
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
        ChangePlayerGfx();
    }


    private void ChangePlayerGfx()
    {
        switch (team)
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
    }

    private void ChangeGfx(Material _mat)
    {
        foreach (SkinnedMeshRenderer renderer in robots)
        {
            renderer.material = _mat;
        }
    }
}
