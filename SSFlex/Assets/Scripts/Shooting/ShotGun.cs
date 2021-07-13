using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotGun : Gun
{
    [SerializeField] protected Vector2[] spreadPattern;

    protected override void FireBullet()
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

        for (int i = 0; i < spreadPattern.Length; i++)
        {
            Vector3 velocity = new Vector3();

            //Check if aiming to Create a SpreadRadius
            if (!isAiming)
            {
                Vector3 direction = (crossHairTarget.position - muzzle.position).normalized;

                Vector3 spreadRangePos = muzzle.position + direction * spreadRange;

                // Add a Random Poition in the SpreadRadius
                spreadRangePos += transform.root.up * spreadPattern[i].y * spreadRadius;
                spreadRangePos += transform.root.right * spreadPattern[i].x * spreadRadius;

                // Set a new Velocity dependend on the BulletSpeed
                velocity = (spreadRangePos - muzzle.position).normalized;
            }
            else if (isAiming)
            {
                Vector3 direction = (crossHairTarget.position - muzzle.position).normalized;

                Vector3 spreadRangePos = muzzle.position + direction * spreadRange;

                // Add a Random Poition in the Aimed SpreadRadius
                spreadRangePos += transform.root.up * spreadPattern[i].y * spreadRadiusAimed;
                spreadRangePos += transform.root.right * spreadPattern[i].x * spreadRadiusAimed;

                velocity = (spreadRangePos - muzzle.position).normalized;
            }

            CreateBullet(muzzle.position, velocity, bulletSpeed);
        }

        // Generates Recoil
        GenerateRecoil();
    }
}
