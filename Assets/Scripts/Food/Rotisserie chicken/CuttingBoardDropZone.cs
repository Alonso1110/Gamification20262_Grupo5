using UnityEngine;
using UnityEngine.EventSystems;

public class CuttingBoardDropZone : MonoBehaviour, IDropHandler, IPointerDownHandler
{
    [Header("References")]
    [SerializeField] private ChickenCuttingBoard cuttingBoardUI;
    [SerializeField] private RectTransform chickenSlot;

    [Header("Chicken Prefab")]
    [SerializeField] private GameObject wholeChickenPrefab;

    private GameObject currentChickenOnBoard;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        if (dropped == null) return;

        if (dropped.TryGetComponent<Ingredient>(out Ingredient ingredient))
        {
            string id = ingredient.IngredientID.ToLower();
            if (id.Contains("chicken") || id.Contains("pollo"))
            {
                CanvasGroup dropCg = dropped.GetComponent<CanvasGroup>();
                if (dropCg == null) dropCg = dropped.AddComponent<CanvasGroup>();
                dropCg.alpha = 0f;
                dropCg.blocksRaycasts = false;
                Destroy(dropped, 0.05f);

                ClearChickenOnBoard();

                GameObject prefabToUse = (wholeChickenPrefab != null) ? wholeChickenPrefab : ingredient.gameObject;
                currentChickenOnBoard = Instantiate(prefabToUse, chickenSlot);

                RectTransform rect = currentChickenOnBoard.GetComponent<RectTransform>();
                if (rect != null)
                {
                    rect.anchoredPosition = Vector2.zero;
                    rect.localScale = Vector3.one;
                }

                DraggableIngredient drag = currentChickenOnBoard.GetComponent<DraggableIngredient>();
                if (drag != null) Destroy(drag);

                CanvasGroup boardCg = currentChickenOnBoard.GetComponent<CanvasGroup>();
                if (boardCg == null) boardCg = currentChickenOnBoard.AddComponent<CanvasGroup>();
                boardCg.blocksRaycasts = false;

                if (cuttingBoardUI != null)
                {
                    cuttingBoardUI.ResetToWholeChicken();
                }
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (currentChickenOnBoard != null && cuttingBoardUI != null && cuttingBoardUI.HasAnyPieceLeft())
        {
            cuttingBoardUI.OpenBoard();
        }
    }

    public void ClearChickenOnBoard()
    {
        if (currentChickenOnBoard != null)
        {
            Destroy(currentChickenOnBoard);
            currentChickenOnBoard = null;
        }

        if (chickenSlot != null)
        {
            foreach (Transform child in chickenSlot)
            {
                Destroy(child.gameObject);
            }
        }
    }
}