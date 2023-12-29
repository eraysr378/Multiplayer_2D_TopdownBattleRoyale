using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Bullet : NetworkBehaviour
{

    [SerializeField] private float bulletRadius;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float bulletDamage;
    [SerializeField] private float lifeTime;

    private Player owner;
    private Vector3 moveDir;
    private NetworkObject bulletNetworkObject;
    private float timer;

    private void Awake()
    {
    }
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

        moveDir = transform.right;
        float moveDistance = bulletSpeed * Time.deltaTime;
        RaycastHit2D[] hitArray = Physics2D.CircleCastAll(transform.position, bulletRadius, moveDir, moveDistance);
        foreach (RaycastHit2D hit in hitArray)
        {
            // If bullet hits a box, destroy the bullet
            if (hit.collider.GetComponent<Box>())
            {
                Debug.Log(hit.collider.name + " hit!");

                Destroy(gameObject);

            }
        }
        // if bullet does not hit anywhere then make it keep moving
        bulletNetworkObject.transform.position += moveDir * moveDistance;


    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
    public static void DestroyBulllet(Bullet bullet)
    {
        ShooterGameMultiplayer.Instance.DestroyBullet(bullet);
    }
    public void FireBullet( float bulletRotationAngle)
    {
        FireBulletServerRpc(NetworkObject, bulletRotationAngle);
    }
    [ServerRpc(RequireOwnership = false)]
    public void FireBulletServerRpc(NetworkObjectReference bulletNetworkObjectReference, float bulletRotationAngle)
    {
        FireBulletClientRpc(bulletNetworkObjectReference, bulletRotationAngle);
    }
    [ClientRpc]
    public void FireBulletClientRpc(NetworkObjectReference bulletNetworkObjectReference, float bulletRotationAngle)
    {
        bulletNetworkObjectReference.TryGet(out bulletNetworkObject);
        bulletNetworkObject.transform.eulerAngles = new Vector3(0, 0, bulletRotationAngle);
    }
    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, moveDir);
        Gizmos.DrawSphere(transform.position, bulletRadius);
    }
    // owner is needed to make sure players are not able to hit themselves
    public void SetOwner(Player player)
    {
        owner = player;
    }
    public Player GetOwner()
    {
        return owner;
    }

}
