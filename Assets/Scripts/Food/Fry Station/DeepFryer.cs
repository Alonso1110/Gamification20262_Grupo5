using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class DeepFryer : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public enum FryState { Empty, Raw, Cooked, Burnt }

    [Header("Current status")]
    public FryState currentState = FryState.Empty;
    public bool isBasketDown = false;

    [Header("Cooking Times")]
    public float timeToCook = 5f;
    public float timeToBurn = 10f;
    private float timer = 0f;

    [Header("Potato Prefabs")]
    [SerializeField] private GameObject rawFriesPrefab;
    [SerializeField] private GameObject cookedFriesPrefab;
    [SerializeField] private GameObject burntFriesPrefab;

    [Header("Basket Visuals Container")]
    [SerializeField] private RectTransform basketFriesSlot;

    [Header("Basket Positions")]
    [SerializeField] private RectTransform basketRectTransform;
    [SerializeField] private float upYPosition = 0f;
    [SerializeField] private float downYPosition = -50f;

    private Canvas mainCanvas;
    private CanvasGroup canvasGroup;
    private Vector2 originalAnchoredPos;
    private bool isDragging = false;
    private GameObject currentBasketFriesInstance;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        mainCanvas = GetComponentInParent<Canvas>();
        if (basketRectTransform == null) basketRectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        originalAnchoredPos = basketRectTransform.anchoredPosition;
        UpdateVisuals();
        MoveBasket(false);
    }

    private void Update()
    {
        if (isBasketDown && currentState != FryState.Empty && currentState != FryState.Burnt)
        {
            timer += Time.deltaTime;

            if (timer >= timeToBurn && currentState == FryState.Cooked)
            {
                currentState = FryState.Burnt;
                UpdateVisuals();
            }
            else if (timer >= timeToCook && currentState == FryState.Raw)
            {
                currentState = FryState.Cooked;
                UpdateVisuals();
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isDragging) return;

        if (currentState == FryState.Empty && !isBasketDown)
        {
            currentState = FryState.Raw;
            timer = 0f;
            UpdateVisuals();
            Debug.Log("Papas crudas añadidas");
        }
        else
        {
            isBasketDown = !isBasketDown;
            MoveBasket(isBasketDown);
        }
    }

    private void MoveBasket(bool goDown)
    {
        if (basketRectTransform != null && !isDragging)
        {
            Vector2 pos = basketRectTransform.anchoredPosition;
            pos.y = goDown ? downYPosition : upYPosition;
            basketRectTransform.anchoredPosition = pos;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isBasketDown || currentState == FryState.Empty) return;

        isDragging = true;
        canvasGroup.blocksRaycasts = false;
        originalAnchoredPos = basketRectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || mainCanvas == null) return;

        basketRectTransform.anchoredPosition += eventData.delta / mainCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        canvasGroup.blocksRaycasts = true;

        CheckDropTarget();

        basketRectTransform.anchoredPosition = originalAnchoredPos;
        isDragging = false;
    }

    private void CheckDropTarget()
    {
        foreach (var plate in PlateSauceOverlay.AllPlates)
        {
            if (plate == null || !plate.gameObject.activeInHierarchy) continue;

            if (RectTransformsOverlap(basketRectTransform, plate.GetComponent<RectTransform>()))
            {
                PlateFriesOverlay friesOverlay = plate.GetComponent<PlateFriesOverlay>();
                if (friesOverlay != null)
                {
                    if (friesOverlay.ServeFries(GetCurrentPrefab()))
                    {
                        Debug.Log($"Papas ({currentState}) servidas en el plato");
                        EmptyBasket();
                    }
                }
                return;
            }
        }

        GameObject trashCan = GameObject.Find("TrashCan");
        if (trashCan != null)
        {
            if (RectTransformsOverlap(basketRectTransform, trashCan.GetComponent<RectTransform>()))
            {
                Debug.Log("Papas descartadas en el tacho de basura");
                EmptyBasket();
                return;
            }
        }
    }

    private void EmptyBasket()
    {
        currentState = FryState.Empty;
        timer = 0f;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (currentBasketFriesInstance != null)
        {
            Destroy(currentBasketFriesInstance);
        }

        GameObject prefabToSpawn = GetCurrentPrefab();
        if (currentState != FryState.Empty && prefabToSpawn != null && basketFriesSlot != null)
        {
            currentBasketFriesInstance = Instantiate(prefabToSpawn, basketFriesSlot);

            RectTransform rect = currentBasketFriesInstance.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = Vector2.zero;
                rect.localScale = Vector3.one;
            }
        }
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

    public GameObject GetCurrentPrefab()
    {
        if (currentState == FryState.Raw) return rawFriesPrefab;
        if (currentState == FryState.Cooked) return cookedFriesPrefab;
        if (currentState == FryState.Burnt) return burntFriesPrefab;
        return null;
    }
}