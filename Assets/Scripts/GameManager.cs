using System;
using Unity.Netcode;
using UnityEngine;
using Cinemachine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{

    public static GameManager Instance { get; private set; }
    public event EventHandler OnStateChanged;
    public event EventHandler OnLocalPlayerReadyChanged;
    private enum State
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver
    }
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Transform playerPrefab;

    private NetworkVariable<State> state = new NetworkVariable<State>(State.WaitingToStart);
    private bool isLocalPlayerReady;
    private NetworkVariable<float> countdownToStartTimer = new NetworkVariable<float>(3f);
    private Dictionary<ulong, bool> playerReadyDictionary;
    private Player playerOnCamera;
    private int alivePlayerCount;
    private bool areBoxesSpawned;
    // Start is called before the first frame update

    private void Awake()
    {
        Instance = this;
        playerReadyDictionary = new Dictionary<ulong, bool>();
        alivePlayerCount = 4;

    }
    public override void OnNetworkSpawn()
    {
        state.OnValueChanged += State_OnValueChanged;
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)

    {

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {

            Transform playerTransform = Instantiate(playerPrefab);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
    }

    private void State_OnValueChanged(State previousValue, State newValue)
    {
        OnStateChanged?.Invoke(this, EventArgs.Empty);

    }

    void Start()
    {
        if (Player.LocalInstance != null)
        {
            virtualCamera.Follow = Player.LocalInstance.transform;
            virtualCamera.LookAt = Player.LocalInstance.transform;
            playerOnCamera = Player.LocalInstance;
            Player.LocalInstance.OnLocalPlayerDied += Player_OnLocalPlayerDied;
        }
        else
        {
            Player.OnAnyPlayerSpawned += Player_OnAnyPlayerSpawned;
        }

    }
    private void Update()
    {
        CheckIfPlayerReady();
        if (!IsServer)
        {
            return;
        }
        // uncomment this to end the game
        if (alivePlayerCount <= 1)
        {
            state.Value = State.GameOver;
        }
        switch (state.Value)
        {

            case State.WaitingToStart:
                break;
            case State.CountdownToStart:
                countdownToStartTimer.Value -= Time.deltaTime;
                if(countdownToStartTimer.Value < 2 && !areBoxesSpawned)
                {
                    areBoxesSpawned = true;
                    for (int i = 0; i < 10; i++)
                    {
                        ShooterGameMultiplayer.Instance.SpawnAbilityBox(new Vector3(UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100),0));
                        ShooterGameMultiplayer.Instance.SpawnEquipmentBox(new Vector3(UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100), 0));
                        ShooterGameMultiplayer.Instance.SpawnWeaponBox(new Vector3(UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100), 0));
                    }
                }
                if (countdownToStartTimer.Value < 0)
                {
                    alivePlayerCount = NetworkManager.Singleton.ConnectedClients.Count;
                    state.Value = State.GamePlaying;
                }
                break;
            case State.GamePlaying:

                break;
            case State.GameOver:
                break;

            default:
                break;
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;
        bool allClientsReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                // player is not ready
                allClientsReady = false;
                break;
            }
        }
        if (allClientsReady)
        {
            state.Value = State.CountdownToStart;
        }
    }
    private void CheckIfPlayerReady()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName(Loader.Scene.GameScene.ToString()) && state.Value == State.WaitingToStart)
        {
            isLocalPlayerReady = true;
            OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
            SetPlayerReadyServerRpc();
        }
    }
    public bool isGamePlaying()
    {
        return state.Value == State.GamePlaying;
    }

    private void Player_OnAnyPlayerSpawned(object sender, System.EventArgs e)
    {
        if (Player.LocalInstance != null)
        {
            virtualCamera.Follow = Player.LocalInstance.transform;
            virtualCamera.LookAt = Player.LocalInstance.transform;
            playerOnCamera = Player.LocalInstance;

            Player.LocalInstance.OnLocalPlayerDied -= Player_OnLocalPlayerDied;
            Player.LocalInstance.OnLocalPlayerDied += Player_OnLocalPlayerDied;
        }
    }
   
    private void Player_OnLocalPlayerDied(object sender, Player.OnLocalPlayerDiedEventArgs e)
    {
        //Debug.Log("Player will be spawned after 2 secs");
        DeactivateDeadPlayerServerRpc(e.player.GetNetworkObject());
        //Invoke("RespawnDeadPlayer", 2f);


    }
    [ServerRpc(RequireOwnership = false)]
    private void DeactivateDeadPlayerServerRpc(NetworkObjectReference playerNetworkObjectReference)
    {
        alivePlayerCount--;
        DeactivateDeadPlayerClientRpc(playerNetworkObjectReference);
    }
    [ClientRpc]
    private void DeactivateDeadPlayerClientRpc(NetworkObjectReference playerNetworkObjectReference)
    {
        playerNetworkObjectReference.TryGet(out NetworkObject playerNetworkObject);
        Player player = playerNetworkObject.GetComponent<Player>();

        player.gameObject.SetActive(false);
        if (player.transform == virtualCamera.Follow)
        {
            Player[] playerList = FindObjectsOfType<Player>();

            foreach (Player playerToWatch in playerList)
            {
                if (playerToWatch.gameObject.activeSelf)
                {
                    virtualCamera.Follow = playerToWatch.transform;
                    virtualCamera.LookAt = playerToWatch.transform;
                    playerOnCamera = playerToWatch;
                    break;
                }
            }
        }
    }
    public Player GetPlayerOnCamera()
    {
        return playerOnCamera;
    }
    private void RespawnDeadPlayer()
    {
        RespawnDeadPlayerServerRpc(Player.LocalInstance.GetNetworkObject());
    }
    [ServerRpc(RequireOwnership = false)]
    private void RespawnDeadPlayerServerRpc(NetworkObjectReference playerNetworkObjectReference)
    {
        playerNetworkObjectReference.TryGet(out NetworkObject playerNetworkObject);
        Player player = playerNetworkObject.GetComponent<Player>();
        player.GetHealthSystem().AddHealth(1000);

        RespawnDeadPlayerClientRpc(playerNetworkObjectReference);

    }
    [ClientRpc]
    private void RespawnDeadPlayerClientRpc(NetworkObjectReference playerNetworkObjectReference)
    {
        playerNetworkObjectReference.TryGet(out NetworkObject playerNetworkObject);
        Player player = playerNetworkObject.GetComponent<Player>();
        player.gameObject.SetActive(true);
        player.transform.position = Player.LocalInstance.GetSpawnPosition();

    }

    public bool IsLocalPlayerReady()
    {
        return isLocalPlayerReady;
    }
    public bool IsGameOver()
    {
        return state.Value == State.GameOver;
    }
    public bool IsGamePlaying()
    {
        return state.Value == State.GamePlaying;
    }
    public bool IsCountdownToStart()
    {
        return state.Value == State.CountdownToStart;
    }

    public float GetCountdownToStartTimer()
    {
        return countdownToStartTimer.Value;
    }

}
