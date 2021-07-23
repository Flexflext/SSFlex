﻿using System.Collections;
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
        if (!playerController.isOnStone && isStoneStep == true)
        {
            Destroy(this.gameObject);
        }
        if (!playerController.isOnGravel && isStoneStep == false)
        {
            Destroy(this.gameObject);
        }
        if (!playerController.isMoving || playerController.isSneaking)
        {
            Destroy(this.gameObject);
        }
    }
}
