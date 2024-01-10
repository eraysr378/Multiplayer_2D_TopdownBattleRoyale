using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Image weaponImage;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private Image dashImage;
    [SerializeField] private Image pistolLaserImage;
    [SerializeField] private Image rifleLaserImage;
    [SerializeField] private TextMeshProUGUI grenadeCountText;
    private Player player;

    private void Start()
    {
        if (Player.LocalInstance != null)
        {
            player = Player.LocalInstance;
            player.GetWeaponController().OnWeaponChanged += Player_OnWeaponChanged;
            player.GetAbilitySystem().OnDashUnlocked += Player_OnDashUnlocked;
            player.GetAbilitySystem().OnPistolLaserUnlocked += Player_OnPistolLaserUnlocked;
            player.GetAbilitySystem().OnRifleLaserUnlocked += Player_OnRifleLaserUnlocked;
        }
        else
        {
            Player.OnAnyPlayerSpawned += Player_OnAnyPlayerSpawned;
        }
    }
    private void Update()
    {
        Gun gun = player.GetCurrentWeapon() as Gun;
        if (gun != null)
        {
            ammoText.text = gun.CurrentMagazineCapacity() + "/" + gun.MaxMagazineCapacity();
        }
        else
        {
            ammoText.text = "infinity";
        }
        grenadeCountText.text = player.GetThrowSystem().GetGrenadeCount().ToString("0");
        if (player.GetAbilitySystem().IsDashCooldown())
        {
            dashImage.color = new Color32(255, 255, 255, 100);
        }
        else
        {
            dashImage.color = new Color32(255, 255, 255, 255);

        }
    }
    private void Player_OnDashUnlocked(object sender, System.EventArgs e)
    {
        dashImage.gameObject.SetActive(true);
    }
    private void Player_OnPistolLaserUnlocked(object sender, System.EventArgs e)
    {
        pistolLaserImage.gameObject.SetActive(true);
    }
    private void Player_OnRifleLaserUnlocked(object sender, System.EventArgs e)
    {
        rifleLaserImage.gameObject.SetActive(true);
    }

    private void Player_OnWeaponChanged(object sender, System.EventArgs e)
    {
        weaponImage.sprite = player.GetCurrentWeapon().GetWeaponSprite();
    }

    private void Player_OnAnyPlayerSpawned(object sender, System.EventArgs e)
    {
        if (Player.LocalInstance != null)
        {
            player = Player.LocalInstance;
            player.GetWeaponController().OnWeaponChanged -= Player_OnWeaponChanged;
            player.GetAbilitySystem().OnDashUnlocked -= Player_OnDashUnlocked;
            player.GetAbilitySystem().OnPistolLaserUnlocked -= Player_OnPistolLaserUnlocked;
            player.GetAbilitySystem().OnRifleLaserUnlocked -= Player_OnRifleLaserUnlocked;


            player.GetWeaponController().OnWeaponChanged += Player_OnWeaponChanged;
            player.GetAbilitySystem().OnDashUnlocked += Player_OnDashUnlocked;
            player.GetAbilitySystem().OnPistolLaserUnlocked += Player_OnPistolLaserUnlocked;
            player.GetAbilitySystem().OnRifleLaserUnlocked += Player_OnRifleLaserUnlocked;
        }
    }

}
