using UnityEngine;

public interface IEnemy
{
    public void Attack();

    public void TakeDamage(int damage, Form? damageType = null);

    public void Die();
}
