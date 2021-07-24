using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class KillFeedKill : MonoBehaviourPunCallbacks
{
    [SerializeField] private Image weaponImg;
    [SerializeField] private TMP_Text killerText;
    [SerializeField] private TMP_Text victimText;
    [SerializeField] private Sprite killImg;

    public void ChangeFeedContent(string _killername, string _victimname)
    {
        weaponImg.sprite = killImg;
        killerText.text = _killername;
        victimText.text = _victimname;      
    }
}
