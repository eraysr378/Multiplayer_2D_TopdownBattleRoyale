using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AbilityBox : Box
{
    [SerializeField] private AbilityOnGroundSO abilityOnGroundSO;
  
    public override ObjectOnGround DropObjectOnGround()
    {
        int random = Random.Range(0, 100);
        if (random < 30)
        {
            return abilityOnGroundSO.dash;
        }
        else if (random < 60)
        {
            return abilityOnGroundSO.pistolLaser;
        }
        else if (random < 90)
        {
            return abilityOnGroundSO.rifleLaser;
        }
        else
        {
            return null;
        }
    }
}