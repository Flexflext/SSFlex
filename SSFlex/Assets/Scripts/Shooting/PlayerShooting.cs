using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine;

public enum EWeaponsAndUtensils
{
    AR,
    Shotgun,
    Sniper,
    Pistol,
    farmTool,
    sword,
}

public enum ELoadout
{
    farmTool,
    primaryWeapon,
    secondaryWeapon,
    knife
}

public class PlayerShooting : MonoBehaviourPunCallbacks
{
    // Script von Felix
    // und teile von Max
    // Purpose: Script for Player Shooting input and Attacking

    [SerializeField] private Camera cam;
    [SerializeField] private EWeaponsAndUtensils primaryWeaponIdx;
    [SerializeField] private Animator thirdPersonAnimator;
    [SerializeField] private GameObject farmTool;

    [Header("CurrentGun")]
    [SerializeField] private Gun currentGun;
    [SerializeField] private Gun primaryGun;
    [SerializeField] private Gun secondaryGun;

    [Header("GrenadeStuff")]
    [SerializeField] private Transform grenadeThrowPosition;
    [SerializeField] private Transform crossHairTarget;
    [SerializeField] private float regenTime;
    [SerializeField] private float grenadeSpeed;
    [SerializeField] private float maxGrenadeDmg;
    [SerializeField] private GameObject grenadePrefab;
    private bool canThrowGrenade = true;
    //Animation
    private bool isGrenadeThrowing;
    private float grenadeTime = 2 / 3f;

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

    [SerializeField] private float fov;

    [SerializeField]
    private GameObject mThirdPersonAR;
    [SerializeField]
    private GameObject mThirdPersonShotgun;
    [SerializeField]
    private GameObject mThirdPersonSniper;
    [SerializeField]
    private GameObject mThirdPersonPistol;
    [SerializeField]
    private GameObject mThirdPersonKnife;

    private GameObject mPrimaryThridPersonGun;
    private GameObject mSecondaryThridPersonGun;

    private List<GameObject> mLoadout;
    private ELoadout mCurrentLoadoutIdx;

    PlayerShooting mThisObj;

    private Animator animator;
    private PlayerLook playerLook;
    private PlayerController controller;
    [SerializeField]
    private PlayerGFXChange mTeamGfx;

    private Vector3 audioPosition;

    private bool imAiming;
    public bool ImAiming => imAiming;

    private bool isMeeleing;
    private float meeleTime = 1.2f;

    private bool isSwitching;
    private bool canDoDmgAgain = true;

    int weaponIdx;
    int toolIdx;
    Team team;

    private bool farmMode = true;

    private bool canShoot = true;

    private bool mChooseWeapon;

    private void Awake()
    {
        // Get Refernces
        animator = GetComponentInChildren<Animator>();
        playerLook = GetComponent<PlayerLook>();
        controller = GetComponent<PlayerController>();

        // Check wich gun is First
        primaryWeaponIdx = GameManager.Instance.StartWeapon;

        mLoadout = new List<GameObject>();

        // Choose a Gun 
        ChooseGun();
    }

    private void Start()
    {
        // Sub to OnEscape Meu Toggle Event
        EscapeMenu.Instance.OnToggle += CanShootToggle;
        GameManager.Instance.OnFovChange += ChangeFov;
        ChangeFov();
    }

