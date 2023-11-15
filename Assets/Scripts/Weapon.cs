using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected Player player;
    [SerializeField] protected Transform firePoint;
    [SerializeField] protected GameObject spentBulletPrefab;
    [SerializeField] protected float spentBulletSpeed;
    [SerializeField] protected float spentBulletLifetime;

    protected Bullet bullet;

    virtual public void Fire() { }
    private void Awake()
    {
        bullet = bulletPrefab.GetComponent<Bullet>();
    }
}
