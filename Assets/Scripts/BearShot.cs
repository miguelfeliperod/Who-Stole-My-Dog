using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BearShot : MonoBehaviour
{
    [SerializeField] LayerMask playerLayer;
    [SerializeField] int damage;
    [SerializeField] float horizontalSpeed;
    [SerializeField] float lifetime;
    [SerializeField] Collider2D hitCollider;
    [SerializeField] AudioClip sfx;
    [SerializeField] AudioSource audioSource;
    VisualEffect trailVFX;
    Rigidbody2D rigidbody2d;
    SpriteRenderer spriteRenderer;
    
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        trailVFX = GetComponentInChildren<VisualEffect>();
        hitCollider = GetComponent<Collider2D>();
        audioSource = GetComponentInChildren<AudioSource>();
    }

    public void Shoot(bool isFlipped, float projectileLifetime)
    {
        Start();
        spriteRenderer.flipX = isFlipped;
        trailVFX.SetBool("FlipSprite", isFlipped);
        StartCoroutine(AutoDestroy(projectileLifetime));
        rigidbody2d.velocity = new Vector2(
             isFlipped
                ? horizontalSpeed 
                : -horizontalSpeed,
                0);
        audioSource.PlayOneShot(sfx);
    }

    IEnumerator AutoDestroy(float projectileLifetime)
    {
        yield return new WaitForSeconds(projectileLifetime * 0.8f);
        trailVFX.Stop();
        rigidbody2d.velocity = Vector2.zero;
        hitCollider.enabled = false;
        yield return new WaitForSeconds(projectileLifetime);

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer & playerLayer) != 0)
        {
            collision.gameObject.GetComponentInParent<PlayerController>().TakeDamage(damage);
        }
    }
}
