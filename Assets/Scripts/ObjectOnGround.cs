using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ObjectOnGround : NetworkBehaviour
{
    [SerializeField] private Light2D light2D;
    [SerializeField] private float maxIntensity;
    [SerializeField] private float minIntensity;
    [SerializeField] private float glowSpeed;
    public override void OnNetworkSpawn()
    {
        light2D = GetComponentInChildren<Light2D>();
        minIntensity = 0.5f;
        maxIntensity = 16f;
        glowSpeed = 20f;
    }
    private void Update()
    {
        if (light2D.intensity <= minIntensity)
        {
            glowSpeed = Mathf.Abs(glowSpeed);
        }
        else if (light2D.intensity >= maxIntensity)
        {
            glowSpeed = -Mathf.Abs(glowSpeed);
        }
        light2D.intensity += Time.deltaTime * glowSpeed;
    }
    public void DestroySelf()
    {
        DestroySelfServerRpc();
    }
    [ServerRpc(RequireOwnership =false)]
    private void DestroySelfServerRpc()
    {
        Destroy(gameObject);
    }
}
