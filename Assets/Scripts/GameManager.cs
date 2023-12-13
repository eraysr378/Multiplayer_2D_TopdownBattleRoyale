using System;
using Unity.Netcode;
using UnityEngine;
using Cinemachine;

public class GameManager : NetworkBehaviour
{
    private enum State
    {
        Start,
        GameOver
    }
    public static GameManager Instance { get; private set; }
    public event EventHandler OnStateChanged;
    [SerializeField] private CinemachineVirtualCamera virtualCamera; 
    private State state;
    private float roundTimer = 10f;
    // Start is called before the first frame update

    private void Awake()
    {
        Instance = this;
        state = State.Start;

    }
    void Start()
    {
        if (Player.LocalInstance != null)
        {
            virtualCamera.Follow = Player.LocalInstance.transform;
            Player.LocalInstance.OnLocalPlayerDied += Player_OnLocalPlayerDied;
        }
        else
        {
            Player.OnAnyPlayerSpawned += Player_OnAnyPlayerSpawned;
        }
        RoundTimer.Instance.StartTimer(roundTimer);
    }
    private void Update()
    {
        switch (state)
        {

            case State.Start:
                roundTimer -= Time.deltaTime;
                if (roundTimer < 0)
                {
                    state = State.GameOver;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);

                }
                break;
            case State.GameOver:
                break;
            default:
                break;
        }
    }
    private void x(object sender, System.EventArgs e)
    {
        UnityEngine.Object[] playerList = FindObjectsOfType(typeof(Player));
        if (playerList.Length == 2)
        {
            Player winnerPlayer = (Player)playerList[0];
            if (playerList[1] != null && ((Player)playerList[1]).GetKillScore() > winnerPlayer.GetKillScore())
            {
                winnerPlayer = (Player)playerList[1];
            }
        }
    }

    private void Player_OnAnyPlayerSpawned(object sender, System.EventArgs e)
    {
        if (Player.LocalInstance != null)
        {
            virtualCamera.Follow = Player.LocalInstance.transform;
            virtualCamera.LookAt = Player.LocalInstance.transform;

            Player.LocalInstance.OnLocalPlayerDied -= Player_OnLocalPlayerDied;
            Player.LocalInstance.OnLocalPlayerDied += Player_OnLocalPlayerDied;
        }
    }

    private void Player_OnLocalPlayerDied(object sender, Player.OnLocalPlayerDiedEventArgs e)
    {
        Debug.Log("Player will be spawned after 2 secs");
        DeactivateDeadPlayerServerRpc(e.player.GetNetworkObject());
        Invoke("RespawnDeadPlayer", 2f);

    }
    [ServerRpc(RequireOwnership = false)]
    private void DeactivateDeadPlayerServerRpc(NetworkObjectReference playerNetworkObjectReference)
    {
        DeactivateDeadPlayerClientRpc(playerNetworkObjectReference);
    }
    [ClientRpc]
    private void DeactivateDeadPlayerClientRpc(NetworkObjectReference playerNetworkObjectReference)
    {
        playerNetworkObjectReference.TryGet(out NetworkObject playerNetworkObject);
        Player player = playerNetworkObject.GetComponent<Player>();
        player.gameObject.SetActive(false);
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


    public bool IsGameOver()
    {
        return state == State.GameOver;
    }


}
