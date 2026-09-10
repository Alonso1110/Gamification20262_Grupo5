using UnityEngine;

public class ConveyorPlate : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 150f;
    [SerializeField] private float endPositionX = 600f;

    [Header("Component References")]
    [SerializeField] private CanvasGroup canvasGroup;

    private RectTransform rectTransform;
    private bool isBeingDragged = false;
    private float currentXPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        }

        currentXPosition = rectTransform.anchoredPosition.x;
    }

    private void Update()
    {
        if (isBeingDragged) return;

        currentXPosition += moveSpeed * Time.deltaTime;

        Vector2 newPos = rectTransform.anchoredPosition;
        newPos.x = currentXPosition;
        rectTransform.anchoredPosition = newPos;

        if (currentXPosition >= endPositionX)
        {
            HandleBeltTimeout();
        }
    }

    private void HandleBeltTimeout()
    {
        Debug.Log("El plato llego al final de la cinta");

        Destroy(gameObject);

        if (PlateSpawner.Instance != null)
        {
            PlateSpawner.Instance.SpawnNewPlate();
        }
    }

    public void OnPickupPlate()
    {
        isBeingDragged = true;
    }

    public void OnReleasePlate()
    {
        isBeingDragged = false;

        Vector2 releasedPos = rectTransform.anchoredPosition;
        releasedPos.x = currentXPosition;
        rectTransform.anchoredPosition = releasedPos;
    }
}