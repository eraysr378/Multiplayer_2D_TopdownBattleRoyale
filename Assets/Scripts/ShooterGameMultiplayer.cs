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
 
    public void SpawnBullet(Player ownerPlayer, GameObject bulletPrefab, float bulletRotationAngle, Transform firePoint)
    {
        int index = -1;
        for (int i = 0; i < bulletPrefabsSO.bulletPrefabList.Count; i++)
        {
            if (bulletPrefabsSO.bulletPrefabList[i] == bulletPrefab)
            {
                index = i; break;
            }
        }

        SpawnBulletServerRpc(ownerPlayer.GetNetworkObject(), index, bulletRotationAngle, firePoint.position.x, firePoint.position.y);
    }
    [ServerRpc(RequireOwnership = false)]
    private void SpawnBulletServerRpc(NetworkObjectReference ownerPlayerNetworkObjectReference, int bulletPrefabIndex, float bulletRotationAngle, float firePointX, float firePointY)
    {
        ownerPlayerNetworkObjectReference.TryGet(out NetworkObject ownerPlayerNetworkObject);
        Player ownerPlayer = ownerPlayerNetworkObject.GetComponent<Player>();

        GameObject bulletPrefab = bulletPrefabsSO.bulletPrefabList[bulletPrefabIndex];
        Vector3 firePoint = new Vector3(firePointX, firePointY, 0);
        GameObject projectileBullet = Instantiate(bulletPrefab, firePoint, Quaternion.identity);
        projectileBullet.GetComponent<Bullet>().SetOwner(ownerPlayer);
        NetworkObject projectileBulletNetworkObject = projectileBullet.GetComponent<NetworkObject>();
        projectileBulletNetworkObject.Spawn(true);

        projectileBulletNetworkObject.GetComponent<Bullet>().FireBullet(bulletRotationAngle);



    }
    public void SpawnBulletShell(GameObject bulletShellPrefab, float bulletRotationAngle,Transform bulletShellSpawnPoint)
    {
        int index = -1;
        for (int i = 0; i < bulletPrefabsSO.bulletShellPrefabList.Count; i++)
        {
            if (bulletPrefabsSO.bulletShellPrefabList[i] == bulletShellPrefab)
            {
                index = i; break;
            }
        }

        SpawnBulletShellServerRpc( index, bulletRotationAngle, bulletShellSpawnPoint.position.x, bulletShellSpawnPoint.position.y);
    }
    [ServerRpc(RequireOwnership = false)]
    private void SpawnBulletShellServerRpc(int bulletShellPrefabIndex, float bulletRotationAngle, float bulletShellSpawnPointX, float bulletShellSpawnPointY)
    {

        GameObject bulletShellPrefab = bulletPrefabsSO.bulletShellPrefabList[bulletShellPrefabIndex];
        Vector3 bulletShellSpawnPoint = new Vector3(bulletShellSpawnPointX, bulletShellSpawnPointY, 0);
        GameObject projectileBulletShell = Instantiate(bulletShellPrefab, bulletShellSpawnPoint, Quaternion.identity);
        NetworkObject projectileBulletShellNetworkObject = projectileBulletShell.GetComponent<NetworkObject>();
        projectileBulletShellNetworkObject.Spawn(true);

        projectileBulletShellNetworkObject.GetComponent<BulletShell>().FireBulletShell(bulletRotationAngle);


    }
    public void DestroyBullet(Bullet bullet)
    {
        DestroyBulletServerRpc(bullet.GetNetworkObject());
    }
    [ServerRpc(RequireOwnership = false)]
    public void DestroyBulletServerRpc(NetworkObjectReference bulletNetworkObjectReference)
    {
        bulletNetworkObjectReference.TryGet(out NetworkObject bulletNetworkObject);
        if(bulletNetworkObject != null)
        {
            Bullet bullet = bulletNetworkObject.GetComponent<Bullet>();
            if(bullet != null)
            {
                bullet.DestroySelf();
            }
            else
            {
                Debug.Log("Bullet does not exist");
            }
        }
        else
        {
            Debug.Log("Bullet network object does not exist");

        }

    }
    public void DestroyBulletShell(BulletShell bulletShell)
    {
        DestroyBulletShellServerRpc(bulletShell.GetNetworkObject());

    }
    [ServerRpc(RequireOwnership =false)]
    public void DestroyBulletShellServerRpc(NetworkObjectReference bulletShellNetworkObjectReference)
    {
        bulletShellNetworkObjectReference.TryGet(out NetworkObject bulletShellNetworkObject);
        if (bulletShellNetworkObject != null)
        {
            BulletShell bulletShell = bulletShellNetworkObject.GetComponent<BulletShell>();
            if (bulletShell != null)
            {
                bulletShell.DestroySelf();
            }
            else
            {
                Debug.Log("BulletShell does not exist");
            }
        }
        else
        {
            Debug.Log("BulletShell network object does not exist");

        }
    }


}
