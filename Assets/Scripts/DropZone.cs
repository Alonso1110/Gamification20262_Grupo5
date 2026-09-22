using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        DraggableIngredient draggedIngredient = eventData.pointerDrag?.GetComponent<DraggableIngredient>();
        if (draggedIngredient == null) return;

        Ingredient ingredientData = draggedIngredient.GetComponent<Ingredient>();
        if (ingredientData == null) return;

        string id = ingredientData.IngredientID.ToLower();

        if (id.Contains("rice") || id.Contains("arroz") || id.Contains("chaufa"))
        {
            PlateRiceOverlay riceOverlay = GetComponent<PlateRiceOverlay>();
            if (riceOverlay != null)
            {
                if (!riceOverlay.ServeRice(draggedIngredient.gameObject))
                {
                    Destroy(draggedIngredient.gameObject);
                }
            }
        }
        else if (id.Contains("salad") || id.Contains("ensalada"))
        {
            PlateSaladOverlay saladOverlay = GetComponent<PlateSaladOverlay>();
            if (saladOverlay != null)
            {
                if (!saladOverlay.ServeSalad(draggedIngredient.gameObject))
                {
                    Destroy(draggedIngredient.gameObject);
                }
            }
        }

        else if (id.Contains("chicken") || id.Contains("pollo") || id.Contains("quarter") || id.Contains("half"))
        {
            PlateChickenOverlay chickenOverlay = GetComponent<PlateChickenOverlay>();
            if (chickenOverlay != null)
            {
                if (!chickenOverlay.ServeChicken(draggedIngredient.gameObject))
                {
                    Destroy(draggedIngredient.gameObject);
                }
            }
        }
    }
}