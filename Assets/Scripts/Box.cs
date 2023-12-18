using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Box : NetworkBehaviour
{
    [SerializeField] private float maxHealth;
    private NetworkVariable<float> currentHealth = new NetworkVariable<float>();
    [SerializeField] private ObjectOnGround droppedObject;

    public override void OnNetworkSpawn()
    {
        maxHealth = 100;
        currentHealth.Value = maxHealth;
        if (droppedObject == null)
        {
            droppedObject = DropObjectOnGround();
        }

    }
    public void TakeDamage(float damage)
    {
        TakeDamageServerRpc(damage);
    }
    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float damage)
    {
        currentHealth.Value -= damage;
        if (currentHealth.Value <= 0)
        {
            if (droppedObject != null)
            {
                SpawnDroppedObject();
            }
            DestroySelfServerRpc();
        }
    }
    public virtual void SpawnDroppedObject()
    {
        if (droppedObject != null)
        {
            ObjectOnGround objectOnGround = Instantiate(droppedObject, transform.position, Quaternion.identity);
            NetworkObject objectOnGroundNetworkObject = objectOnGround.GetComponent<NetworkObject>();
            objectOnGroundNetworkObject.Spawn(true);
        }
    }
    public virtual ObjectOnGround DropObjectOnGround()
    {
        return null;
    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
    [ServerRpc(RequireOwnership = false)]
    public void DestroySelfServerRpc()
    {
        DestroySelfClientRpc();
    }
    [ClientRpc]
    public void DestroySelfClientRpc()
    {
        Destroy(gameObject);
    }
    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
}
