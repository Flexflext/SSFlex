using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // Photon's own namespace.
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

    [SerializeField] private TMP_InputField roomNameInputField;
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
    }

    // When client is successfully connected to the master server this will be called.
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to MasterServer");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("Main");
        Debug.Log("Joined Lobby");
        PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000");
    }

    public void CreateRoom()
    {
        // Prevents client from creating a room without typing anything in InputField.
        if (string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }

        PhotonNetwork.CreateRoom(roomNameInputField.text);

        // Prevents client to click on other buttons while room is being created.
        MenuManager.Instance.OpenMenu("Loading");
    }

    // Callback for successfully joining the room.
    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("Room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;

        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }

        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    // Callback when creating the room has failed. Provides client with an error message.
    public override void OnCreateRoomFailed(short _returnCode, string _message)
    {
        errorText.text = "Room Creation Failed: " + _message;
        MenuManager.Instance.OpenMenu("Error");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("Loading");
    }

    public void JoinRoom(RoomInfo _info)
    {
        PhotonNetwork.JoinRoom(_info.Name);
        MenuManager.Instance.OpenMenu("Loading");

        
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("Main");
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
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(_roomList[i]);
        }
       
    }

    public override void OnPlayerEnteredRoom(Player _newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(_newPlayer);

    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }
}
