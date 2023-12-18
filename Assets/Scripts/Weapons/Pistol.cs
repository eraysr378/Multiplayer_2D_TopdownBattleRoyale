using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Pistol : Gun
{
    private bool canFire = true;
    private float fireTimer = 0;
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
                canFire = true;
                player.SetCanShoot(true);
                fireTimer = 0;
            }
        }
    }
    public override bool StartReloading()
    {
        if (!IsMagazineFull() && !isReloading)
        {
            player.ReloadPistol();
            isReloading = true;
            return true;
        }
        return false;

    }
    public override void EndReloading()
    {
        isReloading = false;
        reloadTimer = 0;
        magazineCurrentCapacity = magazineMaxCapacity;
    }

    override public bool Fire()
    {
        if (IsMagazineEmpty() || isReloading)
        {
            if (!isReloading)
            {
                StartReloading();
            }
            return false;
        }
        if (player.CanShoot() &&  canFire)
        {
            player.SetCanShoot(false);
            magazineCurrentCapacity--;
            canFire = false;
            fireTimer = 0;
            float bulletRotationAngle;
            if (isLaserActive && laser.IsLocked())
            {
                Vector2 dir = laser.GetEnemyPosition() - firePoint.position;
                // ignore mouse position, shoot at where laser is pointing at
                bulletRotationAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                // when laser is active, there is no walking recoil
                ShooterGameMultiplayer.Instance.SpawnBullet(player, bulletPrefab, bulletRotationAngle, firePoint);
            }
            else
            {
                bulletRotationAngle = player.GetAimAngle().z;
                // if player is walking, then accuracy will be decreased
                if (player.IsWalking())
                {
                    ShooterGameMultiplayer.Instance.SpawnBullet(player, bulletPrefab, bulletRotationAngle + Random.Range(-walkingRecoil, walkingRecoil + 1), firePoint);
                }
                else
                {
                    ShooterGameMultiplayer.Instance.SpawnBullet(player, bulletPrefab, bulletRotationAngle, firePoint);

                }
                ShooterGameMultiplayer.Instance.SpawnBulletShell(bulletShellPrefab, bulletRotationAngle, bulletShellSpawnPoint);
            }


            return true;
        }
        return false;
    }

}
