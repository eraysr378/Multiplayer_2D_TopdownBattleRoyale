using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
public enum Ability
{
    Dash,
    PistolLaser,
    RifleLaser,
    None
}
public class AbilitySystem : NetworkBehaviour
{
    public event EventHandler OnDashUnlocked;
    public event EventHandler OnPistolLaserUnlocked;
    public event EventHandler OnRifleLaserUnlocked;
    [SerializeField] private Player player;
    [SerializeField] private float dashCooldown;
    [SerializeField] private float dashDuration;
    [SerializeField] private GameObject dashEffect;
    private List<Ability> unlockedAbilities = new List<Ability>();
    private float dashSpeed;
    bool isDashCooldown;
    bool isDashStarted;

    private float timer;
    private float dashCooldownTimer;
    void Start()
    {
    }

    void Update()
    {
        if (isDashStarted)
        {
            timer += Time.deltaTime;
        }
        if (timer > dashDuration)
        {
            DashEnd();
            timer = 0;
        }

        if (isDashCooldown)
        {
            dashCooldownTimer += Time.deltaTime;
            if (dashCooldownTimer > dashCooldown)
            {
                isDashCooldown = false;
            }
        }

    }
    public void AbilityDash()
    {
        if (!isDashCooldown && !isDashStarted && player.IsWalking())
        {
            isDashCooldown = true;
            dashCooldownTimer = 0;
            timer = 0;
            DashStart();
            ActivateDashEffectServerRpc(player.GetMovingAngle());


        }
    }
    [ServerRpc(RequireOwnership =false)]
    private void ActivateDashEffectServerRpc(float angle)
    {
        ActivateDashEffectClientRpc(angle);
    }
    [ClientRpc]
    private void ActivateDashEffectClientRpc(float angle)
    {
        dashEffect.transform.eulerAngles = new Vector3(angle, -90, 90);
        dashEffect.SetActive(true);
    }
    private void DashStart()
    {
        isDashStarted = true;
        dashSpeed = player.GetSpeed() * 2.5f;
        player.GetRigidBody().velocity = player.GetMoveDirectionNormalized() * dashSpeed;
    }
    private void DashEnd()
    {
        isDashStarted = false;
        player.GetRigidBody().velocity = Vector2.zero;
        player.GetRigidBody().angularVelocity = 0;
    }

    public bool IsPistolLaserUnlocked()
    {
        return unlockedAbilities.Contains(Ability.PistolLaser);
    }
    public bool IsRifleLaserUnlocked()
    {
        return unlockedAbilities.Contains(Ability.RifleLaser);
    }
    public bool IsDashUnlocked()
    {
        return unlockedAbilities.Contains(Ability.Dash);
    }

    public bool UnlockAbility(Ability ability)
    {
        if (!unlockedAbilities.Contains(ability))
        {
            unlockedAbilities.Add(ability);
            switch (ability)
            {
                case Ability.Dash:
                    OnDashUnlocked?.Invoke(this, EventArgs.Empty);
                    break;
                case Ability.PistolLaser:
                    OnPistolLaserUnlocked?.Invoke(this, EventArgs.Empty);
                    break;
                case Ability.RifleLaser:
                    OnRifleLaserUnlocked?.Invoke(this, EventArgs.Empty);
                    break;

                default:
                    break;
            }
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool IsDashCooldown()
    {
        return isDashCooldown;
    }

}
