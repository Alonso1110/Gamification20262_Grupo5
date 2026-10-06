using System;
using UnityEngine;

public class GameEconomy : MonoBehaviour
{
    public static GameEconomy Instance { get; private set; }

    [Header("Money")]
    [SerializeField] private int startingMoney = 50;
    [SerializeField] private int payPerItem = 10;
    [SerializeField] private int expiredPenalty = 0;

    [Header("Tip zones")]
    [SerializeField, Range(0f, 1f)] private float greenZone = 0.6f;
    [SerializeField, Range(0f, 1f)] private float yellowZone = 0.3f;
    [SerializeField] private float greenTipPercent = 0.5f;
    [SerializeField] private float yellowTipPercent = 0.2f;

    public int Money { get; private set; }
    public int Score { get; private set; }

    public event Action<int> OnMoneyChanged;
    public event Action<int> OnScoreChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        Money = startingMoney;
        OnMoneyChanged?.Invoke(Money);
        OnScoreChanged?.Invoke(Score);

        if (OrderManager.Instance != null)
        {
            OrderManager.Instance.OnOrderCompleted += HandleCompleted;
            OrderManager.Instance.OnOrderExpired += HandleExpired;
        }
    }

    private void OnDestroy()
    {
        if (OrderManager.Instance != null)
        {
            OrderManager.Instance.OnOrderCompleted -= HandleCompleted;
            OrderManager.Instance.OnOrderExpired -= HandleExpired;
        }
    }

    private void HandleCompleted(Order order)
    {
        int items = order.recipe.requiredIngredients.Count + order.recipe.requiredSauces.Count;
        int basePay = items * payPerItem;

        float n = order.Normalized;
        float tipPercent = n >= greenZone ? greenTipPercent
                         : n >= yellowZone ? yellowTipPercent
                         : 0f;
        int tip = Mathf.RoundToInt(basePay * tipPercent);

        Money += basePay + tip;
        Score += (basePay + tip) * 10;

        OnMoneyChanged?.Invoke(Money);
        OnScoreChanged?.Invoke(Score);
    }

    private void HandleExpired(Order order)
    {
        if (expiredPenalty <= 0) return;
        Money = Mathf.Max(0, Money - expiredPenalty);
        OnMoneyChanged?.Invoke(Money);
    }

    public bool CanAfford(int cost) => cost <= 0 || Money >= cost;

    public bool TrySpend(int cost)
    {
        if (cost <= 0) return true;
        if (Money < cost) return false;
        Money -= cost;
        OnMoneyChanged?.Invoke(Money);
        return true;
    }
}