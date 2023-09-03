using System.Collections;
using UnityEngine;

public class DobbyProjectile : MonoBehaviour
{
    [SerializeField] Rigidbody2D rigidbody2d;
    SpriteRenderer spriteRenderer;
    float horizontalSpeed;
    float verticalSpeed;
    float speedRange;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] float rotationForce;
    public int damage;
    bool isFlipped;

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetInitialState(bool isFlipped, float projectileHorizontalForce, float projectileVerticalSpeed,float projectileForceRange)
    {
        Start();
        this.isFlipped = isFlipped;
        spriteRenderer.flipX = isFlipped;
        horizontalSpeed = projectileHorizontalForce;
        verticalSpeed = projectileVerticalSpeed;
        speedRange = projectileForceRange;
        rigidbody2d.gravityScale = 0;
    }

    public void StartMove()
    {
        rigidbody2d.gravityScale = 1;
        StartCoroutine(AutoDestroy());
        rigidbody2d.AddForce(new Vector2(
             isFlipped
                ? horizontalSpeed + Random.Range(-speedRange, speedRange)
                : - horizontalSpeed + Random.Range(-speedRange, speedRange),
                verticalSpeed + Random.Range(-speedRange, speedRange)), 
             ForceMode2D.Impulse);
        rigidbody2d.AddTorque(rotationForce);
    }

    IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer & playerLayer) != 0)
        {
            collision.gameObject.GetComponentInParent<PlayerController>().TakeDamage(damage);
            Destroy(gameObject, 0.01f);
        }
    }
}
