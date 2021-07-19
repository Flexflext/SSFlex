using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHud : MonoBehaviour
{
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


    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Chnage the Shield Display on the Player Hud + Plays Hit Effect if Dmged
    /// </summary>
    /// <param name="_max"></param>
    /// <param name="_current"></param>
    /// <param name="_tookDmg"></param>
    public void ChangeShieldAmount(float _max, float _current, bool _tookDmg)
    {
        if (_tookDmg)
        {
            shieldAmimator.Play("GotHitAnimation");
        }

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
        float healthPercent = _current / _max;

        healthBar.fillAmount = healthPercent;

        healthDisplay.alpha = 1 - healthPercent;

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
        killAmount.text = _amount.ToString();
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
    public void DisplayKillOnPlayer()
    {
        Instantiate(killSmthObj, this.transform);
    }
}
