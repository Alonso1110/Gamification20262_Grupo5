using UnityEngine;
using UnityEngine.UI;

public class ReputationBar : MonoBehaviour
{
    [SerializeField] private Image fill;
    [SerializeField] private Color fullColor = new Color(0.35f, 0.75f, 0.35f);
    [SerializeField] private Color emptyColor = new Color(0.85f, 0.25f, 0.25f);

    private void Update()
    {
        if (fill == null || ReputationManager.Instance == null) return;

        float n = ReputationManager.Instance.Normalized;
        fill.fillAmount = n;
        fill.color = Color.Lerp(emptyColor, fullColor, n);
    }
}