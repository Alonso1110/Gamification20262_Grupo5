
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialOrderManager : MonoBehaviour
{
    public static TutorialOrderManager Instance { get; private set; }

    [Header("Tutorial ingredients")]
    [SerializeField] private IngredientSO chickenLeg;
    [SerializeField] private IngredientSO fries;
    [SerializeField] private IngredientSO salad;

    [Header("Scene transition")]
    [SerializeField] private string gameplaySceneName;
    [SerializeField] private float waitBeforeChangingScene = 2f;

    public List<Order> activeOrders = new List<Order>();

    public event Action<Order> OnOrderCompleted;

    private bool orderCompleted;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        GenerateTutorialOrder();
    }

    private void GenerateTutorialOrder()
    {
        if (activeOrders.Count > 0) return;

        if (chickenLeg == null || fries == null || salad == null)
        {
            Debug.LogError("Asigna la pierna, las papas y la ensalada en el Inspector.");
            return;
        }

        RecipeSO recipe = ScriptableObject.CreateInstance<RecipeSO>();

        recipe.recipeID = "tutorial_order";
        recipe.recipeName = "Pedido";

        recipe.requiredIngredients = new List<IngredientSO>
        {
            chickenLeg,
            fries,
            salad
        };

        recipe.requiredSauces = new List<IngredientSO>();

        activeOrders.Add(new Order(recipe, 0f));

        if (OrderBoardUI.Instance != null)
            OrderBoardUI.Instance.UpdateBoard();
    }

    public bool DeliverPlate(RecipeValidator plate, PlateDraggable plateDraggable)
    {
        if (orderCompleted || plate == null || activeOrders.Count == 0)
            return false;

        Order order = activeOrders[0];

        if (!plate.ValidateRecipe(order.recipe))
        {
            Debug.Log("El pedido todavía no está correcto.");
            return false;
        }

        orderCompleted = true;
        activeOrders.Remove(order);

        if (plateDraggable != null)
            plateDraggable.ClearPlate();

        OnOrderCompleted?.Invoke(order);

        if (OrderBoardUI.Instance != null)
            OrderBoardUI.Instance.UpdateBoard();

        Debug.Log("¡Pedido del tutorial completado!");

        StartCoroutine(ChangeToGameplayScene());

        return true;
    }

    private IEnumerator ChangeToGameplayScene()
    {
        yield return new WaitForSeconds(waitBeforeChangingScene);

        if (string.IsNullOrEmpty(gameplaySceneName))
        {
            Debug.LogError("No asignaste el nombre de la escena del gameplay.");
            yield break;
        }

        SceneManager.LoadScene(gameplaySceneName);
    }
}
