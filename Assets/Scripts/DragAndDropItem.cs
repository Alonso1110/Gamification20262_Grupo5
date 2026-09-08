using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class DragAndDropItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI Configuration")]
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Drag Settings")]
    [Range(0.1f, 1f)]
    [SerializeField] private float dragAlpha = 0.6f;
    [SerializeField] private bool isSpawner = true;

    private RectTransform rectTransform;
    private Vector2 startPosition;
    private Transform originalParent;
    private bool isLockedInPlate = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        }

        if (mainCanvas == null)
        {
            mainCanvas = GetComponentInParent<Canvas>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isLockedInPlate) return;

        if (isSpawner)
        {
            GameObject clone = Instantiate(gameObject, mainCanvas.transform);

            DragAndDropItem cloneDrag = clone.GetComponent<DragAndDropItem>();
            cloneDrag.isSpawner = false;
            cloneDrag.mainCanvas = mainCanvas;

            eventData.pointerDrag = clone;
            cloneDrag.OnBeginDrag(eventData);

            return;
        }

        startPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;

        canvasGroup.alpha = dragAlpha;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isLockedInPlate || isSpawner) return;

        if (mainCanvas != null)
        {
            rectTransform.anchoredPosition += eventData.delta / mainCanvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isLockedInPlate || isSpawner) return;

        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;

        if (transform.parent == mainCanvas.transform)
        {
            Destroy(gameObject);
        }
    }

    public void LockInPlace()
    {
        isLockedInPlate = true;
        canvasGroup.blocksRaycasts = true;
    }
}