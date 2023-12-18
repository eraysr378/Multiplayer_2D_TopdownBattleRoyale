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
    private const string SELECT_KNIFE = "SelectKnife";
    private const string KNIFE_ATTACK = "KnifeAttack";
    private const string KNIFE_ATTACK2 = "KnifeAttack2";
    private const string RELOAD_PISTOL = "ReloadPistol";
    private const string RELOAD_RIFLE = "ReloadRifle";
    private const string RELOAD_SHOTGUN = "ReloadShotgun";
    private const string THROW = "Throw";


    [SerializeField] private Player player;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();


    }
    private void Start()
    {
        player.GetWeaponController().OnWeaponChanged += WeaponController_OnWeaponChanged;
        player.OnPistolReload += Player_OnAnyPistolReload;
        player.OnRifleReload += Player_OnAnyRifleReload;
        player.OnShotgunReload += Player_OnAnyShotgunReload;
        player.OnObjectThrow += Player_OnObjectThrow;
    }

    private void Player_OnObjectThrow(object sender, System.EventArgs e)
    {
        animator.SetTrigger(THROW);

    }

    private void Player_OnAnyShotgunReload(object sender, System.EventArgs e)
    {
        animator.SetTrigger(RELOAD_SHOTGUN);
    }

    private void Player_OnAnyRifleReload(object sender, System.EventArgs e)
    {
        animator.SetTrigger(RELOAD_RIFLE);
    }

    private void Player_OnAnyPistolReload(object sender, System.EventArgs e)
    {
        animator.SetTrigger(RELOAD_PISTOL);
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
        else if (player.GetCurrentWeapon() is Knife)
        {
            animator.SetTrigger(SELECT_KNIFE);
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
    public void PlayKnifeAttackAnimtion()
    {
        if (Random.Range(0, 2) == 0)
        {
            animator.SetTrigger(KNIFE_ATTACK);
        }
        else
        {
            animator.SetTrigger(KNIFE_ATTACK2);
        }
    }
}
