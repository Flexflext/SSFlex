using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;


/// <summary>
/// Written by Max
/// 
/// This script is responsible for choosing the own start weapon and displaying it in the lobby
/// </summary>
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
    private Player mLobbyManager;

    private PhotonView mPhotonView;

    // The placeholder on display in the lobby
    [Header("Weapon Placeholder")]
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

    [SerializeField]
    private Team mCurrentTeam;

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

    /// <summary>
    /// Manual kit switch with the arrow keys or A and D
    /// </summary>
    private void InputArrowKeyValidation()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)|| Input.GetKeyDown(KeyCode.A))
            OnClickRight();
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.D))
            OnClickLeft();
    }

    /// <summary>
    /// RPC call to Display the current kit to all player in the Lobby
    /// 
    /// 1. Iterates trough all placeholder and activates the one with the right inderxer
    /// 2. Creates a Hashtable with the given Information (indexer) and sets a Custom Property
    /// </summary>
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

        if (mPhotonView.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("startWeaponKey", (int)mCurrentKitPos);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    /// <summary>
    /// Updates the custom property if the player changed his weapon
    /// </summary>
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!mPhotonView.IsMine && targetPlayer == mPhotonView.Owner && mOwner != null)
        {
            DisplayCurrentKit((int)changedProps["startWeaponKey"]);
        }
    }

    /// <summary>
    /// Methods to toggle trough the Weapon placeholder
    /// 
    /// Calls the DisplayCurrentKit Method with the current indexer
    /// </summary>
    public void OnClickRight()
    {
        if (!mPhotonView.IsMine || mOwner == null)
            return;

        mCurrentKitPos++;
        if ((int)mCurrentKitPos >= (int)EWeaponsAndUtensils.Sniper + 1)
            mCurrentKitPos = 0;

        mPhotonView.RPC("DisplayCurrentKit", RpcTarget.AllBufferedViaServer, mCurrentKitPos);

        GameManager.Instance.SetStartWeapon(mCurrentKitPos);
    }
    public void OnClickLeft()
    {
        if (!mPhotonView.IsMine || mOwner == null)
            return;

        Debug.Log("Click Left");

        mCurrentKitPos--;

        if (mCurrentKitPos < 0)
            mCurrentKitPos = EWeaponsAndUtensils.Sniper;

        mPhotonView.RPC("DisplayCurrentKit", RpcTarget.AllBufferedViaServer, mCurrentKitPos);

        GameManager.Instance.SetStartWeapon(mCurrentKitPos);
    }

    /// <summary>
    /// Transfers the Ownership of the Chosen team to the current Local Player
    /// </summary>
    public void OnSelect()
    {
        mPhotonView.TransferOwnership(PhotonNetwork.LocalPlayer);

        mOwner = this.gameObject;
    }
}
