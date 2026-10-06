using UnityEngine;
using TMPro;

public class EconomyHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Start()
    {
        var eco = GameEconomy.Instance;
        if (eco == null) return;

        eco.OnMoneyChanged += v => { if (moneyText) moneyText.text = $"S/ {v}"; };
        eco.OnScoreChanged += v => { if (scoreText) scoreText.text = v.ToString(); };

        if (moneyText) moneyText.text = $"S/ {eco.Money}";
        if (scoreText) scoreText.text = eco.Score.ToString();
    }
}