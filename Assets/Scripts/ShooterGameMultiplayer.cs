using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ShooterGameMultiplayer : NetworkBehaviour
{
    public static ShooterGameMultiplayer Instance { get; private set; }
    [SerializeField] private BulletPrefabsSO bulletPrefabsSO;
    private void Awake()
    {
        Instance = this;
    }
    public void SpawnBullet(Player ownerPlayer, GameObject bulletPrefab, float playerRotationZ, float aimDirX, float aimDirY, Transform firePoint)
    {
        int index = -1;
        for (int i = 0; i < bulletPrefabsSO.bulletPrefabList.Count; i++)
        {
            if (bulletPrefabsSO.bulletPrefabList[i] == bulletPrefab)
            {
                index = i; break;
            }
        }

        SpawnBulletServerRpc(ownerPlayer.GetNetworkObject(), index, playerRotationZ, aimDirX, aimDirY, firePoint.position.x, firePoint.position.y);
    }
    [ServerRpc(RequireOwnership = false)]
    private void SpawnBulletServerRpc(NetworkObjectReference ownerPlayerNetworkObjectReference, int bulletPrefabIndex, float playerRotationZ, float aimDirX, float aimDirY, float firePointX, float firePointY)
    {
        ownerPlayerNetworkObjectReference.TryGet(out NetworkObject ownerPlayerNetworkObject);
        Player ownerPlayer = ownerPlayerNetworkObject.GetComponent<Player>();

        GameObject bulletPrefab = bulletPrefabsSO.bulletPrefabList[bulletPrefabIndex];

        Vector3 firePoint = new Vector3(firePointX, firePointY, 0);

        GameObject projectileBullet = Instantiate(bulletPrefab, firePoint, Quaternion.identity);
        projectileBullet.GetComponent<Bullet>().SetOwner(ownerPlayer);
        NetworkObject projectileBulletNetworkObject = projectileBullet.GetComponent<NetworkObject>();
        projectileBulletNetworkObject.Spawn(true);

        Vector2 aimDir = new Vector2(aimDirX, aimDirY);
        projectileBulletNetworkObject.GetComponent<Bullet>().FireBullet(aimDir.normalized, playerRotationZ);

    }
    public void DestroyBullet(Bullet bullet)
    {
        DestroyBulletServerRpc(bullet.GetNetworkObject());
    }
    [ServerRpc(RequireOwnership = false)]
    public void DestroyBulletServerRpc(NetworkObjectReference bulletNetworkObjectReference)
    {
        bulletNetworkObjectReference.TryGet(out NetworkObject bulletNetworkObject);
        Bullet bullet = bulletNetworkObject.GetComponent<Bullet>();
        bullet.DestroySelf();
    }


}
