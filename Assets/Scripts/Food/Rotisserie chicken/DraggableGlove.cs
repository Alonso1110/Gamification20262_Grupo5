using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableGlove : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("References")]
    [SerializeField] private ChickenCuttingBoard cuttingBoard;

    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 startAnchoredPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startAnchoredPosition = rectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (cuttingBoard != null)
        {
            cuttingBoard.TryPickUpPiece(eventData);
        }
        rectTransform.anchoredPosition = startAnchoredPosition;
    }
}