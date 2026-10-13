using UnityEngine;

[CreateAssetMenu(fileName = "NewIngredient", menuName = "Restaurant/Ingredient")]
public class IngredientSO : ScriptableObject
{
    [Header("Ingredient Information")]
    public string ingredientID;
    public string ingredientName;
    public Sprite icon;

    [Header("Economy")]
    public int sellPrice = 8;
    public int scorePoints = 10;
}
