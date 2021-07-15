using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.IO;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;

    private Team myTeam;

    Scene mCurrentScene;

    private void Awake()
    {
        if (!photonView.IsMine)
            return;

        mCurrentScene = SceneManager.GetActiveScene();

        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
    }


    // Subscribing and unsubscribing to the callback "sceneLoaded".
    // -> Everytime when loading a new scene the method "OnSceneLoaded" is called.
    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded (Scene _scene, LoadSceneMode _loadSceneMode)
    {

        if (_scene.buildIndex == 2) // MainGameScene
        {
            // Instantiates the PlayerManager
            GameObject playerManager = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
            playerManager.GetComponent<PlayerManager>().ChangeTeam(myTeam);
        }
    }

    public void ChangeTeam(Team _team)
    {
        myTeam = _team;
    }
}
