using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using static UnityEngine.Rendering.DebugUI;

public class BossController : BaseEnemy
{
    float timer = 0;

    [Space(25.0f)]
    [Header("BOSS STATS")]
    [Space(10.0f)]
    BossEvent bossEvent;
    [SerializeField] BossPhase bossPhase = BossPhase.Easy;
    [SerializeField] float easyAttackCooldown;
    [SerializeField] float averageAttackCooldown;
    [SerializeField] float hardAttackCooldown;
    [SerializeField] public VisualEffect chargeVfx;
    [SerializeField] public AudioClip chargeSfx;
    bool isFighting = false;

    public bool IsFighting => isFighting;
    bool isInvencible = false;
    public bool IsInvencible => isInvencible;

    bool canAct = true;

    [SerializeField] Form weakness = Form.Normal;

    [SerializeField] Transform posX1Y1;
    [SerializeField] Transform posX1Y2;
    [SerializeField] Transform posX1Y3;
    [SerializeField] Transform posX2Y1;
    [SerializeField] Transform posX2Y2;
    [SerializeField] Transform posX2Y3;
    [SerializeField] Transform posX3Y1;
    [SerializeField] Transform posX3Y2;
    [SerializeField] Transform posX3Y3;

    public static string kaelCharge = "kaelCharge";
    public static string kaelDefeated = "KaelDefeated";
    public static string kaelIdle = "KaelIdle";
    public static string zeroIdle = "ZeroIdle";
    public static string zeroInvokeButterflyes = "ZeroInvokeButterflies";
    public static string zeroInvokeNumbers = "ZeroInvokeNumbers";
    public static string zeroTomatoShoot = "ZeroTomatoShoot";
    public static string zeroMetro = "ZeroMetro";
    public static string zeroVulnerable = "ZeroVulnerable";

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

    [SerializeField] AudioClip tomatoSfx;

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

    [SerializeField] float metroEasySpeed;
    [SerializeField] float metroAverageSpeed;
    [SerializeField] float metroHardSpeed;

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
        bossEvent = FindObjectOfType<BossEvent>();
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
            var value = UnityEngine.Random.Range(0, 20);
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
        int index = UnityEngine.Random.Range(0, metroTeleportPositions.Count);
        StartCoroutine(TeleportToPosition(metroTeleportPositions[index]));
        IsGravityOn(false);
        yield return new WaitForSeconds(0.5f);
        animator.Play(zeroMetro);
        yield return new WaitForSeconds(1.5f);

