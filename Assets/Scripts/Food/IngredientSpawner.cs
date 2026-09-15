using UnityEngine;
using UnityEngine.EventSystems;

public class IngredientSpawner : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Configuration")]
    [SerializeField] private GameObject ingredientPrefabToSpawn;

    private Canvas mainCanvas;
    private IngredientStation station;
    private DraggableIngredient activeCloneDrag;

    private void Awake()
    {
        mainCanvas = GetComponentInParent<Canvas>();
        station = GetComponentInParent<IngredientStation>();
        if (station == null)
            station = GetComponentInParent<IngredientStation>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (ingredientPrefabToSpawn == null || mainCanvas == null)
        {
            eventData.pointerDrag = null;
            return;
        }

        if (station != null)
        {
            if (!station.TryTakeIngredient())
            {
                eventData.pointerDrag = null;
                return;
            }
        }

        GameObject clone = Instantiate(ingredientPrefabToSpawn, mainCanvas.transform);
        RectTransform cloneRect = clone.GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            mainCanvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );
        cloneRect.anchoredPosition = localPoint;

        activeCloneDrag = clone.GetComponent<DraggableIngredient>();
        if (activeCloneDrag != null)
        {
            eventData.pointerDrag = clone;
            activeCloneDrag.OnBeginDrag(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (activeCloneDrag != null)
        {
            activeCloneDrag.OnDrag(eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (activeCloneDrag != null)
        {
            activeCloneDrag.OnEndDrag(eventData);
            activeCloneDrag = null;
        }
    }
}