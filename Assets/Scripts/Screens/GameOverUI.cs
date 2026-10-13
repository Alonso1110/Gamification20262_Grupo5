using UnityEngine;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Start()
    {
        if (panel != null) panel.SetActive(false);

        if (ReputationManager.Instance != null)
            ReputationManager.Instance.OnGameOver += Show;
    }

    private void OnDestroy()
    {
        if (ReputationManager.Instance != null)
            ReputationManager.Instance.OnGameOver -= Show;
    }

    private void Show()
    {
        if (GameEconomy.Instance != null)
        {
            if (moneyText) moneyText.text = $"S/ {GameEconomy.Instance.Money}";
            if (scoreText) scoreText.text = GameEconomy.Instance.Score.ToString();
        }

        if (panel != null) panel.SetActive(true);
    }
}