using UnityEngine;
using UnityEngine.VFX;

public abstract class BaseEnemy : MonoBehaviour, IEnemy
{
    public int maxHp;
    int currentHp;
    public float attackRate;
    public float attackPower;
    public BoxCollider2D boxCollider;
    public Animator animator;
    public VisualEffect damageVFX;

    private void Awake()
    {
        boxCollider = GetComponentInChildren<BoxCollider2D>();
        animator = GetComponentInChildren<Animator>();
        damageVFX = GetComponentInChildren<VisualEffect>();
        currentHp = maxHp;
    }

    public virtual void Attack()
    {
        print("Attack Called");
    }

    public virtual void TakeDamage(int damage, Form? damageType = null)
    {
        print("Receive Damage Called: " + currentHp + " => " + (currentHp - damage));
        currentHp -= damage;
        animator.SetTrigger("ReceiveDamage");
        damageVFX.SetInt("DamageValue", damage * 3);
        damageVFX.Play();

        if (currentHp <= 0) {
            Die();
        }
    }

    public virtual void Die()
    {
        print("Die Called");

        Destroy(gameObject,0.1f);
    }
}
