using System.Collections.Generic;
using UnityEngine;

public class RecipeValidator : MonoBehaviour
{
    public List<string> GetCurrentIngredientsOnPlate()
    {
        List<string> currentIDs = new List<string>();

        Ingredient[] ingredientsInPlate = GetComponentsInChildren<Ingredient>();
        foreach (Ingredient ing in ingredientsInPlate)
        {
            if (ing != null && !string.IsNullOrEmpty(ing.IngredientID))
            {
                currentIDs.Add(ing.IngredientID.ToLower().Trim());
            }
        }

        PlateSauceOverlay sauceOverlay = GetComponentInChildren<PlateSauceOverlay>();
        if (sauceOverlay != null)
        {
            currentIDs.AddRange(sauceOverlay.GetActiveSauceIDs());
        }

        return currentIDs;
    }

    public bool ValidateRecipe(RecipeSO recipe)
    {
        if (recipe == null) return false;

        List<string> plateItems = GetCurrentIngredientsOnPlate();

        List<string> requiredIDs = new List<string>();

        if (recipe.requiredIngredients != null)
        {
            foreach (var ing in recipe.requiredIngredients)
            {
                if (ing != null && !string.IsNullOrEmpty(ing.ingredientID))
                {
                    requiredIDs.Add(ing.ingredientID.ToLower().Trim());
                }
            }
        }

        if (recipe.requiredSauces != null)
        {
            foreach (var sauce in recipe.requiredSauces)
            {
                if (sauce != null && !string.IsNullOrEmpty(sauce.ingredientID))
                {
                    requiredIDs.Add(sauce.ingredientID.ToLower().Trim());
                }
            }
        }

        if (plateItems.Count != requiredIDs.Count)
        {
            return false;
        }

        List<string> tempPlateItems = new List<string>(plateItems);

        foreach (string reqID in requiredIDs)
        {
            string matchFound = null;

            foreach (string plateID in tempPlateItems)
            {
                if (AreIngredientsMatching(reqID, plateID))
                {
                    matchFound = plateID;
                    break;
                }
            }

            if (matchFound != null)
            {
                tempPlateItems.Remove(matchFound);
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    private bool AreIngredientsMatching(string requiredID, string plateID)
    {
        if (requiredID == plateID) return true;

        if (requiredID.Contains("breast") && plateID.Contains("breast")) return true;
        if (requiredID.Contains("leg") && plateID.Contains("leg")) return true;
        if (requiredID.Contains("half") && plateID.Contains("half")) return true;

        return false;
    }
}