        switch (bossPhase)
        {
            case BossPhase.Easy:
                if (index == 0)
                {
                    if (UnityEngine.Random.Range(0,2) == 0)
                        metro3LeftShooter.ShootMetro(GetMetroSpeedByPhase());
                    else
                        metro3RightShooter.ShootMetro(GetMetroSpeedByPhase());
                } else if(index == 1)
                {
                    if (UnityEngine.Random.Range(0, 2) == 0)
                        metro2LeftShooter.ShootMetro(GetMetroSpeedByPhase());
                    else
                        metro2RightShooter.ShootMetro(GetMetroSpeedByPhase());
                }
                else
                {
                    if (UnityEngine.Random.Range(0, 2) == 0)
                        metro1LeftShooter.ShootMetro(GetMetroSpeedByPhase());
                    else
                        metro1RightShooter.ShootMetro(GetMetroSpeedByPhase());
                }
                break;
            case BossPhase.Average:
                if (index == 0)
                {
                    metro3LeftShooter.ShootMetro(GetMetroSpeedByPhase());
                    metro3RightShooter.ShootMetro(GetMetroSpeedByPhase());
                }
                else if (index == 1)
                {
                    metro2LeftShooter.ShootMetro(GetMetroSpeedByPhase());
                    metro2RightShooter.ShootMetro(GetMetroSpeedByPhase());
                }
                else
                {
                    metro1LeftShooter.ShootMetro(GetMetroSpeedByPhase());
                    metro1RightShooter.ShootMetro(GetMetroSpeedByPhase());
                }
                break;
            case BossPhase.Hard:
            case BossPhase.Impossible:
            default:
                if (index == 0)
                {
                    metro3LeftShooter.ShootMetro(GetMetroSpeedByPhase());
                    metro3RightShooter.ShootMetro(GetMetroSpeedByPhase());
                    metro2LeftShooter.ShootMetro(GetMetroSpeedByPhase());
                    metro2RightShooter.ShootMetro(GetMetroSpeedByPhase());
                }
                else
                {
                    metro1LeftShooter.ShootMetro(GetMetroSpeedByPhase());
                    metro1RightShooter.ShootMetro(GetMetroSpeedByPhase());
                    metro2LeftShooter.ShootMetro(GetMetroSpeedByPhase());
                    metro2RightShooter.ShootMetro(GetMetroSpeedByPhase());
                }
                break;
        }
        yield return new WaitForSeconds(GetActionCooldown());
        canAct = true;
    }

    IEnumerator SummonCocoon() {
        StartCoroutine(TeleportToPosition(cocoonTeleportPositions[UnityEngine.Random.Range(0, cocoonTeleportPositions.Count)]));
        IsGravityOn(false);
        yield return new WaitForSeconds(0.5f);
        animator.Play(zeroInvokeButterflyes);
        yield return new WaitForSeconds(1.5f);

        int randomIndex = UnityEngine.Random.Range(0, cocoonPositions.Count);

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
        StartCoroutine(TeleportToPosition(mathTeleportPositions[UnityEngine.Random.Range(0, mathTeleportPositions.Count)]));
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

        var flip = UnityEngine.Random.Range(0, 2) == 0 ? true : false;

        StartCoroutine(TeleportToPosition(flip ? posX1Y3.position : posX3Y3.position));
        yield return new WaitForSeconds(1.5f);
        sprite.flipX = flip;
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
        GameManager.Instance.audioManager.PlaySFX(tomatoSfx);
        animator.Play("ZeroTomatoShoot");
        yield return new WaitForSeconds(0.3f);
        Instantiate(tomatoShoot, tomatoOriginPoint.position + new Vector3(sprite.flipX ? tomatoOriginPointOffset : -tomatoOriginPointOffset, 0), tomatoOriginPoint.rotation).GetComponent<Tomato>().SetHorizontalVelocity(tomatoHorizontalVelocity);
        GameManager.Instance.audioManager.PlaySFX(tomatoSfx);
        animator.Play("ZeroTomatoShoot");
        yield return new WaitForSeconds(0.3f);
        Instantiate(tomatoShoot, tomatoOriginPoint.position + new Vector3(sprite.flipX ? tomatoOriginPointOffset : -tomatoOriginPointOffset, 0), tomatoOriginPoint.rotation).GetComponent<Tomato>().SetHorizontalVelocity(tomatoHorizontalVelocity);
        GameManager.Instance.audioManager.PlaySFX(tomatoSfx);

        yield return new WaitForSeconds(GetActionCooldown());
        canAct = true;
    }

    public override void TakeDamage(int damage, Form damageType = Form.Normal)
    {
        if (isInvencible) return;
        if (damageType == weakness){
            damage += 2;
            StartCoroutine(BlinkShadowColor(Color.red, 0.07f));
            animator.Play(zeroVulnerable);
        }
        else
        {
            damage = Math.Max(1, damage-1);
            StartCoroutine(BlinkShadowColor(Color.grey, 0.05f));
        }
        print("Receive Damage Called: " + currentHp + " => " + (currentHp - damage));
        currentHp -= damage;
        StartCoroutine(TemporaryInvencibility());
        damageVFX.SetInt("DamageValue", damage * 3);
        damageVFX.Play();
        GetCurrentHPBar().fillAmount = ((float)currentHp / (float)maxHp);

        // PHASE ADVANCE
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
                    bossPhase = BossPhase.Impossible;
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
        animator.Play(zeroVulnerable);
        isFighting = false;
        canAct = false;
        GameManager.Instance.playerController.SetInvencible(true);

        yield return new WaitForSeconds(0.2f);
        GameManager.Instance.fadeManager.PlayFlash(Color.white, 0.05f);
        yield return new WaitForSeconds(0.6f);
        GameManager.Instance.fadeManager.PlayFlash(Color.white, 0.05f);
        yield return new WaitForSeconds(0.3f);
        GameManager.Instance.fadeManager.PlayFlash(Color.white, 0.05f);
        yield return new WaitForSeconds(0.3f);
        GameManager.Instance.fadeManager.PlayFadeOut(Color.white, 0.5f);
        yield return new WaitForSeconds(0.5f);
        animator.Play(zeroVulnerable);
        IsGravityOn(true);
        GameManager.Instance.AdvanceEventCheckpoint(EventCheckpoint.PosBoss);
        bossEvent.gameObject.SetActive(true);
        bossEvent.StartEventByPhase();
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
            yield return new WaitForSeconds(0.05f);
            sprite.color = Color.white;
            yield return new WaitForSeconds(0.05f);
        }
        isInvencible = false;
    }

    float GetMetroSpeedByPhase()
    {
        switch (bossPhase)
        {
            case BossPhase.Easy:
                return metroEasySpeed;
            case BossPhase.Average:
                return metroAverageSpeed;
            case BossPhase.Hard:
            case BossPhase.Impossible:
            default:
                return metroHardSpeed;
        }
    }
}

public enum BossPhase
{
    Easy, Average, Hard, Impossible
}