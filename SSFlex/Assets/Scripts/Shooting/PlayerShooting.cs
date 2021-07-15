using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public enum PrimaryWeapon 
{ 
    AR,
    Shotgun,
    Sniper,
}

public class PlayerShooting : MonoBehaviourPunCallbacks
{
    // Script for Player Shooting input

    [SerializeField] private Camera cam;
    [SerializeField] private PrimaryWeapon weapon;
    [SerializeField] private Animator thirdPersonAnimator;

    [Header("CurrentGun")]
    [SerializeField] private Gun currentGun;
    [SerializeField] private Gun primaryGun;
    [SerializeField] private Gun secondaryGun;

    [Header("GrenadeStuff")]
    [SerializeField] private Transform grenadeThrowPosition;
    [SerializeField] private Transform crossHairTarget;
    [SerializeField] private float regenTime;
    [SerializeField] private float grenadeSpeed;
    [SerializeField] private GameObject grenadePrefab;
    private bool canThrowGrenade = true;
    //Animation
    private bool isGrenadeThrowing;
    private float grenadeTime = 2/3f;

    [Header("Meele")]
    [SerializeField] private float range;
    [SerializeField] private float dmg;
    [SerializeField] private Transform swordPosition;
    [SerializeField] private LayerMask allDmgLayers;

    [Header("Guns")]
    [SerializeField] private Gun Sniper;
    [SerializeField] private Gun AR;
    [SerializeField] private Gun Shotgun;
    [SerializeField] private Gun Pistol;

    private Animator animator;
    private PlayerLook playerLook;
    private PlayerController controller;

    private bool imAiming;
    public bool ImAiming => imAiming;

    private bool isMeeleing;
    private float meeleTime = 1.2f;

    private bool isSwitching;

    [SerializeField] private bool farmMode;


    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        playerLook = GetComponent<PlayerLook>();
        controller = GetComponent<PlayerController>();

        //ChooseGun();

        farmMode = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        //Check that the time Scale isnt 0
        if (Time.timeScale == 0)
        {
            return;
        }


        if (!farmMode)
        {

            if (Input.GetKeyDown(KeyCode.F) && !isMeeleing && !isGrenadeThrowing && !isSwitching)
            {
                isMeeleing = true;
                InterruptReload();
                StartCoroutine(C_MeeleTimer(meeleTime));
                //Set Animator
                animator.SetTrigger("isMeeleing");
                StartCoroutine(MeleeAnimation());
            }

            if (Input.GetKeyDown(KeyCode.Q) && !isGrenadeThrowing && !isMeeleing && canThrowGrenade && !isSwitching)
            {
                isGrenadeThrowing = true;
                InterruptReload();
                StartCoroutine(C_GrenadeTimer(grenadeTime));
                //Set Animator
                animator.SetTrigger("GrenadeThrow");
                StartCoroutine(PlayerGrenadeAnimator());
            }


            #region Aiming

            // Check for Player input
            if (Input.GetButtonDown("Fire2") && !isMeeleing && !isGrenadeThrowing && !isSwitching)
            {
                // Set Bool for Aiming, sets the CurrentAdsmultiplier + Sets Animator and Movement Speed
                imAiming = true;
                InterruptReload();
                playerLook.AdsMultiplier = currentGun.AdsMultiplier;
                animator.SetBool("isAiming", true);
                controller.MovementMultiplier *= currentGun.MovementMultiplier;
            }

            //Update the Zoomin and out of the CurrentWeapon
            //Check weather to aim in or Out
            if (!currentGun.IsAiming && imAiming)
            {
                currentGun.ZoomIn(ref cam, 100, Time.deltaTime);
            }
            else
            {
                if (!imAiming)
                {
                    currentGun.ZoomOut(ref cam, 100, Time.deltaTime);
                }

            }

            // Check for Player input
            if (Input.GetButtonUp("Fire2"))
            {
                // Resets Bool for Aiming, Resets the CurrentAdsmultiplier + Resets Animator and Movement Speed
                imAiming = false;
                playerLook.AdsMultiplier = 1;
                animator.SetBool("isAiming", false);
                controller.MovementMultiplier = 1f;

            }


            #endregion

            #region Fireing

            // Check for Player input
            if (Input.GetButtonDown("Fire1") && !isMeeleing && !isGrenadeThrowing && !isSwitching)
            {
                // Start the Fireing of the CurrentGun
                currentGun.StartFiring();
                InterruptReload();

                // Check if is Fireing + has enough Ammo
                if (currentGun.IsFiring && currentGun.BulletsInMag > 0)
                {
                    // Sets Animator
                    animator.SetBool("isShooting", true);
                }
                else
                {
                    // play No more bullets Sound
                    //AudioManager.Instance.Play("NoMoreBullets");
                }
            }


            // update Fireing if Current gun is fireing
            if (currentGun.IsFiring && !currentGun.IsReloading)
            {
                currentGun.UpdateFiring(Time.deltaTime);
            }
            else // Sets Bool if aint fireing
            {
                animator.SetBool("isShooting", false);
            }

            // Check for Player input
            if (Input.GetButtonUp("Fire1"))
            {
                // Stops fireing of Gun
                currentGun.StopFiring();
                // Resets Animator bool
                animator.SetBool("isShooting", false);
            }

            // Update CurrentBullets
            //currentGun.UpdateBullets(Time.deltaTime);
            #endregion

            #region Reload

            // Check for Player input
            if (Input.GetKeyDown(KeyCode.R) && !isMeeleing && currentGun.BulletsInMag < currentGun.MagSize && !isGrenadeThrowing && !currentGun.IsReloading && !isSwitching)
            {
                //Start Reloading
                currentGun.StartReload();
                animator.SetTrigger("isReloading");
                animator.ResetTrigger("StopReload");

            }

            currentGun.ReloadGun(Time.deltaTime);

            #endregion

            #region ChangeWeapon

            //Check for Wich gun to use as Next weapon
            if (Input.GetKeyDown(KeyCode.Alpha1) && !currentGun.IsAiming && !imAiming && currentGun != primaryGun && !isSwitching)
            {
                SwitchWeapon(primaryGun, secondaryGun);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) && !currentGun.IsAiming && !imAiming && currentGun != secondaryGun && !isSwitching)
            {
                SwitchWeapon(secondaryGun, primaryGun);
            }

