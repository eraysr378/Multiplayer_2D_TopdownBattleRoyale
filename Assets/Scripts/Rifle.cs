using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : Weapon
{
    bool fire = true;
    float time = 0;
    override public void Fire()
    {
      
        if(fire)
        {
            GameObject projectileBullet = Instantiate(bulletPrefab, player.transform.position,Quaternion.identity);
            projectileBullet.transform.Rotate(player.transform.eulerAngles);
            projectileBullet.GetComponent<Rigidbody2D>().AddForce(projectileBullet.transform.up * bulletSpeed, ForceMode2D.Impulse);
            Debug.Log(player.transform.eulerAngles);
            Debug.Log(projectileBullet.transform.eulerAngles);
            fire = false;
        }


    }
    private void Update()
    {
        time += Time.deltaTime;
        if (time > 1)
        {
            fire = true;
            time = 0;
        }
    }
}
