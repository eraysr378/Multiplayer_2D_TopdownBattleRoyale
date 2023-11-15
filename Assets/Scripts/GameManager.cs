using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    // Start is called before the first frame update

    private void Awake()
    {
        Instance = this;

    }
    void Start()
    {
        if (Player.LocalInstance != null)
        {
            Player.LocalInstance.OnLocalPlayerDied += Player_OnLocalPlayerDied;
        }
        else
        {
            Player.OnAnyPlayerSpawned += Player_OnAnyPlayerSpawned;
        }
    }

    private void Player_OnAnyPlayerSpawned(object sender, System.EventArgs e)
    {
        if (Player.LocalInstance != null)
        {
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
        RespawnDeadPlayerClientRpc(playerNetworkObjectReference);
    }
    [ClientRpc]
    private void RespawnDeadPlayerClientRpc(NetworkObjectReference playerNetworkObjectReference)
    {
        playerNetworkObjectReference.TryGet(out NetworkObject playerNetworkObject);
        Player player = playerNetworkObject.GetComponent<Player>();
        player.transform.position = Player.LocalInstance.GetSpawnPosition();
        player.GetHealthSystem().AddHealth(1000);
        player.gameObject.SetActive(true);

    }


}
