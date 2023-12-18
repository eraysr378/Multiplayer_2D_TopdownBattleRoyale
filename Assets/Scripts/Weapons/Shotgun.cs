using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Gun
{
    [SerializeField] private float bulletAmountInOneShot;
    [SerializeField] private float recoil;
    private bool canFire = true;
    private float fireTimer = 0;

    private void Start()
    {
        bulletAmountInOneShot = 5;
        recoil = -10;
    }
    private void Update()
    {

        if (isReloading)
        {
            reloadTimer += Time.deltaTime;
            if (reloadTimer > reloadDuration)
            {
                EndReloading();
            }
        }
        if (!canFire)
        {
            fireTimer += Time.deltaTime;
            if (fireTimer > timeBetweenShots)
            {
                player.SetCanShoot(true);
                canFire = true;
                fireTimer = 0;
            }
        }
    }
    public override bool StartReloading()
    {
        if (!IsMagazineFull() && !isReloading)
        {
            player.ReloadShotgun();
            isReloading = true;
            return true;
        }
        return false;

    }
    public override void EndReloading()
    {
        reloadTimer = 0;
        magazineCurrentCapacity++;
        if (magazineCurrentCapacity == magazineMaxCapacity)
        {
            isReloading = false;
        }
        else
        {
            // continue reloading
            player.ReloadShotgun();
        }
    }
    override public bool Fire()
    {
        if (IsMagazineEmpty())
        {
            if (!isReloading)
            {
                StartReloading();
            }
            return false;
        }
        // cancel reloading even if magazine is not full because shotgun is fired
        isReloading = false;
        player.ResetAudioClip(); // cut the reload audio immediately
        if (player.CanShoot() && canFire)
        {
            player.SetCanShoot(false);
            magazineCurrentCapacity--;
            canFire = false;
            fireTimer = 0;

            float bulletRotationAngle = player.GetAimAngle().z;
            if (bulletRotationAngle < 0)
            {
                bulletRotationAngle += 360;
            }
            float tempRecoil = recoil;
            for (int i = 0; i < bulletAmountInOneShot; i++)
            {
                ShooterGameMultiplayer.Instance.SpawnBullet(player, bulletPrefab, bulletRotationAngle + tempRecoil, firePoint);
                tempRecoil += 5;
            }
            ShooterGameMultiplayer.Instance.SpawnBulletShell(bulletShellPrefab, bulletRotationAngle, bulletShellSpawnPoint);

            return true;
        }
        return false;
    }

}
