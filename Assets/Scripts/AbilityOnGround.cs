using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AbilityOnGround : ObjectOnGround
{
    [SerializeField] private Ability ability;

    public Ability GetAbility()
    {
        return ability;
    }
  

}
