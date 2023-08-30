using UnityEngine;

public class StitchEnemy : BaseEnemy
{
    [SerializeField] float horizontalSpeed;
    [SerializeField] float timeBeforTurnAround;
    [SerializeField] bool goingRight;
    float timer;
    

    void Start()
    {
        rigidbody2d.velocity = new Vector2(horizontalSpeed, 0);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > timeBeforTurnAround) {
            FlipWalkDirection();
            timer = 0;
        }
    }

    void FlipWalkDirection()
    {
        goingRight = !goingRight;
        rigidbody2d.velocity = new Vector2(horizontalSpeed * (goingRight ? 1: -1),0);
        sprite.flipX = rigidbody2d.velocity.x < 0;
    }
}
