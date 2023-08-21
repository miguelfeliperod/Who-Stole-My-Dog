using System;
using System.Collections;
using UnityEngine;

public class OwlEnemy : BaseEnemy
{
    Vector2 initialPosition;
    [SerializeField] float horizontalDistance;
    [SerializeField] float horizontalSpeed;
    [SerializeField] float verticalSpeed;
    [SerializeField] Collider2D owlVision;
    [SerializeField] float owlVisionOffset;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] bool isAttacking = false;
    [SerializeField] float timeSinceLastAttack = 0;

    private void Start()
    {
        initialPosition = transform.position;
        rigidbody2d.velocity = new Vector2(horizontalSpeed,0);
        SetSpriteAndVisionFlip();
    }

    private void Update()
    {
        timeSinceLastAttack += Time.deltaTime;
        SetHorizontalVelocity();
    }

    IEnumerator SetVerticalVelocity()
    {
        rigidbody2d.velocity = new Vector2(rigidbody2d.velocity.x, - verticalSpeed);
        float targetY = GameManager.Instance.playerController.transform.position.y;
        while (transform.position.y >= targetY)
        {
            yield return null;
        }
        rigidbody2d.velocity = new Vector2(rigidbody2d.velocity.x, verticalSpeed);
        while (transform.position.y <= initialPosition.y)
        {
            yield return null;
        }
        isAttacking = false;
        rigidbody2d.velocity = new Vector2(rigidbody2d.velocity.x, 0);
        animator.SetBool("isAttacking", isAttacking);
    }

    private void SetHorizontalVelocity()
    {
        if (rigidbody2d.velocity.x > 0 && ((transform.position.x - initialPosition.x) > horizontalDistance))
        {
            rigidbody2d.velocity = new Vector2(-horizontalSpeed, rigidbody2d.velocity.y);
            SetSpriteAndVisionFlip();
        }
        else if (rigidbody2d.velocity.x < 0 && ((transform.position.x - initialPosition.x) < - horizontalDistance))
        {
            rigidbody2d.velocity = new Vector2(horizontalSpeed, rigidbody2d.velocity.y);
            SetSpriteAndVisionFlip();
        }
    }

    void SetSpriteAndVisionFlip()
    {
        if (rigidbody2d.velocity.x > 0)
        {
            sprite.flipX = true;
            owlVision.gameObject.transform.localPosition= new Vector2 (owlVisionOffset,0);
        }
        else 
        { 
            sprite.flipX = false;
            owlVision.gameObject.transform.localPosition = new Vector2(-owlVisionOffset, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (timeSinceLastAttack < attackRate) return;
        if ((1 << collision.gameObject.layer & playerLayer) == 0) return;
        Attack();
    }

    public override void Attack()
    {
        isAttacking = true;
        timeSinceLastAttack = 0;
        animator.SetBool("isAttacking", isAttacking);
        StartCoroutine(SetVerticalVelocity());
    }
}
