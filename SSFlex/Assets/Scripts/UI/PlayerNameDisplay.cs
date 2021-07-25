using Photon.Pun;
using TMPro;
using UnityEngine;

/// <summary>
/// Written by Max
/// 
/// This script is being used to Display the name of the Player while ingame above him
/// </summary>
public class PlayerNameDisplay : MonoBehaviour
{
    [SerializeField]
    private PhotonView mPlayerPhotonView;

    [SerializeField]
    private TextMeshProUGUI mNameText;

    private void Start()
    {
        if (mPlayerPhotonView.IsMine)
            gameObject.SetActive(false);

        mNameText.text = mPlayerPhotonView.Owner.NickName;
    }
}
