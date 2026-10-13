using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }

    [Header("Required")]
    [SerializeField] private IngredientSO fries;

    [Header("Pools")]
    [SerializeField] private List<IngredientSO> mainPieces = new List<IngredientSO>();
    [SerializeField] private List<IngredientSO> sides = new List<IngredientSO>();
    [SerializeField] private List<IngredientSO> sauces = new List<IngredientSO>();

    [Header("Rules")]
    [SerializeField] private int maxSides = 2;
    [SerializeField] private int maxSauces = 2;
    [SerializeField] private int maxItemsPerOrder = 5;

    [Header("Timing")]
    [SerializeField] private float spawnInterval = 12f;
    [SerializeField] private float baseTime = 40f;
    [SerializeField] private float timePerItem = 15f;
    [SerializeField] private float refillDelay = 2f;

    [Header("Board")]
    [SerializeField] private int initialOrders = 2;
    [SerializeField] private int minOrdersOnBoard = 1;
    [SerializeField] private int maxOrdersOnBoard = 4;

    [Header("Active")]
    public List<Order> activeOrders = new List<Order>();

    public event Action<Order> OnOrderCompleted;
    public event Action<Order> OnOrderExpired;

    [Header("Difficulty")]
    [SerializeField] private float rampDuration = 300f;
    [SerializeField] private float minSpawnInterval = 5f;
    [SerializeField] private float minTimeMultiplier = 0.6f;

    private float elapsed;
    private float Ramp => Mathf.Clamp01(elapsed / rampDuration);

    private float spawnTimer;
    private float refillTimer;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        for (int i = 0; i < initialOrders; i++) GenerateNewOrder();
    }

    private void OnValidate()
    {
        maxItemsPerOrder = Mathf.Clamp(maxItemsPerOrder, 2, 5);
        maxOrdersOnBoard = Mathf.Clamp(maxOrdersOnBoard, 1, 4);
        minOrdersOnBoard = Mathf.Clamp(minOrdersOnBoard, 0, maxOrdersOnBoard);
        initialOrders = Mathf.Clamp(initialOrders, 0, maxOrdersOnBoard);
    }

    private void Update()
    {
        elapsed += Time.deltaTime;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= Mathf.Lerp(spawnInterval, minSpawnInterval, Ramp))
        {
            spawnTimer = 0f;
            GenerateNewOrder();
        }

        bool expiredAny = false;
        for (int i = activeOrders.Count - 1; i >= 0; i--)
        {
            activeOrders[i].timeRemaining -= Time.deltaTime;

            if (activeOrders[i].timeRemaining <= 0f)
            {
                Order expired = activeOrders[i];
                activeOrders.RemoveAt(i);
                OnOrderExpired?.Invoke(expired);
                expiredAny = true;
            }
        }

        if (activeOrders.Count < minOrdersOnBoard)
        {
            refillTimer += Time.deltaTime;
            if (refillTimer >= refillDelay)
            {
                refillTimer = 0f;
                spawnTimer = 0f;
                GenerateNewOrder();
            }
        }
        else
        {
            refillTimer = 0f;
        }

        if (expiredAny && OrderBoardUI.Instance != null)
            OrderBoardUI.Instance.UpdateBoard();
    }

    public void GenerateNewOrder()
    {
        if (activeOrders.Count >= maxOrdersOnBoard) return;

        RecipeSO recipe = BuildRandomRecipe();
        if (recipe == null) return;

        int items = recipe.requiredIngredients.Count + recipe.requiredSauces.Count;
        float time = baseTime + items * timePerItem;

        activeOrders.Add(new Order(recipe, time));

        time *= Mathf.Lerp(1f, minTimeMultiplier, Ramp);

        if (OrderBoardUI.Instance != null)
            OrderBoardUI.Instance.UpdateBoard();
    }

    private RecipeSO BuildRandomRecipe()
    {
        if (mainPieces.Count == 0) return null;

        var recipe = ScriptableObject.CreateInstance<RecipeSO>();
        recipe.recipeID = "proc_" + Guid.NewGuid().ToString("N").Substring(0, 6);
        recipe.recipeName = "Pedido";
        recipe.requiredIngredients = new List<IngredientSO>();
        recipe.requiredSauces = new List<IngredientSO>();

        recipe.requiredIngredients.Add(mainPieces[Random.Range(0, mainPieces.Count)]);
        if (fries != null) recipe.requiredIngredients.Add(fries);

        int slots = Mathf.Max(0, maxItemsPerOrder - recipe.requiredIngredients.Count);

        var optionalSides = sides.FindAll(s => s != fries);
        int sideCount = Mathf.Min(Random.Range(0, maxSides + 1), optionalSides.Count, slots);
        recipe.requiredIngredients.AddRange(PickDistinct(optionalSides, sideCount));
        slots -= sideCount;

        int sauceCount = Mathf.Min(Random.Range(0, maxSauces + 1), sauces.Count, slots);
        recipe.requiredSauces.AddRange(PickDistinct(sauces, sauceCount));

        return recipe;
    }

    private List<IngredientSO> PickDistinct(List<IngredientSO> pool, int count)
    {
        var copy = new List<IngredientSO>(pool);
        var result = new List<IngredientSO>();
        for (int i = 0; i < count && copy.Count > 0; i++)
        {
            int idx = Random.Range(0, copy.Count);
            result.Add(copy[idx]);
            copy.RemoveAt(idx);
        }
        return result;
    }

    public bool DeliverPlate(RecipeValidator plate, PlateDraggable plateDraggable)
    {
        Order matched = null;

        foreach (Order order in activeOrders)
        {
            if (plate.ValidateRecipe(order.recipe))
            {
                matched = order;
                break;
            }
        }

        if (plateDraggable != null)
            plateDraggable.ClearPlate();

        if (matched == null) return false;

        activeOrders.Remove(matched);
        OnOrderCompleted?.Invoke(matched);

        if (OrderBoardUI.Instance != null)
            OrderBoardUI.Instance.UpdateBoard();

        return true;
    }
}