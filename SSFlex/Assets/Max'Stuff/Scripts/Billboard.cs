using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mCurrentCam;

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
