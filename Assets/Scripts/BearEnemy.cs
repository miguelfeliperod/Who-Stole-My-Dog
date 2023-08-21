using System.Collections;
using UnityEngine;

public class BearEnemy : BaseEnemy
{
    [SerializeField] Transform attackPoint;
    [SerializeField] float attackPointHorizontalOffset;
    [SerializeField] float projectileLifetime;
    [SerializeField] GameObject bearProjectile;
    [SerializeField] bool isAttackMode;
    [SerializeField] float attackTimer;
    [SerializeField] LayerMask playerLayer;

    private void Update()
    {
        sprite.flipX = transform.position.x > GameManager.Instance.playerController.transform.position.x ? false : true;

        attackTimer += Time.deltaTime;

        if(attackTimer > attackRate && isAttackMode)
        {
            attackTimer = 0;
            Attack();
        }
    }

    public override void Attack()
    {
        StartCoroutine(PunchAttack());
    }

    IEnumerator PunchAttack()
    {
        animator.Play("BearAttack");

        yield return new WaitForSeconds(1.4f);
        BearShot projectile = Instantiate(bearProjectile, GetProjectileInitialPosition(), attackPoint.transform.rotation).GetComponent<BearShot>();
        projectile.Shoot(sprite.flipX, projectileLifetime);
    }

    Vector3 GetProjectileInitialPosition()
    {
        return attackPoint.transform.position + new Vector3 (sprite.flipX ? attackPointHorizontalOffset : -attackPointHorizontalOffset, 0,0);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer & playerLayer) == 0) return;
        isAttackMode = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer & playerLayer) == 0) return;
        isAttackMode = false;
    }
}
