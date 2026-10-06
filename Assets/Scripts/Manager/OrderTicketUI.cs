using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderTicketUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipeNameText;
    [SerializeField] private TextMeshProUGUI ingredientsListText;
    [SerializeField] private Image timerFill;

    [Header("Timer colors")]
    [SerializeField] private Color fullColor = new Color(0.35f, 0.75f, 0.35f);
    [SerializeField] private Color emptyColor = new Color(0.85f, 0.25f, 0.25f);

    private Order order;

    public void Setup(Order order, int orderNumber)
    {
        this.order = order;
        RecipeSO recipe = order.recipe;

        if (recipeNameText != null)
            recipeNameText.text = $"Pedido #{orderNumber}";

        if (ingredientsListText != null)
        {
            string details = "";
            foreach (var ing in recipe.requiredIngredients)
                if (ing != null) details += "• " + ing.ingredientName + "\n";
            foreach (var sauce in recipe.requiredSauces)
                if (sauce != null) details += "• " + sauce.ingredientName + "\n";
            ingredientsListText.text = details;
        }

        RefreshTimer();
    }

    private void Update() => RefreshTimer();

    private void RefreshTimer()
    {
        if (order == null || timerFill == null) return;
        float n = order.Normalized;
        timerFill.fillAmount = n;
        timerFill.color = Color.Lerp(emptyColor, fullColor, n);
    }
}