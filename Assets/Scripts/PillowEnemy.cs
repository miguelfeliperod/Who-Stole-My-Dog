using System.Collections;
using UnityEngine;

public class PillowEnemy : BaseEnemy
{
    [SerializeField] bool isGoingUp;
    [SerializeField] float verticalSpeed;
    float moveTimer = 0;
    Vector2 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        moveTimer += Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        animator.Play("PillowFalling");
        
    }
}
