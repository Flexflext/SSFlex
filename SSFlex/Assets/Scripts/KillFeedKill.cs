using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KillFeedKill : MonoBehaviour
{
    [SerializeField] private Image weaponImg;
    [SerializeField] private TMP_Text killerText;
    [SerializeField] private TMP_Text victimText;

    public void ChangeFeedContent(Sprite _weaponimg, string _killername, string _victimname)
    {
        weaponImg.sprite = _weaponimg;
        killerText.text = _killername;
        victimText.text = _victimname;
    }
}
