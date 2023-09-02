using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI lifeStockText;
    [SerializeField] TextMeshProUGUI sushiStockText;
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
}
