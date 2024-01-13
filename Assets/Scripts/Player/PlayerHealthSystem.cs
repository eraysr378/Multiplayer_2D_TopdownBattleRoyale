using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerHealthSystem : NetworkBehaviour
{
    public event EventHandler OnPlayerDied;
    [SerializeField] private float maxHealthVal;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private float neededTimeToStartHealing;
    [SerializeField] private float healingAmount;
    [SerializeField] private float neededTimeForHealing;
    private NetworkVariable<float> currentHealth = new NetworkVariable<float>();
    private NetworkVariable<float> maxHealth = new NetworkVariable<float>();

    private float timeSinceLastDamageTaken;
    private float healingTimer;

    private void Update()
    {
        hpText.text = currentHealth.Value.ToString() + "/" + maxHealth.Value.ToString();
        if (IsOwner)
        {
            timeSinceLastDamageTaken += Time.deltaTime;
            if (currentHealth.Value < maxHealth.Value && timeSinceLastDamageTaken > neededTimeToStartHealing)
            {
                healingTimer += Time.deltaTime;
                // if player does not take any damage for some time, then the player starts healing automatically
                if (healingTimer > neededTimeForHealing)
                {
                    AddHealth(healingAmount);
                    healingTimer = 0;
                }
            }
            else
            {
                healingTimer = 0;
            }
        }
     
    }
    public override void OnNetworkSpawn()
    {
        SetHealthOnSpawnServerRpc();

        neededTimeToStartHealing = 6;
        healingAmount = 10;
        neededTimeForHealing = 2;
    }
    [ClientRpc]
    private void UpdateHealthBarClientRpc()
    {
        maxHealth.OnValueChanged?.Invoke(0, maxHealth.Value);
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetHealthOnSpawnServerRpc()
    {
        maxHealth.Value = maxHealthVal;
        currentHealth.Value = maxHealthVal;
        UpdateHealthBarClientRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float damage)
    {
        GetHitClientRpc();
        currentHealth.Value -= damage;
        if (currentHealth.Value <= 0)
        {
            PlayerDiedClientRpc();
        }
    }
    [ClientRpc]
    public void GetHitClientRpc()
    {
        timeSinceLastDamageTaken = 0;
    }
    [ClientRpc]
    public void PlayerDiedClientRpc()
    {
        OnPlayerDied?.Invoke(this, EventArgs.Empty);
    }
    public NetworkVariable<float> GetCurrentHealth()
    {
        return currentHealth;
    }
    public NetworkVariable<float> GetMaxHealth()
    {
        return maxHealth;
    }
    public void AddHealth(float healthAmount)
    {
        AddHealthServerRpc(healthAmount);
    }
    [ServerRpc(RequireOwnership = false)]
    private void AddHealthServerRpc(float healthAmount)
    {
        if (currentHealth.Value + healthAmount > maxHealth.Value)
        {
            currentHealth.Value = maxHealth.Value;
        }
        else
        {
            currentHealth.Value += healthAmount;
        }
    }
    public void IncreaseMaxHealth(float increaseAmount)
    {
        IncreaseMaxHealthServerRpc(increaseAmount);
    }
    [ServerRpc(RequireOwnership = false)]
    private void IncreaseMaxHealthServerRpc(float increaseAmount)
    {
        maxHealth.Value += increaseAmount;
    }

}
