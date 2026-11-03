
using System.Collections;
using UnityEngine;

public class TutorialSalad : MonoBehaviour
{
    public static TutorialSalad Instance { get; private set; }

    [Header("Finger")]
    [SerializeField] private RectTransform fingerIndicator;
    [SerializeField] private float moveDuration = 0.7f;
    [SerializeField] private float tapScale = 0.8f;
    [SerializeField] private float tapDuration = 0.15f;
    [SerializeField] private float waitBetweenTaps = 0.3f;

    [Header("Targets")]
    [SerializeField] private RectTransform saladTarget;
    [SerializeField] private RectTransform plateTarget;
    [SerializeField] private RectTransform bellTarget;

    private Vector3 originalScale;
    private Coroutine guideRoutine;

    private bool tutorialStarted;
    private bool saladServed;
    private bool tutorialFinished;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (fingerIndicator != null)
        {
            originalScale = fingerIndicator.localScale;
            fingerIndicator.gameObject.SetActive(false);
        }

        if (TutorialOrderManager.Instance != null)
        {
            TutorialOrderManager.Instance.OnOrderCompleted += HandleOrderCompleted;
        }
    }

    public void BeginTutorial()
    {
        if (tutorialStarted) return;

        if (fingerIndicator == null || saladTarget == null ||
            plateTarget == null || bellTarget == null)
        {
            return;
        }

        tutorialStarted = true;
        guideRoutine = StartCoroutine(DragSaladLoop());
    }

    private IEnumerator DragSaladLoop()
    {
        fingerIndicator.gameObject.SetActive(true);

        while (!saladServed)
        {
            fingerIndicator.position = saladTarget.position;
            fingerIndicator.localScale = originalScale;

            Vector3 start = saladTarget.position;
            Vector3 end = plateTarget.position;

            float elapsed = 0f;

            while (elapsed < moveDuration && !saladServed)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / moveDuration);

                fingerIndicator.position = Vector3.Lerp(start, end, t);
                yield return null;
            }

            yield return new WaitForSecondsRealtime(0.5f);
        }
    }

    public void SaladServed()
    {
        if (!tutorialStarted || saladServed) return;

        saladServed = true;
        StopGuide();

        guideRoutine = StartCoroutine(TapBellLoop());
    }

    private IEnumerator TapBellLoop()
    {
        fingerIndicator.position = bellTarget.position;
        fingerIndicator.localScale = originalScale;
        fingerIndicator.gameObject.SetActive(true);

        while (!tutorialFinished)
        {
            yield return TapAnimation();
            yield return new WaitForSecondsRealtime(waitBetweenTaps);
        }
    }

    private IEnumerator TapAnimation()
    {
        yield return ScaleFinger(originalScale * tapScale);
        yield return ScaleFinger(originalScale);
    }

    private IEnumerator ScaleFinger(Vector3 destination)
    {
        Vector3 start = fingerIndicator.localScale;
        float elapsed = 0f;

        while (elapsed < tapDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / tapDuration);

            fingerIndicator.localScale =
                Vector3.Lerp(start, destination, t);

            yield return null;
        }

        fingerIndicator.localScale = destination;
    }

    private void HandleOrderCompleted(Order order)
    {
        tutorialFinished = true;
        StopTutorialGuide();
    }

    public void StopTutorialGuide()
    {
        StopGuide();

        if (fingerIndicator != null)
            fingerIndicator.gameObject.SetActive(false);
    }

    private void StopGuide()
    {
        if (guideRoutine != null)
        {
            StopCoroutine(guideRoutine);
            guideRoutine = null;
        }
    }

    private void OnDestroy()
    {
        if (TutorialOrderManager.Instance != null)
        {
            TutorialOrderManager.Instance.OnOrderCompleted -= HandleOrderCompleted;
        }

        if (Instance == this)
            Instance = null;
    }
}
