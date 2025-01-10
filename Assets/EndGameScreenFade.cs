using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class EndGameScreenFade : MonoBehaviour
{
    public Image fadeImage; // Assign your full-screen Image here.
    public float fadeScreenDuration = 1f; // Duration of the fade effect.
    public float fadeTextDuration = 4f;

    public bool fadeOut;

    public TextMeshProUGUI fadeText;
    public bool fadeInText;

    private void Start()
    {
        if (fadeImage == null)
        {
            Debug.LogError("Fade Image not assigned in the inspector.");
        }
    }

    private void Update()
    {
        if (fadeOut)
        {
            fadeOut = false;
            FadeToBlack();
        }
        if (fadeInText)
        {
            fadeInText = false;
            FadeInText();
        }
    }

    public void FadeToBlack()
    {
        print("Fading to black");
        StartCoroutine(Fade(0, 1)); // Fade from transparent to black.
    }

    public void FadeFromBlack()
    {
        StartCoroutine(Fade(1, 0)); // Fade from black to transparent.
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeScreenDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeScreenDuration);
            color.a = alpha;
            fadeImage.color = color;
            yield return null;
        }

        color.a = endAlpha;
        fadeImage.color = color;
    }


    public void FadeInText()
    {
        print("Fading in Text");

        StartCoroutine(FadeInText(0, 1)); // Fade from transparent to black.
    }


    private IEnumerator FadeInText(float startAlpha, float endAlpha)
    {
        yield return new WaitForSeconds(fadeScreenDuration);
        float elapsedTime = 0f;
        Color color = fadeText.color;

        while (elapsedTime < fadeTextDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeTextDuration);
            color.a = alpha;
            fadeText.color = color;
            yield return null;
        }

        color.a = endAlpha;
        fadeText.color = color;
    }
}