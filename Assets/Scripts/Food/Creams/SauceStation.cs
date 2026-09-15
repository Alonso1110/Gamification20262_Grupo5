using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SauceStation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Identifier")]
    public string sauceID = "Ketchup";

    [Header("Stock Configuration")]
    [SerializeField] private float maxStock = 100f;
    [SerializeField] private float timeToReload = 10f;
    [SerializeField] private bool isBottle = true;

    [Header("Real time")]
    [SerializeField] private float currentStock;

    private bool isHoldingToReload = false;
    private float holdTimer = 0f;

    [Header("Visual References")]
    [SerializeField] private Image reloadFillImage;
    [SerializeField] private Image liquidLevelImage;

    private void Start()
    {
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

    public bool TryConsumeSauce(float amount)
    {
        if (currentStock > 0)
        {
            currentStock -= amount;
            if (currentStock < 0) currentStock = 0;
            UpdateVisuals();
            return true;
        }
        return false;
    }

    public bool HasStock() => currentStock > 0;

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

    public void CancelReload()
    {
        ResetHold();
    }

    private void ReloadStation()
    {
        currentStock = maxStock;
        UpdateVisuals();
        ResetHold();
        Debug.Log($"¡Salsa {sauceID} recargada!");
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
        float ratio = currentStock / maxStock;

        if (liquidLevelImage != null)
        {
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
}