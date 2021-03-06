using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerHud : MonoBehaviourPunCallbacks
{
    // Script von Felix
    // Purpose: PlayerHud Funtions to Chnange Different Displays on the Hud

    public static PlayerHud Instance;

    [Header("Shield and Health")]
    [SerializeField] private Image healthBar;
    [SerializeField] private Image shieldBar;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text shieldText;
    [SerializeField] private CanvasGroup healthDisplay;
    [SerializeField] private Image shieldDisplay;
    [SerializeField] private Gradient shieldColorOverLife;
    [SerializeField] private Animator shieldAmimator;

    [Header("Weapons")]
    [SerializeField] private Image currentWeapon;
    [SerializeField] private Image otherWeapon;
    [SerializeField] private TMP_Text currentAmmo;
    [SerializeField] private TMP_Text maxAmmo;
    [SerializeField] private Image grenadeImage;

    [Header("Generell")]
    [SerializeField] private TMP_Text killAmount;
    [SerializeField] private GameObject hitSmthObjPlayer;
    [SerializeField] private GameObject hitSmthObjObject;
    [SerializeField] private GameObject killSmthObj;
    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject deathMenu;
    [SerializeField] private TMP_Text killerText;
    [SerializeField] private GameObject killFeedPos;
    [SerializeField] private GameObject killFeed;



    [SerializeField]
    private Slider mMiningProgress;
    [SerializeField]
    private TextMeshProUGUI mCurrentResources;


    private int currentKills;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Display a MiningProgress
    /// </summary>
    /// <param name="_maxMineDuration"></param>
    /// <param name="_miningProgress"></param>
    public void MiningProgress(float _maxMineDuration, float _miningProgress)
    {
        mMiningProgress.maxValue = _maxMineDuration;
        mMiningProgress.value = _miningProgress;
    }

    /// <summary>
    /// Sets the Current Rescource Amount
    /// </summary>
    /// <param name="_currentResources"></param>
    public void SetCurrentResourceAmount(float _currentResources)
    {
        mCurrentResources.text = "" + _currentResources;
    }

    /// <summary>
    /// Chnage the Shield Display on the Player Hud + Plays Hit Effect if Dmged
    /// </summary>
    /// <param name="_max"></param>
    /// <param name="_current"></param>
    /// <param name="_tookDmg"></param>
    public void ChangeShieldAmount(float _max, float _current, bool _tookDmg)
    {
        //Displays a Got Hit Animation
        if (_tookDmg)
        {
            shieldAmimator.Play("GotHitAnimation");
        }

        //Shows what hsieldDisplay< on the Hud and Changes the Shieldbar Fillamount and Text
        float shieldPercent = _current / _max;

        shieldDisplay.color = shieldColorOverLife.Evaluate(shieldPercent);
        shieldBar.fillAmount = shieldPercent;

        shieldText.text = ((int)_current).ToString();

    }

    /// <summary>
    /// Change the Health Display on the hud
    /// </summary>
    /// <param name="_max"></param>
    /// <param name="_current"></param>
    public void ChangeHealthAmount(float _max, float _current)
    {
        // Change the Health Amount and Displays on the Hud

        float healthPercent = _current / _max;

        healthBar.fillAmount = healthPercent;

        healthDisplay.alpha = 1 - healthPercent;

        //Check that there wont be a 0 on the Health Number
        if (_current > 0.5f)
        {
            healthText.text = ((int)_current).ToString();
        }
        else
        {
            if (_current <= 0)
            {
                healthText.text = "0";
            }
            else
            {
                healthText.text = "1";
            }
            
        }
    }

    /// <summary>
    /// Chnage the Fill Amount on the GrenadeDisplay
    /// </summary>
    /// <param name="_max"></param>
    /// <param name="_current"></param>
    public void ChangeGrenadeFillAmount(float _max, float _current)
    {
        float percent = _current / _max;

        grenadeImage.fillAmount = percent;
    }


    /// <summary>
    /// Change the ammo Stats on the Gun Display
    /// </summary>
    /// <param name="_current"></param>
    /// <param name="_max"></param>
    public void ChangeAmmoAmount(int _current, int _max)
    {
        currentAmmo.text = _current.ToString();
        maxAmmo.text = "/" + _max;
    }

    /// <summary>
    /// Chnage the WeaponSprite on the CurrentWeapon and other Weapon
    /// </summary>
    /// <param name="_newImg"></param>
    /// <param name="_oldImg"></param>
    public void ChangeWeaponImg(Sprite _newImg, Sprite _oldImg)
    {
        otherWeapon.sprite = _oldImg;
        currentWeapon.sprite = _newImg;
    }

    /// <summary>
    /// CHnage the Hud Kill Amount
    /// </summary>
    /// <param name="_amount"></param>
    public void ChangeKillAmount(int _amount)
    {
        currentKills += _amount;
        killAmount.text = currentKills.ToString();
    }

    /// <summary>
    /// Display a Dmg Indicator on the player hud
    /// </summary>
    public void DisplayDmgToPlayer()
    {
        Instantiate(hitSmthObjPlayer, this.transform);
    }


    /// <summary>
    /// Display a Dmg Indicator on the player hud
    /// </summary>
    public void DisplayDmgToObj()
    {
        Instantiate(hitSmthObjObject, this.transform);
    }

    /// <summary>
    /// Display a kill Indicator on the player hud
    /// </summary>
    public void DisplayKillOnPlayer(Sprite _weapon, string _killername, string _victimname)
    {
        Instantiate(killSmthObj, this.transform);

        //RPC Event to Display a KillFeed Kill
        photonView.RPC("RPC_SpawnObj",RpcTarget.All, _killername, _victimname);
    }

    /// <summary>
    /// RPC Call to Spawn a Kill Feed Kill
    /// </summary>
    /// <param name="_killername"></param>
    /// <param name="_victimname"></param>
    [PunRPC]
    private void RPC_SpawnObj(string _killername, string _victimname)
    {
        GameObject killFeedObj = Instantiate(killFeed, killFeedPos.transform.position, Quaternion.identity);
        killFeedObj.transform.parent = killFeedPos.transform;
        killFeedObj.GetComponent<KillFeedKill>().ChangeFeedContent(_killername, _victimname);
    }

    /// <summary>
    /// Displays a Death Display
    /// </summary>
    /// <param name="_killername"></param>
    public void DisplayDeathDisplay(string _killername)
    {
        deathMenu.SetActive(true);
        hud.SetActive(false);
        killerText.text = _killername;
    }

}
