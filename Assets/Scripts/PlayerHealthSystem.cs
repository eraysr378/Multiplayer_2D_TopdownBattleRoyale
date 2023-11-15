using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerHealthSystem : NetworkBehaviour
{
    public event EventHandler OnPlayerDied;
    [SerializeField] private float maxHealth;
    [SerializeField] private TextMeshProUGUI hpText;

    private NetworkVariable<float> currentHealth = new NetworkVariable<float>();


    private void Update()
    {
        hpText.text = currentHealth.Value.ToString();
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        currentHealth.Value = maxHealth;
    }
    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float damage)
    {
        currentHealth.Value -= damage;
        if(currentHealth.Value <= 0)
        {
            PlayerDiedClientRpc();
        }
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
    public float GetMaxHealth()
    {
        return maxHealth;
    }
    public void AddHealth(float healthAmount)
    {
        if(currentHealth.Value + healthAmount > maxHealth)
        {
            currentHealth.Value = maxHealth;
        }
        else
        {
            currentHealth.Value += healthAmount;
        }
    }
}
