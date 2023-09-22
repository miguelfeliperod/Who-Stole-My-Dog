using UnityEngine;

public class StitchEnemy : BaseEnemy
{
    [SerializeField] float horizontalSpeed;
    [SerializeField] float timeBeforTurnAround;
    [SerializeField] bool goingRight;
    [SerializeField] AudioClip sfx;
    [SerializeField] AudioSource audioSource;
    float timer;
    float walkTimer;

    void Start()
    {
        rigidbody2d.velocity = new Vector2(goingRight ? horizontalSpeed : -horizontalSpeed, 0);
        audioSource = GetComponentInChildren<AudioSource>();
    }

    void Update()
    {
        
        timer += Time.deltaTime;
        if (timer > timeBeforTurnAround) {
            FlipWalkDirection();
            timer = 0;
        }

        walkTimer += Time.deltaTime;
        if (walkTimer > 0.35f)
        {
            audioSource.PlayOneShot(sfx);
            walkTimer = 0;
        }
    }

    void FlipWalkDirection()
    {
        goingRight = !goingRight;
        rigidbody2d.velocity = new Vector2(horizontalSpeed * (goingRight ? 1: -1),0);
        sprite.flipX = rigidbody2d.velocity.x < 0;
    }
}
