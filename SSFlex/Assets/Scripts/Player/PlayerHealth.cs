using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using ExitGames.Client.Photon;

public class PlayerHealth : MonoBehaviourPunCallbacks
{
    public const byte ON_DEATH = 66;


    [SerializeField] private float health;
    [SerializeField] private float shield;
    [SerializeField] private float timeUntilShieldRegen;
    [SerializeField] private float regenPerSecond;
    private float maxHealth;
    private float maxShield;
    private float currentTime;

    private bool dead;
    public System.Action<string> OnKill;

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
                PlayerHud.Instance.ChangeShieldAmount(maxShield, shield, false);
                shield += Time.deltaTime * regenPerSecond;
            }
        }
    }

    public void TakeDamage(float _dmg, int _actornum)
    {
        photonView.RPC("RPC_TakeDamage", RpcTarget.All, _dmg, _actornum);
    }

    [PunRPC]
    private void RPC_TakeDamage(float _dmg, int _actornum)
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
            GameObject deadRobot = PhotonNetwork.Instantiate(System.IO.Path.Combine("PhotonPrefabs", "DeadRobot"), this.transform.position, this.transform.rotation);
            deadRobot.GetComponent<DeadRobot>().ChangeApperance(this.GetComponent<PlayerGFXChange>().CurrentTeam);

            string killerName = "";

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player.ActorNumber == _actornum)
                {
                    killerName = player.NickName;
                    break;
                }
            }

            PlayerHud.Instance.DisplayDeathDisplay(killerName);

            object[] content = new object[] { photonView.Owner.NickName };
            RaiseEventOptions eventOptions = new RaiseEventOptions { TargetActors = new int[] { _actornum } };
            PhotonNetwork.RaiseEvent(ON_DEATH, content, eventOptions, SendOptions.SendReliable);

            PhotonNetwork.Destroy(this.gameObject);
            dead = true;
        }


        PlayerHud.Instance.ChangeHealthAmount(maxHealth, health);
        PlayerHud.Instance.ChangeShieldAmount(maxShield, shield, true);
    } 
}

