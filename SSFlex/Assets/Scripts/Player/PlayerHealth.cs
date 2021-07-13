using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerHealth : MonoBehaviourPunCallbacks
{
    [SerializeField] private float health;
    [SerializeField] private float shield;
    [SerializeField] private float timeUntilShieldRegen;
    [SerializeField] private float regenPerSecond;
    private float maxHealth;
    private float maxShield;
    private float currentTime;

    private bool dead;

    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        maxHealth = health;
        maxShield = shield;
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (shield <= maxShield && !dead)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
            }
            else
            {
                shield += Time.deltaTime * regenPerSecond;
            }
        }
    }

    public void TakeDamage(float _dmg)
    {
        photonView.RPC("RPC_TakeDamage", RpcTarget.All, _dmg);   
    }

    [PunRPC]
    private void RPC_TakeDamage(float _dmg)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        Debug.Log("TookDmg " + _dmg);



        float difference = shield - _dmg;
        currentTime = timeUntilShieldRegen;

        if (difference >= 0)
        {
            shield -= _dmg;
        }
        else
        {
            shield = 0f;
            health += difference;
        }

        if (health <= 0)
        {
            Debug.Log("I am Dead");
            dead = true;
        }
    }
}
