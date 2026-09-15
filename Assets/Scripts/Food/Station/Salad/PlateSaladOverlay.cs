using UnityEngine;

public class PlateSaladOverlay : MonoBehaviour
{
    [Header("Salad Slot on Plate")]
    [SerializeField] private RectTransform saladSlot;

    private GameObject currentSaladInstance;

    public bool HasSalad()
    {
        return saladSlot != null && saladSlot.childCount > 0;
    }

    public bool ServeSalad(GameObject saladObject)
    {
        if (HasSalad())
        {
            Debug.Log("El plato ya tiene ensalada");
            return false;
        }

        if (saladObject != null && saladSlot != null)
        {
            saladObject.transform.SetParent(saladSlot, false);

            RectTransform rect = saladObject.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = Vector2.zero;
                rect.localScale = Vector3.one;
            }

            DraggableIngredient drag = saladObject.GetComponent<DraggableIngredient>();
            if (drag != null)
            {
                drag.LockInPlace();
            }

            return true;
        }

        return false;
    }

    public void ClearSalad()
    {
        if (saladSlot != null)
        {
            foreach (Transform child in saladSlot)
            {
                Destroy(child.gameObject);
            }
        }
    }
}