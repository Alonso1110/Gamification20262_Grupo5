using UnityEngine;
using UnityEngine.EventSystems;

public class TrashCan : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject != null)
        {
            if (droppedObject.TryGetComponent<PlateDraggable>(out PlateDraggable plate))
            {
                plate.ClearPlate();
                Debug.Log("Ingredientes del plato eliminados en la basura");
            }

            else
            {
                Destroy(droppedObject);
                Debug.Log("Ingrediente suelto eliminado en la basura");
            }
        }
    }
}