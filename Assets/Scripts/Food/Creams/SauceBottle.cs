using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class SauceBottle : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private SauceStation station;
    [SerializeField] private float flowRate = 0.5f;

    [Header("Tilt Effect")]
    [SerializeField] private float tiltAngle = -45f;
    [SerializeField] private float tiltSpeed = 12f;

    private Canvas mainCanvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Transform originalParent;
    private int originalSiblingIndex;
    private bool isOverPlate = false;

    private GameObject layoutPlaceholder;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        mainCanvas = GetComponentInParent<Canvas>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        if (station == null) station = GetComponent<SauceStation>();
    }

    private void Start()
    {
        originalParent = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex();
    }

    private void Update()
    {
        float targetZ = isOverPlate ? tiltAngle : 0f;
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetZ);
        rectTransform.localRotation = Quaternion.Lerp(rectTransform.localRotation, targetRotation, Time.deltaTime * tiltSpeed);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (station != null)
        {
            station.CancelReload();
        }

        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false;
        }

        originalParent = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex();

        layoutPlaceholder = new GameObject("BottlePlaceholder", typeof(RectTransform), typeof(LayoutElement));

        RectTransform placeholderRect = layoutPlaceholder.GetComponent<RectTransform>();
        Vector2 currentSize = rectTransform.sizeDelta;
        placeholderRect.sizeDelta = currentSize;

        LayoutElement le = layoutPlaceholder.GetComponent<LayoutElement>();
        le.preferredWidth = currentSize.x;
        le.preferredHeight = currentSize.y;
        le.minWidth = currentSize.x;
        le.minHeight = currentSize.y;
        le.flexibleWidth = 0;
        le.flexibleHeight = 0;

        layoutPlaceholder.transform.SetParent(originalParent, false);
        layoutPlaceholder.transform.SetSiblingIndex(originalSiblingIndex);

        if (mainCanvas != null)
        {
            transform.SetParent(mainCanvas.transform, true);
            transform.SetAsLastSibling();
        }

        if (originalParent.TryGetComponent<RectTransform>(out RectTransform parentRect))
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (mainCanvas == null) return;

        rectTransform.anchoredPosition += eventData.delta / mainCanvas.scaleFactor;

        CheckAndPourSauce(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true;
        }

        isOverPlate = false;

        if (originalParent != null)
        {
            transform.SetParent(originalParent, false);
            transform.SetSiblingIndex(originalSiblingIndex);

            if (layoutPlaceholder != null)
            {
                Destroy(layoutPlaceholder);
            }

            if (originalParent.TryGetComponent<RectTransform>(out RectTransform parentRect))
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);
            }
        }

        rectTransform.localRotation = Quaternion.identity;
    }

    private void CheckAndPourSauce(PointerEventData eventData)
    {
        bool foundPlate = false;

        foreach (var plate in PlateSauceOverlay.AllPlates)
        {
            if (plate == null || !plate.gameObject.activeInHierarchy) continue;

            if (RectTransformsOverlap(rectTransform, plate.GetComponent<RectTransform>()))
            {
                foundPlate = true;

                float sauceAmount = flowRate * Time.deltaTime * 20f;

                if (station != null && station.TryConsumeSauce(sauceAmount))
                {
                    plate.AddSauce(station.sauceID, flowRate * Time.deltaTime);
                }
                break;
            }
        }

        isOverPlate = foundPlate;
    }

    private bool RectTransformsOverlap(RectTransform a, RectTransform b)
    {
        Vector3[] aCorners = new Vector3[4];
        Vector3[] bCorners = new Vector3[4];

        a.GetWorldCorners(aCorners);
        b.GetWorldCorners(bCorners);

        Rect aRect = new Rect(aCorners[0].x, aCorners[0].y, aCorners[2].x - aCorners[0].x, aCorners[2].y - aCorners[0].y);
        Rect bRect = new Rect(bCorners[0].x, bCorners[0].y, bCorners[2].x - bCorners[0].x, bCorners[2].y - bCorners[0].y);

        return aRect.Overlaps(bRect);
    }
}