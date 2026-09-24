using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }

    [Header("All Possible Recipes in the Game")]
    [SerializeField] private List<RecipeSO> possibleRecipes = new List<RecipeSO>();

    [Header("Active Orders on the Board")]
    public List<RecipeSO> activeOrders = new List<RecipeSO>();

    [SerializeField] private int maxOrdersOnBoard = 6;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        GenerateNewOrder();
        GenerateNewOrder();
        GenerateNewOrder();
        GenerateNewOrder();
    }

    public void GenerateNewOrder()
    {
        if (activeOrders.Count >= maxOrdersOnBoard) return;
        if (possibleRecipes.Count == 0) return;

        RecipeSO randomRecipe = possibleRecipes[Random.Range(0, possibleRecipes.Count)];
        activeOrders.Add(randomRecipe);

        if (OrderBoardUI.Instance != null)
            OrderBoardUI.Instance.UpdateBoard();
    }

    public bool DeliverPlate(RecipeValidator plate, PlateDraggable plateDraggable)
    {
        RecipeSO matchedRecipe = null;

        foreach (RecipeSO order in activeOrders)
        {
            if (plate.ValidateRecipe(order))
            {
                matchedRecipe = order;
                break;
            }
        }

        if (plateDraggable != null)
        {
            plateDraggable.ClearPlate();
        }

        if (matchedRecipe != null)
        {
            Debug.Log($"PEDIDO CORRECTO: {matchedRecipe.recipeName}");
            activeOrders.Remove(matchedRecipe);

            if (OrderBoardUI.Instance != null)
                OrderBoardUI.Instance.UpdateBoard();

            return true;
        }
        else
        {
            Debug.Log("ERROR DE ENTREGA");
            return false;
        }
    }
}