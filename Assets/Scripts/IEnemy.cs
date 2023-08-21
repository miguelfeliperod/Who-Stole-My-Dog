using System.Collections;
using UnityEngine;

public interface IEnemy
{
    public void Attack();

    public void TakeDamage(int damage, Form damageType = Form.Normal);

    public IEnumerator Die();
}
