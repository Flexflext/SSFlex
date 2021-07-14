using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponKitSlider : MonoBehaviour
{
    public enum EKits
    {
        rifle,
        sniper,
        shotgun,
        count
    }

    public int CurrentKitIdx => mCurrentKitPos;

    [Header("The names of the different kits")]
    [SerializeField]
    private List<string> mAllKitNames;

    [Header("The default kit the Player has at the start")]
    [SerializeField]
    private EKits mDefaultKit;

    [Header("The Kit name text")]
    [SerializeField]
    private TextMeshProUGUI mText_KitName;

    [SerializeField]
    private AudioClip mButtonClickSound;

    [SerializeField]
    private GameObject mRiflePlaceholder;
    [SerializeField]
    private GameObject mSniperPlaceholder;
    [SerializeField]
    private GameObject mShotgunPlaceholder;

    private GameObject mCurrentPlaceholder;
    private List<GameObject> mAllPlaceholder;

    private int mCurrentKitPos;

    private void Start()
    {
        mAllPlaceholder = new List<GameObject>()
        {
            mRiflePlaceholder,
            mSniperPlaceholder,
            mShotgunPlaceholder
        };

        DisplayCurrentKit(mAllPlaceholder[mCurrentKitPos]);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            OnClickLeft();
    }

    private void DisplayCurrentKit(GameObject _currentKit)
    {
        foreach (GameObject placeholder in mAllPlaceholder)
        {
            if (placeholder == _currentKit)
            {
                placeholder.SetActive(true);
                mText_KitName.text = mAllKitNames[mCurrentKitPos];
            }
            else
                placeholder.SetActive(false);
        }
    }

    public void OnClickRight()
    {
        mCurrentKitPos++;
        if (mCurrentKitPos >= (int)EKits.count)
            mCurrentKitPos = 0;

        DisplayCurrentKit(mAllPlaceholder[mCurrentKitPos]);

    }

    public void OnClickLeft()
    {   
        mCurrentKitPos--;

        if (mCurrentKitPos < 0)
            mCurrentKitPos = (int)EKits.count - 1;

        DisplayCurrentKit(mAllPlaceholder[mCurrentKitPos]);
    }
}
