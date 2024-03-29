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
        SetTriggerServerRpc(THROW);

    }

    private void Player_OnAnyShotgunReload(object sender, System.EventArgs e)
    {
        SetTriggerServerRpc(RELOAD_SHOTGUN);
    }

    private void Player_OnAnyRifleReload(object sender, System.EventArgs e)
    {
        SetTriggerServerRpc(RELOAD_RIFLE);
    }

    private void Player_OnAnyPistolReload(object sender, System.EventArgs e)
    {
        SetTriggerServerRpc(RELOAD_PISTOL);
    }


    private void WeaponController_OnWeaponChanged(object sender, System.EventArgs e)
    {

        if (player.GetCurrentWeapon() is Pistol)
        {
            SetTriggerServerRpc(SELECT_PISTOL);

        }
        else if (player.GetCurrentWeapon() is Rifle)
        {
            SetTriggerServerRpc(SELECT_RIFLE);
        }
        else if (player.GetCurrentWeapon() is Shotgun)
        {
            SetTriggerServerRpc(SELECT_SHOTGUN);
        }
        else if (player.GetCurrentWeapon() is Knife)
        {
            SetTriggerServerRpc(SELECT_KNIFE);
        }
    }

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
        SetTriggerServerRpc(KNIFE_ATTACK); 

    }

    [ServerRpc(RequireOwnership =false)]
    private void SetTriggerServerRpc(string trigger)
    {
        SetTriggerClientRpc(trigger);
    }
    [ClientRpc]
    private void SetTriggerClientRpc(string trigger)
    {
        animator.SetTrigger(trigger);
    }
}
