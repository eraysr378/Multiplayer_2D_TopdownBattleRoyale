using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingUI : MonoBehaviour
{
    private void Start()
    {
        ShooterGameMultiplayer.Instance.OnTryingToJoinGame += ShooterGameMultiplayer_OnTryingToJoinGame;
        ShooterGameMultiplayer.Instance.OnFailedToJoinGame += ShooterGameMultiplayer_OnFailedToJoinGame;
        Hide();
    }

    private void ShooterGameMultiplayer_OnFailedToJoinGame(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void ShooterGameMultiplayer_OnTryingToJoinGame(object sender, System.EventArgs e)
    {
        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);

    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        ShooterGameMultiplayer.Instance.OnTryingToJoinGame -= ShooterGameMultiplayer_OnTryingToJoinGame;
        ShooterGameMultiplayer.Instance.OnFailedToJoinGame -= ShooterGameMultiplayer_OnFailedToJoinGame;
    }
}
