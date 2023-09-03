using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Tomato : MonoBehaviour
{
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] int damage;
    [SerializeField] float horizontalSpeed;
    [SerializeField] float lifetime;
    Collider2D hitCollider;
    VisualEffect onHitEffect;
    Rigidbody2D rigidbody2d;
    SpriteRenderer spriteRenderer;
    Transform bossTransform;

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        hitCollider = GetComponent<Collider2D>();
        onHitEffect = GetComponentInChildren<VisualEffect>();
        Shoot();
    }

    public void Shoot()
    {
        StartCoroutine(AutoDestroy(lifetime));
        rigidbody2d.velocity = new Vector2(horizontalSpeed, 0);
    }

    public void SetHorizontalVelocity(float speed)
    {
        horizontalSpeed = speed;
    }

    IEnumerator AutoDestroy(float projectileLifetime)
    {
        yield return new WaitForSeconds(projectileLifetime);
        rigidbody2d.velocity = Vector2.zero;
        hitCollider.enabled = false;
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer & ignoreLayer) == 0)
        {
            var player = collision.gameObject.GetComponentInParent<PlayerController>();
            
            if (player != null)
            {
                player.TakeDamage(damage);
                if (player.IsDodging)
                {
                    return;
                }
            }
            StartCoroutine(OnHit());
        }
    }

    public virtual IEnumerator OnHit()
    {
        print("Die Called");
        rigidbody2d.Sleep();
        hitCollider.enabled = false;
        spriteRenderer.enabled = false;
        onHitEffect.Play();
        Destroy(gameObject, 1f);
        yield return new WaitForSeconds(0.2f);
    }
}
