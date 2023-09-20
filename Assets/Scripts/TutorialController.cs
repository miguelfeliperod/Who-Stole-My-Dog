using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    [SerializeField] List<GameObject> pages;
    [SerializeField] Button leftArrow;
    [SerializeField] Button rightArrow;
    [SerializeField] Button closeButton;
    [SerializeField] TextMeshProUGUI currentPageText;
    int currentPage = 0;

    private void OnEnable()
    {
        if (GameManager.Instance == null || GameManager.Instance.audioManager == null) return;
        OnTutorialButtonClick();
    }

    void LateUpdate()
    {
        GameManager.Instance.playerController.IsMovementBlocked = true;
        GameManager.Instance.playerController.IsGameplayBlocked = true;
    }

    void OnDisable()
    {
        GameManager.Instance.playerController.IsMovementBlocked = false;
        GameManager.Instance.playerController.IsGameplayBlocked = false;
    }
    public void OnTutorialButtonClick()
    {
        currentPage = 0;
        leftArrow.gameObject.SetActive(true);
        rightArrow.gameObject.SetActive(true);
        closeButton.gameObject.SetActive(true);
        currentPageText.gameObject.SetActive(true);
        GameManager.Instance.audioManager.FadeInMusic(GameManager.Instance.audioManager.audioPool.Menu, 1);
        ToggleCurrentPage();
        UpdateArrows();
    }

    public void OnRightArrowClick() 
    {
        ToggleCurrentPage();
        currentPage++;
        ToggleCurrentPage();
        UpdateArrows();
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnLeftArrowClick() 
    {
        ToggleCurrentPage();
        currentPage--;
        ToggleCurrentPage();
        UpdateArrows();
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnCloseButtonClick()
    {
        leftArrow.gameObject.SetActive(false);
        rightArrow.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
        currentPageText.gameObject.SetActive(false);
        foreach (GameObject page in pages)
            page.gameObject.SetActive(false);
        GameManager.Instance.audioManager.StartCrossFadeMusic(GameManager.Instance.audioManager.GetCurrentLevelMusic(SceneManager.GetActiveScene().name), 1);
        gameObject.SetActive(false);
    }

    void UpdateArrows()
    {
        if (currentPage == 0)
            leftArrow.gameObject.SetActive(false);
        else
            leftArrow.gameObject.SetActive(true);

        if (currentPage == pages.Count - 1)
            rightArrow.gameObject.SetActive(false);
        else
            rightArrow.gameObject.SetActive(true);
        
        UpdateText();
    }

    void UpdateText()
    {
        currentPageText.text = (currentPage + 1) + "/" + pages.Count;
    }

    void ToggleCurrentPage()
    {
        pages[currentPage].SetActive(!pages[currentPage].activeSelf);
    }
}
