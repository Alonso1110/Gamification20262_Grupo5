using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Restaurant/Recipe")]
public class RecipeSO : ScriptableObject
{
    [Header("Recipe Information")]
    public string recipeID;
    public string recipeName;
    public Sprite recipeIcon;

    [Header("Required Ingredients")]
    public List<IngredientSO> requiredIngredients = new List<IngredientSO>();

    [Header("Required Sauces")]
    public List<IngredientSO> requiredSauces = new List<IngredientSO>();
}