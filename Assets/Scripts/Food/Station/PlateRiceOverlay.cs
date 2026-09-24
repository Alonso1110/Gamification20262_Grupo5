using UnityEngine;

public class PlateRiceOverlay : MonoBehaviour
{
    [Header("Slots")]
    [SerializeField] private RectTransform riceSlot;
    [SerializeField] private RectTransform extraRiceSlot;

    public bool ServeRice(GameObject riceObject)
    {
        if (riceSlot != null && riceSlot.childCount == 0)
        {
            SetToSlot(riceObject, riceSlot);
            return true;
        }
        else if (extraRiceSlot != null && extraRiceSlot.childCount == 0)
        {
            SetToSlot(riceObject, extraRiceSlot);
            return true;
        }

        Debug.Log("Ya no cabe más arroz en este plato");
        return false;
    }

    private void SetToSlot(GameObject obj, RectTransform slot)
    {
        obj.transform.SetParent(slot, false);
        RectTransform rect = obj.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchoredPosition = Vector2.zero;
            rect.localScale = Vector3.one;
        }

        DraggableIngredient drag = obj.GetComponent<DraggableIngredient>();
        if (drag != null) drag.LockInPlace();
    }

    public void ClearRice()
    {
        if (riceSlot != null)
            foreach (Transform child in riceSlot) Destroy(child.gameObject);

        if (extraRiceSlot != null)
            foreach (Transform child in extraRiceSlot) Destroy(child.gameObject);
    }
}