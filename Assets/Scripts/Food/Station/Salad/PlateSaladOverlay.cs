using UnityEngine;

public class PlateSaladOverlay : MonoBehaviour
{
    [Header("Slots")]
    [SerializeField] private RectTransform saladSlot;
    [SerializeField] private RectTransform extraSaladSlot;

    public bool ServeSalad(GameObject saladObject)
    {
        if (saladSlot != null && saladSlot.childCount == 0)
        {
            SetToSlot(saladObject, saladSlot);
            return true;
        }
        else if (extraSaladSlot != null && extraSaladSlot.childCount == 0)
        {
            SetToSlot(saladObject, extraSaladSlot);
            return true;
        }

        Debug.Log("Ya no cabe más ensalada en este plato");
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

    public void ClearSalad()
    {
        if (saladSlot != null)
            foreach (Transform child in saladSlot) Destroy(child.gameObject);

        if (extraSaladSlot != null)
            foreach (Transform child in extraSaladSlot) Destroy(child.gameObject);
    }
}