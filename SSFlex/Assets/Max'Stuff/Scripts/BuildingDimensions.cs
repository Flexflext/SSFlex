using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AvailableBuildingDimensions
{
    /// <summary>
    /// Class to save the actual size of the Buildings that ought to be build
    /// </summary>
    public class BuildingDimensions : MonoBehaviour
    {
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
    }
}
