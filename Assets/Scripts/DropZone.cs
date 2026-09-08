using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject != null)
        {
            droppedObject.transform.SetParent(transform);

            if (droppedObject.TryGetComponent<DragAndDropItem>(out DragAndDropItem dragScript))
            {
                dragScript.LockInPlace();
            }

            if (droppedObject.TryGetComponent<Ingredient>(out Ingredient ingredient))
            {
                Debug.Log("Ingrediente fijado en el plato: " + ingredient.IngredientID);
            }
        }
    }
}
