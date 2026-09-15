using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlateSauceOverlay : MonoBehaviour
{
    public static List<PlateSauceOverlay> AllPlates = new List<PlateSauceOverlay>();

    [System.Serializable]
    public struct SauceData
    {
        public string sauceID;
        public Image sauceImage;
    }

    [Header("Layers of Sauces")]
    [SerializeField] private List<SauceData> sauces = new List<SauceData>();

    private Dictionary<string, Image> sauceMap = new Dictionary<string, Image>();

    private void OnEnable() => AllPlates.Add(this);
    private void OnDisable() => AllPlates.Remove(this);

    private void Awake()
    {
        foreach (var s in sauces)
        {
            if (s.sauceImage != null && !sauceMap.ContainsKey(s.sauceID))
            {
                sauceMap.Add(s.sauceID, s.sauceImage);

                Color c = s.sauceImage.color;
                c.a = 0f;
                s.sauceImage.color = c;
            }
        }
    }

    public void AddSauce(string sauceID, float amount)
    {
        if (sauceMap.TryGetValue(sauceID, out Image sauceImg))
        {
            Color c = sauceImg.color;
            if (c.a < 1.0f)
            {
                c.a = Mathf.Min(1.0f, c.a + amount);
                sauceImg.color = c;
            }

            sauceImg.transform.SetAsLastSibling();
        }
    }

    public void ClearSauces()
    {
        foreach (var kvp in sauceMap)
        {
            Color c = kvp.Value.color;
            c.a = 0f;
            kvp.Value.color = c;
        }
    }
}