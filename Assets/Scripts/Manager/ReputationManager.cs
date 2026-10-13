using System;
using UnityEngine;

public class ReputationManager : MonoBehaviour
{
    public static ReputationManager Instance { get; private set; }

    [Header("Reputation")]
    [SerializeField] private float maxReputation = 100f;
    [SerializeField] private float lostPerExpired = 25f;
    [SerializeField] private float gainPerDelivery = 5f;

    private float reputation;

    public float Normalized => reputation / maxReputation;
    public bool IsGameOver { get; private set; }

    public event Action OnGameOver;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        reputation = maxReputation;

        if (OrderManager.Instance != null)
        {
            OrderManager.Instance.OnOrderExpired += HandleExpired;
            OrderManager.Instance.OnOrderCompleted += HandleCompleted;
        }
    }

    private void OnDestroy()
    {
        if (OrderManager.Instance != null)
        {
            OrderManager.Instance.OnOrderExpired -= HandleExpired;
            OrderManager.Instance.OnOrderCompleted -= HandleCompleted;
        }
    }

    private void Update()
    {
        if (IsGameOver) return;
    }

    private void HandleExpired(Order order)
    {
        if (IsGameOver) return;

        reputation = Mathf.Max(0f, reputation - lostPerExpired);
        if (reputation <= 0f) EndGame();
    }

    private void HandleCompleted(Order order)
    {
        if (IsGameOver) return;
        reputation = Mathf.Min(maxReputation, reputation + gainPerDelivery);
    }

    private void EndGame()
    {
        IsGameOver = true;
        Time.timeScale = 0f;
        OnGameOver?.Invoke();
    }
}