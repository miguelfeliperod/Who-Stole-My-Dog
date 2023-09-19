using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    [SerializeField] Image fadeScreen;
   
    void Start()
    {
        DontDestroyOnLoad(this);
        fadeScreen = GetComponentInChildren<Image>();    
    }

    public void PlayFadeOut(Color targetColor, float duration = 1)
    {
        StartCoroutine(FadeOut(targetColor, duration));
    }

    public void PlayFadeIn(float duration = 1)
    {
        StartCoroutine(FadeIn(duration));
    }

    public void PlayCrossFade(Color targetColor, float duration = 2f)
    {
        StartCoroutine(CrossFade(targetColor, duration));
    }

    public void PlayFlash(Color targetColor, float duration = 0.4f)
    {
        StartCoroutine(Flash(targetColor, duration));
    }

    IEnumerator FadeOut(Color targetColor, float duration = 1)
    {
        float elapsedTime = 0;
        Color currentColor = fadeScreen.color;

        while (elapsedTime < duration)
        {
            fadeScreen.color = Color.Lerp(currentColor, targetColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fadeScreen.color = targetColor;
        yield return null;
    }

    IEnumerator FadeIn(float duration = 1)
    {
        float elapsedTime = 0;
        Color currentColor = fadeScreen.color;

        while (elapsedTime < duration)
        {
            fadeScreen.color = Color.Lerp(currentColor, Color.clear, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fadeScreen.color = Color.clear;
        yield return null;
    }

    IEnumerator CrossFade(Color targetColor, float duration)
    {
        StartCoroutine(FadeOut(targetColor, duration / 2));
        yield return new WaitForSeconds(duration / 2);
        StartCoroutine(FadeIn(duration / 2));
        yield return null;
    }
    IEnumerator Flash(Color targetColor, float duration = 1)
    {
        Color currentColor = fadeScreen.color;

        fadeScreen.color = targetColor;
        yield return new WaitForSeconds(duration);
        fadeScreen.color = currentColor;
        yield return null;
    }
}
