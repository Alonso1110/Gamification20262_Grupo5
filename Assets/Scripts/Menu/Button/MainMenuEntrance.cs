using UnityEngine;
using TMPro;
using System.Collections;

public class MainMenuEntrance : MonoBehaviour
{
    [Header("Title Settings")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private float titleDropHeight = 120f;
    [SerializeField] private float letterDuration = 0.35f;
    [SerializeField] private float letterDelay = 0.04f;

    [Header("Buttons Settings")]
    [SerializeField] private RectTransform[] menuButtons;
    [SerializeField] private float buttonSlideDistance = 500f;
    [SerializeField] private float buttonDuration = 0.35f;
    [SerializeField] private float buttonDelay = 0.08f;
    private static bool hasPlayedIntro = false;

    private Vector2[] buttonsOriginalPos;
    private float[] letterOffsets;
    private bool animateTitle = false;

    private void Awake()
    {
        if (menuButtons != null && menuButtons.Length > 0)
        {
            buttonsOriginalPos = new Vector2[menuButtons.Length];
            for (int i = 0; i < menuButtons.Length; i++)
            {
                if (menuButtons[i] != null)
                {
                    buttonsOriginalPos[i] = menuButtons[i].anchoredPosition;
                }
            }
        }
    }

    private void Start()
    {
        if (hasPlayedIntro)
        {
            SkipIntroDirectly();
        }
        else
        {
            hasPlayedIntro = true;
            StartCoroutine(PlayEntranceSequence());
        }
    }

    private void SkipIntroDirectly()
    {
        if (menuButtons != null)
        {
            for (int i = 0; i < menuButtons.Length; i++)
            {
                if (menuButtons[i] != null)
                {
                    menuButtons[i].anchoredPosition = buttonsOriginalPos[i];
                }
            }
        }

        if (titleText != null)
        {
            titleText.ForceMeshUpdate();
            int charCount = titleText.textInfo.characterCount;
            letterOffsets = new float[charCount];
            for (int i = 0; i < charCount; i++)
            {
                letterOffsets[i] = 0f;
            }
            animateTitle = true;
        }
    }

    private void LateUpdate()
    {
        if (!animateTitle || titleText == null) return;

        titleText.ForceMeshUpdate();
        TMP_TextInfo textInfo = titleText.textInfo;

        if (letterOffsets == null || letterOffsets.Length != textInfo.characterCount) return;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            Vector3 offset = new Vector3(0f, letterOffsets[i], 0f);
            vertices[vertexIndex + 0] += offset;
            vertices[vertexIndex + 1] += offset;
            vertices[vertexIndex + 2] += offset;
            vertices[vertexIndex + 3] += offset;
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            titleText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }

    private IEnumerator PlayEntranceSequence()
    {
        if (menuButtons != null)
        {
            for (int i = 0; i < menuButtons.Length; i++)
            {
                if (menuButtons[i] != null)
                {
                    menuButtons[i].anchoredPosition = buttonsOriginalPos[i] + new Vector2(buttonSlideDistance, 0f);
                }
            }
        }

        if (titleText != null)
        {
            titleText.ForceMeshUpdate();
            int charCount = titleText.textInfo.characterCount;
            letterOffsets = new float[charCount];

            for (int i = 0; i < charCount; i++)
            {
                letterOffsets[i] = titleDropHeight;
            }

            animateTitle = true;

            for (int i = 0; i < charCount; i++)
            {
                if (titleText.textInfo.characterInfo[i].isVisible)
                {
                    StartCoroutine(DropLetterRoutine(i));
                    yield return new WaitForSecondsRealtime(letterDelay);
                }
            }
        }

        if (menuButtons != null)
        {
            for (int i = 0; i < menuButtons.Length; i++)
            {
                if (menuButtons[i] != null)
                {
                    StartCoroutine(SlideButtonIn(menuButtons[i], buttonsOriginalPos[i]));
                    yield return new WaitForSecondsRealtime(buttonDelay);
                }
            }
        }
    }

    private IEnumerator DropLetterRoutine(int index)
    {
        float elapsed = 0f;
        while (elapsed < letterDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / letterDuration);

            float c1 = 1.70158f;
            float c3 = c1 + 1f;
            float easeOutBack = 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);

            letterOffsets[index] = Mathf.LerpUnclamped(titleDropHeight, 0f, easeOutBack);
            yield return null;
        }
        letterOffsets[index] = 0f;
    }

    private IEnumerator SlideButtonIn(RectTransform btnRect, Vector2 targetPos)
    {
        float elapsed = 0f;
        Vector2 startPos = btnRect.anchoredPosition;

        while (elapsed < buttonDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / buttonDuration);

            float smoothT = 1f - Mathf.Pow(1f - t, 3f);
            btnRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, smoothT);

            yield return null;
        }
        btnRect.anchoredPosition = targetPos;
    }
}