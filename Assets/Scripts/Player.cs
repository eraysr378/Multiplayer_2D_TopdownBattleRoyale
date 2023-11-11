using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 7f;

    private bool isWalking;

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        HandleMovement();
    }

    private void HandleMovement()
    {

        Vector2 inputVector = new Vector2(0, 0);
        if (Input.GetKey(KeyCode.W))
        {
            inputVector.y += 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputVector.y -= 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputVector.x += 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputVector.x -= 1;
        }
        inputVector = inputVector.normalized;
        Vector3 moveDir = new Vector3(inputVector.x, inputVector.y, 0);

        isWalking = moveDir != Vector3.zero;

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = 1f;
        bool canMove = !Physics2D.CircleCast(transform.position, playerRadius, moveDir, moveDistance);
        if (canMove)
        {
            transform.position += moveDir * moveDistance;
        }

        
        transform.eulerAngles = GetAimDirection();
    }
    public bool IsWalking()
    {
        return isWalking;
    }
    public Vector3 GetAimDirection()
    {
        // make player look at mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 dirToCrosshair = (ray.origin - transform.position).normalized;
        float angle = Mathf.Atan2(dirToCrosshair.y, dirToCrosshair.x);
        return new Vector3(0, 0, angle * Mathf.Rad2Deg);
    }
}
