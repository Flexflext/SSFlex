using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public Team CurrentTeam => team;

    [Header("Select Name Text")]
    [SerializeField] private TMP_Text playerRed;
    [SerializeField] private TMP_Text playerBlue;
    [SerializeField] private TMP_Text playerYellow;
    [SerializeField] private TMP_Text playerGreen;

    [Header("Button GameObejcts")]
    [SerializeField] private GameObject selectRedButton;
    [SerializeField] private GameObject selectBlueButton;
    [SerializeField] private GameObject selectYellowButton;
    [SerializeField] private GameObject selectGreenButton;

    [SerializeField] private GameObject startGameButton;

    [SerializeField] private Dictionary<int, bool> playerIdTeam = new Dictionary<int, bool>();

    [Space]
    [SerializeField]
    private GameObject mWeaponSlider_Red;
    [SerializeField]
    private GameObject mWeaponSlider_Blue;
    [SerializeField]
    private GameObject mWeaponSlider_Yellow;
    [SerializeField]
    private GameObject mWeaponSlider_Green;
    [Space]
    [SerializeField]
    private WeaponKitSlider mKitSliderPlayer_Red;
    [SerializeField]
    private WeaponKitSlider mKitSliderPlayer_Blue;
    [SerializeField]
    private WeaponKitSlider mKitSliderPlayer_Yellow;
    [SerializeField]
    private WeaponKitSlider mKitSliderPlayer_Green;

    private Team team;
    private bool canStartGame;
    private bool mTeamSet;

    private void Awake()
    {
        photonView.RPC("AddToDictionary", RpcTarget.AllBufferedViaServer, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    private void Start()
    {
        startGameButton.SetActive(false);
    }

    private void Update()
    { 
        if (canStartGame)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (!startGameButton.activeSelf)
                {
                    startGameButton.SetActive(true);
                }

                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space))
                    StartGame();
            }
        }
        else
        {
            if (startGameButton.activeSelf)
            {
                startGameButton.SetActive(false);
            }  
        }

        if(!mTeamSet)
            InputNumberKeyValidation();

        //if (Input.GetKeyDown(KeyCode.Escape))
        //    ReturnMenu();   
    }

    private void InputNumberKeyValidation()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && selectRedButton.activeSelf)
        {
            mWeaponSlider_Red.SetActive(true);
            mKitSliderPlayer_Red.OnSelect();
            ChangeTeam(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && selectBlueButton.activeSelf)
        {
            mWeaponSlider_Blue.SetActive(true);
            mKitSliderPlayer_Blue.OnSelect();
            ChangeTeam(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && selectYellowButton.activeSelf)
        {
            mWeaponSlider_Yellow.SetActive(true);
            mKitSliderPlayer_Yellow.OnSelect();
            ChangeTeam(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) && selectGreenButton.activeSelf)
        {
            mWeaponSlider_Green.SetActive(true);
            mKitSliderPlayer_Green.OnSelect();
            ChangeTeam(4);
        }
    }

    private void CheckIfCanChange()
    {
        foreach (var player in playerIdTeam)
        {
            if (!playerIdTeam[player.Key])
            {
                canStartGame = false;
                return;
            }
        }
        canStartGame = true;
    }


    public void ChangeTeam(int _teamnum)
    {
        switch (_teamnum)
        {
            case 1:
                team = Team.Red;
                selectBlueButton.SetActive(false);
                selectYellowButton.SetActive(false);
                selectGreenButton.SetActive(false);
                break;
            case 2:
                team = Team.Blue;
                selectRedButton.SetActive(false);
                selectYellowButton.SetActive(false);
                selectGreenButton.SetActive(false);
                break;
            case 3:
                team = Team.Yellow;
                selectBlueButton.SetActive(false);
                selectRedButton.SetActive(false);
                selectGreenButton.SetActive(false);
                break;
            case 4:
                team = Team.Green;
                selectBlueButton.SetActive(false);
                selectRedButton.SetActive(false);
                selectYellowButton.SetActive(false);
                break;

            default:
                break;
        }

        mTeamSet = true;
        RoomManager.Instance.ChangeTeam(team);
        photonView.RPC("DisplayTeam", RpcTarget.AllBufferedViaServer, PhotonNetwork.LocalPlayer.NickName, _teamnum, PhotonNetwork.LocalPlayer.ActorNumber);
    }


    public void ReturnMenu()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.AdminLoadingMenu();

    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(2);
    }

    // If the master of the room has left but there is still one client in this client will become the master and gets the startgame button.
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.LoadMainMenu();
    }


    #region RPC Calls

    [PunRPC]
    private void DisplayTeam(string _name, int _team, int _id)
    {

        switch (_team)
        {
            case 1:
                playerRed.text = _name;
                selectRedButton.SetActive(false);

                break;
            case 2:
                playerBlue.text = _name;
                selectBlueButton.SetActive(false);
                break;
            case 3:
                playerYellow.text = _name;
                selectYellowButton.SetActive(false);
                break;
            case 4:
                playerGreen.text = _name;
                selectGreenButton.SetActive(false);
                break;
            default:
                break;
        }

        playerIdTeam[_id] = true;
        CheckIfCanChange();
    }

    [PunRPC]
    private void AddToDictionary(int _id)
    {
        playerIdTeam.Add(_id, false);
        canStartGame = false;
        startGameButton.SetActive(false);
    }

    #endregion
}
