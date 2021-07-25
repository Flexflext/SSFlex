using Photon.Pun;
using TMPro;
using UnityEngine;


/// <summary>
/// Written by Max
/// 
/// This Script saves the player name trough playerprefs
/// </summary>
public class PlayerNameManager : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField mPlayerNameInput;

    [SerializeField]
    private string mDefaultName;

    private void Start()
    {
        
        if (PlayerPrefs.HasKey("playername"))
        {
            mPlayerNameInput.text = PlayerPrefs.GetString("playername");
            PhotonNetwork.NickName = PlayerPrefs.GetString("playername");
        }
        else
        {
            PhotonNetwork.NickName = mDefaultName + " " + Random.Range(0,10000).ToString("0000");
            mPlayerNameInput.text = PhotonNetwork.NickName;
            OnPlayerNameChange();
        }
    }

    public void OnPlayerNameChange()
    {   
        PhotonNetwork.NickName = mPlayerNameInput.text;
        PlayerPrefs.SetString("playername", mPlayerNameInput.text);
    }
}
