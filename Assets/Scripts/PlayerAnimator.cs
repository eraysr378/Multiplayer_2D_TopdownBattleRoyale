using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour
{
    private const string IS_WALKING = "IsWalking";
    private const string SELECT_PISTOL = "SelectPistol";
    private const string SELECT_RIFLE = "SelectRifle";
    private const string SELECT_SHOTGUN = "SelectShotgun";

    [SerializeField] private Player player;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();


    }
    private void Start()
    {
        player.GetWeaponController().OnWeaponChanged += WeaponController_OnWeaponChanged;
    }

    private void WeaponController_OnWeaponChanged(object sender, System.EventArgs e)
    {
        if (player.GetCurrentWeapon() is Pistol)
        {
            animator.SetTrigger(SELECT_PISTOL);
        }
        else if (player.GetCurrentWeapon() is Rifle)
        {
            animator.SetTrigger(SELECT_RIFLE);

        }
        else if (player.GetCurrentWeapon() is Shotgun)
        {
            animator.SetTrigger(SELECT_SHOTGUN);

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        animator.SetBool(IS_WALKING, player.IsWalking());
    }
}
