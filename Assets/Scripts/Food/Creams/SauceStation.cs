using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SauceStation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Identifier")]
    public string sauceID = "Ketchup";

    [Header("Stock")]
    [SerializeField] private float maxStock = 100f;
    [SerializeField] private bool isBottle = true;
    [SerializeField] private float currentStock;

    [Header("Reload")]
    [SerializeField] private float fullReloadTime = 10f;
    [SerializeField] private int fullReloadCost = 3;

    [Header("Visuals")]
    [SerializeField] private Image reloadFillImage;
    [SerializeField] private Image liquidLevelImage;

    private bool isHoldingToReload;
    private float holdTimer;
    private float reloadDuration;
    private int reloadCost;

    private float stockBeforeReload;

    private void Start()
    {
        currentStock = maxStock;
        UpdateVisuals();
        HideReloadBar();
    }

    private void Update()
    {
        if (!isHoldingToReload) return;

        holdTimer += Time.deltaTime;
        float progress = Mathf.Clamp01(holdTimer / reloadDuration);

        if (reloadFillImage != null)
        {
            if (!reloadFillImage.gameObject.activeSelf)
                reloadFillImage.gameObject.SetActive(true);

            reloadFillImage.fillAmount = progress;
        }

        if (liquidLevelImage != null)
        {
            float currentAnimatedStock = Mathf.Lerp(stockBeforeReload, maxStock, progress);
            float ratio = currentAnimatedStock / maxStock;

            if (isBottle)
            {
                liquidLevelImage.fillAmount = ratio;
            }
            else
            {
                liquidLevelImage.fillAmount = 1f;
                Color c = liquidLevelImage.color;
                c.a = ratio;
                liquidLevelImage.color = c;
            }
        }

        if (holdTimer >= reloadDuration)
            ReloadStation();
    }

    public bool TryConsumeSauce(float amount)
    {
        if (currentStock <= 0) return false;

        currentStock = Mathf.Max(0f, currentStock - amount);
        UpdateVisuals();
        return true;
    }

    public bool HasStock() => currentStock > 0;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (currentStock >= maxStock) return;

        float missing = (maxStock - currentStock) / maxStock;

        reloadDuration = Mathf.Max(0.5f, fullReloadTime * missing);
        reloadCost = fullReloadCost <= 0 ? 0 : Mathf.Max(1, Mathf.CeilToInt(fullReloadCost * missing));

        if (GameEconomy.Instance != null && !GameEconomy.Instance.CanAfford(reloadCost))
            return;

        stockBeforeReload = currentStock;

        holdTimer = 0f;
        isHoldingToReload = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ResetHold();
        UpdateVisuals();
    }

    public void CancelReload()
    {
        ResetHold();
        UpdateVisuals();
    }

    private void ReloadStation()
    {
        if (GameEconomy.Instance != null && !GameEconomy.Instance.TrySpend(reloadCost))
        {
            ResetHold();
            UpdateVisuals();
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
        if (liquidLevelImage == null) return;

        float ratio = currentStock / maxStock;

        if (isBottle)
        {
            liquidLevelImage.fillAmount = ratio;
        }
        else
        {
            liquidLevelImage.fillAmount = 1f;
            Color c = liquidLevelImage.color;
            c.a = ratio;
            liquidLevelImage.color = c;
        }
    }
}