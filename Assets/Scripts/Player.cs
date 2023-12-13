using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class Player : NetworkBehaviour
{
    public static event EventHandler OnAnyPlayerSpawned;
    public static event EventHandler OnAnyRifleFired;
    public static event EventHandler OnAnyShotgunFired;
    public static event EventHandler OnAnyPistolFired;
    public static void ResetStaticData()
    {
        OnAnyPlayerSpawned = null;
        OnAnyPistolFired = null;
        OnAnyRifleFired = null;
        OnAnyShotgunFired = null;
    }
    public static Player LocalInstance { get; private set; }

    public event EventHandler<OnLocalPlayerDiedEventArgs> OnLocalPlayerDied;
    public class OnLocalPlayerDiedEventArgs : EventArgs
    {
        public Player player;
    }

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private List<Vector3> spawnPositionsList;
    [SerializeField] private float killScore;

    private WeaponController weaponController;
    private PlayerHealthSystem playerHealthSystem;
    private bool isWalking;






    private void Awake()
    {



    }
    private void Start()
    {
    }

    private void PlayerHealthSystem_OnPlayerDied(object sender, System.EventArgs e)
    {
        OnLocalPlayerDied?.Invoke(this, new OnLocalPlayerDiedEventArgs { player = this });
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        HandleMovement();
        HandleShooting();
        if (Input.GetKeyDown(KeyCode.B))
        {
            GetComponent<PlayerHealthSystem>().TakeDamageServerRpc(10);
            Debug.Log("player get hit, health: " + playerHealthSystem.GetCurrentHealth().Value);
        }
    }
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }
        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);

        playerHealthSystem = GetComponentInChildren<PlayerHealthSystem>();
        weaponController = GetComponentInChildren<WeaponController>();
        playerHealthSystem.OnPlayerDied += PlayerHealthSystem_OnPlayerDied;

        transform.position = spawnPositionsList[(int)OwnerClientId]; // this will change when lobby is added

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer)
            return;
        if (collision.GetComponent<Bullet>() && collision.GetComponent<Bullet>().GetOwner() != this)
        {
            GetComponent<PlayerHealthSystem>().TakeDamageServerRpc(10);
            Bullet.DestroyBulllet(collision.GetComponent<Bullet>());
        }


    }
    private void HandleShooting()
    {
        if (Input.GetMouseButton(0))
        {
            if (weaponController.Fire())
            {
                if (GetCurrentWeapon() is Pistol)
                {
                    AnyPistolFiredServerRpc();
                }
                else if (GetCurrentWeapon() is Rifle)
                {
                    AnyRifleFiredServerRpc();
                }
                else if (GetCurrentWeapon() is Shotgun)
                {
                    AnyShotgunFiredServerRpc();
                }
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void AnyShotgunFiredServerRpc()
    {
        AnyShotgunFiredClientRpc();

    }
    [ClientRpc]
    private void AnyShotgunFiredClientRpc()
    {
        OnAnyShotgunFired?.Invoke(this, EventArgs.Empty);
    }
    [ServerRpc(RequireOwnership = false)]
    private void AnyRifleFiredServerRpc()
    {
        AnyRifleFiredClientRpc();

    }
    [ClientRpc]
    private void AnyRifleFiredClientRpc()
    {
        OnAnyRifleFired?.Invoke(this, EventArgs.Empty);
    }
    [ServerRpc(RequireOwnership = false)]
    private void AnyPistolFiredServerRpc()
    {
        AnyPistolFiredClientRpc();

    }
    [ClientRpc]
    private void AnyPistolFiredClientRpc()
    {
        OnAnyPistolFired?.Invoke(this, EventArgs.Empty);
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
        //float playerRadius = 1f;
        //bool canMove = !Physics2D.CircleCast(transform.position, playerRadius, moveDir, moveDistance);
        //if (canMove)
        //{
        transform.position += moveDir * moveDistance;
        //}


        transform.eulerAngles = GetAimAngle();
    }
    public bool IsWalking()
    {
        return isWalking;
    }
    public Vector3 GetAimDirectionNormalized()
    {
        // make player look at mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return (ray.origin - transform.position).normalized;

    }
    public Vector3 GetAimAngle()
    {
        Vector3 dirToCrosshair = GetAimDirectionNormalized();
        float angle = Mathf.Atan2(dirToCrosshair.y, dirToCrosshair.x);
        return new Vector3(0, 0, angle * Mathf.Rad2Deg);
    }
    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }


    public PlayerHealthSystem GetHealthSystem()
    {
        return playerHealthSystem;
    }
    public Vector3 GetSpawnPosition()
    {
        return spawnPositionsList[(int)OwnerClientId];
    }
    public float GetKillScore()
    {
        return killScore;
    }
    public Weapon GetCurrentWeapon()
    {
        return weaponController.GetCurrentWeapon();
    }
    public WeaponController GetWeaponController()
    {
        return weaponController;
    }
  

}


