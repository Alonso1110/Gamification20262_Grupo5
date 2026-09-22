using UnityEngine;
using UnityEngine.EventSystems;

public class TrashCan : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject == null) return;

        if (droppedObject.TryGetComponent<PlateDraggable>(out PlateDraggable plate))
        {
            plate.ClearPlate();
            Debug.Log("Se limpiaron todas las guarniciones y salsas del plato");
        }

        else if (droppedObject.GetComponent<DeepFryer>() != null)
        {
            return;
        }
        else
        {
            if (droppedObject.TryGetComponent<DraggableIngredient>(out DraggableIngredient dragScript))
            {
                dragScript.LockInPlace();
            }

            Destroy(droppedObject);
            Debug.Log("Ingrediente suelto eliminado en el tacho");
        }
    }
}