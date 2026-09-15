using UnityEngine;
using UnityEngine.UI;

public class PlateFriesOverlay : MonoBehaviour
{
    [Header("Container")]
    [SerializeField] private RectTransform friesSlot;

    private GameObject currentFriesInstance;

    public bool HasFries()
    {
        return friesSlot != null && friesSlot.childCount > 0;
    }

    public bool ServeFries(GameObject friesPrefab)
    {
        if (HasFries())
        {
            Debug.Log("El plato ya tiene papas");
            return false;
        }

        if (friesPrefab != null && friesSlot != null)
        {
            currentFriesInstance = Instantiate(friesPrefab, friesSlot);

            RectTransform rect = currentFriesInstance.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = Vector2.zero;
                rect.localScale = Vector3.one;
            }

            DraggableIngredient drag = currentFriesInstance.GetComponent<DraggableIngredient>();
            if (drag != null)
            {
                drag.LockInPlace();
            }

            Graphic graphic = currentFriesInstance.GetComponent<Graphic>();
            if (graphic != null)
            {
                graphic.raycastTarget = false;
            }

            CanvasGroup cg = currentFriesInstance.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.blocksRaycasts = false;
            }

            return true;
        }

        return false;
    }

    public void ClearFries()
    {
        if (friesSlot != null)
        {
            foreach (Transform child in friesSlot)
            {
                Destroy(child.gameObject);
            }
        }
    }
}