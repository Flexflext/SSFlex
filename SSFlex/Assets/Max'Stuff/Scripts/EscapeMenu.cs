using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EscapeMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject mEscapeMenuContent;

    [SerializeField]
    private GameObject mPlayerHud;
    [SerializeField]
    private GameObject mOptionsMenu;

    private void Awake()
    {
        ToggleEscapeMenu();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ToggleEscapeMenu();
    }

    private void ToggleEscapeMenu()
    {
        if (mEscapeMenuContent.activeSelf)
        {
            mPlayerHud.SetActive(true);
            mEscapeMenuContent.SetActive(false);
        }
        else
        {
            mPlayerHud.SetActive(false);
            mEscapeMenuContent.SetActive(true);
        }
    }

    public void ToggleOptionsMenu()
    {
        if (mOptionsMenu.activeSelf)
            mOptionsMenu.SetActive(false);
        else
            mOptionsMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        ToggleEscapeMenu();
    }

    public void LoadMainMenu()
    {
        Destroy(RoomManager.Instance.gameObject);
        PhotonNetwork.LoadLevel(0);
    }
}
