using UnityEngine;

public class PlateRiceOverlay : MonoBehaviour
{
    [Header("Rice Slot on Plate")]
    [SerializeField] private RectTransform riceSlot;

    public bool HasRice()
    {
        return riceSlot != null && riceSlot.childCount > 0;
    }

    public bool ServeRice(GameObject riceObject)
    {
        if (HasRice())
        {
            Debug.Log("El plato ya tiene arroz");
            return false;
        }

        if (riceObject != null && riceSlot != null)
        {
            riceObject.transform.SetParent(riceSlot, false);

            RectTransform rect = riceObject.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = Vector2.zero;
                rect.localScale = Vector3.one;
            }

            DraggableIngredient drag = riceObject.GetComponent<DraggableIngredient>();
            if (drag != null)
            {
                drag.LockInPlace();
            }

            return true;
        }

        return false;
    }

    public void ClearRice()
    {
        if (riceSlot != null)
        {
            foreach (Transform child in riceSlot)
            {
                Destroy(child.gameObject);
            }
        }
    }
}