using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDropItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI Configuration")]
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Drag Settings")]
    [Range(0.1f, 1f)]
    [SerializeField] private float dragAlpha = 0.6f;
    [SerializeField] private bool returnToOrigin = true;

    private RectTransform rectTransform;
    private Vector2 startPosition;
    private Transform originalParent;

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
        startPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;

        canvasGroup.alpha = dragAlpha;
        canvasGroup.blocksRaycasts = false;

        Debug.Log("Inicio de arrastre: " + gameObject.name);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (mainCanvas != null)
        {
            rectTransform.anchoredPosition += eventData.delta / mainCanvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;

        if (returnToOrigin && transform.parent == originalParent)
        {
            rectTransform.anchoredPosition = startPosition;
            Debug.Log("Objeto devuelto al origen: " + gameObject.name);
        }
    }
}
