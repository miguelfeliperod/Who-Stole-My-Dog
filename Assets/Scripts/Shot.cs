using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Shot : MonoBehaviour
{
    public Rigidbody2D rigidbody2;
    SpriteRenderer spriteRenderer;
    [SerializeField] float horizontalSpeed;
    [SerializeField] LayerMask targetLayer;
    VisualEffect effect;
    public int damage;
    public bool destroyOnContact = true;
    [SerializeField] float aliveTime = 1.2f;
    [SerializeField] AudioClip sfx;
    public Form shotDamageType = Form.Mahou;

    protected void Start()
    {
        GameManager.Instance.audioManager.PlaySFX(sfx);
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
        yield return new WaitForSeconds(aliveTime);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer & targetLayer) != 0){
            collision.gameObject.GetComponentInParent<BaseEnemy>().TakeDamage(damage, shotDamageType);
            if(destroyOnContact)
                Destroy(gameObject, 0.01f);
        }
    }
}