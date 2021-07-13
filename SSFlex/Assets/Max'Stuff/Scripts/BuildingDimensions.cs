using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDimensions : MonoBehaviour
{
    public static BuildingDimensions Instance;

    public enum EBuildingDimensions
    {
        cube,
        normalWall,
        halfWall,
        normalStairs
    }

    public readonly Vector3[] mBuildingDimensions =
    {
        new Vector3(1f, 1f, 1f),
        new Vector3(5.2f, 5.1f, 2.2f),
        new Vector3(3.95f, 1f, 0.4f),
        new Vector3(3.6f, 4.9f,7.8f)
    };

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }
}