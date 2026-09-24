using UnityEngine;
using UnityEngine.EventSystems;

public class BellButton : MonoBehaviour, IPointerClickHandler
{
    [Header("Dish Associated with this Hood")]
    [SerializeField] private RecipeValidator associatedPlateValidator;
    [SerializeField] private PlateDraggable associatedPlateDraggable;

    public void OnPointerClick(PointerEventData eventData)
    {
        RingBell();
    }

    public void RingBell()
    {
        Debug.Log("SE TOCÓ LA CAMPANA");

        if (associatedPlateValidator == null)
        {
            Debug.Log("No asignaste el plato a la campana en el Inspector");
            return;
        }

        if (OrderManager.Instance != null)
        {
            OrderManager.Instance.DeliverPlate(associatedPlateValidator, associatedPlateDraggable);
        }
        else
        {
            Debug.Log("No se encontró el OrderManager en la escena");
        }
    }
}