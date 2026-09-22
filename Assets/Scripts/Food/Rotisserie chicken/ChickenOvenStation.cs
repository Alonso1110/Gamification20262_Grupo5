using UnityEngine;
using UnityEngine.EventSystems;

public class ChickenOvenStation : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Chicken Prefab")]
    [SerializeField] private GameObject wholeChickenPrefab;

    private Canvas mainCanvas;
    private DraggableIngredient activeCloneDrag;

    private void Awake()
    {
        mainCanvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (wholeChickenPrefab == null || mainCanvas == null) return;

        GameObject clone = Instantiate(wholeChickenPrefab, mainCanvas.transform);
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
        if (activeCloneDrag != null && activeCloneDrag.gameObject != null && activeCloneDrag.gameObject.activeInHierarchy)
        {
            activeCloneDrag.OnEndDrag(eventData);
        }
        activeCloneDrag = null;
    }
}