    /// <summary>
    /// Deaktiviert das Rendern der 3rd Person Waffe 
    /// </summary>
    private void OnPreRender()
    {
        if (photonView.IsMine)
            mPrimaryThridPersonGun.GetComponent<Renderer>().enabled = false;
    }
    private void OnPostRender()
    {
        if (photonView.IsMine)
            mPrimaryThridPersonGun.GetComponent<Renderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Check that Photonview is Mine
        if (!photonView.IsMine)
        {
            return;
        }

        //Check that the time Scale isnt 0
        if (Time.timeScale == 0)
        {
            return;
        }

        //Check if Prep Phase is Over
        if (!RoundStartManager.Instance.PreparationPhase && !mChooseWeapon)
        {
            mChooseWeapon = true;
            farmMode = false;
            SwitchWeapon(primaryGun, secondaryGun);
        }

        
        //CHck if Can Even Shoot
        if (!canShoot)
        {
            //Check that isnt Aiming and Sets Fov Acorrdingly
            if (!imAiming)
            {
                currentGun.ZoomOut(ref cam, fov, Time.deltaTime);
            }

            return;
        }


        //Check that isnt in Farm Mode to Stop Alll Weapon Funktions in FarmMode
        if (!farmMode)
        {
            
            //Chck for Player Input to Attack witch a Meele Attack
            if (Input.GetKeyDown(KeyCode.F) && !isMeeleing && !isGrenadeThrowing && !isSwitching)
            {
                //Set Meele Bool + Interrupt Reload + StartCoroutine for Starting anothewr meele Attack
                isMeeleing = true;
                InterruptReload();
                StartCoroutine(C_MeeleTimer(meeleTime));
                //Set Animator
                animator.SetTrigger("isMeeleing");
                StartCoroutine(MeleeAnimation());

                //Check if photonView is Mine to Start a Sound
                if (photonView.IsMine)
                {
                    photonView.RPC("SyncAudio", RpcTarget.Others, Path.Combine("PhotonPrefabs", "SyncSounds", "SyncMelee"), this.transform.position);
                }
            }

            //Check for Player Input to Throw a Grenade
            if (Input.GetKeyDown(KeyCode.Q) && !isGrenadeThrowing && !isMeeleing && canThrowGrenade && !isSwitching)
            {
                // Set Bool + Interrupt Reload + StartCoroutine to be Able to start the another Grenade Throw
                isGrenadeThrowing = true;
                InterruptReload();
                StartCoroutine(C_GrenadeTimer(grenadeTime));
                //Set Animator + Coroutine
                animator.SetTrigger("GrenadeThrow");
                StartCoroutine(PlayerGrenadeAnimator());
                StartCoroutine(GrenadeAudioTimer());
            }


            #region Aiming

            // Check for Player input
            if (Input.GetButtonDown("Fire2") && !isMeeleing && !isGrenadeThrowing && !isSwitching)
            {
                // Set Bool for Aiming, sets the CurrentAdsmultiplier + Sets Animator and Movement Speed
                imAiming = true;
                InterruptReload();
                // Set the Ads Multiplier
                playerLook.AdsMultiplier = currentGun.AdsMultiplier;
                // Set the bool
                animator.SetBool("isAiming", true);

                // set the movemnt Multiplier
                controller.MovementMultiplier *= currentGun.MovementMultiplier;
            }

            //Update the Zoomin and out of the CurrentWeapon
            //Check weather to aim in or Out
            if (!currentGun.IsAiming && imAiming)
            {
                // Set the CurrentZoom
                currentGun.ZoomIn(ref cam, fov, Time.deltaTime);
            }
            else
            {
                //Set the Aiming Zoom to Zoom Out
                if (!imAiming)
                {
                    currentGun.ZoomOut(ref cam, fov, Time.deltaTime);
                }

            }

            // Check for Player input
            if (Input.GetButtonUp("Fire2"))
            {
                // Resets Bool for Aiming, Resets the CurrentAdsmultiplier + Resets Animator and Movement Speed
                imAiming = false;
                playerLook.AdsMultiplier = 1;
                // Stop the Aiming Bool
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
                // Change the Ammon Amount
                PlayerHud.Instance.ChangeAmmoAmount(currentGun.BulletsInMag, currentGun.CurrentAmmo);

                // Check if is Fireing + has enough Ammo
                if (currentGun.IsFiring && currentGun.BulletsInMag > 0)
                {
                    // Sets Animator
                    animator.SetBool("isShooting", true);
                    thirdPersonAnimator.SetBool("isShooting", true);

                    // Check what gun to Play the Different Sounds
                    if (currentGun == AR)
                    {
                        if (photonView.IsMine)
                        {
                            photonView.RPC("SyncAudio", RpcTarget.Others, Path.Combine("PhotonPrefabs", "SyncSounds", "SyncShootAR"), this.transform.position);
                        }
                    }
                    if (currentGun == Shotgun)
                    {
                        if (photonView.IsMine)
                        {
                            photonView.RPC("SyncAudio", RpcTarget.Others, Path.Combine("PhotonPrefabs", "SyncSounds", "SyncShootShotgun"), this.transform.position);
                        }
                    }
                    if (currentGun == Sniper)
                    {
                        if (photonView.IsMine)
                        {
                            photonView.RPC("SyncAudio", RpcTarget.Others, Path.Combine("PhotonPrefabs", "SyncSounds", "SyncShootSniper"), this.transform.position);
                        }
                    }
                    if (currentGun == Pistol)
                    {
                        if (photonView.IsMine)
                        {
                            photonView.RPC("SyncAudio", RpcTarget.Others, Path.Combine("PhotonPrefabs", "SyncSounds", "SyncShootPistolSound"), this.transform.position);
                        }
                    }
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
                // update the Firening of the Current Gun
                currentGun.UpdateFiring(Time.deltaTime);

                // Change the Ammo Hud 
                PlayerHud.Instance.ChangeAmmoAmount(currentGun.BulletsInMag, currentGun.CurrentAmmo);
            }
            else // Sets Bool if aint fireing
            {
                animator.SetBool("isShooting", false);
                thirdPersonAnimator.SetBool("isShooting", false);
            }

            // Check for Player input
            if (Input.GetButtonUp("Fire1"))
            {
                // Stops fireing of Gun
                currentGun.StopFiring();
                // Resets Animator bool
                animator.SetBool("isShooting", false);
                thirdPersonAnimator.SetBool("isShooting", false);
            }

            // Update CurrentBullets
            //currentGun.UpdateBullets(Time.deltaTime);
            #endregion

            #region Reload

            // Check for Player input
            if (Input.GetKeyDown(KeyCode.R) && !isMeeleing && currentGun.BulletsInMag < currentGun.MagSize && !isGrenadeThrowing && !currentGun.IsReloading && !isSwitching)
            {
                //Start Reloading and Set all Animation Trigger + Resets the Stop Reload Trigger
                currentGun.StartReload();
                StartCoroutine(ReloadAudioTimer());
                animator.SetTrigger("isReloading");
                animator.ResetTrigger("StopReload");
                StartCoroutine(ReloadAnimatorTimer());
            }

            //Update the Reload
            currentGun.ReloadGun(Time.deltaTime);

            #endregion

            #region ChangeWeapon

            //Check for Wich gun to use as Next weapon
            if (Input.GetKeyDown(KeyCode.Alpha1) && !currentGun.IsAiming && !imAiming && currentGun != primaryGun && !isSwitching)
            {
                // Switch Primary to Secondary
                SwitchWeapon(primaryGun, secondaryGun);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) && !currentGun.IsAiming && !imAiming && currentGun != secondaryGun && !isSwitching)
            {
                // Switch Secondary to Primary
                SwitchWeapon(secondaryGun, primaryGun);
            }

            #endregion

        }
        else
        {
            //Check that isnt Aiming
            if (!imAiming)
            {
                currentGun.ZoomOut(ref cam, fov, Time.deltaTime);
            }
        }

    }

