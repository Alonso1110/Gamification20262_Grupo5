
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableGlove : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
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

        if (TutorialManager.Instance != null)
            TutorialManager.Instance.GloveDragStarted();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition +=
            eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (cuttingBoard != null)
        {
            bool pickedUp = cuttingBoard.TryPickUpPiece(eventData);

            if (pickedUp && TutorialManager.Instance != null)
                TutorialManager.Instance.GlovePickUpCompleted();
        }

        rectTransform.anchoredPosition = startAnchoredPosition;
    }
}
