using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class DraggableIngredient : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas mainCanvas;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private bool isLocked = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        mainCanvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isLocked) return;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isLocked || mainCanvas == null) return;
        rectTransform.anchoredPosition += eventData.delta / mainCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isLocked) return;
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;

        if (transform.parent == mainCanvas.transform)
        {
            Destroy(gameObject);
        }
    }

    public void LockInPlace()
    {
        isLocked = true;
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1.0f;
            canvasGroup.blocksRaycasts = false;
        }
        this.enabled = false;
    }
}