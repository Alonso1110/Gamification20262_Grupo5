using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableKnife : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("References")]
    [SerializeField] private ChickenCuttingBoard cuttingBoard;
    [SerializeField] private RectTransform chickenContainer;

    [Header("Cut Sensitivity")]
    [SerializeField] private float minCutDistance = 70f;

    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 startAnchoredPosition;
    private Vector2 dragStartPosition;
    private bool isCutting = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startAnchoredPosition = rectTransform.anchoredPosition;
        dragStartPosition = eventData.position;
        isCutting = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        if (RectTransformUtility.RectangleContainsScreenPoint(chickenContainer, eventData.position, eventData.pressEventCamera))
        {
            if (!isCutting)
            {
                dragStartPosition = eventData.position;
                isCutting = true;
            }
            else
            {
                Vector2 delta = eventData.position - dragStartPosition;

                if (delta.magnitude >= minCutDistance)
                {
                    EvaluateCutDirection(delta, eventData.position, eventData.pressEventCamera);
                    dragStartPosition = eventData.position;
                }
            }
        }
        else
        {
            isCutting = false;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition = startAnchoredPosition;
        isCutting = false;
    }

    private void EvaluateCutDirection(Vector2 delta, Vector2 screenPoint, Camera cam)
    {
        float absX = Mathf.Abs(delta.x);
        float absY = Mathf.Abs(delta.y);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(chickenContainer, screenPoint, cam, out Vector2 localPoint);

        if (absY > absX)
        {
            cuttingBoard.PerformVerticalCut(localPoint);
        }
        else if (absX > absY)
        {
            cuttingBoard.PerformHorizontalCut(localPoint);
        }
    }
}