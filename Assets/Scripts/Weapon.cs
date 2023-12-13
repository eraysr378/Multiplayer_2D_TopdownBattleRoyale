using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected GameObject bulletShellPrefab;
    [SerializeField] protected Player player;
    [SerializeField] protected Transform firePoint;
    [SerializeField] protected Transform bulletShellSpawnPoint;
    [SerializeField] protected float timeBetweenShots;
    [SerializeField] protected float walkingRecoil;

    protected Bullet bullet;

    virtual public bool Fire() { return false; }
    private void Awake()
    {
        bullet = bulletPrefab.GetComponent<Bullet>();

    }

}

