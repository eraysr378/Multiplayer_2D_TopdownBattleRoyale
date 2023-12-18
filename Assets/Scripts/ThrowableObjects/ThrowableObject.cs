using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ThrowableObject : NetworkBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private float throwableObjectSpeed;
    [SerializeField] private float lifeTime;

    private Player owner;
    private Vector3 moveDir;
    private NetworkObject throwableObjectNetworkObject;
    private float timer;
    private void Update()
    {
        if (IsServer)
        {
            if (timer > lifeTime)
            {
                Destroy(gameObject);
                timer = -100; // to not try to destroy again
            }
            timer += Time.deltaTime;
        }

        float moveDistance = throwableObjectSpeed * Time.deltaTime;

        // if bullet does not hit anywhere then make it keep moving
        throwableObjectNetworkObject.transform.position += moveDir * moveDistance;
    }

 
    public void ThrowGrenade(Vector3 moveDir)
    {
        ThrowGrenadeServerRpc(NetworkObject,moveDir.x,moveDir.y);
    }
    [ServerRpc(RequireOwnership = false)]
    public void ThrowGrenadeServerRpc(NetworkObjectReference throwableObjecttNetworkObjectReference, float moveDirX, float moveDirY)
    {
        ThrowGrenadeClientRpc(throwableObjecttNetworkObjectReference,  moveDirX,  moveDirY);
    }
    [ClientRpc]
    public void ThrowGrenadeClientRpc(NetworkObjectReference throwableObjecttNetworkObjectReference, float moveDirX, float moveDirY)
    {
        throwableObjecttNetworkObjectReference.TryGet(out throwableObjectNetworkObject);
        moveDir = new Vector3(moveDirX, moveDirY,0);
    }
    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
    public GameObject GetPrefab()
    {
        return prefab;
    }
    public Player GetOwner()
    {
        return owner;
    }
    public void SetOwner(Player player)
    {
        owner = player;
    }
}