    /// <summary>
    /// Choose wich Gun to Set as CurrentGun
    /// </summary>
    public void ChooseGun()
    {
        // Set Animator to FarmMode
        animator.SetBool("Farm", false);

        //Change Primary and Secondary Weapons
        secondaryGun = Pistol;
        mSecondaryThridPersonGun = mThirdPersonPistol;

        //Check wich Gun to Equip as Secondary
        switch (primaryWeaponIdx)
        {
            case EWeaponsAndUtensils.AR:
                primaryGun = AR;
                mPrimaryThridPersonGun = mThirdPersonAR;
                break;
            case EWeaponsAndUtensils.Shotgun:
                primaryGun = Shotgun;
                mPrimaryThridPersonGun = mThirdPersonShotgun;
                break;
            case EWeaponsAndUtensils.Sniper:
                primaryGun = Sniper;
                mPrimaryThridPersonGun = mThirdPersonSniper;
                break;
            default:
                break;
        }

        //Add the Loadaout and Display via Rpc Call
        mLoadout.Add(farmTool);
        mLoadout.Add(mPrimaryThridPersonGun);
        mLoadout.Add(mSecondaryThridPersonGun);
        mLoadout.Add(mThirdPersonKnife);

        mCurrentLoadoutIdx = ELoadout.farmTool;

        if (photonView.IsMine)
            photonView.RPC("DisplayObject", RpcTarget.All, (int)mCurrentLoadoutIdx);         
    }

    /// <summary>
    /// Switch the Weapon and Starts the Coroutine
    /// </summary>
    /// <param name="_switchtogun"></param>
    /// <param name="_switchfromgun"></param>
    private void SwitchWeapon(Gun _switchtogun, Gun _switchfromgun)
    {
        // Set Animator to Start the Switch
        animator.SetBool(_switchfromgun.GunName, false);
        thirdPersonAnimator.SetBool(_switchfromgun.GunName, false);

        // Start the Coroutine to switch the weapon
        StartCoroutine(C_WeaponSwitch(_switchtogun, _switchfromgun));
    }

