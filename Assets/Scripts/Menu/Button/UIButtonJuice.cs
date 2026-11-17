using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class UIButtonJuice : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;
    private Vector3 originalScale;
    private Coroutine currentScaleRoutine;

    [Header("Juice Settings")]
    [SerializeField] private float shrinkScale = 0.8f;
    [SerializeField] private float animationDuration = 0.15f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StartScaleAnimation(originalScale * shrinkScale, animationDuration);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StartScaleAnimation(originalScale, animationDuration);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartScaleAnimation(originalScale * 1.05f, animationDuration);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartScaleAnimation(originalScale, animationDuration);
    }

    private void StartScaleAnimation(Vector3 targetScale, float duration)
    {
        if (currentScaleRoutine != null)
        {
            StopCoroutine(currentScaleRoutine);
        }
        currentScaleRoutine = StartCoroutine(ScaleRoutine(targetScale, duration));
    }

    private IEnumerator ScaleRoutine(Vector3 target, float duration)
    {
        float time = 0;
        Vector3 startScale = rectTransform.localScale;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / duration;
            rectTransform.localScale = Vector3.Lerp(startScale, target, t);
            yield return null;
        }

        rectTransform.localScale = target;
    }
}