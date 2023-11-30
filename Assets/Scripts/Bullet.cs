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
    private Rigidbody2D rb;
    private Vector3 moveDir;
    private NetworkObject bulletNetworkObject;
    private float timer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
       

        float moveDistance = bulletSpeed * Time.deltaTime;
        RaycastHit2D[] hitArray = Physics2D.CircleCastAll(transform.position, bulletRadius, moveDir, moveDistance);
        foreach (RaycastHit2D hit in hitArray)
        {
            // If player gets hit, then it destroys the bullet so that sometimes below code does not work
            if (!hit.collider.GetComponent<Bullet>())
            {
                Debug.Log(hit.collider.name + " hit!");

                Player player = hit.collider.GetComponent<Player>();
                if (player != null)
                {
                    //Debug.Log("This is player hp: " + player.GetCurrentHealth());
                }
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
    public void FireBullet(Vector2 bulletDirectionNormalized, float bulletAngle)
    {
        FireBulletServerRpc(NetworkObject, bulletDirectionNormalized.x, bulletDirectionNormalized.y, bulletAngle);
    }
    [ServerRpc(RequireOwnership = false)]
    public void FireBulletServerRpc(NetworkObjectReference bulletNetworkObjectReference, float bulletDirX, float bulletDirY, float bulletAngle)
    {
        FireBulletClientRpc(bulletNetworkObjectReference, bulletDirX, bulletDirY, bulletAngle);
    }
    [ClientRpc]
    public void FireBulletClientRpc(NetworkObjectReference bulletNetworkObjectReference, float bulletDirX, float bulletDirY, float bulletAngle)
    {
        bulletNetworkObjectReference.TryGet(out bulletNetworkObject);
        moveDir = new Vector3(bulletDirX, bulletDirY, 0);
        bulletNetworkObject.transform.eulerAngles = new Vector3(0, 0, bulletAngle);

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
