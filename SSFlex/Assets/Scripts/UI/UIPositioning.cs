using UnityEngine;

/// <summary>
/// Written by Max
/// 
/// This script is used to position the Player name above his picked Team
/// </summary>
public class UIPositioning : MonoBehaviour
{
    [SerializeField]
    private RectTransform mNameText;

    [SerializeField]
    private RectTransform mArrowLeft;
    [SerializeField]
    private RectTransform mArrowRight;

    [SerializeField]
    private float mTextHeight;

    [SerializeField]
    private float mArrowAlignment;
    [SerializeField]
    private float mArrowHeight;

    [SerializeField]
    private GameObject mAnkerPoint;

    [SerializeField]
    private Camera mMainCam;

    /// <summary>
    /// Translates the World coordinates of the anke point into Screen coordinates and lets the mimik it position
    /// </summary>
    private void Update()
    {
        Vector3 ankerPos = mMainCam.WorldToScreenPoint(mAnkerPoint.transform.position);

        mNameText.transform.position = new Vector3(ankerPos.x, ankerPos.y + mTextHeight, ankerPos.z);

        mArrowLeft.transform.position = new Vector3(ankerPos.x - mArrowAlignment, ankerPos.y - mArrowHeight, ankerPos.z);
        mArrowRight.transform.position = new Vector3(ankerPos.x + mArrowAlignment, ankerPos.y - mArrowHeight, ankerPos.z);

    }
}
