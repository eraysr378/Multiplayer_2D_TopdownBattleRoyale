using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WeaponController : NetworkBehaviour
{
    public event EventHandler OnWeaponChanged;
    [SerializeField] private Weapon weapon;
    [SerializeField] private Player player;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
        {
            return;
        }
       
        if (weapon is not Pistol && Input.GetKeyDown(KeyCode.Alpha1))
        {
            weapon = GetComponentInChildren<Pistol>();
            OnWeaponChanged?.Invoke(this, EventArgs.Empty);
        }
        if (weapon is not Rifle && Input.GetKeyDown(KeyCode.Alpha2))
        {
            weapon = GetComponentInChildren<Rifle>();
            OnWeaponChanged?.Invoke(this, EventArgs.Empty);
        }
        if (weapon is not Shotgun && Input.GetKeyDown(KeyCode.Alpha3))
        {
            weapon = GetComponentInChildren<Shotgun>();
            OnWeaponChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    public bool Fire()
    {
        return weapon.Fire();
    }
    public Weapon GetCurrentWeapon()
    {
        return weapon;
    }
}
