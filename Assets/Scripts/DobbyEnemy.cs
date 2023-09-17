using System.Collections;
using UnityEngine;

public class DobbyEnemy : BaseEnemy
{
    [SerializeField] GameObject projectile;
    [SerializeField] Transform projectileSource;
    [SerializeField] float projectileSourceXOffset;
    [SerializeField] float projectileHorizontalForce;
    [SerializeField] float projectileVerticalForce;  
    [SerializeField] float projectileForceRange;
    [SerializeField] float timeBetweenShots;
    [SerializeField] float shotsQuantity;
    [SerializeField] AudioClip prepareSfx;
    [SerializeField] AudioClip shootSfx;
    [SerializeField] AudioSource audioSource;
    float timer;

    private void Start()
    {
        audioSource = GetComponentInChildren<AudioSource>();
    }

    private void Update()
    {
        sprite.flipX = transform.position.x > GameManager.Instance.playerController.transform.position.x ? false : true;

        timer += Time.deltaTime;
        if (timer > attackRate)
        {
            timer = 0;
            Attack();
        }
    }

    public override void Attack()
    {
        StartCoroutine(RangedAttack());
    }

    IEnumerator RangedAttack()
    {
        for (int i = 0; i < shotsQuantity; i++)
        {
            animator.Play("DobbyPrepareAttack");
            DobbyProjectile dobbyProjectile = CreateProjectile();
            yield return new WaitForSeconds(timeBetweenShots/2);
            Shoot(dobbyProjectile);
            yield return new WaitForSeconds(timeBetweenShots/2);
        }
        yield return null;
    }

    DobbyProjectile CreateProjectile()
    {
        audioSource.PlayOneShot(prepareSfx);
        var gameObject = Instantiate(projectile, projectileSource.transform.position + new Vector3(sprite.flipX ? - projectileSourceXOffset : projectileSourceXOffset, 0), projectile.transform.rotation);
        var createdProjectile = gameObject.GetComponent<DobbyProjectile>();
        createdProjectile.SetInitialState(sprite.flipX, projectileHorizontalForce, projectileVerticalForce, projectileForceRange);
        return gameObject.GetComponent<DobbyProjectile>();

    }

    void Shoot(DobbyProjectile dobbyProjectile)
    {
        audioSource.PlayOneShot(shootSfx);
        dobbyProjectile.StartMove();
        animator.Play("DobbyAttack");
    }
}
