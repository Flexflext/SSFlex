using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NamePositioning : MonoBehaviour
{
    [SerializeField]
    private RectTransform mNameText;

    [SerializeField]
    private float mTextHeight;

    [SerializeField]
    private GameObject mAnkerPoint;

    [SerializeField]
    private Camera mMainCam;

    private void Update()
    {
        Vector3 ankerPos = mMainCam.WorldToScreenPoint(mAnkerPoint.transform.position);

        mNameText.transform.position = new Vector3(ankerPos.x, ankerPos.y + mTextHeight, ankerPos.z);
    }
}
