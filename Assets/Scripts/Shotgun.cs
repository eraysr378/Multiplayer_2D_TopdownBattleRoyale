using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon
{

    bool fire = true;
    float time = 0;

    private void Update()
    {
        time += Time.deltaTime;
        if (time > 0.1f)
        {
            fire = true;
            time = 0;
        }
    }
    override public void Fire()
    {
        if (fire)
        {
            fire = false;
            time = 0;

            Vector3 aimDir = player.GetAimDirectionNormalized();
            ShooterGameMultiplayer.Instance.SpawnBullet(player,bulletPrefab, player.GetAimAngle().z, aimDir.x, aimDir.y, firePoint);

        }
    }
}
