using UnityEngine;
using UnityEngine.UI;

public class FormUI : MonoBehaviour
{
    [SerializeField] Image hpBar;
    [SerializeField] Image mpBar;
    [SerializeField] Image hungryBar;

    PlayerController playerController;

    // Start is called before the first frame update
    void Awake()
    {
        playerController = GameManager.Instance.playerController;

        hpBar = transform.GetChild(1).GetComponent<Image>();
        mpBar = transform.GetChild(2).GetComponent<Image>();
        hungryBar = transform.GetChild(3).GetComponent<Image>();

        UpdateAllBars();
    }
    private void OnEnable()
    {
        UpdateAllBars();
    }

    public void UpdateAllBars()
    {
        UpdateHPBar();
        UpdateMPBar();
        UpdateHungryBar();
    }

    void UpdateHPBar() => hpBar.fillAmount = (float) playerController.CurrentHP/ PlayerController.MAX_HP;
    void UpdateMPBar() => mpBar.fillAmount = (float) playerController.CurrentMP / PlayerController.MAX_MP;
    void UpdateHungryBar() => hungryBar.fillAmount = (float) playerController.CurrentHungry / PlayerController.MAX_HUNGRY;
}
