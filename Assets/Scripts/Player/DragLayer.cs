using UnityEngine;
using UnityEngine.UI;

public static class DragLayer
{
    public static void Raise(GameObject go, int order = 500)
    {
        Canvas c = go.GetComponent<Canvas>();
        if (c == null) c = go.AddComponent<Canvas>();
        c.overrideSorting = true;
        c.sortingOrder = order;
    }

    public static void Restore(GameObject go)
    {
        GraphicRaycaster gr = go.GetComponent<GraphicRaycaster>();
        if (gr != null) Object.Destroy(gr);

        Canvas c = go.GetComponent<Canvas>();
        if (c != null) Object.Destroy(c);
    }
}
