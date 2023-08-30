using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class FormUI : MonoBehaviour
{
    [SerializeField] Image hpBar;
    [SerializeField] Image mpBar;
    [SerializeField] Image hungryBar;
    [SerializeField] VisualEffect vfx;

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
        if (playerController.CurrentForm == Form.Hungry)
            vfx.enabled = true;
        else
            vfx.enabled = false;
    }

    public void UpdateAllBars()
    {
        UpdateHPBar();
        UpdateMPBar();
        UpdateHungryBar();
    }

    void UpdateHPBar()
    {
        if (playerController.CurrentForm == Form.Mahou)
            hpBar.fillAmount = (int)playerController.CurrentHP / PlayerController.MAX_HP;
        else
            hpBar.fillAmount = (float)playerController.CurrentHP / PlayerController.MAX_HP;
    }
    void UpdateMPBar() {
        if (playerController.CurrentForm == Form.Mahou)
            mpBar.fillAmount = (int)playerController.CurrentMP / PlayerController.MAX_MP;
        else
            mpBar.fillAmount = (float)playerController.CurrentMP / PlayerController.MAX_MP;
    }
    void UpdateHungryBar()
    {
        if (playerController.CurrentForm == Form.Mahou)
            hungryBar.fillAmount = (int)playerController.CurrentHungry / PlayerController.MAX_HUNGRY;
        else
            hungryBar.fillAmount = (float)playerController.CurrentHungry / PlayerController.MAX_HUNGRY;
    }
}
