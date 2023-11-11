using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected Player player;
    [SerializeField] protected float bulletSpeed;
    virtual public void Fire() { }
   
}
