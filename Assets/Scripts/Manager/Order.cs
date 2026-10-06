using UnityEngine;

[System.Serializable]
public class Order
{
    public RecipeSO recipe;
    public float timeLimit;
    public float timeRemaining;

    public float Normalized => timeLimit <= 0f ? 0f : Mathf.Clamp01(timeRemaining / timeLimit);

    public Order(RecipeSO recipe, float timeLimit)
    {
        this.recipe = recipe;
        this.timeLimit = timeLimit;
        this.timeRemaining = timeLimit;
    }
}