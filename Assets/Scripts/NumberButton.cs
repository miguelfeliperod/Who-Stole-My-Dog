using TMPro;
using UnityEngine;

public class NumberButton : BaseEnemy
{
    float value;
    public float Value => value;
    [SerializeField] TextMeshPro text;
    NumberManager numberManager;

    public override void TakeDamage(int damage, Form damageType = Form.Normal)
    {
        StartCoroutine(BlinkShadowColor(Color.red, 0.05f));

        numberManager.OnHitButton(value);
    }

    public void SetButtonManager(NumberManager manager) 
    {
        numberManager = manager;
    }

    public void SetValue(float value)
    {
        this.value = value;
        text.text = value.ToString();
    }

    public void SetColor(Color value)
    {
        sprite.color = value;
    }
}
