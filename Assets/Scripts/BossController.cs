using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class BossController : BaseEnemy
{
    float timer = 0;

    [Space(25.0f)]
    [Header("BOSS STATS")]
    [Space(10.0f)]
    [SerializeField] BossPhase bossPhase = BossPhase.Easy;
    [SerializeField] float easyAttackCooldown;
    [SerializeField] float averageAttackCooldown;
    [SerializeField] float hardAttackCooldown;
    bool isFighting = true; // false;
    public bool IsFighting => isFighting;
    bool isInvencible = false;
    public bool IsInvencible => isInvencible;

    bool canAct = true;

    [SerializeField] Transform posX1Y1;
    [SerializeField] Transform posX1Y2;
    [SerializeField] Transform posX1Y3;
    [SerializeField] Transform posX2Y1;
    [SerializeField] Transform posX2Y2;
    [SerializeField] Transform posX2Y3;
    [SerializeField] Transform posX3Y1;
    [SerializeField] Transform posX3Y2;
    [SerializeField] Transform posX3Y3;

    static string kaelCharge = "kaelCharge";
    static string kaelDefeated = "KaelDefeated";
    static string kaelIdle = "KaelIdle";
    static string zeroIdle = "ZeroIdle";
    static string zeroInvokeButterflyes = "ZeroInvokeButterflies";
    static string zeroInvokeNumbers = "ZeroInvokeNumbers";
    static string zeroTomatoShoot = "ZeroTomatoShoot";
    static string zeroMetro = "ZeroMetro";
    static string zeroVulnerable = "ZeroVulnerable";

    //=================== HUD ====================
    [SerializeField] Image hpBar1;
    [SerializeField] Image hpBar2;
    [SerializeField] Image hpBar3;

    //=================== TOMATO ====================
    [Space(25.0f)]
    [Header("TOMATO")]
    [Space(10.0f)]

    [SerializeField] GameObject tomato1Prefab;
    [SerializeField] GameObject tomato2Prefab;
    [SerializeField] GameObject tomato3Prefab;

    [SerializeField] float tomato1Speed;
    [SerializeField] float tomato2Speed;
    [SerializeField] float tomato3Speed;

    [SerializeField] Transform tomatoOriginPoint;
    [SerializeField] float tomatoOriginPointOffset;

    //=================== METRO ====================
    [Space(25.0f)]
    [Header("METRO")]
    [Space(10.0f)]

    [SerializeField] MetroShooter metro1RightShooter;
    [SerializeField] MetroShooter metro2RightShooter;
    [SerializeField] MetroShooter metro3RightShooter;
    [SerializeField] MetroShooter metro1LeftShooter;
    [SerializeField] MetroShooter metro2LeftShooter;
    [SerializeField] MetroShooter metro3LeftShooter;

    [SerializeField] float easySpeed;
    [SerializeField] float averageSpeed;
    [SerializeField] float hardSpeed;

    List<Vector2> metroTeleportPositions;

    //=================== COCOON ====================  
    [Space(25.0f)]
    [Header("COCOON")]
    [Space(10.0f)]

    [SerializeField] List<Transform>cocoonPositions;

    [SerializeField] GameObject CocoonPrefab;

    List<Vector2> cocoonTeleportPositions;

    //=================== NUMBERS ====================
    [Space(25.0f)]
    [Header("NUMBERS")]
    [Space(10.0f)]

    [SerializeField] NumberManager numberManager;
    List<Vector2> mathTeleportPositions;

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
    void Start()
    {
        metroTeleportPositions = new List<Vector2>() { posX2Y1.position, posX2Y2.position, posX2Y3.position };
        cocoonTeleportPositions = new List<Vector2>() { posX1Y1.position, posX1Y2.position, posX2Y1.position, posX2Y2.position , posX3Y1.position, posX3Y2.position };
        mathTeleportPositions = new List<Vector2>() { posX1Y1.position, posX1Y2.position , posX2Y2.position, posX3Y1.position, posX3Y2.position };
    }

    void Update()
    {
        if (!isFighting || !canAct) return;
        canAct = false;
        {
            var value = Random.Range(0, 20);
            switch (value)
            {
                case <= 4:
                    print("Value: " + value + " and called tomato");
                    StartCoroutine(ShootTomato());
                    break;
                case <= 10:
                    print("Value: " + value + " and called math");
                    StartCoroutine(SummonMath());
                    break;
                case <= 15:
                    print("Value: " + value + " and called metro");
                    StartCoroutine(ShootMetro());
                    break;
                case <= 20:
                    print("Value: " + value + " and called cocoon");
                    StartCoroutine(SummonCocoon());
                    break;
            }
            timer = 0;
        }
    }

    Image GetCurrentHPBar()
    {
        switch (bossPhase)
        {
            case BossPhase.Easy:
                return hpBar1;
            case BossPhase.Average:
                return hpBar2;
            case BossPhase.Hard:
            case BossPhase.Impossible:
            default:
                return hpBar3;
        }
    }

    float GetActionCooldown()
    {
        switch (bossPhase)
        {
            case BossPhase.Easy:
                return easyAttackCooldown;
            case BossPhase.Average:
                return averageAttackCooldown;
            case BossPhase.Hard:
            case BossPhase.Impossible:
            default:
                return hardAttackCooldown;
        }
    }

    IEnumerator TeleportToPosition(Vector2 targetPosition, float vanishDuration = 0.5f, float invisibleDuration = 0.5f, float appearDuration = 0.5f)
    {
        boxCollider.gameObject.layer = LayerMask.NameToLayer("Default");
        float timer = 0;

        while(timer < vanishDuration)
        {
            timer += Time.deltaTime;
            sprite.color = Color.Lerp(Color.white, Color.clear, timer/vanishDuration);
            yield return null;
        }
        sprite.color = Color.clear;

        transform.position = targetPosition;
        yield return new WaitForSeconds(invisibleDuration);

        timer = 0;
        while (timer < appearDuration)
        {
            timer += Time.deltaTime;
            sprite.color = Color.Lerp(Color.clear, Color.white, timer / appearDuration);
            yield return null;
        }
        sprite.color = Color.white;
        boxCollider.gameObject.layer = LayerMask.NameToLayer("Enemy");
    }

    public override void Attack()
    {
        print("Attack Called");
    }

    IEnumerator ShootMetro()
    {
        StartCoroutine(TeleportToPosition(metroTeleportPositions[Random.Range(0, metroTeleportPositions.Count -1)]));
        IsGravityOn(false);
        yield return new WaitForSeconds(0.5f);
        animator.Play(zeroMetro);
        yield return new WaitForSeconds(1.5f);

        yield return new WaitForSeconds(GetActionCooldown());
        canAct = true;
    }

    IEnumerator SummonCocoon() {
        StartCoroutine(TeleportToPosition(cocoonTeleportPositions[Random.Range(0, cocoonTeleportPositions.Count - 1)]));
        IsGravityOn(false);
        yield return new WaitForSeconds(0.5f);
        animator.Play(zeroInvokeButterflyes);
        yield return new WaitForSeconds(1.5f);

        int randomIndex = Random.Range(0, cocoonPositions.Count);

        switch (bossPhase)
        {
            case BossPhase.Easy:
                Instantiate(CocoonPrefab, cocoonPositions[randomIndex]);
                break;
            case BossPhase.Average:
                Instantiate(CocoonPrefab, cocoonPositions[randomIndex]);
                Instantiate(CocoonPrefab, cocoonPositions[(randomIndex + 1)% cocoonPositions.Count]);
                break;
            case BossPhase.Hard:
            case BossPhase.Impossible:
            default:
                Instantiate(CocoonPrefab, cocoonPositions[0]);
                Instantiate(CocoonPrefab, cocoonPositions[1]);
                Instantiate(CocoonPrefab, cocoonPositions[2]);
                break;
        }

        yield return new WaitForSeconds(GetActionCooldown());
        canAct = true;
    }
    
    IEnumerator SummonMath()
    {
        StartCoroutine(TeleportToPosition(mathTeleportPositions[Random.Range(0, mathTeleportPositions.Count - 1)]));
        IsGravityOn(false);
        yield return new WaitForSeconds(0.5f);
        animator.Play(zeroInvokeNumbers);
        yield return new WaitForSeconds(1.5f);
        numberManager.gameObject.SetActive(true);
        numberManager.SetTest(bossPhase);

        yield return new WaitForSeconds(GetActionCooldown());
        numberManager.gameObject.SetActive(false);
        canAct = true;
    }

    IEnumerator ShootTomato()
    {
        GameObject tomatoShoot;

        sprite.flipX = Random.Range(0, 2) == 0 ? true : false;

        StartCoroutine(TeleportToPosition(sprite.flipX ? posX1Y3.position : posX3Y3.position));
        yield return new WaitForSeconds(1.5f);
        IsGravityOn(true);
        animator.Play(zeroIdle);
        yield return new WaitForSeconds(0.5f);

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
        Instantiate(tomatoShoot, tomatoOriginPoint.position + new Vector3(sprite.flipX ? tomatoOriginPointOffset : -tomatoOriginPointOffset, 0), tomatoOriginPoint.rotation).GetComponent<Tomato>().SetHorizontalVelocity(tomatoHorizontalVelocity);
        animator.Play("ZeroTomatoShoot");
        yield return new WaitForSeconds(0.3f);
        Instantiate(tomatoShoot, tomatoOriginPoint.position + new Vector3(sprite.flipX ? tomatoOriginPointOffset : -tomatoOriginPointOffset, 0), tomatoOriginPoint.rotation).GetComponent<Tomato>().SetHorizontalVelocity(tomatoHorizontalVelocity);
        animator.Play("ZeroTomatoShoot");
        yield return new WaitForSeconds(0.3f);
        Instantiate(tomatoShoot, tomatoOriginPoint.position + new Vector3(sprite.flipX ? tomatoOriginPointOffset : -tomatoOriginPointOffset, 0), tomatoOriginPoint.rotation).GetComponent<Tomato>().SetHorizontalVelocity(tomatoHorizontalVelocity);

        yield return new WaitForSeconds(GetActionCooldown());
        canAct = true;
    }

    public override void TakeDamage(int damage, Form damageType = Form.Normal)
    {
        if (isInvencible) return;
        print("Receive Damage Called: " + currentHp + " => " + (currentHp - damage));
        currentHp -= damage;
        StartCoroutine(BlinkShadowColor(Color.red, 0.05f));
        StartCoroutine(TemporaryInvencibility());
        damageVFX.SetInt("DamageValue", damage * 3);
        damageVFX.Play();
        animator.Play(zeroVulnerable);
        GetCurrentHPBar().fillAmount = ((float)currentHp / (float)maxHp);

        if (currentHp <= 0)
        {
            switch (bossPhase)
            {
                case BossPhase.Easy:
                    currentHp = maxHp;
                    bossPhase = BossPhase.Average;
                    break;
                case BossPhase.Average:
                    bossPhase = BossPhase.Hard;
                    currentHp = maxHp;
                    break;
                case BossPhase.Hard:
                    bossPhase = BossPhase.Hard;
                    StartCoroutine(Die());
                    break;
                default:
                case BossPhase.Impossible:
                    break;
            }
        }
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

    public void SetFightStatus(bool status) => isFighting = status;

    public void IsGravityOn(bool value) 
    {
        rigidbody2d.gravityScale = value ? 1 : 0;
        rigidbody2d.constraints =  value ? RigidbodyConstraints2D.FreezeRotation : RigidbodyConstraints2D.FreezeAll;
    }

    IEnumerator TemporaryInvencibility()
    {
        isInvencible = true;
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < 10; i++)
        {
            sprite.color = Color.clear;
            yield return new WaitForSeconds(0.06f);
            sprite.color = Color.white;
            yield return new WaitForSeconds(0.06f);
        }
        isInvencible = false;
    }
}

public enum BossPhase
{
    Easy, Average, Hard, Impossible
}