    /// <summary>
    /// Coroutine to Switch the CurrentWeapon and Display on the Hud + Plays all Animations
    /// </summary>
    /// <param name="_switchtogun"></param>
    /// <param name="_switchfromgun"></param>
    /// <returns></returns>
    private IEnumerator C_WeaponSwitch(Gun _switchtogun, Gun _switchfromgun)
    {
        //Check if im thew owner of the photonview and CHnage the Hud Elements
        if (photonView.IsMine)
        {
            PlayerHud.Instance.ChangeWeaponImg(_switchtogun.GunImg, _switchfromgun.GunImg);
        }
        
        // tart the Animation and wait .25 secondsa to Switch an actuall current weapon
        isSwitching = true;
        animator.SetBool(_switchtogun.GunName, true);
        thirdPersonAnimator.SetBool(_switchtogun.GunName, true);
        yield return new WaitForSeconds(0.25f);

        // Sets new Gun as Currentgun
        currentGun.gameObject.SetActive(false);
        farmTool.SetActive(false);

        currentGun = _switchtogun;

        // Check the Loadout
        if (currentGun == secondaryGun && !isMeeleing)
            mCurrentLoadoutIdx = ELoadout.secondaryWeapon;
        else if(!isMeeleing)
            mCurrentLoadoutIdx = ELoadout.primaryWeapon;
        else if (isMeeleing)
            mCurrentLoadoutIdx = ELoadout.knife;


        //Changes the Ads Multiplier and Resets it
        if (imAiming)
        {
            playerLook.AdsMultiplier = currentGun.AdsMultiplier;
        }
        else
        {
            playerLook.AdsMultiplier = 1;
        }


        //Changes the waepon and Changes the UI Images and Text
        if(photonView.IsMine)
            photonView.RPC("DisplayObject", RpcTarget.All, (int)mCurrentLoadoutIdx);

        // Activate the Current weapon
        currentGun.gameObject.SetActive(true);

        // Chang the Ammo Amount
        if (photonView.IsMine)
        {
            PlayerHud.Instance.ChangeAmmoAmount(currentGun.BulletsInMag, currentGun.CurrentAmmo);
        }

        Debug.LogError(mCurrentLoadoutIdx);

        yield return new WaitForSeconds(0.25f);
        isSwitching = false;
    }

    [PunRPC]
    public void DisplayObject(int _currentWeapon)
    {
        for (int i = 0; i < mLoadout.Count; i++)
        {
            if (i == _currentWeapon)
            {
                mLoadout[i].gameObject.SetActive(true);
                Debug.Log(mLoadout[i]);
            }
            else
            {
                mLoadout[i].gameObject.SetActive(false);
                Debug.Log(mLoadout[i]);
            }
        }
    }

