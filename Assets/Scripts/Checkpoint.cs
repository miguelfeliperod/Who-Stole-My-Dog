using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Animator animator;
    [SerializeField] LayerMask playerLayer;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer & playerLayer) != 0)
        {
            animator.SetBool("isActive", true);
        }
    }
}