            #endregion

        }

    }

    private void ChooseGun()
    {
        secondaryGun = Pistol;

        switch (weapon)
        {
            case PrimaryWeapon.AR:
                primaryGun = AR;
                break;
            case PrimaryWeapon.Shotgun:
                primaryGun = Shotgun;
                break;
            case PrimaryWeapon.Sniper:
                primaryGun = Sniper;
                break;
            default:
                break;
        }

        SwitchWeapon(primaryGun, secondaryGun);
    }

    private void SwitchWeapon(Gun _switchtogun, Gun _switchfromgun)
    {
        animator.SetBool(_switchfromgun.GunName, false);

        StartCoroutine(C_WeaponSwitch(_switchtogun, _switchfromgun));
    }

    private IEnumerator C_WeaponSwitch(Gun _switchtogun, Gun _switchfromgun)
    {
        isSwitching = true;
        animator.SetBool(_switchtogun.GunName, true);
        yield return new WaitForSeconds(0.25f);

        // Sets new Gun as Currentgun
        currentGun.gameObject.SetActive(false);
        currentGun = _switchtogun;

        //Changes the Ads Multiplier
        if (imAiming)
        {
            playerLook.AdsMultiplier = currentGun.AdsMultiplier;
        }
        else
        {
            playerLook.AdsMultiplier = 1;
        }

        //Changes the waepon and Changes the UI Images and Text
        currentGun.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.25f);
        isSwitching = false;
    }

    /// <summary>
    /// Interrupts the Reloading Process of the Current Gun
    /// </summary>
    public void InterruptReload()
    {
        currentGun.StopReload();
        animator.SetTrigger("StopReload");
    }

    /// <summary>
    /// Instantiates a Grendae and Adds an upward ands forward force to it + Starts the RegenTimer
    /// </summary>
    private void ThrowGrenade()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        Vector3 direction = (crossHairTarget.position - grenadeThrowPosition.position).normalized + Vector3.up / 4;

        GameObject grenade = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs","Grenade"), grenadeThrowPosition.position, Quaternion.identity);

        Rigidbody grendadeRb = grenade.GetComponent<Rigidbody>();

        grendadeRb.AddForce(direction * grenadeSpeed, ForceMode.Impulse);

        canThrowGrenade = false;
        StartCoroutine(C_GrenadeRegenTimer(regenTime));
    }

    private void DoMeeleDmg()
    {
        Collider[] allDmgColliders = Physics.OverlapSphere(swordPosition.position, range, allDmgLayers);

        foreach (Collider dmgObj in allDmgColliders)
        {
            Debug.Log("Do "+ dmg + " Dmg to: " + dmgObj.gameObject.name);
        }
    }

    private IEnumerator C_MeeleTimer(float _time)
    {
        yield return new WaitForSeconds(_time / 6);
        DoMeeleDmg();
        yield return new WaitForSeconds(_time - _time / 6);
        isMeeleing = false;
    }

    private IEnumerator C_GrenadeTimer(float _time)
    {
        
        yield return new WaitForSeconds(_time - _time * 1/3f);
        
        ThrowGrenade();
        yield return new WaitForSeconds(_time * 1 / 3f);
        
        isGrenadeThrowing = false;
    }

    private IEnumerator C_GrenadeRegenTimer(float _time)
    {
        yield return new WaitForSeconds(_time);
        canThrowGrenade = true;
    }

    private IEnumerator PlayerGrenadeAnimator()
    {
        thirdPersonAnimator.SetBool("isThrowing", true);
        yield return new WaitForSeconds(1f);
        thirdPersonAnimator.SetBool("isThrowing", false);
    }

    private IEnumerator MeleeAnimation()
    {
        thirdPersonAnimator.SetBool("isStabbing", true);
        yield return new WaitForSeconds(1);
        thirdPersonAnimator.SetBool("isStabbing", false);
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(swordPosition.position, range);
    }
}
