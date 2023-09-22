using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject pauseScreen;
    [SerializeField] TextMeshProUGUI lifeStockText;
    [SerializeField] TextMeshProUGUI sushiStockText;
    [SerializeField] FormUI normalFormUI;
    [SerializeField] FormUI mahouFormUI;
    [SerializeField] FormUI darkFormUI;
    [SerializeField] FormUI hungryFormUI;
    [SerializeField] Image darkDiedImage;
    [SerializeField] Image victoryImage;

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

    public IEnumerator ShowDiedImage(float duration = 1)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            darkDiedImage.color = Color.Lerp(Color.clear, Color.white, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        darkDiedImage.color = Color.white;
        yield return null;
    }

    public IEnumerator ShowVictoryImage(float fadeDuration = 1, float stayDuration= 1)
    {
        float elapsedTime = 0;

        while (elapsedTime < fadeDuration)
        {
            victoryImage.color = Color.Lerp(Color.clear, Color.white, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        victoryImage.color = Color.white;
        yield return new WaitForSeconds(stayDuration);


        elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            victoryImage.color = Color.Lerp(Color.white, Color.clear, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        victoryImage.color = Color.clear;
        yield return null;
    }

    public IEnumerator HideDiedImage(float duration = 1)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            darkDiedImage.color = Color.Lerp(Color.white, Color.clear, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        darkDiedImage.color = Color.clear;
        yield return null;
    }

    public void ShowPauseScreen() => pauseScreen.SetActive(true);
    public void HidePauseScreen() => pauseScreen.SetActive(false);
}
