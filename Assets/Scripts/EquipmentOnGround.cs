using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class EquipmentOnGround : ObjectOnGround
{
    [SerializeField] private Equipment equipment;

    public Equipment GetEquipment()
    {
        return equipment;
    }


}
