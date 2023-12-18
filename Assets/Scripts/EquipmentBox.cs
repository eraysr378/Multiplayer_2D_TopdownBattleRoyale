using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EquipmentBox : Box
{
    [SerializeField] private EquipmentOnGroundSO equipmentOnGroundSO;

    public override ObjectOnGround DropObjectOnGround()
    {
        int random = Random.Range(0, 100);
        if (random < 20)
        {
            return equipmentOnGroundSO.vest;
        }
        else if (random < 40)
        {
            return equipmentOnGroundSO.boots;
        }
        else if (random < 60)
        {
            return equipmentOnGroundSO.pistolMag;
        }
        else if (random < 80)
        {
            return equipmentOnGroundSO.rifleMag;
        }else if(random < 100)
        {
            return equipmentOnGroundSO.grenade;
        }
        return null;
    }

}