    /// <summary>
    /// Interrupts the Reloading Process of the Current Gun
    /// </summary>
    public void InterruptReload()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        // interrupt Reload + Sets the Trigger
        currentGun.StopReload();
        animator.SetTrigger("StopReload");
    }

    /// <summary>
    /// Instantiates a Grendae and Adds an upward ands forward force to it + Starts the RegenTimer
    /// </summary>
    private void ThrowGrenade()
    {
        // Check that is Mine
        if (!photonView.IsMine)
        {
            return;
        }

        // Calculate the Direction and Instantiates a GameObject
        Vector3 direction = (crossHairTarget.position - grenadeThrowPosition.position).normalized + Vector3.up / 4;

        GameObject grenade = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Grenade"), grenadeThrowPosition.position, Quaternion.identity);

        Rigidbody grendadeRb = grenade.GetComponent<Rigidbody>();

        // Adds Force to Grenade and adds Event
        grendadeRb.AddForce(direction * grenadeSpeed, ForceMode.Impulse);

        grenade.GetComponent<Grenade>().HitAnything += HitAnythingWithGrenade;

        canThrowGrenade = false;

        // Start the Reload Timer
        StartCoroutine(C_GrenadeRegenTimer(regenTime));
    }

    /// <summary>
    /// Do a Meele Attack and Check all Colliders that were hit
    /// </summary>
    private void DoMeeleDmg()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        //Debug.Log("Meele");

        // Check for all Hit Colliders with the Sphere Cast
        Collider[] allDmgColliders = Physics.OverlapSphere(swordPosition.position, range, allDmgLayers);


        foreach (Collider dmgObj in allDmgColliders)
        {
            //Debug.Log("Do " + dmg + " Dmg to: " + dmgObj.gameObject.name);
            HitAnything(dmg, dmgObj.gameObject);
        }
    }

    /// <summary>
    /// Wait till do actuall Meele Dmg to Fit Animation
    /// </summary>
    /// <param name="_time"></param>
    /// <returns></returns>
    private IEnumerator C_MeeleTimer(float _time)
    {
        yield return new WaitForSeconds(_time / 6);
        
        // Do Meele Dmg after  time of Meele Attack
        DoMeeleDmg();

        if (currentGun == primaryGun)
            SwitchWeapon(primaryGun, secondaryGun);
        else
            SwitchWeapon(secondaryGun, primaryGun);

        // RPC Call 
        if (photonView.IsMine)
            photonView.RPC("DisplayObject", RpcTarget.OthersBuffered, mCurrentLoadoutIdx);

        yield return new WaitForSeconds(_time - _time / 6);
        isMeeleing = false;

        if (currentGun == primaryGun)
            SwitchWeapon(primaryGun, secondaryGun);
        else
            SwitchWeapon(secondaryGun, primaryGun);

        // RPC Call 
        if (photonView.IsMine)
            photonView.RPC("DisplayObject", RpcTarget.OthersBuffered, mCurrentLoadoutIdx);
    }

    /// <summary>
    /// Throw Grenade After some Time
    /// </summary>
    /// <param name="_time"></param>
    /// <returns></returns>
    private IEnumerator C_GrenadeTimer(float _time)
    {
        yield return new WaitForSeconds(_time - _time * 1 / 3f);
        // Throw Grenade
        ThrowGrenade();
        yield return new WaitForSeconds(_time * 1 / 3f);

        // Throw grenade bool  = false to be able to do other shit again
        isGrenadeThrowing = false;
    }

    /// <summary>
    /// Play Audio Sounds for Explosion
    /// </summary>
    /// <returns></returns>
    private IEnumerator GrenadeAudioTimer()
    {
        // Throw Sound
        if (photonView.IsMine)
        {
            photonView.RPC("SyncAudio", RpcTarget.Others, Path.Combine("PhotonPrefabs", "SyncSounds", "SyncGrenadeStart"), this.transform.position);
        }

        yield return new WaitForSeconds(5);

        // Explosion Sound
        if (photonView.IsMine)
        {
            photonView.RPC("SyncAudio", RpcTarget.Others, Path.Combine("PhotonPrefabs", "SyncSounds", "SyncGrenadeExplosion"), this.transform.position);
        }
    }

    /// <summary>
    /// Play Audio of Reload
    /// </summary>
    /// <returns></returns>
    private IEnumerator ReloadAudioTimer()
    {
        //AudioManager.Instance.Play("ReloadStart");

        if (photonView.IsMine)
        {
            photonView.RPC("SyncAudio", RpcTarget.Others, Path.Combine("PhotonPrefabs", "SyncSounds", "SyncReloadStart"), this.transform.position);
        }
        yield return new WaitForSeconds(1f);
        //AudioManager.Instance.Play("ReloadEnd");

        if (photonView.IsMine)
        {
            photonView.RPC("SyncAudio", RpcTarget.Others, Path.Combine("PhotonPrefabs", "SyncSounds", "SyncReloadEnd"), this.transform.position);
        }
    }

    /// <summary>
    /// Reload Animation Timer for 3rd Person Char
    /// </summary>
    /// <returns></returns>
    private IEnumerator ReloadAnimatorTimer()
    {
        thirdPersonAnimator.SetBool("isReloading", true);
        yield return new WaitForSeconds(2);
        thirdPersonAnimator.SetBool("isReloading", false);
    }

    /// <summary>
    /// Check what Dmg to Do and to what
    /// </summary>
    /// <param name="_gameobject"></param>
    public void HitAnythingWithGrenade(GameObject _gameobject, float _percent)
    {
        if (!photonView.IsMine)
        {
            return;
        }
    
        // Check how much dmg to do to with Grenade
        float grenadeDmg =( maxGrenadeDmg * ( _percent));

        // Hit Anything with Grenade
        HitAnything(grenadeDmg, _gameobject);

    }

    /// <summary>
    /// Do Dmg to different Colliders
    /// </summary>
    /// <param name="_dmg"></param>
    /// <param name="_gameobject"></param>
    private void HitAnything(float _dmg, GameObject _gameobject)
    {
        if (!photonView.IsMine)
        {

            return;
        }

        // Check if hit Player
        if (_gameobject.CompareTag("Player") || _gameobject.layer == 14 || _gameobject.CompareTag("DmgPlayer"))
        {
            // Check if Can do Dmg (only do dmg to Player on one collider)
            if (!canDoDmgAgain)
            {
                return;
            }

            // Coroutine to do dmg again
            canDoDmgAgain = false;
            StartCoroutine(C_TimeTillDmgAgain());

            // Try to do Dmg to Player
            PlayerHealth health = _gameobject.GetComponentInParent<PlayerHealth>();
            health.TakeDamage(_dmg, photonView.OwnerActorNr);
            Debug.Log(_dmg);

            PlayerHud.Instance.DisplayDmgToPlayer();
        }
        // Do Dmg to Object
        else if (_gameobject.layer == 8)
        {
            _gameobject.GetComponent<NormalBuildingInfo>().TakeDamage(_dmg);
            PlayerHud.Instance.DisplayDmgToObj();
        }
    }

    /// <summary>
    /// Timer till can Do Dmg again
    /// </summary>
    /// <returns></returns>
    private IEnumerator C_TimeTillDmgAgain()
    {
        yield return new WaitForSeconds(0.08f);

        canDoDmgAgain = true;
    }

    // Instantiates a 3D AudioPrefab for everyone at a specific point.
    [PunRPC]
    public void SyncAudio(string _prefabName, Vector3 _prefabPosition)
    {
        PhotonNetwork.Instantiate(_prefabName, _prefabPosition, Quaternion.identity);
    }

    /// <summary>
    /// Wait time till Player can throw another grenade and Displays on Hud
    /// </summary>
    /// <param name="_time"></param>
    /// <returns></returns>
    private IEnumerator C_GrenadeRegenTimer(float _time)
    {

        float curTime = 0f;

        while (curTime < _time)
        {
            curTime += Time.deltaTime;

            // Display Current Time on Hud
            PlayerHud.Instance.ChangeGrenadeFillAmount(_time, curTime);

            yield return null;
        }

        // Can throw grandew again
        canThrowGrenade = true;
    }

    /// <summary>
    /// Play 3rd Person Grewnade Animation
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayerGrenadeAnimator()
    {
        thirdPersonAnimator.SetBool("isThrowing", true);
        yield return new WaitForSeconds(1f);
        thirdPersonAnimator.SetBool("isThrowing", false);
    }

    /// <summary>
    /// 3rd person meele Animation
    /// </summary>
    /// <returns></returns>
    private IEnumerator MeleeAnimation()
    {
        thirdPersonAnimator.SetBool("isStabbing", true);
        yield return new WaitForSeconds(1f);
        thirdPersonAnimator.SetBool("isStabbing", false);
    }

    /// <summary>
    /// Chnage Fov Event
    /// </summary>
    private void ChangeFov()
    {
        fov = GameManager.Instance.Fov;
    }

    /// <summary>
    /// Change the Shoot Toggle to can and cant shoot
    /// </summary>
    private void CanShootToggle()
    {
        Debug.Log("Hier");
        canShoot = !canShoot;
    }

    /// <summary>
    /// Check what trigger hit to add ammo to CurrentAmmo
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        // Check if Pistol
        if (other.CompareTag("AmmoPistol"))
        {
            Pistol.CurrentAmmo += 100;

        }
        // Check if Shotgun
        else if (other.CompareTag("AmmoShotgun"))
        {
            Shotgun.CurrentAmmo += 50;
        }
        // Check if Ar
        else if (other.CompareTag("AmmoAR"))
        {
            AR.CurrentAmmo += 200;
        }
        // Check if Sniper
        else if (other.CompareTag("AmmoSniper"))
        {
            Sniper.CurrentAmmo += 20;
        }

        // Dsiplay current Ammo on Hud
        PlayerHud.Instance.ChangeAmmoAmount(currentGun.BulletsInMag, currentGun.CurrentAmmo);
    }

    private void OnDestroy()
    {
        //Unsub to events
        GameManager.Instance.OnFovChange -= ChangeFov;
        EscapeMenu.Instance.OnToggle -= CanShootToggle;
    }
}
