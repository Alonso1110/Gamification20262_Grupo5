using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChickenCuttingBoard : MonoBehaviour
{
    [Header("UI Panel & Buttons")]
    [SerializeField] private GameObject cuttingWindowPanel;
    [SerializeField] private Button closeButton;

    [Header("UI Visual Pieces")]
    [SerializeField] private GameObject wholeChickenUI;
    [SerializeField] private GameObject halfLeftUI;
    [SerializeField] private GameObject halfRightUI;
    [SerializeField] private GameObject quarterBreastLeftUI;
    [SerializeField] private GameObject quarterLegLeftUI;
    [SerializeField] private GameObject quarterBreastRightUI;
    [SerializeField] private GameObject quarterLegRightUI;

    [Header("Prefabs")]
    [SerializeField] private GameObject wholeChickenPrefab;
    [SerializeField] private GameObject halfLeftPrefab;
    [SerializeField] private GameObject halfRightPrefab;
    [SerializeField] private GameObject quarterBreastLeftPrefab;
    [SerializeField] private GameObject quarterLegLeftPrefab;
    [SerializeField] private GameObject quarterBreastRightPrefab;
    [SerializeField] private GameObject quarterLegRightPrefab;

    [Header("References")]
    [SerializeField] private CuttingBoardDropZone dropZone;

    private Canvas mainCanvas;

    private void Awake()
    {
        mainCanvas = GetComponentInParent<Canvas>();

        if (dropZone == null)
            dropZone = FindAnyObjectByType<CuttingBoardDropZone>();

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseBoard);

        if (cuttingWindowPanel != null)
            cuttingWindowPanel.SetActive(false);
    }

    public void ResetToWholeChicken()
    {
        if (wholeChickenUI) wholeChickenUI.SetActive(true);
        if (halfLeftUI) halfLeftUI.SetActive(false);
        if (halfRightUI) halfRightUI.SetActive(false);
        if (quarterBreastLeftUI) quarterBreastLeftUI.SetActive(false);
        if (quarterLegLeftUI) quarterLegLeftUI.SetActive(false);
        if (quarterBreastRightUI) quarterBreastRightUI.SetActive(false);
        if (quarterLegRightUI) quarterLegRightUI.SetActive(false);
    }

    public void OpenBoard()
    {
        if (cuttingWindowPanel != null)
            cuttingWindowPanel.SetActive(true);
    }

    public void CloseBoard()
    {
        if (cuttingWindowPanel != null)
            cuttingWindowPanel.SetActive(false);
    }

    public bool HasAnyPieceLeft()
    {
        return (wholeChickenUI != null && wholeChickenUI.activeSelf) ||
               (halfLeftUI != null && halfLeftUI.activeSelf) ||
               (halfRightUI != null && halfRightUI.activeSelf) ||
               (quarterBreastLeftUI != null && quarterBreastLeftUI.activeSelf) ||
               (quarterLegLeftUI != null && quarterLegLeftUI.activeSelf) ||
               (quarterBreastRightUI != null && quarterBreastRightUI.activeSelf) ||
               (quarterLegRightUI != null && quarterLegRightUI.activeSelf);
    }

    public void PerformVerticalCut(Vector2 localPosition)
    {
        if (wholeChickenUI != null && wholeChickenUI.activeSelf)
        {
            wholeChickenUI.SetActive(false);
            if (halfLeftUI) halfLeftUI.SetActive(true);
            if (halfRightUI) halfRightUI.SetActive(true);
        }
    }

    public void PerformHorizontalCut(Vector2 localPosition)
    {
        if (localPosition.x < 0)
        {
            if (halfLeftUI != null && halfLeftUI.activeSelf)
            {
                halfLeftUI.SetActive(false);
                if (quarterBreastLeftUI) quarterBreastLeftUI.SetActive(true);
                if (quarterLegLeftUI) quarterLegLeftUI.SetActive(true);
            }
        }
        else
        {
            if (halfRightUI != null && halfRightUI.activeSelf)
            {
                halfRightUI.SetActive(false);
                if (quarterBreastRightUI) quarterBreastRightUI.SetActive(true);
                if (quarterLegRightUI) quarterLegRightUI.SetActive(true);
            }
        }
    }

    public bool TryPickUpPiece(PointerEventData eventData)
    {
        if (TryTakePiece(quarterBreastLeftUI, quarterBreastLeftPrefab, eventData)) return true;
        if (TryTakePiece(quarterLegLeftUI, quarterLegLeftPrefab, eventData)) return true;
        if (TryTakePiece(quarterBreastRightUI, quarterBreastRightPrefab, eventData)) return true;
        if (TryTakePiece(quarterLegRightUI, quarterLegRightPrefab, eventData)) return true;

        if (TryTakePiece(halfLeftUI, halfLeftPrefab, eventData)) return true;
        if (TryTakePiece(halfRightUI, halfRightPrefab, eventData)) return true;

        if (TryTakePiece(wholeChickenUI, wholeChickenPrefab, eventData)) return true;

        return false;
    }

    private bool TryTakePiece(GameObject uiPiece, GameObject prefab, PointerEventData eventData)
    {
        if (uiPiece == null || !uiPiece.activeSelf || prefab == null) return false;

        RectTransform rect = uiPiece.GetComponent<RectTransform>();
        if (rect != null)
        {
            Camera camToUse = GetCanvasCamera();

            if (RectTransformUtility.RectangleContainsScreenPoint(rect, eventData.position, camToUse))
            {
                uiPiece.SetActive(false);
                SpawnPieceAtPointer(prefab, eventData);
                CloseBoard();

                if (dropZone == null) dropZone = FindAnyObjectByType<CuttingBoardDropZone>();

                if (!HasAnyPieceLeft() && dropZone != null)
                {
                    dropZone.ClearChickenOnBoard();
                }

                return true;
            }
        }

        return false;
    }

    private void SpawnPieceAtPointer(GameObject prefab, PointerEventData eventData)
    {
        if (mainCanvas == null) mainCanvas = GetComponentInParent<Canvas>();

        GameObject spawnedPiece = Instantiate(prefab, mainCanvas.transform);
        RectTransform rect = spawnedPiece.GetComponent<RectTransform>();

        if (rect != null)
        {
            Camera camToUse = GetCanvasCamera();

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                mainCanvas.transform as RectTransform,
                eventData.position,
                camToUse,
                out Vector2 localPoint
            );
            rect.anchoredPosition = localPoint;
        }
    }

    private Camera GetCanvasCamera()
    {
        if (mainCanvas == null) mainCanvas = GetComponentInParent<Canvas>();
        if (mainCanvas != null && mainCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            return null;
        }
        return mainCanvas != null ? mainCanvas.worldCamera : Camera.main;
    }
}