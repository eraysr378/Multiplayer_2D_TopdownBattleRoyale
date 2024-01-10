using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour, Weapon
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected GameObject bulletShellPrefab;
    [SerializeField] protected Player player;
    [SerializeField] protected Transform firePoint;
    [SerializeField] protected Transform bulletShellSpawnPoint;
    [SerializeField] protected float timeBetweenShots;
    [SerializeField] protected float walkingRecoil;
    [SerializeField] protected bool isLaserActive;
    [SerializeField] protected float magazineMaxCapacity;
    [SerializeField] protected float magazineCurrentCapacity;
    [SerializeField] protected float reloadDuration;
    [SerializeField] protected bool isReloading;
    [SerializeField] protected Sprite sprite;

    protected Bullet bullet;
    protected Laser laser;
    protected float reloadTimer ;

    private void Awake()
    {
        bullet = bulletPrefab.GetComponent<Bullet>();
        magazineCurrentCapacity = magazineMaxCapacity;
        laser = GetComponentInChildren<Laser>();

    }
    public Vector3 GetFirePoint()
    {
        return firePoint.position;
    }
    virtual public bool Fire() { return true; }
    virtual public bool StartReloading(){ return true; }
    virtual public void EndReloading(){}
    public void IncreaseMagCapacity(float increaseAmount)
    {
        magazineMaxCapacity += increaseAmount;
    }
    public void CancelReloading()
    {
        isReloading = false;
        reloadTimer = 0;
    }
    public bool IsMagazineFull()
    {
        return magazineCurrentCapacity == magazineMaxCapacity;
    }
    public bool IsMagazineEmpty()
    {
        return magazineCurrentCapacity == 0;
    }
    public void ActivateLaser()
    {
        isLaserActive = true;
    }
    public void DeactivateLaser()
    {
        isLaserActive = false;
    }
    public bool IsLaserActive()
    {
        return isLaserActive;
    }

    public Sprite GetWeaponSprite()
    {
        return sprite; 
    }
    public float CurrentMagazineCapacity()
    {
        return magazineCurrentCapacity;
    }
    public float MaxMagazineCapacity()
    {
        return magazineMaxCapacity ;
    }
}
