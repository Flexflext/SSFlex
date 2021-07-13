using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Photon.Pun;
using System.IO;
using Photon.Realtime;

public class Gun : MonoBehaviour
{
    //Bullet Behavior
    [SerializeField] protected bool isShooting;
    [SerializeField] protected float dmg;
    [SerializeField] protected float fireRate;
    [SerializeField] protected float fireRateFullAuto;
    [SerializeField] protected float bulletSpeed;

    [SerializeField]
    protected bool isFiring;
    public bool IsFiring => isFiring;
    protected bool canFire = true;

    //Magazine
    [SerializeField] protected bool isReloading;
    public bool IsReloading => isReloading;

    [SerializeField] protected bool isFullAuto;
    [SerializeField] protected int magSize;
    public int MagSize => magSize;

    [SerializeField] protected float reloadTime;

    [SerializeField] protected int currentBulletsInMag;
    public int BulletsInMag => currentBulletsInMag;

    protected bool canReload;
    protected float accumulatedTime;
    protected float nextTimeToFire;

    protected float currentReloadTime;

    [SerializeField]
    protected int currentAmmo;
    [SerializeField] protected int maxAmmo;

    //BulletSpread
    [SerializeField] protected bool isAiming;
    public bool IsAiming => isAiming;

    [SerializeField] protected float spreadRadius;
    [SerializeField] protected float spreadRadiusAimed;
    [SerializeField] protected float spreadRange;

    [SerializeField] protected float adsMultiplier;
    public float AdsMultiplier => adsMultiplier;

    [SerializeField] private float movementMultiplier;
    public float MovementMultiplier => movementMultiplier;

    //Zoom
    [SerializeField] protected float zoomMultiplier;
    [SerializeField] protected float zoomTime;

    //Recoil
    [SerializeField] protected float duration;
    protected float currentRecoilTime;
    [SerializeField] protected Vector2[] recoilPatternArray;
    protected float xRecoil;
    protected float yRecoil;
    protected int index;

    [SerializeField] protected string name;
    public string GunName => name;


    //Refs
    [Space]
    [SerializeField] protected VisualEffect muzzleFlash;
    [SerializeField] protected GameObject normalCanvas;
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected Transform crossHairTarget;
    [SerializeField] protected Transform muzzle;

    protected PlayerLook playerLook;


    private void Start()
    {
        playerLook = GetComponentInParent<PlayerLook>();
        currentBulletsInMag = magSize;
        currentReloadTime = 0;
        currentRecoilTime = duration;
    }


    /// <summary>
    /// Fire a Bullet (Check if Can fire and Where to Fire)
    /// </summary>
    protected virtual void FireBullet()
    {  
        // No more Reloading
        isReloading = false;

        // Check if can even fire
        if (currentBulletsInMag <= magSize)
        {
            canReload = true;
        }

        // Cant Fire anymore
        if (currentBulletsInMag == 0)
        {
            isFiring = false;
            return;
        }

        // Chenge Bullets in Mag + Change Hud
        if (currentBulletsInMag > 0)
        {
            currentBulletsInMag--;
            //PlayerHud.Instance.ChangeAmmoAmount(currentBulletsInMag, currentAmmo);
        }

        // Player Weapon ShotSound
        //AudioManager.Instance.PlayRandom(weaponName, weaponName + "Shot");

        // play MuzzleFlash
        muzzleFlash.Play();

        Vector3 velocity = new Vector3();

        //Check if aiming to Create a SpreadRadius
        if (!isAiming)
        {
            Vector3 direction = (crossHairTarget.position - muzzle.position).normalized;

            Vector3 spreadRangePos = muzzle.position + direction * spreadRange;

            // Add a Random Poition in the SpreadRadius
            Vector3 randomDestination = new Vector3(spreadRangePos.x + Random.Range(-spreadRadius, spreadRadius), spreadRangePos.y + Random.Range(-spreadRadius, spreadRadius), spreadRangePos.z + Random.Range(-spreadRadius, spreadRadius));

            // Set a new Velocity dependend on the BulletSpeed
            velocity = (randomDestination - muzzle.position).normalized;
        }
        else if (isAiming)
        {
            Vector3 direction = (crossHairTarget.position - muzzle.position).normalized;

            Vector3 spreadRangePos = muzzle.position + direction * spreadRange;

            // Add a Random Poition in the Aimed SpreadRadius
            Vector3 randomDestination = new Vector3(spreadRangePos.x + Random.Range(-spreadRadiusAimed, spreadRadiusAimed), spreadRangePos.y + Random.Range(-spreadRadiusAimed, spreadRadiusAimed), spreadRangePos.z + Random.Range(-spreadRadiusAimed, spreadRadiusAimed));
            velocity = (randomDestination - muzzle.position).normalized;
        }

        CreateBullet(muzzle.position, velocity, bulletSpeed);

        // Generates Recoil
        GenerateRecoil();
    }

    protected void CreateBullet(Vector3 _pos, Vector3 _direction, float _speed)
    {
        //Instantiate Bullet
        GameObject bullet = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Bullet"), _pos, Quaternion.identity);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        Bullet bulletScript = bullet.GetComponent<Bullet>();

        bulletScript.OnHit += HitAnything;
        rb.AddForce(_direction * _speed);
    }


    /// <summary>
    /// Check what Dmg to Do and to what
    /// </summary>
    /// <param name="_gameobject"></param>
    public void HitAnything(GameObject _gameobject)
    {
        Debug.Log("Hit Smth");
    }

