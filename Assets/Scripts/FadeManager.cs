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

    public void PlayFadeIn(Color targetColor, float duration = 1)
    {
        StartCoroutine(FadeIn(targetColor, duration));
    }

    public void PlayFadeOut(float duration = 1)
    {
        StartCoroutine(FadeOut(duration));
    }

    public void PlayCrossFade(Color targetColor, float duration = 2f)
    {
        StartCoroutine(CrossFade(targetColor, duration));
    }

    public void PlayFlash(Color targetColor, float duration = 2f)
    {
        StartCoroutine(Flash(targetColor, duration));
    }

    IEnumerator FadeIn(Color targetColor, float duration = 1)
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

    IEnumerator FadeOut(float duration = 1)
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
        StartCoroutine(FadeIn(targetColor, duration / 2));
        yield return new WaitForSeconds(duration / 2);
        StartCoroutine(FadeOut(duration / 2));
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
