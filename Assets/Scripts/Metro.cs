using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Metro : MonoBehaviour
{
    TrailRenderer trail;
    VisualEffect smoke;
    SpriteRenderer sprite;
    public Rigidbody2D rigidbody2;
    [SerializeField] float horizontalSpeed;
    [SerializeField] LayerMask targetLayer;
    public int damage;
    public bool destroyOnContact = true;
    [SerializeField] float aliveTime = 1.2f;
    [SerializeField] Color color;
    AudioSource audioSource;
    [SerializeField] AudioClip sfx;

    public virtual void Start()
    {
        trail = GetComponentInChildren<TrailRenderer>();
        smoke = GetComponentInChildren<VisualEffect>();
        sprite = GetComponent<SpriteRenderer>();
        rigidbody2 = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        rigidbody2.velocity = new Vector2(horizontalSpeed, 0);

        bool isFlipped = rigidbody2.velocity.x < 0;
        sprite.flipX = isFlipped;
        smoke.SetBool("FlipSprite", isFlipped);
        StartCoroutine(AutoDestroy());

        audioSource.PlayOneShot(sfx);

        if (horizontalSpeed > 0)
        {
            smoke.transform.localPosition = new Vector2(-sprite.size.x / 2, smoke.transform.localPosition.y);
            trail.transform.localPosition = new Vector2(-sprite.size.x / 2, trail.transform.localPosition.y);
        }
        else
        {
            smoke.transform.localPosition = new Vector2(sprite.size.x / 2, smoke.transform.localPosition.y);
            trail.transform.localPosition = new Vector2(sprite.size.x / 2, trail.transform.localPosition.y);
        }
        SetColor(color);

    }

    IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(aliveTime);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer & targetLayer) != 0)
        {
            collision.gameObject.GetComponentInParent<PlayerController>().TakeDamage(damage);
            if (destroyOnContact)
                Destroy(gameObject, 0.01f);
        }
    }
    public void SetInitialState(float speed, Color color)
    {
        this.color = color;
        horizontalSpeed = speed;
    }

    void SetColor(Color color)
    {
        sprite.color = color;
        Gradient gradient = trail.colorGradient;
        var colors = new GradientColorKey[] { new GradientColorKey(color, 0) };
        gradient.SetKeys(colors, gradient.alphaKeys);
        trail.colorGradient = gradient;

        gradient = smoke.GetGradient("GroundDustColor");
        gradient.SetKeys(colors, gradient.alphaKeys);
        smoke.SetGradient("GroundDustColor", gradient);
    }
}
