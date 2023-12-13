using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon
{
    [SerializeField] private float bulletAmountInOneShot;
    [SerializeField] private float recoil;
    bool fire = true;
    float time = 0;

    private void Start()
    {
        bulletAmountInOneShot = 5;
        recoil = -10;
    }
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
            if (bulletRotationAngle < 0)
            {
                bulletRotationAngle += 360;
            }
            float tempRecoil = recoil;
            for (int i = 0; i < bulletAmountInOneShot; i++)
            {
                ShooterGameMultiplayer.Instance.SpawnBullet(player, bulletPrefab, bulletRotationAngle + tempRecoil, firePoint);
                tempRecoil +=  5;
            }
            ShooterGameMultiplayer.Instance.SpawnBulletShell(bulletShellPrefab, bulletRotationAngle, bulletShellSpawnPoint);

            return true;
        }
        return false;
    }

    /*public IEnumerator ShotgunFire(Vector2 aimDirection)
    {
        if (shotgunBullet.currentMagazine <= 0)
        {
            StartCoroutine(PlayReloadAnimation(shotgunBullet));
            yield break;
        }
        StartCoroutine(DisplayGunFireEffect());
        player.animator.SetTrigger("Shoot");
        audioManager.Play("ShotgunShoot");
        StartCoroutine(ShotgunPushBack(aimDirection));
        float recoil = -1 * shotgunBullet.recoil;
        for (int i = 0; i < shotgunBullet.shotgunBulletAmount; i++)
        {
            GameObject projectileShotgun = objectPooler.SpawnFromPool("ShotgunBullet", shotgunFirePoint.position, player.transform.rotation);
            projectileShotgun.transform.Rotate(new Vector3(0, 0, recoil));
            projectileShotgun.GetComponent<Rigidbody2D>().AddForce(projectileShotgun.transform.up * shotgunBullet.speed, ForceMode2D.Impulse);
            recoil += 2 * shotgunBullet.recoil / shotgunBullet.shotgunBulletAmount;
            StartCoroutine(Deactivate(projectileShotgun, 2f));

        }

        cameraController.StartExplosion(shotgunBullet.shakingTime, shotgunBullet.shakingForce);
        GameObject spentBullet = objectPooler.SpawnFromPool("SpentShotgunBullet", spentBulletSpawnPosition.position, player.transform.rotation);
        spentBullet.GetComponent<Rigidbody2D>().AddForce
              ((spentBullet.transform.position - player.transform.position).normalized * spentBulletSpeed, ForceMode2D.Impulse);
        // to make the spent bullet more realistic, add random rotation
        float random = Random.Range(-30, 30);
        spentBullet.transform.Rotate(new Vector3(0, 0, random));
        StartCoroutine(Deactivate(spentBullet, spentBulletLifeTime));
        // Decrease the bullet amount in the shotgun magazine
        shotgunBullet.currentMagazine--;
        // prevent shooting without stopping
        PlayerController.isShotgunShooting = true;
        yield return new WaitForSeconds(shotgunBullet.spawnTime);
        PlayerController.isShotgunShooting = false;
    }*/
}
