using UnityEngine;
using UnityEngine.VFX;

public class Checkpoint : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Animator animator;
    [SerializeField] LayerMask playerLayer;
    VisualEffect visualEffect;
    bool alreadyActivated = false;
    [SerializeField] AudioClip sfx;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        visualEffect = GetComponentInChildren<VisualEffect>();
        visualEffect.Stop();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (alreadyActivated == true) return;
        if ((1 << collision.gameObject.layer & playerLayer) != 0)
        {
            animator.SetBool("isActive", true);
            GameManager.Instance.playerController.SetFullStats(true);
            GameManager.Instance.SetLastCheckpointPosition(transform.position);
            GameManager.Instance.audioManager.PlaySFX(sfx);
            alreadyActivated = true;
            visualEffect.Play();
        }
    }
}
