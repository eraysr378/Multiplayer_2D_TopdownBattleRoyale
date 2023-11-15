using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WeaponController : NetworkBehaviour
{
    [SerializeField] private Weapon weapon;
    [SerializeField] private Player player;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        if (Input.GetMouseButton(0))
        {
            weapon.Fire();
        }
    }
}
