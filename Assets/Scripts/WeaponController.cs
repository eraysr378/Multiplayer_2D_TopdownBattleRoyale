using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
public enum WeaponType
{
    Knife,
    Pistol,
    Rifle,
    Shotgun
}
public class WeaponController : NetworkBehaviour
{
    public event EventHandler OnWeaponChanged;
    [SerializeField] private Weapon weapon;
    [SerializeField] private Weapon previousWeapon;

    [SerializeField] private Player player;
    [SerializeField] private List<WeaponType> unlockedWeaponList = new List<WeaponType>();

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        unlockedWeaponList.Add(WeaponType.Knife);
        weapon = GetComponentInChildren<Knife>();
        OnWeaponChanged += WeaponController_OnWeaponChanged;
        OnWeaponChanged?.Invoke(this, EventArgs.Empty);


    }

    private void WeaponController_OnWeaponChanged(object sender, EventArgs e)
    {
        if (previousWeapon != null && previousWeapon is not Knife)
        {
            player.ResetAudioClip();
            Gun previousGun = previousWeapon as Gun;
            previousGun.CancelReloading();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (weapon is not Pistol && unlockedWeaponList.Contains(WeaponType.Pistol) && Input.GetKeyDown(KeyCode.Alpha1))
        {
            previousWeapon = weapon;
            weapon = GetComponentInChildren<Pistol>();
            OnWeaponChanged?.Invoke(this, EventArgs.Empty);

        }
        else if (weapon is not Rifle && unlockedWeaponList.Contains(WeaponType.Rifle) && Input.GetKeyDown(KeyCode.Alpha2))
        {

            previousWeapon = weapon;
            weapon = GetComponentInChildren<Rifle>();
            OnWeaponChanged?.Invoke(this, EventArgs.Empty);
        }
        else if (weapon is not Shotgun && unlockedWeaponList.Contains(WeaponType.Shotgun) && Input.GetKeyDown(KeyCode.Alpha3))
        {

            previousWeapon = weapon;
            weapon = GetComponentInChildren<Shotgun>();
            OnWeaponChanged?.Invoke(this, EventArgs.Empty);
        }
        else if (weapon is not Knife && Input.GetKeyDown(KeyCode.Alpha4))
        {

            previousWeapon = weapon;
            weapon = GetComponentInChildren<Knife>();
            OnWeaponChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    public void ChangeWeapon()
    {
        OnWeaponChanged?.Invoke(this, EventArgs.Empty);
    }
    public Rifle GetRifle()
    {
        return GetComponentInChildren<Rifle>();
    }
    public Pistol GetPistol()
    {
        return GetComponentInChildren<Pistol>();
    }
    public Shotgun GetShotgun()
    {
        return GetComponentInChildren<Shotgun>();
    }
    public bool Fire()
    {
        return weapon.Fire();
    }
    public Weapon GetCurrentWeapon()
    {
        return weapon;
    }
    public bool IsWeaponUnlocked(WeaponType weaponType)
    {
        return unlockedWeaponList.Contains(weaponType);
    }
    public bool UnlockWeapon(WeaponType weaponType)
    {
        if (unlockedWeaponList.Contains(weaponType))
        {
            return false;
        }
        else
        {
            unlockedWeaponList.Add(weaponType);
            return true;
        }
   
    }
}
