
using System.Collections;
using UnityEngine;

public class TutorialFries : MonoBehaviour
{
    public static TutorialFries Instance { get; private set; }

    [Header("Finger")]
    [SerializeField] private RectTransform fingerIndicator;
    [SerializeField] private float tapScale = 0.8f;
    [SerializeField] private float tapDuration = 0.15f;
    [SerializeField] private float waitBetweenTaps = 0.3f;
    [SerializeField] private float moveDuration = 0.7f;

    [Header("Targets")]
    [SerializeField] private RectTransform fryerTarget;
    [SerializeField] private RectTransform plateTarget;
    [SerializeField] private RectTransform trashTarget;

    private Vector3 originalScale;
    private Coroutine guideRoutine;

    private bool tutorialStarted;
    private bool friesAdded;
    private bool basketLowered;
    private bool friesCooked;
    private bool basketRaised;
    private bool friesBurnt;

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
    }

    public void BeginTutorial()
    {
        if (tutorialStarted) return;

        tutorialStarted = true;
        guideRoutine = StartCoroutine(TapFryerLoop());
    }

    private IEnumerator TapFryerLoop()
    {
        fingerIndicator.position = fryerTarget.position;
        fingerIndicator.localScale = originalScale;
        fingerIndicator.gameObject.SetActive(true);

        while (!friesAdded)
        {
            yield return TapAnimation();
            yield return new WaitForSecondsRealtime(waitBetweenTaps);
        }
    }

    public void FriesAdded()
    {
        if (!tutorialStarted || friesAdded) return;

        friesAdded = true;
        StopGuide();
        guideRoutine = StartCoroutine(TapFryerLoopForBasket());
    }

    private IEnumerator TapFryerLoopForBasket()
    {
        fingerIndicator.position = fryerTarget.position;
        fingerIndicator.gameObject.SetActive(true);

        while (!basketLowered)
        {
            yield return TapAnimation();
            yield return new WaitForSecondsRealtime(waitBetweenTaps);
        }
    }

    public void BasketLowered()
    {
        if (basketLowered) return;

        basketLowered = true;
        StopGuide();
        fingerIndicator.gameObject.SetActive(false);
    }

    public void FriesCooked()
    {
        if (!tutorialStarted || friesCooked) return;

        friesCooked = true;
        guideRoutine = StartCoroutine(TapFryerLoopForRaising());
    }

    private IEnumerator TapFryerLoopForRaising()
    {
        fingerIndicator.position = fryerTarget.position;
        fingerIndicator.localScale = originalScale;
        fingerIndicator.gameObject.SetActive(true);

        while (!basketRaised)
        {
            yield return TapAnimation();
            yield return new WaitForSecondsRealtime(waitBetweenTaps);
        }
    }

    public void BasketRaised()
    {
        if (basketRaised) return;

        basketRaised = true;
        StopGuide();
        guideRoutine = StartCoroutine(DragToPlateLoop());
    }

    private IEnumerator DragToPlateLoop()
    {
        while (!friesBurnt)
        {
            fingerIndicator.position = fryerTarget.position;
            fingerIndicator.localScale = originalScale;
            fingerIndicator.gameObject.SetActive(true);

            Vector3 start = fryerTarget.position;
            Vector3 end = plateTarget.position;

            float elapsed = 0f;

            while (elapsed < moveDuration && !friesBurnt)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / moveDuration);

                fingerIndicator.position = Vector3.Lerp(start, end, t);
                yield return null;
            }

            yield return new WaitForSecondsRealtime(0.5f);
        }
    }

    public void FriesBurnt()
    {
        if (friesBurnt) return;

        friesBurnt = true;
        StopGuide();

        if (trashTarget != null)
            guideRoutine = StartCoroutine(DragToTrashLoop());
    }

    private IEnumerator DragToTrashLoop()
    {
        while (true)
        {
            fingerIndicator.position = fryerTarget.position;
            fingerIndicator.gameObject.SetActive(true);

            Vector3 start = fryerTarget.position;
            Vector3 end = trashTarget.position;

            float elapsed = 0f;

            while (elapsed < moveDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / moveDuration);

                fingerIndicator.position = Vector3.Lerp(start, end, t);
                yield return null;
            }

            yield return new WaitForSecondsRealtime(0.5f);
        }
    }

    public void FriesServed()
    {
        StopGuide();

        if (fingerIndicator != null)
            fingerIndicator.gameObject.SetActive(false);

        if (TutorialSalad.Instance != null)
            TutorialSalad.Instance.BeginTutorial();

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
        if (Instance == this)
            Instance = null;
    }
}
