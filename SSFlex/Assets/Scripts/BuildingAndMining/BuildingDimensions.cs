using UnityEngine;

namespace AvailableBuildingDimensions
{
    /// <summary>
    /// Written by Max
    /// 
    /// Class to save the actual size of the Buildings that ought to be build
    /// </summary>
    public class BuildingDimensions : MonoBehaviour
    {
        public enum EBuildingDimensions
        {
            cube,
            normalWall,
            normalGate,
            halfWall,
            normalStairs
        }

        public readonly Vector3[] mBuildingDimensions =
        {
        new Vector3(1f, 1f, 1f),
        new Vector3(5.25f, 5.1f, 2.2f),
        new Vector3(5f, 4.9f, 2.2f),
        new Vector3(3.95f, 1f, 0.4f),
        new Vector3(3.6f, 4.9f,7.8f)
        };

        public readonly float[] mHeightAdjDimensions =
        {
            0,
            0,
            2.6f,
            1.3f,
            2.15f
        };
    }
}
