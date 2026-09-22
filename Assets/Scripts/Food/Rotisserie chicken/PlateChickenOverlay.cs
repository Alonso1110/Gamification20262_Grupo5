using UnityEngine;

public class PlateChickenOverlay : MonoBehaviour
{
    [Header("Chicken Slot")]
    [SerializeField] private RectTransform chickenSlot;

    public bool HasChicken()
    {
        return chickenSlot != null && chickenSlot.childCount > 0;
    }

    public bool ServeChicken(GameObject chickenObject)
    {
        if (HasChicken())
        {
            Debug.Log("El plato ya tiene pollo");
            return false;
        }

        if (chickenObject != null && chickenSlot != null)
        {
            chickenObject.transform.SetParent(chickenSlot, false);

            RectTransform rect = chickenObject.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = Vector2.zero;
                rect.localScale = Vector3.one;
            }

            DraggableIngredient drag = chickenObject.GetComponent<DraggableIngredient>();
            if (drag != null)
            {
                drag.LockInPlace();
            }

            return true;
        }

        return false;
    }

    public void ClearChicken()
    {
        if (chickenSlot != null)
        {
            foreach (Transform child in chickenSlot)
            {
                Destroy(child.gameObject);
            }
        }
    }
}