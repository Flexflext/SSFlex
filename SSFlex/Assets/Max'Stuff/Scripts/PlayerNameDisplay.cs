using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

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
