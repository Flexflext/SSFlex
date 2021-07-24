using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerHealth : MonoBehaviourPunCallbacks
{
    // Script von Felix
    // Purpose: Leben vom Spieler + VFX Play


    // EventCode für ON_Death Raise Event
    public const byte ON_DEATH = 66;

    [Header("Stats")]
    [SerializeField] private float health;
    [SerializeField] private float shield;
    [SerializeField] private float timeUntilShieldRegen;
    [SerializeField] private float regenPerSecond;

    [Header("Refs")]
    [SerializeField] private VisualEffect impactEffect;
    [SerializeField] private ParticleSystem regenEffect;

    //MaxStets
    private float maxHealth;
    private float maxShield;
    private float currentTime;

    private bool dead;


    //public System.Action<string> OnKill;

    // Start is called before the first frame update
    void Start()
    {
        // Add to life Dictionary
        LevelManager.Instance.AddDictionary(photonView.Owner.UserId, true);
        maxHealth = health;
        maxShield = shield;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the Player Owns this GameObject
        if (!photonView.IsMine)
        {
            return;
        }

        //Check if the Player has to Regen his Shield
        if (shield <= maxShield && !dead)
        {

            //Check if Regen Timer has Passed
            if (currentTime > 0)
            {
                //Stop Regen
                if (regenEffect.isPlaying)
                {
                    photonView.RPC("RPC_RegenEffect", RpcTarget.All, false);
                }

                currentTime -= Time.deltaTime;
            }
            else
            {
                // Start VFX Effect if the Player starts Regenniníng
                if (!regenEffect.isPlaying)
                {
                    photonView.RPC("RPC_RegenEffect", RpcTarget.All, true);
                }

                //Add Shield
                shield += Time.deltaTime * regenPerSecond;

                //Check if the Shield Regen is Done and Stopps it
                if (shield >= maxShield)
                {
                    photonView.RPC("RPC_RegenEffect", RpcTarget.All, false);
                }

                //Changes the Hud Shield Display
                if (photonView.IsMine)
                {
                    PlayerHud.Instance.ChangeShieldAmount(maxShield, shield, false);
                }
            }
        }
    }

    /// <summary>
    /// Lets Players Take Dmg and Broadcast it ´with an RPC Call
    /// </summary>
    /// <param name="_dmg"></param>
    /// <param name="_actornum"></param>
    public void TakeDamage(float _dmg, int _actornum)
    {
        //RPC Call for Taking Dmg
        photonView.RPC("RPC_TakeDamage", RpcTarget.All, _dmg, _actornum);
    }


    /// <summary>
    /// RPC Call to Start and Stop the RegenEffect
    /// </summary>
    /// <param name="_onoff"></param>
    [PunRPC]
    private void RPC_RegenEffect(bool _onoff)
    {
        if (_onoff)
        {
            regenEffect.Play();
        }
        else
        {
            regenEffect.Stop();
        }
    }

    /// <summary>
    /// RPC Call to Lets the Player Take Dmg (to shield or health) if the PhotonView is Mine
    /// </summary>
    /// <param name="_dmg"></param>
    /// <param name="_actornum"></param>
    [PunRPC]
    private void RPC_TakeDamage(float _dmg, int _actornum)
    {
        // Take Dmg only if the Networking Client is mine
        if (!photonView.IsMine)
        {
            impactEffect.Play();
            return;
        }

        //Debug.Log("TookDmg " + _dmg);


        //Check how much Dmg to do to Shield or to Health
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

        //Check if the Player is Dead
        if (health <= 0)
        {
            //Let the Player Die
            Die(_actornum);
        }

        //Display Health and Shield on Hud
        PlayerHud.Instance.ChangeHealthAmount(maxHealth, health);
        PlayerHud.Instance.ChangeShieldAmount(maxShield, shield, true);
    }


    /// <summary>
    /// Lets the Player Die and Starts Raise Event
    /// </summary>
    /// <param name="_actornum"></param>
    private void Die(int _actornum)
    {
        // Spawns a deadRobot
        PhotonNetwork.Instantiate(System.IO.Path.Combine("PhotonPrefabs", "DeadRobot"), this.transform.position, this.transform.rotation);

        string killerName = "";

        // Check what name the killer has
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.ActorNumber == _actornum)
            {
                killerName = player.NickName;
                break;
            }
        }

        // Display the Death Display
        PlayerHud.Instance.DisplayDeathDisplay(killerName);

        // Raise Event to Display as a Callback to the killer
        object[] content = new object[] { photonView.Owner.NickName };
        RaiseEventOptions eventOptions = new RaiseEventOptions { TargetActors = new int[] { _actornum } };
        PhotonNetwork.RaiseEvent(ON_DEATH, content, eventOptions, SendOptions.SendReliable);

        //Updates the Dictionary of all Players Alive
        LevelManager.Instance.UpdateDictionary(photonView.Owner.UserId, false);

        //Check if the Player is the Only on Left
        if (LevelManager.Instance.CheckIfTheOnlyOneAlive())
        {
            //Check wich of the Players has Won to Display who won
            LevelManager.Instance.CheckIfWon();
        }

        //Destroys this GameObject
        PhotonNetwork.Destroy(this.gameObject);
        dead = true;
    }
}




