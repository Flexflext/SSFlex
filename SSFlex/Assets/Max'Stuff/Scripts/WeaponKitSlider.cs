using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using System.IO;

public class WeaponKitSlider : MonoBehaviourPunCallbacks
{
    [Header("The names of the different kits")]
    [SerializeField]
    private List<string> mAllKitNames;

    [Header("The default kit the Player has at the start")]
    [SerializeField]
    private EWeaponsAndUtensils mDefaultKit;

    [Header("The Kit name text")]
    [SerializeField]
    private TextMeshProUGUI mText_KitName;

    [SerializeField]
    private AudioClip mButtonClickSound;

    [SerializeField]
    private Player mLobbyManager;

    private PhotonView mPhotonView;

    [SerializeField]
    private GameObject mRiflePlaceholder;
    [SerializeField]
    private GameObject mShotgunPlaceholder;
    [SerializeField]
    private GameObject mSniperPlaceholder;

    private GameObject mCurrentPlaceholder;
    private List<GameObject> mAllPlaceholder;

    private EWeaponsAndUtensils mCurrentKitPos;

    [SerializeField]
    private GameObject mOwner;


    private void Start()
    {
        mPhotonView = GetComponent<PhotonView>();


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
        if (Input.GetKeyDown(KeyCode.L))
            OnClickLeft();

        InputArrowKeyValidation();

        Debug.Log(mPhotonView.IsMine);
    }

    private void InputArrowKeyValidation()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)|| Input.GetKeyDown(KeyCode.A))
            OnClickRight();
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.D))
            OnClickLeft();
    }

    [PunRPC]
    private void DisplayCurrentKit(int _currentKitIdx)
    {


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


        //if (mPhotonView.IsMine)
        //{
        //    Hashtable hash = new Hashtable();
        //    hash.Add("kitKey", (int)mCurrentKitPos);
        //    PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        //}    
    }


    //public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    //{
    //    if (!mPhotonView.IsMine && targetPlayer == mPhotonView.Owner && mOwner != null)
    //    {
    //        DisplayCurrentKit((int)changedProps["weaponKey"]);
    //        Debug.Log("OnPlayerPropertiesUpdate");
    //    }
    //}

    public void OnClickRight()
    {
        if (!mPhotonView.IsMine || mOwner == null)
            return;

        Debug.Log("Click Righ");

        mCurrentKitPos++;
        if ((int)mCurrentKitPos >= (int)EWeaponsAndUtensils.Sniper + 1)
            mCurrentKitPos = 0;

        //DisplayCurrentKit((int)mCurrentKitPos);

        mPhotonView.RPC("DisplayCurrentKit", RpcTarget.AllBufferedViaServer, mCurrentKitPos);

        GameManager.Instance.SetStartWeapon(mCurrentKitPos);

        GameManager.Instance.GetComponent<PhotonView>().RPC("SetStartWeapon", RpcTarget.OthersBuffered, mCurrentKitPos);

        //GameManager.Instance.SetStartWeapon(mCurrentKitPos);
    }

    public void OnClickLeft()
    {
        if (!mPhotonView.IsMine || mOwner == null)
            return;

        Debug.Log("Click Left");

        mCurrentKitPos--;

        if (mCurrentKitPos < 0)
            mCurrentKitPos = EWeaponsAndUtensils.Sniper;


        //DisplayCurrentKit((int)mCurrentKitPos);

        mPhotonView.RPC("DisplayCurrentKit", RpcTarget.AllBufferedViaServer, mCurrentKitPos);

        GameManager.Instance.SetStartWeapon(mCurrentKitPos);

        GameManager.Instance.GetComponent<PhotonView>().RPC("SetStartWeapon", RpcTarget.OthersBuffered, mCurrentKitPos);

        //GameManager.Instance.SetStartWeapon(mCurrentKitPos);
    }

    public void OnSelect()
    {
        mPhotonView.TransferOwnership(PhotonNetwork.LocalPlayer);
        Debug.Log("DDD");

        mOwner = this.gameObject;
    }
}
