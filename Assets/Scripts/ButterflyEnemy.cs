using System.Collections;
using System.Threading;
using UnityEngine;

public class ButterflyEnemy : BaseEnemy
{
    [SerializeField] float butterflySpeedMultiplier;
    Transform playerTransform;
    AudioSource audioSource;
    [SerializeField] AudioClip sfx;
    float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.pitch = 1.2f;
        rigidbody2d.AddForce(new Vector2(0,6), ForceMode2D.Impulse);
        playerTransform = GameManager.Instance.playerController.transform;
    }

    public override IEnumerator Die()
    {
        audioSource.Stop();
        return base.Die();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer += Time.deltaTime;
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        rigidbody2d.velocity = direction * butterflySpeedMultiplier;
        sprite.flipX = rigidbody2d.velocity.x > 0;

        if(timer > 0.3)
        {
            timer = 0;
            audioSource.PlayOneShot(sfx);
        }
    }
}
