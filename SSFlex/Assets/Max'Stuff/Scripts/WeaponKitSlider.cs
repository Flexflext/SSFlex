using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;

public class WeaponKitSlider : MonoBehaviourPunCallbacks
{
    public PrimaryWeapon CurrentKitIdx => mCurrentKitPos;

    [Header("The names of the different kits")]
    [SerializeField]
    private List<string> mAllKitNames;

    [Header("The default kit the Player has at the start")]
    [SerializeField]
    private PrimaryWeapon mDefaultKit;

    [Header("The Kit name text")]
    [SerializeField]
    private TextMeshProUGUI mText_KitName;


    [SerializeField]
    private PhotonView mPhotonView;

    [SerializeField]
    private AudioClip mButtonClickSound;

    [SerializeField]
    private GameObject mRiflePlaceholder;
    [SerializeField]
    private GameObject mShotgunPlaceholder;
    [SerializeField]
    private GameObject mSniperPlaceholder;

    private GameObject mCurrentPlaceholder;
    private List<GameObject> mAllPlaceholder;

    private PrimaryWeapon mCurrentKitPos;



    private void Start()
    {
        mAllPlaceholder = new List<GameObject>()
        {
            mRiflePlaceholder,
            mShotgunPlaceholder,
            mSniperPlaceholder
        };

        DisplayCurrentKit((int)mCurrentKitPos);
    }

    private void Update()
    {
        if (!mPhotonView.IsMine)
            return;

        if (Input.GetKeyDown(KeyCode.L))
            OnClickLeft();
    }

    private void DisplayCurrentKit(int _currentKitIdx)
    {
        if (!mPhotonView.IsMine)
            return;


        for (int i = 0; i < mAllPlaceholder.Count; i++)
        {
            if (i == _currentKitIdx)
            {
                mAllPlaceholder[i].SetActive(true);
                mText_KitName.text = mAllKitNames[(int)mCurrentKitPos];
            }
            else
                mAllPlaceholder[i].SetActive(false);

        }

        Hashtable hash = new Hashtable();
        hash.Add("weaponKey", (int)mCurrentKitPos);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!mPhotonView.IsMine && targetPlayer == mPhotonView.Owner)
        {
            DisplayCurrentKit((int)changedProps["weaponKey"]);
        }
    }

    public void OnClickRight()
    {
        if (!mPhotonView.IsMine)
            return;

        mCurrentKitPos++;
        if ((int)mCurrentKitPos >= (int)PrimaryWeapon.Sniper + 1)
            mCurrentKitPos = 0;

        DisplayCurrentKit((int)mCurrentKitPos);

    }

    public void OnClickLeft()
    {
        if (!mPhotonView.IsMine)
            return;

        mCurrentKitPos--;

        if (mCurrentKitPos < 0)
            mCurrentKitPos = PrimaryWeapon.Sniper;

        DisplayCurrentKit((int)mCurrentKitPos);
    }
}
