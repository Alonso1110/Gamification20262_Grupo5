using UnityEngine;

public class Ingredient : MonoBehaviour
{
    [Header("Ingredient Details")]
    [SerializeField] private string ingredientID = "Meow";

    public string IngredientID => ingredientID;
}
