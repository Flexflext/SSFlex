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
    // Script for Player Shooting input

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

    private bool mChooseWeapon;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        playerLook = GetComponent<PlayerLook>();
        controller = GetComponent<PlayerController>();

        primaryWeaponIdx = GameManager.Instance.StartWeapon;

        mLoadout = new List<GameObject>();

        ChooseGun();
    }

    private void Start()
    {
        GameManager.Instance.OnFovChange += ChangeFov;
        ChangeFov();
    }

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
        if (!photonView.IsMine)
        {
            return;
        }

        //Check that the time Scale isnt 0
        if (Time.timeScale == 0)
        {
            return;
        }

        if (!RoundStartManager.Instance.PreparationPhase && !mChooseWeapon)
        {
            mChooseWeapon = true;
            farmMode = false;
            SwitchWeapon(primaryGun, secondaryGun);
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

                if (photonView.IsMine)
                {
                    photonView.RPC("SyncAudio", RpcTarget.Others, Path.Combine("PhotonPrefabs", "SyncSounds", "SyncMelee"), this.transform.position);
                }
            }

            if (Input.GetKeyDown(KeyCode.Q) && !isGrenadeThrowing && !isMeeleing && canThrowGrenade && !isSwitching)
            {
                isGrenadeThrowing = true;
                InterruptReload();
                StartCoroutine(C_GrenadeTimer(grenadeTime));
                //Set Animator
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
                playerLook.AdsMultiplier = currentGun.AdsMultiplier;
                animator.SetBool("isAiming", true);
                controller.MovementMultiplier *= currentGun.MovementMultiplier;
            }

            //Update the Zoomin and out of the CurrentWeapon
            //Check weather to aim in or Out
            if (!currentGun.IsAiming && imAiming)
            {
                currentGun.ZoomIn(ref cam, fov, Time.deltaTime);
            }
            else
            {
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
                PlayerHud.Instance.ChangeAmmoAmount(currentGun.BulletsInMag, currentGun.CurrentAmmo);

                // Check if is Fireing + has enough Ammo
                if (currentGun.IsFiring && currentGun.BulletsInMag > 0)
                {
                    // Sets Animator
                    animator.SetBool("isShooting", true);
                    thirdPersonAnimator.SetBool("isShooting", true);
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
                currentGun.UpdateFiring(Time.deltaTime);
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
                //Start Reloading
                currentGun.StartReload();
                StartCoroutine(ReloadAudioTimer());
                animator.SetTrigger("isReloading");
                animator.ResetTrigger("StopReload");
                StartCoroutine(ReloadAnimatorTimer());
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
        else
        {
            if (!imAiming)
            {
                currentGun.ZoomOut(ref cam, fov, Time.deltaTime);
            }
        }

    }

    public void ChooseGun()
    {
        animator.SetBool("Farm", false);
        secondaryGun = Pistol;
        mSecondaryThridPersonGun = mThirdPersonPistol;

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


        mLoadout.Add(farmTool);
        mLoadout.Add(mPrimaryThridPersonGun);
        mLoadout.Add(mSecondaryThridPersonGun);
        mLoadout.Add(mThirdPersonKnife);

        mCurrentLoadoutIdx = ELoadout.farmTool;

        if (photonView.IsMine)
            photonView.RPC("DisplayObject", RpcTarget.OthersBuffered, (int)mCurrentLoadoutIdx);         
    }

    private void SwitchWeapon(Gun _switchtogun, Gun _switchfromgun)
    {
        animator.SetBool(_switchfromgun.GunName, false);
        thirdPersonAnimator.SetBool(_switchfromgun.GunName, false);


        StartCoroutine(C_WeaponSwitch(_switchtogun, _switchfromgun));
    }

    private IEnumerator C_WeaponSwitch(Gun _switchtogun, Gun _switchfromgun)
    {
        if (photonView.IsMine)
        {
            PlayerHud.Instance.ChangeWeaponImg(_switchtogun.GunImg, _switchfromgun.GunImg);
        }
        
        isSwitching = true;
        animator.SetBool(_switchtogun.GunName, true);
        thirdPersonAnimator.SetBool(_switchtogun.GunName, true);
        yield return new WaitForSeconds(0.25f);

        // Sets new Gun as Currentgun
        currentGun.gameObject.SetActive(false);
        farmTool.SetActive(false);

        currentGun = _switchtogun;


        if (currentGun == secondaryGun && !isMeeleing)
            mCurrentLoadoutIdx = ELoadout.secondaryWeapon;
        else if(!isMeeleing)
            mCurrentLoadoutIdx = ELoadout.primaryWeapon;
        else if (isMeeleing)
            mCurrentLoadoutIdx = ELoadout.knife;


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
        if(photonView.IsMine)
            photonView.RPC("DisplayObject", RpcTarget.OthersBuffered, (int)mCurrentLoadoutIdx);

        currentGun.gameObject.SetActive(true);

        if (photonView.IsMine)
        {
            PlayerHud.Instance.ChangeAmmoAmount(currentGun.BulletsInMag, currentGun.CurrentAmmo);
        }


        if (photonView.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("weaponKey", (int)mCurrentLoadoutIdx);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }

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

        GameObject grenade = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Grenade"), grenadeThrowPosition.position, Quaternion.identity);

        Rigidbody grendadeRb = grenade.GetComponent<Rigidbody>();

        grendadeRb.AddForce(direction * grenadeSpeed, ForceMode.Impulse);

        grenade.GetComponent<Grenade>().HitAnything += HitAnythingWithGrenade;

        canThrowGrenade = false;
        StartCoroutine(C_GrenadeRegenTimer(regenTime));
    }

    private void DoMeeleDmg()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        Debug.Log("Meele");

        Collider[] allDmgColliders = Physics.OverlapSphere(swordPosition.position, range, allDmgLayers);

        foreach (Collider dmgObj in allDmgColliders)
        {
            Debug.Log("Do " + dmg + " Dmg to: " + dmgObj.gameObject.name);
            HitAnything(dmg, dmgObj.gameObject);
        }
    }

    private IEnumerator C_MeeleTimer(float _time)
    {
        yield return new WaitForSeconds(_time / 6);
        
        DoMeeleDmg();

        if (currentGun == primaryGun)
            SwitchWeapon(primaryGun, secondaryGun);
        else
            SwitchWeapon(secondaryGun, primaryGun);

        if (photonView.IsMine)
            photonView.RPC("DisplayObject", RpcTarget.OthersBuffered, mCurrentLoadoutIdx);

        yield return new WaitForSeconds(_time - _time / 6);
        isMeeleing = false;

        if (currentGun == primaryGun)
            SwitchWeapon(primaryGun, secondaryGun);
        else
            SwitchWeapon(secondaryGun, primaryGun);

        if (photonView.IsMine)
            photonView.RPC("DisplayObject", RpcTarget.OthersBuffered, mCurrentLoadoutIdx);
    }

    private IEnumerator C_GrenadeTimer(float _time)
    {
        yield return new WaitForSeconds(_time - _time * 1 / 3f);

        ThrowGrenade();
        yield return new WaitForSeconds(_time * 1 / 3f);

        isGrenadeThrowing = false;
    }

    private IEnumerator GrenadeAudioTimer()
    {

        if (photonView.IsMine)
        {
            photonView.RPC("SyncAudio", RpcTarget.Others, Path.Combine("PhotonPrefabs", "SyncSounds", "SyncGrenadeStart"), this.transform.position);
        }

        yield return new WaitForSeconds(5);

        if (photonView.IsMine)
        {
            photonView.RPC("SyncAudio", RpcTarget.Others, Path.Combine("PhotonPrefabs", "SyncSounds", "SyncGrenadeExplosion"), this.transform.position);
        }
    }

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


        float grenadeDmg =( maxGrenadeDmg * ( _percent));
        

        HitAnything(grenadeDmg, _gameobject);

    }

    private void HitAnything(float _dmg, GameObject _gameobject)
    {
        Debug.Log("Hit Smth");
        if (!photonView.IsMine || !canDoDmgAgain)
        {
            return;
        }

        canDoDmgAgain = false;
        StartCoroutine(C_TimeTillDmgAgain());


        if (_gameobject.CompareTag("Player") || _gameobject.layer == 14 || _gameobject.CompareTag("DmgPlayer"))
        {
            Debug.LogWarning("Hit Player");
            PlayerHealth health = _gameobject.GetComponentInParent<PlayerHealth>();
            health.TakeDamage(_dmg, photonView.OwnerActorNr);
            Debug.Log(_dmg);

            PlayerHud.Instance.DisplayDmgToPlayer();
        }
        else if (_gameobject.layer == 8)
        {
            _gameobject.GetComponent<NormalBuildingInfo>().TakeDamage(_dmg);
            PlayerHud.Instance.DisplayDmgToObj();
        }
    }

    private IEnumerator C_TimeTillDmgAgain()
    {
        yield return new WaitForSeconds(0.05f);

        canDoDmgAgain = true;
    }


    // Instantiates a 3D AudioPrefab for everyone at a specific point.
    [PunRPC]
    public void SyncAudio(string _prefabName, Vector3 _prefabPosition)
    {
        PhotonNetwork.Instantiate(_prefabName, _prefabPosition, Quaternion.identity);
    }



    private IEnumerator C_GrenadeRegenTimer(float _time)
    {

        float curTime = 0f;

        while (curTime < _time)
        {
            curTime += Time.deltaTime;


            PlayerHud.Instance.ChangeGrenadeFillAmount(_time, curTime);

            yield return null;
        }


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
        yield return new WaitForSeconds(1f);
        thirdPersonAnimator.SetBool("isStabbing", false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (other.CompareTag("AmmoPistol"))
        {
            Pistol.CurrentAmmo += 100;

        }
        else if (other.CompareTag("AmmoShotgun"))
        {
            Shotgun.CurrentAmmo += 50;
        }
        else if (other.CompareTag("AmmoAR"))
        {
            AR.CurrentAmmo += 200;
        }
        else if (other.CompareTag("AmmoSniper"))
        {
            Sniper.CurrentAmmo += 20;
        }

        PlayerHud.Instance.ChangeAmmoAmount(currentGun.BulletsInMag, currentGun.CurrentAmmo);
    }

    private void ChangeFov()
    {
        fov = GameManager.Instance.Fov;
    }


    private void OnDestroy()
    {
        GameManager.Instance.OnFovChange -= ChangeFov;
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(swordPosition.position, range);
    }
}
