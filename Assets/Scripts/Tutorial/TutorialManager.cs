
using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("Indicator")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private RectTransform fingerIndicator;

    [Header("Route points")]
    [SerializeField] private RectTransform[] targets;
    [SerializeField] private float moveDuration = 0.7f;
    [SerializeField] private float waitAtTarget = 1.2f;
    [SerializeField] private float waitBeforeRepeat = 0.4f;

    [Header("Animation")]
    [SerializeField] private float tapScale = 0.8f;
    [SerializeField] private float tapDuration = 0.15f;
    [SerializeField] private float waitBetweenTaps = 0.2f;

    [Header("Knife tutorial")]
    [SerializeField] private RectTransform knifeTarget;
    [SerializeField] private RectTransform verticalStartTarget;
    [SerializeField] private RectTransform verticalEndTarget;
    [SerializeField] private RectTransform horizontalStartTarget;
    [SerializeField] private RectTransform horizontalEndTarget;

    [Header("Glove tutorial")]
    [SerializeField] private RectTransform gloveTarget;
    [SerializeField] private RectTransform legTarget;

    [Header("Plate tutorial")]
    [SerializeField] private RectTransform chickenOnHandTarget;
    [SerializeField] private RectTransform plateTarget;

    private bool chickenPlacedOnPlate;

    private bool gloveDragging;
    private bool legPickedUp;

    private bool horizontalCutDone;

    private Coroutine guideRoutine;
    private bool chickenPlaced;
    private Vector3 originalScale;
    private bool boardOpened;

    private bool knifeDragging;
    private bool verticalCutDone;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (tutorialPanel != null)
            tutorialPanel.SetActive(true);

        if (fingerIndicator == null ||
            targets == null ||
            targets.Length < 2 ||
            targets[0] == null ||
            targets[1] == null)
        {
            Debug.LogWarning(
                "Tutorial: asigna el dedo y los dos puntos."
            );
            return;
        }

        originalScale = fingerIndicator.localScale;
        chickenPlaced = false;

        guideRoutine = StartCoroutine(GuideLoop());
    }

    private IEnumerator GuideLoop()
    {
        while (!chickenPlaced)
        {
            fingerIndicator.position = targets[0].position;
            fingerIndicator.localScale = originalScale;
            fingerIndicator.gameObject.SetActive(true);

            yield return new WaitForSecondsRealtime(0.3f);

            Vector3 start = targets[0].position;
            Vector3 end = targets[1].position;
            float elapsed = 0f;

            while (elapsed < moveDuration)
            {
                if (chickenPlaced)
                    yield break;

                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / moveDuration);
                t = t * t * (3f - 2f * t);

                fingerIndicator.position =
                    Vector3.Lerp(start, end, t);

                yield return null;
            }

            fingerIndicator.position = end;

            float waited = 0f;

            while (waited < waitAtTarget && !chickenPlaced)
            {
                waited += Time.unscaledDeltaTime;
                yield return null;
            }

            if (chickenPlaced)
                yield break;

            fingerIndicator.gameObject.SetActive(false);

            yield return new WaitForSecondsRealtime(
                waitBeforeRepeat
            );
        }
    }

    public void ChickenPlacedOnBoard()
    {
        if (chickenPlaced)
            return;

        chickenPlaced = true;

        if (guideRoutine != null)
            StopCoroutine(guideRoutine);

        if (fingerIndicator != null && targets.Length >= 2)
        {
            guideRoutine = StartCoroutine(TapLoop());
        }
    }

    private IEnumerator TapLoop()
    {
        fingerIndicator.position = targets[1].position;
        fingerIndicator.localScale = originalScale;
        fingerIndicator.gameObject.SetActive(true);

        while (true)
        {
            yield return ScaleFinger(
                originalScale * tapScale,
                tapDuration
            );

            yield return ScaleFinger(
                originalScale,
                tapDuration
            );

            yield return new WaitForSecondsRealtime(
                waitBetweenTaps
            );
        }
    }

    public void BoardOpened()
    {
        if (boardOpened)
            return;

        boardOpened = true;

        if (guideRoutine != null)
            StopCoroutine(guideRoutine);

        if (fingerIndicator != null)
            guideRoutine = StartCoroutine(KnifeBlinkLoop());
    }

    private IEnumerator ScaleFinger(
        Vector3 destination,
        float duration)
    {
        Vector3 start = fingerIndicator.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            fingerIndicator.localScale =
                Vector3.Lerp(start, destination, t);

            yield return null;
        }

        fingerIndicator.localScale = destination;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private IEnumerator KnifeBlinkLoop()
    {
        fingerIndicator.position = knifeTarget.position;
        fingerIndicator.localScale = originalScale;
        fingerIndicator.gameObject.SetActive(true);

        while (!knifeDragging)
        {
            yield return ScaleFinger(
                originalScale * tapScale,
                tapDuration
            );

            yield return ScaleFinger(
                originalScale,
                tapDuration
            );

            yield return new WaitForSecondsRealtime(
                waitBetweenTaps
            );
        }
    }

    public void KnifeDragStarted()
    {
        if (knifeDragging)
            return;

        knifeDragging = true;

        if (guideRoutine != null)
            StopCoroutine(guideRoutine);

        fingerIndicator.gameObject.SetActive(true);
        guideRoutine = StartCoroutine(VerticalSwipeLoop());
    }

    private IEnumerator VerticalSwipeLoop()
    {
        while (!verticalCutDone)
        {
            fingerIndicator.position = verticalStartTarget.position;

            Vector3 start = verticalStartTarget.position;
            Vector3 end = verticalEndTarget.position;

            float elapsed = 0f;

            while (elapsed < moveDuration && !verticalCutDone)
            {
                elapsed += Time.unscaledDeltaTime;

                float t = Mathf.Clamp01(elapsed / moveDuration);

                fingerIndicator.position = Vector3.Lerp(start, end, t);

                yield return null;
            }

            yield return new WaitForSecondsRealtime(0.5f);
        }

        fingerIndicator.gameObject.SetActive(false);
    }

    public void VerticalCutCompleted()
    {
        verticalCutDone = true;

        if (guideRoutine != null)
            StopCoroutine(guideRoutine);

        guideRoutine = StartCoroutine(HorizontalSwipeLoop());
    }

    private IEnumerator HorizontalSwipeLoop()
    {
        while (!horizontalCutDone)
        {
            fingerIndicator.position = horizontalStartTarget.position;

            Vector3 start = horizontalStartTarget.position;
            Vector3 end = horizontalEndTarget.position;

            float elapsed = 0f;

            while (elapsed < moveDuration && !horizontalCutDone)
            {
                elapsed += Time.unscaledDeltaTime;

                float t = Mathf.Clamp01(elapsed / moveDuration);

                fingerIndicator.position = Vector3.Lerp(start, end, t);

                yield return null;
            }

            yield return new WaitForSecondsRealtime(0.5f);
        }

        fingerIndicator.gameObject.SetActive(false);
    }

    public void HorizontalCutCompleted()
    {
        horizontalCutDone = true;

        if (guideRoutine != null)
            StopCoroutine(guideRoutine);

        guideRoutine = StartCoroutine(GloveBlinkLoop());
    }

    private IEnumerator GloveBlinkLoop()
    {
        fingerIndicator.position = gloveTarget.position;
        fingerIndicator.localScale = originalScale;
        fingerIndicator.gameObject.SetActive(true);

        while (!gloveDragging)
        {
            yield return ScaleFinger(originalScale * tapScale, tapDuration);
            yield return ScaleFinger(originalScale, tapDuration);
            yield return new WaitForSecondsRealtime(waitBetweenTaps);
        }
    }

    public void GloveDragStarted()
    {
        if (gloveDragging)
            return;

        gloveDragging = true;

        if (guideRoutine != null)
            StopCoroutine(guideRoutine);

        guideRoutine = StartCoroutine(GloveSwipeLoop());
    }

    private IEnumerator GloveSwipeLoop()
    {
        while (!legPickedUp)
        {
            fingerIndicator.position = gloveTarget.position;

            Vector3 start = gloveTarget.position;
            Vector3 end = legTarget.position;
            float elapsed = 0f;

            while (elapsed < moveDuration && !legPickedUp)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / moveDuration);

                fingerIndicator.position = Vector3.Lerp(start, end, t);

                yield return null;
            }

            yield return new WaitForSecondsRealtime(0.5f);
        }

        fingerIndicator.gameObject.SetActive(false);
    }

    public void GlovePickUpCompleted()
    {
        legPickedUp = true;

        if (guideRoutine != null)
            StopCoroutine(guideRoutine);

        guideRoutine = StartCoroutine(PlateSwipeLoop());
    }

    private IEnumerator PlateSwipeLoop()
    {
        while (!chickenPlacedOnPlate)
        {
            fingerIndicator.position = chickenOnHandTarget.position;
            fingerIndicator.localScale = originalScale;
            fingerIndicator.gameObject.SetActive(true);

            Vector3 start = chickenOnHandTarget.position;
            Vector3 end = plateTarget.position;

            float elapsed = 0f;

            while (elapsed < moveDuration && !chickenPlacedOnPlate)
            {
                elapsed += Time.unscaledDeltaTime;

                float t = Mathf.Clamp01(elapsed / moveDuration);

                fingerIndicator.position = Vector3.Lerp(start, end, t);

                yield return null;
            }

            yield return new WaitForSecondsRealtime(0.5f);
        }

        fingerIndicator.gameObject.SetActive(false);
    }

    public void ChickenPlacedOnPlate()
    {
        chickenPlacedOnPlate = true;

        if (guideRoutine != null)
            StopCoroutine(guideRoutine);

        fingerIndicator.gameObject.SetActive(false);
    }
}
