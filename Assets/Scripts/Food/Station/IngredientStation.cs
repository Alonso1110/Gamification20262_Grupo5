using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IngredientStation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Stock")]
    [SerializeField] private int maxStock = 10;

    [Header("Reload")]
    [SerializeField] private float fullReloadTime = 10f;
    [SerializeField] private int fullReloadCost = 6;

    [Header("Visuals")]
    [SerializeField] private Image reloadFillImage;
    [SerializeField] private Image foodOverlayImage;

    private int currentStock;
    private bool isHoldingToReload;
    private float holdTimer;
    private float reloadDuration;
    private int reloadCost;

    private void Start()
    {
        if (foodOverlayImage == null)
            foodOverlayImage = GetComponent<Image>();

        currentStock = maxStock;
        UpdateVisuals();
        HideReloadBar();
    }

    private void Update()
    {
        if (!isHoldingToReload) return;

        holdTimer += Time.deltaTime;

        if (reloadFillImage != null)
        {
            if (!reloadFillImage.gameObject.activeSelf)
                reloadFillImage.gameObject.SetActive(true);

            reloadFillImage.fillAmount = Mathf.Clamp01(holdTimer / reloadDuration);
        }

        if (holdTimer >= reloadDuration)
            ReloadStation();
    }

    public bool TryTakeIngredient()
    {
        if (currentStock <= 0) return false;

        currentStock--;
        UpdateVisuals();
        return true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (currentStock >= maxStock) return;

        float missing = (float)(maxStock - currentStock) / maxStock;

        reloadDuration = Mathf.Max(0.5f, fullReloadTime * missing);
        reloadCost = fullReloadCost <= 0 ? 0 : Mathf.Max(1, Mathf.CeilToInt(fullReloadCost * missing));

        if (GameEconomy.Instance != null && !GameEconomy.Instance.CanAfford(reloadCost))
            return;

        holdTimer = 0f;
        isHoldingToReload = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ResetHold();
    }

    private void ReloadStation()
    {
        if (GameEconomy.Instance != null && !GameEconomy.Instance.TrySpend(reloadCost))
        {
            ResetHold();
            return;
        }

        currentStock = maxStock;
        UpdateVisuals();
        ResetHold();
    }

    private void ResetHold()
    {
        isHoldingToReload = false;
        holdTimer = 0f;
        HideReloadBar();
    }

    private void HideReloadBar()
    {
        if (reloadFillImage == null) return;

        reloadFillImage.fillAmount = 0f;
        reloadFillImage.gameObject.SetActive(false);
    }

    private void UpdateVisuals()
    {
        if (foodOverlayImage == null) return;

        Color c = foodOverlayImage.color;
        c.a = (float)currentStock / maxStock;
        foodOverlayImage.color = c;
    }
}