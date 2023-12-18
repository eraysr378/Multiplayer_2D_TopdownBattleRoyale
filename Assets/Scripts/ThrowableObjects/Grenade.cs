using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Grenade : ThrowableObject
{
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float damage;
    [SerializeField] private float explosionRadius;
    [SerializeField] private float rotateSpeed = 1;
    private void FixedUpdate()
    {
        transform.Rotate(0, 0, rotateSpeed);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsServer)
        {
            if (collision.GetComponent<Player>() != null && collision.GetComponent<Player>() == GetOwner())
                return;

            RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, explosionRadius, Vector3.zero);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.transform.gameObject.GetComponent<Player>() != null)
                {
                    hit.transform.gameObject.GetComponent<Player>().TakeDamage(damage);
                }
            }

            NetworkObject explosionNetworkObject = Instantiate(explosionPrefab, transform.position, Quaternion.identity).GetComponent<NetworkObject>();
            explosionNetworkObject.Spawn(true);

            Destroy(gameObject);
        }

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color32(0, 255, 0, 100);
        Gizmos.DrawSphere(transform.position, explosionRadius);
    }
}
