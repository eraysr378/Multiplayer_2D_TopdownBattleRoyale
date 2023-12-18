using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowSystem : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private float grenadeAmount;
    [SerializeField] private float throwAnimationDuration;

    private bool canThrow = true;
    private float throwTimer = 0;
    private bool wasGunLaserActive;
    private void Update()
    {
        if (!canThrow)
        {
            throwTimer += Time.deltaTime;
            if (throwTimer > throwAnimationDuration)
            {
                canThrow = true;
                throwTimer = 0;
                player.GetWeaponController().ChangeWeapon(); // animation does not change, so force it to change to the current weapon
                player.SetCanShoot(true);
                if (wasGunLaserActive)
                {
                    (player.GetCurrentWeapon() as Gun).ActivateLaser();
                }

            }
        }
    }
    public void IncreaseGrenadeAmount(int increaseAmount)
    {
        grenadeAmount += increaseAmount;
    }
    public bool ThrowGrenade()
    {
        if (player.CanShoot() && canThrow && grenadeAmount > 0)
        {
            wasGunLaserActive = false;
            // if player has a gun and tries to reload when throwing grenade, dont allow it
            Gun gun = player.GetCurrentWeapon() as Gun;
            if (gun != null)
            {
                gun.CancelReloading();
                if (gun.IsLaserActive())
                {
                    wasGunLaserActive = true;
                    gun.DeactivateLaser();
                }
            }
            
            player.SetCanShoot(false);
            canThrow = false;
            grenadeAmount--;
            Invoke(nameof(SpawnGrenade), throwAnimationDuration / 2);
            return true;
        }
        return false;
    }

    private void SpawnGrenade()
    {
        ShooterGameMultiplayer.Instance.SpawnGrenade(player, transform.position, player.GetAimDirectionNormalized());
    }
}
