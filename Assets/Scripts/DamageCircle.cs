
using System.Drawing;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class DamageCircle : NetworkBehaviour
{

    public static DamageCircle Instance;

    [SerializeField] private Transform targetCircleTransform;

    [SerializeField] private Transform circleTransform;
    [SerializeField] private Transform topTransform;
    [SerializeField] private Transform bottomTransform;
    [SerializeField] private Transform leftTransform;
    [SerializeField] private Transform rightTransform;

    private float circleShrinkSpeed;
    private NetworkVariable<float> shrinkTimer = new NetworkVariable<float>(1);

    private Vector3 circleSize;
    private Vector3 circlePosition;

    private Vector3 targetCircleSize;
    private Vector3 targetCirclePosition;

    private void Awake()
    {
       
    }
    public override void OnNetworkSpawn()
    {
        Instance = this;

        circleShrinkSpeed = 10f;

        SetCircleSizeServerRpc(new Vector3(0, 0), new Vector3(300, 300));

        SetTargetCircleServerRpc(new Vector3(0, 0), new Vector3(250, 250),60f);
    }

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }

        shrinkTimer.Value -= Time.deltaTime;

        if (shrinkTimer.Value < 0)
        {
            Vector3 sizeChangeVector = (targetCircleSize - circleSize).normalized;
            Vector3 newCircleSize = circleSize + sizeChangeVector * Time.deltaTime * circleShrinkSpeed;

            Vector3 circleMoveDir = (targetCirclePosition - circlePosition).normalized;
            Vector3 newCirclePosition = circlePosition + circleMoveDir * Time.deltaTime * circleShrinkSpeed;

            SetCircleSizeServerRpc(newCirclePosition, newCircleSize);

            float distanceTestAmount = .1f;
            if (Vector3.Distance(newCircleSize, targetCircleSize) < distanceTestAmount && Vector3.Distance(newCirclePosition, targetCirclePosition) < distanceTestAmount)
            {
                GenerateTargetCircle();
            }
        }
    }

    private void GenerateTargetCircle()
    {
        float shrinkSizeAmount = Random.Range(30f,50f);
        Vector3 generatedTargetCircleSize = circleSize - new Vector3(shrinkSizeAmount, shrinkSizeAmount) * 2f;

        // Set a minimum size
        if (generatedTargetCircleSize.x < 10f) generatedTargetCircleSize = Vector3.one * 10f;

        Vector3 generatedTargetCirclePosition = circlePosition +
            new Vector3(Random.Range(-shrinkSizeAmount, shrinkSizeAmount), Random.Range(-shrinkSizeAmount, shrinkSizeAmount));

        float shrinkTime = 60f;

        SetTargetCircleServerRpc(generatedTargetCirclePosition, generatedTargetCircleSize, shrinkTime);
    }

    [ServerRpc(RequireOwnership =false)]
    private void SetCircleSizeServerRpc(Vector3 position, Vector3 size)
    {
        SetCircleSizeClientRpc(position,size);
    }
    [ClientRpc]
    private void SetCircleSizeClientRpc(Vector3 position, Vector3 size)
    {
        circlePosition = position;
        circleSize = size;

        transform.position = position;

        circleTransform.localScale = size;

        topTransform.localScale = new Vector3(2000, 2000);
        topTransform.localPosition = new Vector3(0, topTransform.localScale.y * .5f + size.y * .5f);

        bottomTransform.localScale = new Vector3(2000, 2000);
        bottomTransform.localPosition = new Vector3(0, -topTransform.localScale.y * .5f - size.y * .5f);

        leftTransform.localScale = new Vector3(2000, size.y);
        leftTransform.localPosition = new Vector3(-leftTransform.localScale.x * .5f - size.x * .5f, 0f);

        rightTransform.localScale = new Vector3(2000, size.y);
        rightTransform.localPosition = new Vector3(+leftTransform.localScale.x * .5f + size.x * .5f, 0f);
    }
    [ServerRpc(RequireOwnership =false)]
    private void SetTargetCircleServerRpc(Vector3 position, Vector3 size, float shrinkTimer)
    {
        this.shrinkTimer.Value = shrinkTimer;

        targetCircleTransform.position = position;
        targetCircleTransform.localScale = size;

        targetCirclePosition = position;
        targetCircleSize = size;
    }

    private bool IsOutsideCircle(Vector3 position)
    {
        return Vector3.Distance(position, circlePosition) > circleSize.x * .5f;
    }

    public static bool IsOutsideCircle_Static(Vector3 position)
    {
        return Instance.IsOutsideCircle(position);
    }
    public float GetShrinkTimer()
    {
        return shrinkTimer.Value;
    }
}
