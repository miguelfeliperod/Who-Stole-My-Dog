using System.Collections;
using UnityEngine;

public class PillowEnemy : BaseEnemy
{
    [SerializeField] bool isGoingUp;
    [SerializeField] float verticalSpeed;
    [SerializeField] AudioClip sfx;
    [SerializeField] AudioSource audioSource;
    [SerializeField] float delayStart;

    private void Start()
    {
        audioSource = GetComponentInChildren<AudioSource>();
        rigidbody2d.gravityScale = 0;
        StartCoroutine(DelayStart());
    }

    IEnumerator DelayStart()
    {
        yield return new WaitForSeconds(delayStart);
        rigidbody2d.gravityScale = 1;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        animator.Play("PillowFalling");
        audioSource.PlayOneShot(sfx);
    }
}
