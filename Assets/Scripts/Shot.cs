using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Shot : MonoBehaviour
{
    Rigidbody2D rigidbody2;
    SpriteRenderer spriteRenderer;
    [SerializeField] float horizontalSpeed;
    CircleCollider2D circleCollider;
    [SerializeField] LayerMask enemyLayer;
    VisualEffect effect;
    public int damage;

    void Start()
    {
        bool isFlipped = GameManager.Instance.playerController.Sprite.flipX;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2 = GetComponent<Rigidbody2D>();
        effect = GetComponentInChildren<VisualEffect>();
        rigidbody2.velocity = new Vector2(
             isFlipped
            ? -horizontalSpeed 
            : horizontalSpeed, 0);
        spriteRenderer.flipX = isFlipped;
        effect.SetBool("FlipSprite", isFlipped);
        StartCoroutine(AutoDestroy());
    }

    IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(1.2f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer & enemyLayer) != 0){
            collision.gameObject.GetComponentInParent<BaseEnemy>().TakeDamage(damage, Form.Mahou);
            Destroy(gameObject, 0.01f);
        }
    }
}