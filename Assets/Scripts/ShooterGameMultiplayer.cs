using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShooterGameMultiplayer : NetworkBehaviour
{
    public const int MAX_PLAYER_AMOUNT = 4;
    private const string PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER = "PlayerNameMultiplayer";
    public static ShooterGameMultiplayer Instance { get; private set; }
    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;
    public event EventHandler OnPlayerDataNetworkListChanged;
    [SerializeField] private BulletPrefabsSO bulletPrefabsSO;
    [SerializeField] private ThrowableObjectsSO throwableObjectsSO;
    private NetworkList<PlayerData> playerDataNetworkList;
    private string playerName;
    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);

        playerName = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, "Player" + UnityEngine.Random.Range(100, 1000));
        playerDataNetworkList = new NetworkList<PlayerData>();
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;

    }
    public string GetPlayerName()
    {
        return playerName;
    }
    public void SetPlayerName(string playerName)
    {
        this.playerName = playerName;
        PlayerPrefs.SetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, playerName);
    }
    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SpawnGrenade(Player ownerPlayer, Vector3 position, Vector3 moveDir)
    {
        SpawnGrenadeServerRpc(ownerPlayer.GetNetworkObject(), position.x, position.y, moveDir.x, moveDir.y);
    }
    [ServerRpc(RequireOwnership = false)]
    public void SpawnGrenadeServerRpc(NetworkObjectReference ownerPlayerNetworkObjectReference, float firePointX, float firePointY, float moveDirX, float moveDirY)
    {
        ownerPlayerNetworkObjectReference.TryGet(out NetworkObject ownerPlayerNetworkObject);
        Player ownerPlayer = ownerPlayerNetworkObject.GetComponent<Player>();
        Vector3 firePoint = new Vector3(firePointX, firePointY, 0);
        Vector3 moveDir = new Vector3(moveDirX, moveDirY, 0);

        ThrowableObject projectileGrenade = Instantiate(throwableObjectsSO.grenade, firePoint, Quaternion.identity);
        projectileGrenade.SetOwner(ownerPlayer);
        NetworkObject projectileGrenadeNetworkObject = projectileGrenade.GetComponent<NetworkObject>();
        projectileGrenadeNetworkObject.Spawn(true);
        projectileGrenadeNetworkObject.GetComponent<Grenade>().ThrowGrenade(moveDir);

    }

    public void SpawnBullet(Player ownerPlayer, GameObject bulletPrefab, float bulletRotationAngle, Transform firePoint)
    {
        int index = -1;
        for (int i = 0; i < bulletPrefabsSO.bulletPrefabList.Count; i++)
        {
            if (bulletPrefabsSO.bulletPrefabList[i] == bulletPrefab)
            {
                index = i; break;
            }
        }

        SpawnBulletServerRpc(ownerPlayer.GetNetworkObject(), index, bulletRotationAngle, firePoint.position.x, firePoint.position.y);
    }
    [ServerRpc(RequireOwnership = false)]
    private void SpawnBulletServerRpc(NetworkObjectReference ownerPlayerNetworkObjectReference, int bulletPrefabIndex, float bulletRotationAngle, float firePointX, float firePointY)
    {
        ownerPlayerNetworkObjectReference.TryGet(out NetworkObject ownerPlayerNetworkObject);
        Player ownerPlayer = ownerPlayerNetworkObject.GetComponent<Player>();

        GameObject bulletPrefab = bulletPrefabsSO.bulletPrefabList[bulletPrefabIndex];
        Vector3 firePoint = new Vector3(firePointX, firePointY, 0);
        GameObject projectileBullet = Instantiate(bulletPrefab, firePoint, Quaternion.identity);
        projectileBullet.GetComponent<Bullet>().SetOwner(ownerPlayer);
        NetworkObject projectileBulletNetworkObject = projectileBullet.GetComponent<NetworkObject>();
        projectileBulletNetworkObject.Spawn(true);

        projectileBulletNetworkObject.GetComponent<Bullet>().FireBullet(bulletRotationAngle);



    }
    public void SpawnBulletShell(GameObject bulletShellPrefab, float bulletRotationAngle, Transform bulletShellSpawnPoint)
    {
        int index = -1;
        for (int i = 0; i < bulletPrefabsSO.bulletShellPrefabList.Count; i++)
        {
            if (bulletPrefabsSO.bulletShellPrefabList[i] == bulletShellPrefab)
            {
                index = i; break;
            }
        }

        SpawnBulletShellServerRpc(index, bulletRotationAngle, bulletShellSpawnPoint.position.x, bulletShellSpawnPoint.position.y);
    }
    [ServerRpc(RequireOwnership = false)]
    private void SpawnBulletShellServerRpc(int bulletShellPrefabIndex, float bulletRotationAngle, float bulletShellSpawnPointX, float bulletShellSpawnPointY)
    {

        GameObject bulletShellPrefab = bulletPrefabsSO.bulletShellPrefabList[bulletShellPrefabIndex];
        Vector3 bulletShellSpawnPoint = new Vector3(bulletShellSpawnPointX, bulletShellSpawnPointY, 0);
        GameObject projectileBulletShell = Instantiate(bulletShellPrefab, bulletShellSpawnPoint, Quaternion.identity);
        NetworkObject projectileBulletShellNetworkObject = projectileBulletShell.GetComponent<NetworkObject>();
        projectileBulletShellNetworkObject.Spawn(true);

        projectileBulletShellNetworkObject.GetComponent<BulletShell>().FireBulletShell(bulletRotationAngle);


    }
    public void DestroyBullet(Bullet bullet)
    {
        DestroyBulletServerRpc(bullet.GetNetworkObject());
    }
    [ServerRpc(RequireOwnership = false)]
    public void DestroyBulletServerRpc(NetworkObjectReference bulletNetworkObjectReference)
    {
        bulletNetworkObjectReference.TryGet(out NetworkObject bulletNetworkObject);
        if (bulletNetworkObject != null)
        {
            Bullet bullet = bulletNetworkObject.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet.DestroySelf();
            }
            else
            {
                Debug.Log("Bullet does not exist");
            }
        }
        else
        {
            Debug.Log("Bullet network object does not exist");

        }

    }
    public void DestroyBulletShell(BulletShell bulletShell)
    {
        DestroyBulletShellServerRpc(bulletShell.GetNetworkObject());

    }
    [ServerRpc(RequireOwnership = false)]
    public void DestroyBulletShellServerRpc(NetworkObjectReference bulletShellNetworkObjectReference)
    {
        bulletShellNetworkObjectReference.TryGet(out NetworkObject bulletShellNetworkObject);
        if (bulletShellNetworkObject != null)
        {
            BulletShell bulletShell = bulletShellNetworkObject.GetComponent<BulletShell>();
            if (bulletShell != null)
            {
                bulletShell.DestroySelf();
            }
            else
            {
                Debug.Log("BulletShell does not exist");
            }
        }
        else
        {
            Debug.Log("BulletShell network object does not exist");

        }
    }
    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartHost();


    }

    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            PlayerData playerData = playerDataNetworkList[i];
            if(playerData.clientId == clientId)
            {
                //player disconnected
                playerDataNetworkList.RemoveAt(i);
            }
        }
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        playerDataNetworkList.Add(new PlayerData
        {
            clientId = clientId,
        });
        SetPlayerNameServerRpc(GetPlayerName());
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);

    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        if (SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectScene.ToString())
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game has already started";
            return;

        }
        if (NetworkManager.Singleton.ConnectedClientsIds.Count > MAX_PLAYER_AMOUNT)
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game is full";
            return;
        }
        connectionApprovalResponse.Approved = true;
    }

    public void StartClient()
    {
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;

        NetworkManager.Singleton.StartClient();

    }

    private void NetworkManager_Client_OnClientConnectedCallback(ulong clientId)
    {
        SetPlayerNameServerRpc(GetPlayerName());
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
    }

    [ServerRpc(RequireOwnership =false)]
    private void SetPlayerNameServerRpc(string playerName,ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.playerName = playerName;
        playerDataNetworkList[playerDataIndex] = playerData;
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.playerId = playerId;
        playerDataNetworkList[playerDataIndex] = playerData;
    }

    private void NetworkManager_Client_OnClientDisconnectCallback(ulong obj)
    {
        OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);

    }
    public bool IsPlayerIndexConnected(int playerIndex)
    {
        return playerIndex < playerDataNetworkList.Count;
    }
    public int GetPlayerDataIndexFromClientId(ulong clientId)
    {
        for(int i = 0; i < playerDataNetworkList.Count; i++)
        {
            if (playerDataNetworkList[i].clientId == clientId )
            {
                return i;
            }
        }
       
        return -1;
    }
    public PlayerData GetPlayerDataFromClientId(ulong clientId)
    {
        foreach(PlayerData playerData in playerDataNetworkList)
        {
            if(playerData.clientId== clientId)
            {
                return playerData;
            }
        }
        return default;
    }
    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        return playerDataNetworkList[playerIndex];
    }
    public void KickPlayer(ulong clientId)
    {
        NetworkManager.Singleton.DisconnectClient(clientId);
        NetworkManager_Server_OnClientDisconnectCallback(clientId);
    }

}
