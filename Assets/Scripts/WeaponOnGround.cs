using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering.Universal;

public class WeaponOnGround : ObjectOnGround
{

    [SerializeField] private WeaponType weaponType;

    public WeaponType GetWeaponType()
    {
        return weaponType;
    }
 


}
