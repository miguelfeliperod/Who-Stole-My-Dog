using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public abstract class BaseEnemy : MonoBehaviour, IEnemy
{
    public int maxHp;
    int currentHp;
    public float attackRate;
    public float attackPower;
    public BoxCollider2D boxCollider;
    public Rigidbody2D rigidbody2d;
    public Animator animator;
    public VisualEffect damageVFX;
    public SpriteRenderer sprite;
    public SpriteRenderer shadow;
    public List<Form> weaknessDamageList;
    public List<Form> resistanceDamageList;
    public List<Form> imunityDamageList;

    private void Awake()
    {
        boxCollider = GetComponentInChildren<BoxCollider2D>();
        animator = GetComponentInChildren<Animator>();
        damageVFX = GetComponentInChildren<VisualEffect>();
        rigidbody2d = GetComponentInChildren<Rigidbody2D>();
        currentHp = maxHp;
        sprite = GetComponentsInChildren<SpriteRenderer>()[0];
        shadow = GetComponentsInChildren<SpriteRenderer>()[1]; ;
}

    public virtual void Attack()
    {
        print("Attack Called");
    }

    public virtual void TakeDamage(int damage, Form damageType = Form.Normal)
    {
        if (imunityDamageList.Contains(damageType)) damage = 0;
        if (resistanceDamageList.Contains(damageType)) damage = damage/2;
        if (weaknessDamageList.Contains(damageType)) damage = (damage + (damage/2));
        print("Receive Damage Called: " + currentHp + " => " + (currentHp - damage));
        currentHp -= damage;
        StartCoroutine(BlinkShadowColor(Color.red,0.05f));
        damageVFX.SetInt("DamageValue", damage * 3);
        damageVFX.Play();

        if (currentHp <= 0) {
            StartCoroutine(Die());
        }
    }

    private IEnumerator BlinkShadowColor(Color targetColor, float duration)
    {
        shadow.color = targetColor;
        yield return new WaitForSeconds(duration);
        shadow.color = Color.clear;
    }

    public virtual IEnumerator Die()
    {
        print("Die Called");
        Color startColor = Color.red;
        GetComponent<Rigidbody2D>().Sleep();

        shadow.color = startColor;
        sprite.color = Color.clear;
        Destroy(gameObject, 1f);

        float elapsedTime = 0f;
        float duration = 1;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                shadow.color = Color.Lerp(startColor, Color.clear, t);
                yield return null;
            }

        shadow.color = Color.clear;
        yield return new WaitForSeconds(0.2f);
    }
}
