using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI killScore;
    [SerializeField] private Button menuButton;
    [SerializeField] private TextMeshProUGUI winnerText;
     

    private void Awake()
    {

        menuButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }
    private void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        Hide();
    }

    private void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsGameOver())
        {
            PlayerData winnerData = ShooterGameMultiplayer.Instance.GetPlayerDataFromClientId(GameManager.Instance.GetPlayerOnCamera().OwnerClientId);
            winnerText.text = "WINNER IS: " + winnerData.playerName.ToString();
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);

    }
}
