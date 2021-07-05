using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    // Script for Player Shooting input

    [SerializeField] private Camera cam;

    [SerializeField] private Gun currentGun;

    [Header("Guns")]
    //[SerializeField] private Gun Sniper;
    [SerializeField] private Gun AR;
    //[SerializeField] private Gun Handgun;

    private Animator animator;
    //private PlayerMovementNew movement;

    private bool imAiming;

    private bool isMeeleing;
    private float meeleTime = 1.2f;
    
    private bool isGrenadeThrowing;
    private float grenadeTime = 1.4f;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        //movement = GetComponent<PlayerMovementNew>();
    }

    // Update is called once per frame
    void Update()
    {
        //Check that the time Scale isnt 0
        if (Time.timeScale == 0)
        {
            return;
        }




        if (Input.GetKeyDown(KeyCode.F) && !isMeeleing && !isGrenadeThrowing)
        {
            isMeeleing = true;
            StartCoroutine(C_MeeleTimer(meeleTime));
            //Set Animator
            animator.SetTrigger("isMeeleing");
        }

        if (Input.GetKeyDown(KeyCode.Q) && !isGrenadeThrowing && !isMeeleing)
        {
            isGrenadeThrowing = true;
            StartCoroutine(C_GrenadeTimer(grenadeTime));
            //Set Animator
            animator.SetTrigger("GrenadeThrow");
        }


        #region Aiming

        // Check for Player input
        if (Input.GetButtonDown("Fire2") && !isMeeleing && !isGrenadeThrowing)
        {
            // Set Bool for Aiming, sets the CurrentAdsmultiplier + Sets Animator and Movement Speed
            imAiming = true;
            //PlayerLook.Instance.AdsMultiplier = currentGun.AdsMultiplier;
            animator.SetBool("isAiming", true);
            //movement.CurrentSpeed *= currentGun.ZoomMovementMultiplier;
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
            //PlayerLook.Instance.AdsMultiplier = 1;
            animator.SetBool("isAiming", false);
            //movement.CurrentSpeed = movement.MaxSpeed;

        }


        #endregion

        #region Fireing

        // Check for Player input
        if (Input.GetButtonDown("Fire1") && !isMeeleing && !isGrenadeThrowing)
        {
            // Start the Fireing of the CurrentGun
            currentGun.StartFiring();
            animator.SetTrigger("StopReload");

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
        if (currentGun.IsFiring)
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
        if (Input.GetKeyDown(KeyCode.R) && !isMeeleing && currentGun.BulletsInMag < currentGun.MagSize && !isGrenadeThrowing && !currentGun.IsReloading)
        {
            //Start Reloading
            currentGun.StartReload();
            animator.SetTrigger("isReloading");
            animator.ResetTrigger("StopReload");

        }

        //update Reload for Currentgun
        if (!isMeeleing && !isGrenadeThrowing)
        {
            currentGun.ReloadGun(Time.deltaTime);
        }
        
        #endregion

        #region ChangeWeapon

        // Check for Wich gun to use as Next weapon
        //if (Input.GetKeyDown(KeyCode.Alpha3) && !currentGun.isAiming && !imAiming)
        //{
        //    // Sets aniamtor Bools
        //    animator.SetBool("Sniper", true);
        //    animator.SetBool("Handgun", false);

        //    // Sets new Gun as Currentgun
        //    currentGun.gameObject.SetActive(false);
        //    currentGun = Sniper;

        //    //Changes the Ads Multiplier
        //    if (imAiming)
        //    {
        //        PlayerLook.Instance.AdsMultiplier = currentGun.AdsMultiplier;
        //    }
        //    else
        //    {
        //        PlayerLook.Instance.AdsMultiplier = 1;
        //    }

        //    //Changes the waepon and Changes the UI Images and Text
        //    currentGun.gameObject.SetActive(true);
        //    PlayerHud.Instance.ChangeWeapon(currentGun.WeaponIcon, currentGun.AmmoInMag, currentGun.CurrentAmmo);
        //}

        #endregion
    }

    private IEnumerator C_MeeleTimer(float _time)
    {
        yield return new WaitForSeconds(_time);
        isMeeleing = false;
    }

    private IEnumerator C_GrenadeTimer(float _time)
    {
        yield return new WaitForSeconds(_time);
        isGrenadeThrowing = false;
    }
}
