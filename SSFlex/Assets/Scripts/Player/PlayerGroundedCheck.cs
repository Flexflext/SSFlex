﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedCheck : MonoBehaviour
{
    private PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter(Collider _other)
    {
        // With itself -> return.
        if (_other.gameObject == playerController.gameObject)
        {
            return;
        }


        // Trigger with GroundStone
        if (_other.gameObject.layer == LayerMask.NameToLayer("GroundStone"))
        {
            Debug.Log("Entered Trigger GroundStone");
            playerController.SetIsStoneGroundedState(true);
        }

        // Trigger with GroundGravel
        if (_other.gameObject.layer == LayerMask.NameToLayer("GroundGravel"))
        {
            playerController.SetIsGravelGroundedState(true);
        }

        // Just Trigger with Ground
        playerController.SetIsGroundedState(true);
    }

    private void OnTriggerExit(Collider _other)
    {
        if (_other.gameObject == playerController.gameObject)
        {
            return;
        }

        if (_other.gameObject.layer == LayerMask.NameToLayer("GroundStone"))
        {
            playerController.SetIsStoneGroundedState(false);
        }

        if (_other.gameObject.layer == LayerMask.NameToLayer("GroundGravel"))
        {
            playerController.SetIsGravelGroundedState(false);
        }

        playerController.SetIsGroundedState(false);
    }

    private void OnTriggerStay(Collider _other)
    {
        if (_other.gameObject == playerController.gameObject)
        {
            return;
        }

        if (_other.gameObject.layer == LayerMask.NameToLayer("GroundStone"))
        {
            playerController.SetIsStoneGroundedState(true);
        }

        if (_other.gameObject.layer == LayerMask.NameToLayer("GroundGravel"))
        {
            playerController.SetIsGravelGroundedState(true);
        }

        playerController.SetIsGroundedState(true);
    }

    private void OnCollisionEnter(Collision _collision)
    {
        if (_collision.gameObject == playerController.gameObject)
        {
            return;
        }

        // Collision with GroundStone
        if (_collision.collider.gameObject.layer == LayerMask.NameToLayer("GroundStone"))
        {
            playerController.SetIsStoneGroundedState(true);
        }

        // Collision with GroundGravel
        if (_collision.gameObject.layer == LayerMask.NameToLayer("GroundGravel"))
        {
            playerController.SetIsGravelGroundedState(true);
        }

        playerController.SetIsGroundedState(true);
    }

    private void OnCollisionExit(Collision _collision)
    {
        if (_collision.gameObject == playerController.gameObject)
        {
            return;
        }

        if (_collision.gameObject.layer == LayerMask.NameToLayer("GroundStone"))
        {
            playerController.SetIsStoneGroundedState(false);
        }

        if (_collision.gameObject.layer == LayerMask.NameToLayer("GroundGravel"))
        {
            playerController.SetIsGravelGroundedState(false);
        }

        playerController.SetIsGroundedState(false);
    }

    private void OnCollisionStay(Collision _collision)
    {
        if (_collision.gameObject == playerController.gameObject)
        {
            return;
        }

        if (_collision.gameObject.layer == LayerMask.NameToLayer("GroundStone"))
        {
            playerController.SetIsStoneGroundedState(true);
        }

        if (_collision.gameObject.layer == LayerMask.NameToLayer("GroundGravel"))
        {
            playerController.SetIsGravelGroundedState(true);
        }

        playerController.SetIsGroundedState(true);
    }
}