using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject != null)
        {
            if (droppedObject.TryGetComponent<DraggableIngredient>(out DraggableIngredient dragScript))
            {
                droppedObject.transform.SetParent(transform);
                dragScript.LockInPlace();

                if (droppedObject.TryGetComponent<Ingredient>(out Ingredient ingredient))
                {
                    Debug.Log("Ingrediente fijado en el plato: " + ingredient.IngredientID);
                }
            }
        }
    }
}