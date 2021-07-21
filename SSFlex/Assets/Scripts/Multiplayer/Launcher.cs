using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; 
using TMPro;
using Photon.Realtime;
using System.Linq;


// "MonoBehaviourPunCallbacks" gives access to callbacks for joining lobbies, room creations, errors... to connect to the Photonservers
public class Launcher : MonoBehaviourPunCallbacks 
{
    // Code: Haoke
    // This script is responsible for:
    // + Connecting to the PhotonServices.
    // 

    public static Launcher Instance;

    [SerializeField] private TextMeshProUGUI roomName;
    [SerializeField] private TMP_Text errorText;
    [SerializeField] private TMP_Text roomNameText;
    [SerializeField] private Transform roomListContent;
    [SerializeField] private Transform playerListContent;
    [SerializeField] private GameObject roomListItemPrefab;
    [SerializeField] private GameObject playerListItemPrefab;
    [SerializeField] private GameObject startGameButton;


    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        // Connects to the Photon's Master server using our own "PhotonServerSettings" File.
        PhotonNetwork.ConnectUsingSettings();
        AudioManager.Instance.Play("MainTheme");
    }

    public void SetRoomName(string _roomName)
    {
        roomName.text = _roomName;
    }

    // When client is successfully connected to the master server this will be called.
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // When client successfully joined the lobby the client is visualized by "Player + a random number from 0000 to 0999".
    public override void OnJoinedLobby()
    {
        //MenuManager.Instance.AdminMainMenu();
    }

    public void CreateRoom()
    {
        // Prevents client from creating a room without typing anything in InputField.
        if (string.IsNullOrEmpty(roomName.text))
        {
            return;
        }

        RoomOptions roomOpt = new RoomOptions { MaxPlayers = 4 };
        PhotonNetwork.CreateRoom(roomName.text, roomOpt);

        // Prevents client to click on other buttons while room is being created.
        MenuManager.Instance.AdminLoadingMenu();
    }

    // Callback for successfully joining the room.
    public override void OnJoinedRoom()
    {
        MenuManager.Instance.AdminRoomMenu();
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;


        // Deletes all former playerLists so if master client leaves a former room and
        // creates a new one, all former clients should be cleared.
        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }

        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    // If the master of the room has left but there is still one client in this client will become the master and gets the startgame button.
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    // Callback when creating the room has failed. Provides client with an error message.
    public override void OnCreateRoomFailed(short _returnCode, string _message)
    {
        errorText.text = "Room Creation Failed: " + _message;
        MenuManager.Instance.AdminErrorMenu();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("HAHA U LOOSE " + message);
    }


    public void JoinRoom(RoomInfo _info)
    {
        Debug.Log("Join Room");
        PhotonNetwork.JoinRoom(_info.Name);
        MenuManager.Instance.AdminLoadingMenu();
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.AdminLoadingMenu();
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.AdminMainMenu();
    }

    public override void OnRoomListUpdate(List<RoomInfo> _roomList)
    {
        foreach (Transform transform in roomListContent)
        {
            Destroy(transform.gameObject);
        }

        // Instantiates roomListItemPrefabs in the roomLstContainer and calls the setup Method with the roomInfo.
        for (int i = 0; i < _roomList.Count; i++)
        {
            // Prevents former rooms to still exist and only continues if the room which the master client has left
            // is actually removed.
            if (_roomList[i].RemovedFromList || !_roomList[i].IsOpen)
            {
                continue;
            }

            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(_roomList[i]);
        }
       
    }

    public override void OnPlayerEnteredRoom(Player _newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(_newPlayer);
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(2);
    }

    public void StartLobby()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;

        PhotonNetwork.LoadLevel(1);

    }
}
