using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WeaponBox : Box
{
    [SerializeField] private WeaponOnGroundSO weaponOnGroundSO;

    public override ObjectOnGround DropObjectOnGround()
    {
        int random = UnityEngine.Random.Range(0, 100);
        if (random < 30)
        {
            return weaponOnGroundSO.pistol;
        }
        else if (random < 60)
        {
            return weaponOnGroundSO.rifle;
        }
        else if (random < 90)
        {
            return weaponOnGroundSO.shotgun;
        }
        else
        {
            return null;
        }
    }

}
