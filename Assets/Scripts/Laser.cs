using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Laser : NetworkBehaviour
{
    [SerializeField] private float maxDistanceRay = 100f;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Vector3 direction;
    [SerializeField] Player player;
    [SerializeField] bool isLocked;
    [SerializeField] Vector3 enemyPosition;
    [SerializeField] LayerMask layer;
    private Vector3 endPoint;
    private float distance;

    private void Awake()
    {
        distance = 20;
    }
    private void Update()
    {
        if (!IsOwner) return;

        Gun playerGun = player.GetWeaponController().GetCurrentWeapon() as Gun;
        if (playerGun != null && playerGun.IsLaserActive())
        {
            direction = player.GetAimDirectionNormalized();
            endPoint = transform.position + player.GetAimDirectionNormalized() * distance;
            ShootLaserServerRpc(direction.x, direction.y, endPoint.x, endPoint.y);
        }
        else
        {
            HideLaserServerRpc();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void ShootLaserServerRpc(float directionX, float directionY, float endPointX, float endPointY)
    {
        ShootLaserClientRpc(directionX, directionY, endPointX, endPointY);
    }
    [ClientRpc]
    public void ShootLaserClientRpc(float directionX, float directionY, float endPointX, float endPointY)
    {
        Vector2 direction = new Vector2(directionX, directionY);
        Vector2 endPoint = new Vector2(endPointX, endPointY);

        if (Physics2D.Raycast(transform.position, direction, distance))
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, distance, layer);

            foreach (RaycastHit2D hit in hits)
            {

                if (hit.transform.gameObject.GetComponent<Player>() != null)
                {
                    isLocked = true;
                    enemyPosition = hit.collider.transform.position;
                    Draw2DRay(transform.position, hit.collider.transform.position);

                }
                else
                {
                    Draw2DRay(transform.position, hit.point);

                }
                return;

            }
        }
        isLocked = false;
        Draw2DRay(transform.position, endPoint);
    }
    [ServerRpc(RequireOwnership = false)]

    public void HideLaserServerRpc()
    {
        HideLaserClientRpc();
    }
    [ClientRpc]
    public void HideLaserClientRpc()
    {
        Draw2DRay(new Vector3(999, 999, 0), new Vector3(999, 999, 0));
    }
    void Draw2DRay(Vector2 startPos, Vector2 endPos)
    {
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }

    public bool IsLocked()
    {
        return isLocked;
    }
    public Vector3 GetEnemyPosition()
    {
        return enemyPosition;
    }



}
