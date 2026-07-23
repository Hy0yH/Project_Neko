using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CutsceneController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject cutscenePanel;
    [SerializeField] private Image cutsceneImage;
    [SerializeField] private Image fadeOverlay;

    [Header("Cutscene")]
    [SerializeField] private Sprite[] cutsceneSprites;
    [SerializeField] private string nextSceneName = "RoofTop_Street";

    [Header("Transition")]
    [SerializeField, Min(0f)] private float fadeDuration = 0.25f;

    [Header("Auto Advance")]
    [SerializeField] private float autoAdvanceDelay = 3f;

    [Header("Second Cut Shake")]
    [SerializeField, Min(0f)] private float shakeDuration = 0.4f;
    [SerializeField, Min(0f)] private float shakeStrength = 20f;
    [SerializeField, Range(1f, 1.1f)] private float shakeScale = 1.04f;

    private int currentIndex;
    private bool isPlaying;
    private bool isTransitioning;
    private float autoAdvanceTimer;

    private RectTransform imageRect;
    private Vector2 originalPosition;
    private Vector3 originalScale;

    private void Awake()
    {
        imageRect = cutsceneImage.rectTransform;
        originalPosition = imageRect.anchoredPosition;
        originalScale = imageRect.localScale;

        cutscenePanel.SetActive(false);
    }

   private void Update()
    {
        if (!isPlaying || isTransitioning)
            return;

        autoAdvanceTimer += Time.unscaledDeltaTime;

        bool mousePressed =
            Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame;

        bool keyboardPressed =
            Keyboard.current != null &&
            (
                Keyboard.current.spaceKey.wasPressedThisFrame ||
                Keyboard.current.enterKey.wasPressedThisFrame
            );

        bool timeExpired =
            autoAdvanceDelay > 0f &&
            autoAdvanceTimer >= autoAdvanceDelay;

        if (mousePressed || keyboardPressed || timeExpired)
            Advance();
    }

    public void Play()
    {
        if (isPlaying || cutsceneSprites.Length == 0)
            return;

        isPlaying = true;
        currentIndex = 0;
        autoAdvanceTimer = 0f;

        cutscenePanel.SetActive(true);
        StartCoroutine(ShowFirstCut());
    }

    public void Advance()
    {
        if (!isPlaying || isTransitioning)
            return;
        
        autoAdvanceTimer = 0f;
        StartCoroutine(AdvanceRoutine());
    }

    private IEnumerator ShowFirstCut()
    {
        isTransitioning = true;

        SetImageTransformToOriginal();
        cutsceneImage.sprite = cutsceneSprites[currentIndex];
        SetFadeAlpha(1f);

        yield return Fade(1f, 0f);

        isTransitioning = false;
    }

    private IEnumerator AdvanceRoutine()
    {
        isTransitioning = true;

        yield return Fade(0f, 1f);

        currentIndex++;

        if (currentIndex >= cutsceneSprites.Length)
        {
            SceneManager.LoadScene(nextSceneName);
            yield break;
        }

        SetImageTransformToOriginal();
        cutsceneImage.sprite = cutsceneSprites[currentIndex];

        yield return Fade(1f, 0f);

        // 배열의 두 번째 이미지이므로 인덱스는 1입니다.
        if (currentIndex == 1)
            yield return ShakeImage();

        isTransitioning = false;
    }

    private IEnumerator Fade(float from, float to)
    {
        if (fadeDuration <= 0f)
        {
            SetFadeAlpha(to);
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsed / fadeDuration);
            float alpha = Mathf.Lerp(from, to, progress);

            SetFadeAlpha(alpha);
            yield return null;
        }

        SetFadeAlpha(to);
    }

    private IEnumerator ShakeImage()
    {
        float elapsed = 0f;

        imageRect.localScale = originalScale * shakeScale;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsed / shakeDuration);
            float currentStrength = shakeStrength * (1f - progress);
            Vector2 offset = Random.insideUnitCircle * currentStrength;

            imageRect.anchoredPosition = originalPosition + offset;
            yield return null;
        }

        SetImageTransformToOriginal();
    }

    private void SetFadeAlpha(float alpha)
    {
        Color color = fadeOverlay.color;
        color.a = alpha;
        fadeOverlay.color = color;
    }

    private void SetImageTransformToOriginal()
    {
        imageRect.anchoredPosition = originalPosition;
        imageRect.localScale = originalScale;
    }
}