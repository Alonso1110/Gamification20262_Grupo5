using UnityEngine;
using System.Collections;

public class UIWindowPopUp : MonoBehaviour
{
    [SerializeField] private RectTransform windowRect;

    [Header("Pop-Up Settings")]
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private float overshootScale = 1f;

    private void OnEnable()
    {
        if (windowRect == null) windowRect = GetComponent<RectTransform>();

        StopAllCoroutines();
        StartCoroutine(PopOpenRoutine());
    }

    private IEnumerator PopOpenRoutine()
    {
        float time = 0;
        windowRect.localScale = Vector3.zero;

        while (time < animationDuration)
        {
            time += Time.unscaledDeltaTime;
            float progress = time / animationDuration;

            float scale = Mathf.Lerp(0f, overshootScale, progress);
            if (progress >= 1f) scale = 1f;

            windowRect.localScale = new Vector3(scale, scale, 1f);
            yield return null;
        }

        windowRect.localScale = Vector3.one;
    }

    public void CloseWindow()
    {
        StartCoroutine(PopCloseRoutine());
    }

    private IEnumerator PopCloseRoutine()
    {
        float time = 0;
        Vector3 startScale = windowRect.localScale;

        while (time < animationDuration)
        {
            time += Time.unscaledDeltaTime;
            float progress = time / animationDuration;
            float scale = Mathf.Lerp(1f, 0f, progress);

            windowRect.localScale = new Vector3(scale, scale, 1f);
            yield return null;
        }

        windowRect.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }
}