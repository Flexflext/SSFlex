using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class RoomListItem : MonoBehaviourPunCallbacks
{
    // Responsible for displaying room's name and by clicking it -> joining


    [SerializeField] private TMP_Text text;

    [SerializeField]
    private float mRoomFullDisplayDuration;
    private int mMaxPlayerCount;

    [SerializeField]
    private TextMeshProUGUI mText_PlayerCount;
    [SerializeField]
    private TextMeshProUGUI mText_RoomFull;

    public RoomInfo info;

    public void SetUp(RoomInfo _info)
    {
        mMaxPlayerCount = GameManager.Instance.MaxPlayer;

        info = _info;
        text.text = _info.Name;

        mText_PlayerCount.text = "" + info.PlayerCount + "/" + mMaxPlayerCount;
    }

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
