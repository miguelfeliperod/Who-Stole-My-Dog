using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButterflyEnemy : BaseEnemy
{
    [SerializeField] float butterflySpeedMultiplier;
    Transform playerTransform;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d.AddForce(new Vector2(0,6), ForceMode2D.Impulse);
        playerTransform = GameManager.Instance.playerController.transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        rigidbody2d.velocity = direction * butterflySpeedMultiplier;
        sprite.flipX = rigidbody2d.velocity.x > 0;
    }
}
