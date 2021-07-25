using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class RoomListItem : MonoBehaviourPunCallbacks
{
    // Code: Haoke
    // Responsible for: Displaying room's name and by clicking it -> Player joins if it's not full.

    [SerializeField] private TMP_Text text;
    [SerializeField] private TextMeshProUGUI mText_PlayerCount;
    [SerializeField] private TextMeshProUGUI mText_RoomFull;
    [SerializeField] private float mRoomFullDisplayDuration;

    public RoomInfo info;
    private int mMaxPlayerCount;

    // Displays room. 
    public void SetUp(RoomInfo _info)
    {
        mMaxPlayerCount = GameManager.Instance.MaxPlayer;

        info = _info;
        text.text = _info.Name;

        mText_PlayerCount.text = "" + info.PlayerCount + "/" + mMaxPlayerCount;
    }


    // Button-Method for joining room.
    // If room full, activate RoomFull UI.
    public void  OnClick()
    {
        if(info.PlayerCount < mMaxPlayerCount)
            Launcher.Instance.JoinRoom(info);
        else
            StartCoroutine(RommFullDisplay());
    }

    private IEnumerator RommFullDisplay()
    {
        mText_RoomFull.gameObject.SetActive(true);
        yield return new WaitForSeconds(mRoomFullDisplayDuration);
        mText_RoomFull.gameObject.SetActive(false);

        StopAllCoroutines();
    }
}
