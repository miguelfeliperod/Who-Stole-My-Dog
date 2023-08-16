using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI lifeStockText;
    [SerializeField] TextMeshProUGUI sushiStockText;
    [SerializeField] Image fadeScreen;
    [SerializeField] FormUI normalFormUI;
    [SerializeField] FormUI mahouFormUI;
    [SerializeField] FormUI darkFormUI;
    [SerializeField] FormUI hungryFormUI;

    public void UpdateUIValues()
    {
        lifeStockText.text = GameManager.Instance.playerController.LifeStock.ToString();
        sushiStockText.text = GameManager.Instance.playerController.SushiStock.ToString();
    }

    public void UpdateFormUI()
    {
        switch (GameManager.Instance.playerController.CurrentForm)
        {
            case Form.Normal:
                normalFormUI.UpdateAllBars();
                break;
            case Form.Mahou:
                mahouFormUI.UpdateAllBars();
                break;
            case Form.Dark:
                darkFormUI.UpdateAllBars();
                break;
            case Form.Hungry:
                hungryFormUI.UpdateAllBars();
                break;
        }
    }

    public void DisableAllFormUI()
    {
        darkFormUI.gameObject.SetActive(false);
        normalFormUI.gameObject.SetActive(false);
        mahouFormUI.gameObject.SetActive(false);
        hungryFormUI.gameObject.SetActive(false);
    }

    public void EnableFormUI(Form form)
    {
        switch (form)
        {
            case Form.Normal:
                normalFormUI.gameObject.SetActive(true);
                break;
            case Form.Mahou:
                mahouFormUI.gameObject.SetActive(true);
                break;
            case Form.Dark:
                darkFormUI.gameObject.SetActive(true);
                break;
            case Form.Hungry:
                hungryFormUI.gameObject.SetActive(true);
                break;
        }
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

    IEnumerator FadeIn(Color targetColor, float duration = 1) {
        float elapsedTime = 0;
        Color currentColor = fadeScreen.color;

        while (elapsedTime < duration)
        {
            fadeScreen.color = Color.Lerp(currentColor, targetColor, elapsedTime/duration);     
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fadeScreen.color = targetColor;
        yield return null;
    }

    IEnumerator FadeOut(float duration = 1) {
        float elapsedTime = 0;
        Color currentColor = fadeScreen.color;

        while (elapsedTime < duration)
        {
            fadeScreen.color = Color.Lerp(currentColor, Color.clear, elapsedTime/duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fadeScreen.color = Color.clear;
        yield return null;
    }

    IEnumerator CrossFade(Color targetColor, float duration) {
        StartCoroutine(FadeIn(targetColor, duration/2));
        yield return new WaitForSeconds(duration/2);
        StartCoroutine(FadeOut(duration/2));
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
