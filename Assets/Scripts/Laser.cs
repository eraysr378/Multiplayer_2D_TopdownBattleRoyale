using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private float maxDistanceRay = 100f;
    [SerializeField] Transform laserFirePoint;
    [SerializeField] LineRenderer lineRenderer;
    private Transform m_transform;

    private void Awake()
    {
        m_transform = GetComponent<Transform>();

    }
    private void Update()
    {
        ShootLaser();
    }
    void ShootLaser()
    {
        if (Physics2D.Raycast(transform.position, transform.right, maxDistanceRay))
        {
            RaycastHit2D hit = Physics2D.Raycast(laserFirePoint.position, transform.right);
            Draw2DRay(laserFirePoint.position,hit.point);
        }
        else
        {
            Draw2DRay(laserFirePoint.position, laserFirePoint.transform.right *maxDistanceRay);

        }
    }
    void Draw2DRay(Vector2 startPos,Vector2 endPos)
    {
        lineRenderer.SetPosition(0,startPos);
        lineRenderer.SetPosition(1,endPos);
    }



}
