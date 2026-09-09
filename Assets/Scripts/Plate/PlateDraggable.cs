using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class PlateDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas mainCanvas;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector2 startPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        mainCanvas = GetComponentInParent<Canvas>();
    }

    private void Start()
    {
        startPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.7f;
        canvasGroup.blocksRaycasts = false;
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
        if (gameObject == null || !gameObject.activeInHierarchy) return;

        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;

        rectTransform.anchoredPosition = startPosition;
    }

    public void ClearPlate()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}