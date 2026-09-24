using UnityEngine;
using TMPro;

public class OrderTicketUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipeNameText;
    [SerializeField] private TextMeshProUGUI ingredientsListText;

    public void Setup(RecipeSO recipe, int orderNumber)
    {
        if (recipe == null) return;

        if (recipeNameText != null)
        {
            recipeNameText.text = $"Pedido #{orderNumber}";
        }

        if (ingredientsListText != null)
        {
            string details = "";

            if (recipe.requiredIngredients != null)
            {
                foreach (var ing in recipe.requiredIngredients)
                {
                    if (ing != null) details += "• " + ing.ingredientName + "\n";
                }
            }

            if (recipe.requiredSauces != null)
            {
                foreach (var sauce in recipe.requiredSauces)
                {
                    if (sauce != null) details += "• " + sauce.ingredientName + "\n";
                }
            }

            ingredientsListText.text = details;
        }
    }
}