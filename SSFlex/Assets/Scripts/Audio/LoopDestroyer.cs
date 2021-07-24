using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopDestroyer : MonoBehaviour
{
    private PlayerController playerController;
    [SerializeField] private bool isStoneStep;


    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>().GetComponent<PlayerController>();
    }
    private void Update()
    {
        AudioDestroyer();
    }

    private void AudioDestroyer()
    {
        if (playerController.isStoneWalking == false && isStoneStep == true)
        {
            Destroy(this.gameObject);
        }
        if (playerController.isGravelWalking == false && isStoneStep == false)
        {
            Destroy(this.gameObject);
        }
        if (playerController.isSneaking)
        {
            Destroy(this.gameObject);
        }
    }
}
