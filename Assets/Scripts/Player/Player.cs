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
    public static event EventHandler OnAnyRifleReload;
    public static event EventHandler OnAnyShotgunReload;
    public static event EventHandler OnAnyPistolReload;
    public event EventHandler OnRifleReload;
    public event EventHandler OnShotgunReload;
    public event EventHandler OnPistolReload;
    public event EventHandler OnObjectThrow;
    [SerializeField] private GameObject enemyMinimapIcon;
    [SerializeField] private GameObject playerMinimapIcon;


    public static void ResetStaticData()
    {
        OnAnyPlayerSpawned = null;
        OnAnyPistolFired = null;
        OnAnyRifleFired = null;
        OnAnyShotgunFired = null;
        OnAnyRifleReload = null;
        OnAnyShotgunReload = null;
        OnAnyPistolReload = null;

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
    [SerializeField] private PlayerSpeedsSO playerSpeedsSO;
    private AudioSource audioSource;
    private AbilitySystem abilitySystem;
    private PlayerAnimator playerAnimator;
    private WeaponController weaponController;
    private PlayerHealthSystem playerHealthSystem;
    private EquipmentSystem equipmentSystem;
    private bool isWalking;
    private Rigidbody2D rb;
    private Vector3 moveDir;
    private ThrowSystem throwSystem;
    private bool canShoot;
    private float circleCheckTimer;




    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
            playerMinimapIcon.SetActive(true);
        }
        else
        {
            enemyMinimapIcon.SetActive(true);
        }
        rb = GetComponent<Rigidbody2D>();
        throwSystem = GetComponentInChildren<ThrowSystem>();
        audioSource = GetComponent<AudioSource>();
        abilitySystem = GetComponentInChildren<AbilitySystem>();
        equipmentSystem = GetComponentInChildren<EquipmentSystem>();
        playerAnimator = GetComponentInChildren<PlayerAnimator>();
        playerHealthSystem = GetComponentInChildren<PlayerHealthSystem>();
        weaponController = GetComponentInChildren<WeaponController>();
        playerHealthSystem.OnPlayerDied += PlayerHealthSystem_OnPlayerDied;
        weaponController.OnWeaponChanged += WeaponController_OnWeaponChanged;

        transform.position = spawnPositionsList[ShooterGameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)]; // this will change when lobby is added
        canShoot = true;

        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;

        }

    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        if(clientId == OwnerClientId)
        {
            // if anything needs to be destroyed do here
        }
    }

    private void WeaponController_OnWeaponChanged(object sender, EventArgs e)
    {
        Weapon weapon = weaponController.GetCurrentWeapon();
        switch (weapon)
        {
            case Rifle:
                SetMoveSpeed(playerSpeedsSO.rifleSpeed);
                break;
            case Knife:
                SetMoveSpeed(playerSpeedsSO.knifeSpeed);
                break;
            case Pistol:
                SetMoveSpeed(playerSpeedsSO.pistolSpeed);
                break;
            case Shotgun:
                SetMoveSpeed(playerSpeedsSO.shotgunSpeed);
                break;
            default:
                break;
        }
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
        if (!GameManager.Instance.isGamePlaying())
        {
            return;
        }
        CheckIsInCircle();
        HandleGrenadeThrow();
        HandleMovement();
        HandleShooting();
        HandleReload();
        HandleDash();
        HandleLaserActivation();
        if (Input.GetKeyDown(KeyCode.B))
        {
            playerHealthSystem.TakeDamageServerRpc(10);

            Debug.Log("player get hit, health: " + playerHealthSystem.GetCurrentHealth().Value);
        }

    }
    // If the player is not in the safe circle zone, then it will take damage until it gets in the safe zome
    private void CheckIsInCircle()
    {
        circleCheckTimer += Time.deltaTime;
        if(circleCheckTimer > 1)
        {
            if (DamageCircle.IsOutsideCircle_Static(transform.position))
            {
                playerHealthSystem.TakeDamageServerRpc(10);
                circleCheckTimer = 0;
            }

        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsOwner)
        {
            if (collision.GetComponent<ObjectOnGround>() != null)
            {
                TakeObjectOnGround(collision.GetComponent<ObjectOnGround>());
            }
        }



        if (!IsServer)
            return;
        if (collision.GetComponent<Bullet>() && collision.GetComponent<Bullet>().GetOwner() != this)
        {
            playerHealthSystem.TakeDamageServerRpc(10);
            Bullet.DestroyBulllet(collision.GetComponent<Bullet>());
        }


    }
    // Take the object from the ground the player does not have the object ( weapon,ability or equipment)
    private void TakeObjectOnGround(ObjectOnGround objectOnGround)
    {
        if (objectOnGround.GetComponent<EquipmentOnGround>() != null)
        {
            if (equipmentSystem.EquipGiven(objectOnGround.GetComponent<EquipmentOnGround>()))
            {
                objectOnGround.DestroySelf();

            }

        }
        else if (objectOnGround.GetComponent<WeaponOnGround>() != null)
        {
            if (weaponController.UnlockWeapon(objectOnGround.GetComponent<WeaponOnGround>().GetWeaponType()))
            {
                objectOnGround.DestroySelf();

            }

        }

        else if (objectOnGround.GetComponent<AbilityOnGround>() != null)
        {
            if (abilitySystem.UnlockAbility(objectOnGround.GetComponent<AbilityOnGround>().GetAbility()))
            {
                objectOnGround.DestroySelf();
            }
           
        }
    }
    // when key G is pressed, throw a grenade if the player has a grenade
    private void HandleGrenadeThrow()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (throwSystem.ThrowGrenade())
            {
                OnObjectThrow?.Invoke(this, EventArgs.Empty);
            }
        }
    }
    // when key M is pressed use dash ability if it is usable
    private void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (abilitySystem.IsDashUnlocked())
            {
                abilitySystem.AbilityDash();
            }
        }
    }
    // activate and deactivate laser if player has it.
    private void HandleLaserActivation()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {

            Gun gun = weaponController.GetCurrentWeapon() as Gun;
            if (gun != null && gun is Pistol)
            {
                if (abilitySystem.IsPistolLaserUnlocked())
                {
                    if (gun.IsLaserActive())
                    {
                        gun.DeactivateLaser();

                    }
                    else
                    {
                        gun.ActivateLaser();
                    }
                }
            }
            if (gun != null && gun is Rifle)
            {
                if (abilitySystem.IsRifleLaserUnlocked())
                {
                    if (gun.IsLaserActive())
                    {
                        gun.DeactivateLaser();

                    }
                    else
                    {
                        gun.ActivateLaser();
                    }
                }
            }
        }

    }
    private void HandleReload()
    {
        if (Input.GetKeyDown(KeyCode.R) && weaponController.GetCurrentWeapon() is Gun)
        {
            ((Gun)weaponController.GetCurrentWeapon()).StartReloading();
        }
    }
    private void HandleShooting()
    {
        if (GetCurrentWeapon() is Pistol)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (weaponController.Fire())
                {

                    AnyPistolFiredServerRpc();
                }
            }
        }
        else if (Input.GetMouseButton(0))
        {
            if (weaponController.Fire())
            {
                if (GetCurrentWeapon() is Rifle)
                {
                    AnyRifleFiredServerRpc();
                }
                else if (GetCurrentWeapon() is Shotgun)
                {
                    AnyShotgunFiredServerRpc();
                }
                else if (GetCurrentWeapon() is Knife)
                {
                    playerAnimator.PlayKnifeAttackAnimtion();
                }
            }
        }
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
        moveDir = new Vector3(inputVector.x, inputVector.y, 0);

        isWalking = moveDir != Vector3.zero;

        float moveDistance = moveSpeed * Time.deltaTime;

        transform.position += moveDir * moveDistance;


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
        Vector3 dir = (ray.origin - weaponController.GetCurrentWeapon().GetFirePoint());
        dir.z = 0;
        return dir.normalized;

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
    public void TakeDamage(float damage)
    {
        playerHealthSystem.TakeDamageServerRpc(damage);
    }
    public Rigidbody2D GetRigidBody()
    {
        return rb;
    }
    public Vector3 GetMoveDirectionNormalized()
    {
        return moveDir.normalized;
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

    [ServerRpc(RequireOwnership = false)]
    public void AnyPistolReloadServerRpc()
    {
        AnyPistolReloadClientRpc();
    }
    [ClientRpc]
    public void AnyPistolReloadClientRpc()
    {
        OnAnyPistolReload?.Invoke(this, EventArgs.Empty);

    }
    [ServerRpc(RequireOwnership = false)]
    public void AnyRifleReloadServerRpc()
    {
        AnyRifleReloadClientRpc();
    }
    [ClientRpc]
    public void AnyRifleReloadClientRpc()
    {
        OnAnyRifleReload?.Invoke(this, EventArgs.Empty);

    }
    [ServerRpc(RequireOwnership = false)]
    public void AnyShotgunReloadServerRpc()
    {
        AnyShotgunReloadClientRpc();
    }
    [ClientRpc]
    public void AnyShotgunReloadClientRpc()
    {
        OnAnyShotgunReload?.Invoke(this, EventArgs.Empty);
    }
    public void ReloadShotgun()
    {
        OnShotgunReload?.Invoke(this, EventArgs.Empty);
        AnyShotgunReloadServerRpc();
    }
    public void ReloadRifle()
    {
        OnRifleReload?.Invoke(this, EventArgs.Empty);
        AnyRifleReloadServerRpc();
    }
    public void ReloadPistol()
    {
        OnPistolReload?.Invoke(this, EventArgs.Empty);
        AnyPistolReloadServerRpc();
    }
    public void PlayAudioClip(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }
    public void ResetAudioClip()
    {
        audioSource.Pause();
        audioSource.clip = null;
    }
    public void IncreaseMoveSpeed(float increaseAmount)
    {
        moveSpeed += increaseAmount;
    }
    public bool CanShoot()
    {
        return canShoot;
    }
    public void SetCanShoot(bool value)
    {
        canShoot = value;
    }
    public ThrowSystem GetThrowSystem()
    {
        return throwSystem;
    }
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }
    public float GetSpeed()
    {
        return moveSpeed;
    }
    public AbilitySystem GetAbilitySystem()
    {
        return abilitySystem;
    }

}


