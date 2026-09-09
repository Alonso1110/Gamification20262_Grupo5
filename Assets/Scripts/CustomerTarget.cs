using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomerTarget : MonoBehaviour, IDropHandler
{
    [Header("IDs")]
    [SerializeField] private List<string> requiredIngredientIDs = new List<string>() { "Meow01", "Meow02" };

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject != null)
        {
            if (droppedObject.TryGetComponent<PlateDraggable>(out PlateDraggable plate))
            {
                ValidateOrder(plate);
            }
        }
    }

    private void ValidateOrder(PlateDraggable plate)
    {
        List<string> servedIngredientIDs = new List<string>();

        foreach (Transform child in plate.transform)
        {
            if (child.TryGetComponent<Ingredient>(out Ingredient ingredient))
            {
                servedIngredientIDs.Add(ingredient.IngredientID);
            }
        }

        if (AreListsEqual(servedIngredientIDs, requiredIngredientIDs))
        {
            Debug.Log("¡ORDEN CORRECTA!");
        }
        else
        {
            Debug.Log("¡ORDEN INCORRECTA!");
        }

        plate.gameObject.SetActive(false);
        Destroy(plate.gameObject);

        if (PlateSpawner.Instance != null)
        {
            PlateSpawner.Instance.SpawnNewPlate();
        }
    }

    private bool AreListsEqual(List<string> list1, List<string> list2)
    {
        if (list1.Count != list2.Count) return false;

        List<string> temp1 = new List<string>(list1);
        List<string> temp2 = new List<string>(list2);

        temp1.Sort();
        temp2.Sort();

        for (int i = 0; i < temp1.Count; i++)
        {
            if (temp1[i] != temp2[i]) return false;
        }

        return true;
    }
}