using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class BossController : BaseEnemy
{
    float timer = 0;
    [SerializeField] BossPhase bossPhase = BossPhase.Easy;

    //=================== TOMATO ====================

    [SerializeField] GameObject tomato1Prefab;
    [SerializeField] GameObject tomato2Prefab;
    [SerializeField] GameObject tomato3Prefab;

    [SerializeField] float tomato1Speed;
    [SerializeField] float tomato2Speed;
    [SerializeField] float tomato3Speed;

    [SerializeField] Transform tomatoOriginPoint;
    [SerializeField] float tomatoOriginPointOffset;


    public override void Awake()
    {
        boxCollider = GetComponentInChildren<BoxCollider2D>();
        animator = GetComponentInChildren<Animator>();
        damageVFX = GetComponentInChildren<VisualEffect>();
        rigidbody2d = GetComponentInChildren<Rigidbody2D>();
        sprite = GetComponentsInChildren<SpriteRenderer>()[0];
        shadow = GetComponentsInChildren<SpriteRenderer>()[1];
        currentHp = maxHp;
    }

    void Update()
    {
        timer += Time.deltaTime;
        //if(timer > 3)
        //{
        //    timer = 0;
        //    StartCoroutine(ShootTomato());
        //}
    }

    public override void Attack()
    {
        print("Attack Called");
    }

    IEnumerator ShootTomato()
    {
        GameObject tomatoShoot;
        float tomatoHorizontalVelocity = sprite.flipX? 1f : -1f;
        switch (bossPhase)
        {
            case BossPhase.Easy:
                tomatoShoot = tomato1Prefab;
                tomatoHorizontalVelocity *= tomato1Speed;
                break;
            case BossPhase.Average:
                tomatoShoot = tomato2Prefab;
                tomatoHorizontalVelocity *= tomato2Speed;
                break;
            case BossPhase.Hard:
            case BossPhase.Impossible:
            default:
                tomatoShoot = tomato3Prefab;
                tomatoHorizontalVelocity *= tomato3Speed;
                break;
        }
        animator.Play("ZeroTomatoShoot");
        yield return new WaitForSeconds(0.3f);
        Instantiate(tomatoShoot, tomatoOriginPoint.position, tomatoOriginPoint.rotation).GetComponent<Tomato>().SetHorizontalVelocity(tomatoHorizontalVelocity);
        animator.Play("ZeroTomatoShoot");
        yield return new WaitForSeconds(0.3f);
        Instantiate(tomatoShoot, tomatoOriginPoint.position, tomatoOriginPoint.rotation).GetComponent<Tomato>().SetHorizontalVelocity(tomatoHorizontalVelocity);
        animator.Play("ZeroTomatoShoot");
        yield return new WaitForSeconds(0.3f);
        Instantiate(tomatoShoot, tomatoOriginPoint.position, tomatoOriginPoint.rotation).GetComponent<Tomato>().SetHorizontalVelocity(tomatoHorizontalVelocity);

    }

    public override void TakeDamage(int damage, Form damageType = Form.Normal)
    {
        if (imunityDamageList.Contains(damageType)) damage = 0;
        print("Receive Damage Called: " + currentHp + " => " + (currentHp - damage));
        currentHp -= damage;
        StartCoroutine(BlinkShadowColor(Color.red, 0.05f));
        damageVFX.SetInt("DamageValue", damage * 3);
        damageVFX.Play();

        if (currentHp <= 0)
        {
            StartCoroutine(Die());
        }
    }

    private IEnumerator BlinkShadowColor(Color targetColor, float duration)
    {
        shadow.color = targetColor;
        yield return new WaitForSeconds(duration);
        shadow.color = Color.clear;
    }

    public override IEnumerator Die()
    {
        print("Die Called");
        Color startColor = Color.red;
        GetComponent<Rigidbody2D>().Sleep();

        shadow.color = startColor;
        sprite.color = Color.clear;
        Destroy(gameObject, 1f);

        float elapsedTime = 0f;
        float duration = 1;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            shadow.color = Color.Lerp(startColor, Color.clear, t);
            yield return null;
        }

        shadow.color = Color.clear;
        yield return new WaitForSeconds(0.2f);
    }
}

public enum BossPhase
{
    Easy, Average, Hard, Impossible
}