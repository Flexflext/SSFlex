using UnityEngine;

/// <summary>
/// Writte by Max
/// 
/// This script is used to rotate the name of the Player, which is displayed above him, in the direction of the viewer
/// </summary>
public class Billboard : MonoBehaviour
{
    private Camera mCurrentCam;

    /// <summary>
    /// 1. Null checks the cam
    /// 2. Lets it Look at the viewer
    /// 3. Rotates it by 180° since it will be mirrored
    /// </summary>
    private void Update()
    {
        if (mCurrentCam == null)
            mCurrentCam = FindObjectOfType<Camera>();

        if (mCurrentCam == null)
            return;

        transform.LookAt(mCurrentCam.transform);
        transform.Rotate(Vector3.up * 180);
    }
}
