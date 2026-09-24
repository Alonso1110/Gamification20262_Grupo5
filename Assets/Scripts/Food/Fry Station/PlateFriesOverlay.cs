using UnityEngine;
using UnityEngine.UI;

public class PlateFriesOverlay : MonoBehaviour
{
    [Header("Containers")]
    [SerializeField] private RectTransform friesSlot;
    [SerializeField] private RectTransform extraFriesSlot;

    public bool ServeFries(GameObject friesPrefab)
    {
        if (friesPrefab == null) return false;

        if (friesSlot != null && friesSlot.childCount == 0)
        {
            InstantiateFriesInSlot(friesPrefab, friesSlot);
            return true;
        }

        else if (extraFriesSlot != null && extraFriesSlot.childCount == 0)
        {
            InstantiateFriesInSlot(friesPrefab, extraFriesSlot);
            return true;
        }

        Debug.Log("El plato ya tiene el máximo de papas permitido");
        return false;
    }

    private void InstantiateFriesInSlot(GameObject friesPrefab, RectTransform slot)
    {
        GameObject instance = Instantiate(friesPrefab, slot);

        RectTransform rect = instance.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchoredPosition = Vector2.zero;
            rect.localScale = Vector3.one;
        }

        DraggableIngredient drag = instance.GetComponent<DraggableIngredient>();
        if (drag != null)
        {
            drag.LockInPlace();
        }

        Graphic graphic = instance.GetComponent<Graphic>();
        if (graphic != null)
        {
            graphic.raycastTarget = false;
        }

        CanvasGroup cg = instance.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.blocksRaycasts = false;
        }
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

        if (extraFriesSlot != null)
        {
            foreach (Transform child in extraFriesSlot)
            {
                Destroy(child.gameObject);
            }
        }
    }
}