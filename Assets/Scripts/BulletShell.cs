using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletShell : NetworkBehaviour
{

    private NetworkObject bulletShellNetworkObject;
    [SerializeField] private float lifeTime;
    [SerializeField] private float bulletShellSpeed;
    private Vector3 moveDir;
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

        moveDir = -transform.up;
        bulletShellNetworkObject.transform.position += moveDir * bulletShellSpeed * Time.deltaTime;


    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
    public static void DestroyBullletShell(BulletShell bulletShell)
    {
        ShooterGameMultiplayer.Instance.DestroyBulletShell(bulletShell);
    }
    public void FireBulletShell(float bulletShellRotationAngle)
    {
        FireBulletShellServerRpc(NetworkObject, bulletShellRotationAngle);
    }
    [ServerRpc(RequireOwnership = false)]
    public void FireBulletShellServerRpc(NetworkObjectReference bulletShellNetworkObjectReference, float bulletShellRotationAngle)
    {
        FireBulletShellClientRpc(bulletShellNetworkObjectReference, bulletShellRotationAngle);
    }
    [ClientRpc]
    public void FireBulletShellClientRpc(NetworkObjectReference bulletShellNetworkObjectReference, float bulletShellRotationAngle)
    {
        bulletShellNetworkObjectReference.TryGet(out bulletShellNetworkObject);
        bulletShellNetworkObject.transform.eulerAngles = new Vector3(0, 0, bulletShellRotationAngle);
    }
    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
}