    /// <summary>
    /// Start the Fireing of the Gun if can Fire
    /// </summary>
    public void StartFiring()
    {
        // Reset the Recoil
        ResetRecoil();

        //Check if Can Fire
        if (canFire)
        {
            //Reset the time and sets the FireingBool
            accumulatedTime = 0;
            nextTimeToFire = 0;

            isFiring = true;
        }

        // Check if can is actually full auto and stops Fireing
        if (!isFullAuto)
        {
            canFire = false;
            StartCoroutine(TimeBetweenAttacks(fireRate));
        }
    }

    /// <summary>
    /// Update fireing
    /// </summary>
    /// <param name="_deltatime"></param>
    public void UpdateFiring(float _deltatime)
    {
        // Check if the Gun is Full Auto
        if (isFullAuto)
        {
            // Add Time
            accumulatedTime += _deltatime;

            float fireIntervall = 1f / fireRate;

            //Check if can Fire
            if (!canFire)
            {
                return;
            }

            // Check if Can already Fir
            if (accumulatedTime >= nextTimeToFire)
            {
                nextTimeToFire = accumulatedTime + fireIntervall;
                FireBullet();
            }
        }
        else
        {
            isFiring = false;
            FireBullet();
        }
    }

    /// <summary>
    /// Stops the Fireing of the Gun
    /// </summary>
    public void StopFiring()
    {
        isFiring = false;
    }

    /// <summary>
    /// Starts the Reload of the Gun
    /// </summary>
    public void StartReload()
    {
        isReloading = true;
        currentReloadTime = 0;
    }

    /// <summary>
    /// Stops the Reload of the Gun
    /// </summary>
    public void StopReload()
    {
        isReloading = false;
        currentReloadTime = 0;
    }

    /// <summary>
    /// Updates the Reload of the Gun
    /// </summary>
    /// <param name="_deltatime"></param>
    public void ReloadGun(float _deltatime)
    {
        if (!canReload)
        {
            return;
        }

        if (isReloading)
        {
            if (currentReloadTime <= reloadTime)
            {
                currentReloadTime += _deltatime;
                //PlayerHud.Instance.UpdateReload(currentReloadTime, reloadTime);
            }
            else
            {
                if (currentAmmo >= magSize)
                {
                    currentBulletsInMag = magSize;
                    currentAmmo -= magSize;
                }
                else
                {
                    currentBulletsInMag = currentAmmo;
                    currentAmmo -= currentAmmo;
                }

                //PlayerHud.Instance.ChangeAmmoAmount(currentBulletsInMag, currentAmmo);
                //AudioManager.Instance.PlayRandom(weaponName, weaponName + "Reload");
                isReloading = false;
                canReload = false;
            }
        }
        else
        {
            currentReloadTime = 0;
            //PlayerHud.Instance.OpenCloseReload(false);
        }
    }

    /// <summary>
    /// Waits till the Gun can Fire Again
    /// </summary>
    /// <param name="_time"></param>
    /// <returns></returns>
    protected IEnumerator TimeBetweenAttacks(float _time)
    {
        yield return new WaitForSecondsRealtime(_time);
        canFire = true;
        StopAllCoroutines();
    }

    /// <summary>
    /// Zooms in and Sets Bool isAiming
    /// </summary>
    /// <param name="_cam"></param>
    public virtual void ZoomIn(ref Camera _cam, float _normal, float _delateTime)
    {
        //Check if FOV of Cam is Bigger than the aimed FOV
        if (_cam.fieldOfView > (_normal / zoomMultiplier))
        {
            // Reduce the FOV
            _cam.fieldOfView -= _delateTime * zoomTime;
        }
        else
        {
            // Aimed is True + right FOV
            isAiming = true;
            normalCanvas.SetActive(false);
            _cam.fieldOfView = _normal / zoomMultiplier;
        }
    }


    /// <summary>
    /// Zooms out and Sets Bool isAiming
    /// </summary>
    /// <param name="_cam"></param>
    /// <param name="_normal"></param>
    public virtual void ZoomOut(ref Camera _cam, float _normal, float _delateTime)
    {
        //Check if FOV of Cam is smaller than the normal FOV
        if (_cam.fieldOfView < _normal)
        {
            // Adds Fov
            _cam.fieldOfView += _delateTime * zoomTime;
        }
        else
        {
            // No mnore aiming + FOV = normal
            isAiming = false;
            normalCanvas.SetActive(true);
            _cam.fieldOfView = _normal;
        }
    }


    /// <summary>
    /// Generates Recoil for the Weapon
    /// </summary>
    protected void GenerateRecoil()
    {
        // set CuurentRecoil Time
        currentRecoilTime = duration;

        // Check the Recoil Pattern
        xRecoil = recoilPatternArray[index].x;
        yRecoil = recoilPatternArray[index].y;

        // index of Recoil pattern restarts when end of the Array
        index = (index + 1) % recoilPatternArray.Length;
    }

    /// <summary>
    /// Resets the Recoil Index to 0
    /// </summary>
    protected void ResetRecoil()
    {
        index = 0;
    }

    //Update the Recoil
    private void Update()
    {
        //Check if CurrentRecoil Time is bigger than 0
        if (currentRecoilTime >= 0)
        {
            // Adds vertical and horizontal Recoil
            float yValue = ((yRecoil / 10) * Time.deltaTime) / duration;
            float xValue = ((xRecoil / 10) * Time.deltaTime) / duration;

            // Adds Recoil to Cam
            playerLook.AddRecoil(yValue, xValue);

            // Reduce Recoil Time
            currentRecoilTime -= Time.deltaTime;
        }
    }
}
