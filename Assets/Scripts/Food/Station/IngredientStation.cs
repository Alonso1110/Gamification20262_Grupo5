using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IngredientStation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Stock Configuration")]
    [SerializeField] private int maxStock = 10;
    [SerializeField] private float timeToReload = 10f;

    private int currentStock;
    private bool isHoldingToReload = false;
    private float holdTimer = 0f;

    [Header("Visual References")]
    [SerializeField] private Image reloadFillImage;
    [SerializeField] private Image foodOverlayImage;

    private void Start()
    {
        if (foodOverlayImage == null)
        {
            foodOverlayImage = GetComponent<Image>();
        }

        currentStock = maxStock;
        UpdateVisuals();

        if (reloadFillImage != null)
        {
            reloadFillImage.fillAmount = 0f;
            reloadFillImage.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (isHoldingToReload && currentStock < maxStock)
        {
            holdTimer += Time.deltaTime;
            float progress = holdTimer / timeToReload;

            if (reloadFillImage != null)
            {
                if (!reloadFillImage.gameObject.activeSelf)
                    reloadFillImage.gameObject.SetActive(true);

                reloadFillImage.fillAmount = progress;
            }

            if (holdTimer >= timeToReload)
            {
                ReloadStation();
            }
        }
    }

    public bool TryTakeIngredient()
    {
        if (currentStock > 0)
        {
            currentStock--;
            UpdateVisuals();
            return true;
        }
        else
        {
            Debug.Log("Sin stock");
            return false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (currentStock < maxStock)
        {
            isHoldingToReload = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ResetHold();
    }

    private void ReloadStation()
    {
        currentStock = maxStock;
        UpdateVisuals();
        ResetHold();
        Debug.Log("Estación recargada al 100%");
    }

    private void ResetHold()
    {
        isHoldingToReload = false;
        holdTimer = 0f;

        if (reloadFillImage != null)
        {
            reloadFillImage.fillAmount = 0f;
            reloadFillImage.gameObject.SetActive(false);
        }
    }

    private void UpdateVisuals()
    {
        if (foodOverlayImage != null)
        {
            Color c = foodOverlayImage.color;

            c.a = (float)currentStock / maxStock;

            foodOverlayImage.color = c;
        }
    }
}