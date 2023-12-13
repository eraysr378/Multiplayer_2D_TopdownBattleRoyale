using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Rifle : Weapon
{
    private const string IS_MUZZLE_FIRING = "IsMuzzleFiring";



    bool fire = true;
    float time = 0;
   
    private void Update()
    {
        if (!fire)
        {
            time += Time.deltaTime;
            if (time > timeBetweenShots)
            {
                fire = true;
                time = 0;
            }
        }
       
    }
    override public bool Fire()
    {
        if (fire)
        {
            fire = false;
            time = 0;
            float bulletRotationAngle = player.GetAimAngle().z;
            // if player is walking, then accuracy will be decreased
            if (player.IsWalking())
            {
                ShooterGameMultiplayer.Instance.SpawnBullet(player, bulletPrefab, bulletRotationAngle + Random.Range(-walkingRecoil,walkingRecoil+1), firePoint);
            }
            else
            {
                ShooterGameMultiplayer.Instance.SpawnBullet(player, bulletPrefab, bulletRotationAngle, firePoint);

            }
            ShooterGameMultiplayer.Instance.SpawnBulletShell(bulletShellPrefab,bulletRotationAngle,bulletShellSpawnPoint);
            animator.SetTrigger(IS_MUZZLE_FIRING);
            return true;
        }
        return false;
    }

}
