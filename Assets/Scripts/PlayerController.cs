using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class PlayerController : MonoBehaviour
{
    GameManager gameManager;
    UIManager uiManager;
    PlayerControlls playerControlls;

    int playerLayerID;
    int enemyLayerID;
    bool isGroundedLastValue = true;
    [SerializeField] LayerMask groundLayer;

    [Space(25.0f)]
    [Header("IMMUTABLE CHARACTER STATS")]
    [Space(25.0f)]
    public static float MAX_HP = 20;
    public static float MAX_MP = 16;
    public static float MAX_HUNGRY = 13;

    [Header("BODY COMPONENTS")]
    [Space(25.0f)]
    [SerializeField] Rigidbody2D rigidBody;
    [SerializeField] CapsuleCollider2D characterCollider;
    [SerializeField] SpriteRenderer sprite;
    public SpriteRenderer Sprite => sprite;
    [SerializeField] SpriteRenderer upperShadowSprite;
    [SerializeField] Animator animator;

    [Space(25.0f)]
    [Header("FORMS COMPONENTS")]
    [Space(25.0f)]

    PlayerForm currentPlayerForm;
    [SerializeField] PlayerForm normalForm;
    [SerializeField] PlayerForm mahouForm;
    [SerializeField] PlayerForm darkForm;
    [SerializeField] PlayerForm hungryForm;

    [SerializeField] GameObject normalVision;
    [SerializeField] GameObject mahouVision;
    [SerializeField] GameObject darkVision;
    [SerializeField] GameObject hungryVision;

    [Space(25.0f)]
    [Header("ATTACK COMPONENTS")]
    [Space(25.0f)]
    [SerializeField] Transform attackPoint;
    [SerializeField] Vector3 normalAttackPointOffset;
    [SerializeField] Vector3 darkAttackPointOffset;
    [SerializeField] Vector3 hungryAttackPointOffset;
    [SerializeField] Vector3 hungryFireballPointOffset;
    [SerializeField] int darkAttackCount;
    [SerializeField] float attackRange;
    [SerializeField] float bonusDarkAttackRangePerHit;
    [SerializeField] float hungryAttackRange;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] Transform rightShotSource;
    [SerializeField] Transform leftShotSource;
    [SerializeField] Transform fireballLeftSource;
    [SerializeField] Transform fireballRightSource;

    float currentChargedTime;
    bool isCharging = false;

    [Space(25.0f)]
    [Header("TIMERS AND COOLDOWNS")]
    [Space(25.0f)]
    [SerializeField] float shot2MinimumChargeTime;
    [SerializeField] float shot3MinimumChargeTime;
    float darkComboTimer;
    [SerializeField] float darkComboTimerLimit;
    float heartbeatTimer = 0;

    [Space(25.0f)]
    [Header("COLOR COMPONENTS")]
    [Space(25.0f)]
    [SerializeField] Transform getGroundColorPoint;
    [SerializeField] Color receiveDamageColor;
    [SerializeField] Color transformationNormalColor;
    [SerializeField] Color transformationMahouColor;
    [SerializeField] Color transformationDarkColor;
    [SerializeField] Color transformationHungryColor;
    [SerializeField] Gradient dustRunGradient;

    [Space(25.0f)]
    [Header("VISUAL EFFECTS")]
    [Space(25.0f)]
    [SerializeField] VisualEffect transformationNormalVFX;
    [SerializeField] VisualEffect transformationMahouVFX;
    [SerializeField] VisualEffect transformationDarkVFX;
    [SerializeField] VisualEffect transformationHungryVFX;
    [SerializeField] VisualEffect walkNormalVFX;
    [SerializeField] VisualEffect deathNormalVFX;
    [SerializeField] VisualEffect deathMahouVFX;
    [SerializeField] VisualEffect deathDarkVFX;
    [SerializeField] VisualEffect deathHungryVFX;
    [SerializeField] VisualEffect landingNormalVFX;
    [SerializeField] VisualEffect chargingVFX;
    [SerializeField] VisualEffect hungryHit;
    [SerializeField] VisualEffect hungryWalk;
    public VisualEffect ChargingVFX
    {
        set { chargingVFX = value; }
        get { return chargingVFX; }
    }

    [Space(25.0f)]
    [Header("PHYSIC VALUES")]
    [Space(25.0f)]
    [SerializeField] Vector2 darkDodgeImpulseValue;
    [SerializeField] float playerGravityScale;
    public float PlayerGravityScale => playerGravityScale;

    [Space(25.0f)]
    [Header("INPUTS")]
    [Space(25.0f)]
    InputAction move;
    InputAction jump;
    InputAction fall;
    InputAction attack;
    InputAction charge;
    InputAction special;
    InputAction changeFormNormal;
    InputAction changeFormMahou;
    InputAction changeFormDark;
    InputAction pause;


    [SerializeField] float hungryDepletionTime;
    [SerializeField] int sushiStock;
    public int SushiStock => sushiStock;
    [SerializeField] int lifeStock;
    public int LifeStock => lifeStock;
    [SerializeField] float darkDodgeInvencibilityDuration;

    [Space(25.0f)]
    [Header("MUTABLE CHARACTER STATS")]
    [Space(25.0f)]
    [SerializeField] Form currentForm;
    public Form CurrentForm => currentForm;

    [SerializeField] float jumpForce;
    [SerializeField] float horizontalBaseAcceleration;
    [SerializeField] float maxSpeed;

    [SerializeField] bool isInvencible = false;
    [SerializeField] bool isDodging = false;
    public bool IsDodging => isDodging;
    [SerializeField] bool isMovementBlocked = false;
    public bool IsMovementBlocked {
        get => isMovementBlocked;
        set { isMovementBlocked = value; }
    }

    [SerializeField] bool isGameplayBlocked = false;
    public bool IsGameplayBlocked
    {
        get => isGameplayBlocked;
        set { isGameplayBlocked = value; }
    }

    [SerializeField] bool isStarving = false;
    public bool IsStarving
    {
        get => isStarving;
        set { isStarving = value; }
    }

    bool IsAlive => currentHP > 0;

    [SerializeField] float currentHP;
    [SerializeField] float currentMP;
    [SerializeField] float currentHungry;
    [SerializeField] int baseAttackDamage;

    public float CurrentHP => currentHP;
    public float CurrentMP => currentMP;
    public float CurrentHungry => currentHungry;

    [Space(25.0f)]
    [Header("AudioClips")]
    [Space(25.0f)]

    [SerializeField] AudioClip jumpSound;
    [SerializeField] AudioClip chargeSound;
    [SerializeField] AudioClip headbutHit;
    [SerializeField] AudioClip headbutMiss;
    [SerializeField] AudioClip eat;
    [SerializeField] AudioClip heal;
    [SerializeField] AudioClip darkCombo1;
    [SerializeField] AudioClip darkCombo2;
    [SerializeField] AudioClip darkCombo3;
    [SerializeField] AudioClip darkComboMiss;
    [SerializeField] AudioClip dodge;
    [SerializeField] AudioClip punchHit;
    [SerializeField] AudioClip punchMiss;
    [SerializeField] AudioClip normalTransformation;
    [SerializeField] AudioClip mahouTransformation;
    [SerializeField] AudioClip darkTransformation;
    [SerializeField] AudioClip hungryTransformation;
    [SerializeField] AudioClip heartbeat;


    void Awake()
    {
        playerControlls = new PlayerControlls();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        gameManager = GameManager.Instance;
        uiManager = GameManager.Instance.uiManager;
        playerLayerID = LayerMask.NameToLayer("Player");
        enemyLayerID = LayerMask.NameToLayer("Enemy");
        SetPlayerForm(Form.Hungry);
    }

    void Update()
    {
        if(CurrentHP <= (MAX_HP/4))
        {
            heartbeatTimer += Time.deltaTime;
            if (heartbeatTimer > 0.8f)
            {
                GameManager.Instance.audioManager.PlaySFX(heartbeat);
                heartbeatTimer = 0;
            }
        }
        
        animator.SetFloat("hSpeed", Mathf.Abs(rigidBody.velocity.x));
        animator.SetFloat("vSpeed", rigidBody.velocity.y);
        animator.SetBool("isGrounded", IsGrounded());

        if (IsGameplayBlocked) return;

        if (currentForm == Form.Mahou)
        {
            if (isCharging)
                currentChargedTime += Time.deltaTime;
            else
            {
                chargingVFX.Stop();
            }
            chargingVFX.SetBool("isCharged", currentChargedTime >= shot3MinimumChargeTime);
        }
        else if (currentForm == Form.Hungry)
        {
            if (MAX_HUNGRY <= currentHungry)
            {
                SetPlayerForm(Form.Normal);
            }
        }
            walkNormalVFX.enabled = move.IsPressed() && IsGrounded() && Mathf.Abs(rigidBody.velocity.x) > 0.01f;

        darkComboTimer += Time.deltaTime;
        if (MAX_MP > currentMP)
            RegenMp(currentPlayerForm.manaRegenRate);

        if (currentHungry > 0 && !isGameplayBlocked)
            ConsumeHungry(currentPlayerForm.hungryDepletionRate);

        if (currentHungry <= 0)
        {
            Starve();
        }
    }

    void OnEnable()
    {
        move = playerControlls.Player.Move;
        move.Enable();

        jump = playerControlls.Player.Jump;
        jump.Enable();
        jump.performed += Jump;

        fall = playerControlls.Player.Fall;
        fall.Enable();
        fall.performed += Fall;

        attack = playerControlls.Player.Attack;
        attack.Enable();
        attack.performed += Attack;

        charge = playerControlls.Player.Charge;
        charge.Enable();
        charge.performed += Charge;

        special = playerControlls.Player.Special;
        special.Enable();
        special.performed += Special;

        changeFormNormal = playerControlls.Player.ChangeFormNormal;
        changeFormNormal.Enable();
        changeFormNormal.performed += ChangeFormNormal;

        changeFormMahou = playerControlls.Player.ChangeFormMahou;
        changeFormMahou.Enable();
        changeFormMahou.performed += ChangeFormMahou;

        changeFormDark = playerControlls.Player.ChangeFormDark;
        changeFormDark.Enable();
        changeFormDark.performed += ChangeFormDark;

        pause = playerControlls.Game.Pause;
        pause.Enable();
        pause.performed += GameManager.Instance.PauseGame;
    }

    void Jump(InputAction.CallbackContext context)
    {
        if (!IsGrounded() || isMovementBlocked) return;
        rigidBody.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        GameManager.Instance.audioManager.PlaySFX(jumpSound);
    }

    void Fall(InputAction.CallbackContext context)
    {
        if (rigidBody.velocity.y <= 0) return;
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
    }

    void Attack(InputAction.CallbackContext context)
    {
        GameManager.Instance.audioManager.StopChargeAudioSource();
        if (isMovementBlocked) return;
        if (!ConsumeMp(currentPlayerForm.attackMPCost)) return;
        switch (currentForm)
        {
            case Form.Normal:
                StartCoroutine(BlockMovementForTime(currentPlayerForm.cooldownAttack));
                animator.Play("NormalHit");
                Collider2D[] normalHitEnemies = Physics2D.OverlapCircleAll(new Vector2(
                    attackPoint.position.x + (sprite.flipX ? -normalAttackPointOffset.x : normalAttackPointOffset.x),
                    attackPoint.position.y + normalAttackPointOffset.y),
                    attackRange, enemyLayer);
                foreach (Collider2D enemy in normalHitEnemies)
                {
                    enemy.GetComponentInParent<BaseEnemy>().TakeDamage(baseAttackDamage);
                }
                PlayHitSound(normalHitEnemies);
                break;
            case Form.Mahou:
                StartCoroutine(BlockMovementForTime(currentPlayerForm.cooldownAttack));
                GameObject shotObject = GetShot();
                if (sprite.flipX)
                    Instantiate(shotObject, leftShotSource.position, leftShotSource.rotation);
                else
                    Instantiate(shotObject, rightShotSource.position, rightShotSource.rotation);
                animator.Play("NormalHit");
                if (shotObject == mahouForm.projectile3)
                {
                    ConsumeMp(currentPlayerForm.specialMPCost);
                }
                isCharging = false;
                break;
            case Form.Dark:
                if (darkComboTimer > darkComboTimerLimit || move.IsPressed() || !IsGrounded())
                    darkAttackCount = 0;
                StartCoroutine(BlockMovementForTime(currentPlayerForm.cooldownAttack));
                Collider2D[] darkHitEnemies = Physics2D.OverlapCircleAll(attackPoint.position +

                    (sprite.flipX ? -darkAttackPointOffset : darkAttackPointOffset),
                    attackRange + (bonusDarkAttackRangePerHit * darkAttackCount), enemyLayer);
                
                foreach (Collider2D enemy in darkHitEnemies)
                {
                    enemy.GetComponentInParent<BaseEnemy>().TakeDamage(baseAttackDamage + darkAttackCount, Form.Dark);
                }
                PlayHitSound(darkHitEnemies, darkAttackCount);
                switch (darkAttackCount)
                {
                    case 1:
                        darkComboTimer = 0;
                        darkAttackCount = 2;
                        animator.Play("DarkHit2");
                        break;
                    case 2:
                        animator.Play("DarkHit3");
                        darkAttackCount = 0;
                        break;
                    default:
                        darkComboTimer = 0;
                        animator.Play("DarkHit1");
                        darkAttackCount = 1;
                        break;
                }

                break;
            case Form.Hungry:
                StartCoroutine(BlockMovementForTime(currentPlayerForm.cooldownAttack));
                animator.Play("NormalHit");
                hungryHit.SetBool("isFlipped", sprite.flipX);
                hungryHit.SetVector3("positionOffset", sprite.flipX ? -hungryAttackPointOffset : hungryAttackPointOffset);
                hungryHit.Play();

                Collider2D[] hungryHitEnemies = Physics2D.OverlapCircleAll(new Vector2(
                    attackPoint.position.x + (sprite.flipX ? -hungryAttackPointOffset.x : hungryAttackPointOffset.x),
                    attackPoint.position.y + hungryAttackPointOffset.y),
                    hungryAttackRange, enemyLayer);
                foreach (Collider2D enemy in hungryHitEnemies)
                {
                    enemy.GetComponentInParent<BaseEnemy>().TakeDamage(baseAttackDamage, Form.Hungry);
                }
                PlayHitSound(hungryHitEnemies);
                break;
        }
    }

    void PlayHitSound(Collider2D[] hitList, int comboHit = 0)
    {
        switch (CurrentForm)
        {
            case Form.Normal:
                if (hitList.Length > 0) GameManager.Instance.audioManager.PlaySFX(headbutHit);
                else GameManager.Instance.audioManager.PlaySFX(headbutMiss);
                break;
            case Form.Dark:
                if (hitList.Length > 0) 
                    GameManager.Instance.audioManager.PlaySFX(comboHit == 1? darkCombo2 : comboHit == 2 ? darkCombo3 : darkCombo1);
                else GameManager.Instance.audioManager.PlaySFX(darkComboMiss,false, 1, comboHit == 1 ? 0.9f : comboHit == 2 ? 0.7f : 1.1f);
                break;
            case Form.Hungry:
                if (hitList.Length > 0) GameManager.Instance.audioManager.PlaySFX(punchHit);
                else GameManager.Instance.audioManager.PlaySFX(punchMiss);
                break;
            case Form.Mahou:
            default:
                break;
        }
    }

    void Charge(InputAction.CallbackContext context)
    {
        currentChargedTime = 0;
        chargingVFX.SetBool("isCharged", false);
        isCharging = true;
        if (currentForm != Form.Mahou) return;
        GameManager.Instance.audioManager.FadeInSFXLoop(chargeSound);
        chargingVFX.Play();
    }

    void Special(InputAction.CallbackContext context)
    {
        if (isMovementBlocked) return;

        switch (currentForm)
        {
            case Form.Normal:
                if (!ConsumeFood(1)) return;
                animator.Play("NormalSpecial");
                GameManager.Instance.audioManager.PlayDelayedSFX(eat, 0.3f);
                StartCoroutine(BlockMovementForTime(2f));
                break;
            case Form.Mahou:
                Attack(context);
                break;
            case Form.Dark:
                if (!ConsumeMp(currentPlayerForm.specialMPCost)) return;
                if (!IsGrounded()) return;
                StartCoroutine(DodgeRoll());
                animator.Play("NormalSpecial");
                break;
            case Form.Hungry:
                if (!ConsumeMp(currentPlayerForm.specialMPCost)) return;
                StartCoroutine(BlockMovementForTime(1f));
                StartCoroutine(DelayedFireball(0.7f));
                animator.Play("NormalSpecial");
                break;
        }
    }

    IEnumerator DodgeRoll()
    {
        isDodging = true;
        isInvencible = true;
        GameManager.Instance.audioManager.PlaySFX(dodge);
        Physics2D.IgnoreLayerCollision(playerLayerID, enemyLayerID, true);
        StartCoroutine(BlockMovementForTime(darkDodgeInvencibilityDuration));
        rigidBody.AddForce(sprite.flipX ? -darkDodgeImpulseValue : darkDodgeImpulseValue, ForceMode2D.Impulse);
        yield return new WaitForSeconds(darkDodgeInvencibilityDuration);
        Physics2D.IgnoreLayerCollision(playerLayerID, enemyLayerID, false);
        isInvencible = false;
        isDodging = false;
    }

    IEnumerator DelayedFireball(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        if (sprite.flipX)
            Instantiate(hungryForm.projectile1, fireballLeftSource.position, fireballLeftSource.rotation);
        else
            Instantiate(hungryForm.projectile1, fireballRightSource.position, fireballRightSource.rotation);
    }

    GameObject GetShot()
    {
        if (currentChargedTime >= shot3MinimumChargeTime && isCharging)
            return mahouForm.projectile3;
        else if (currentChargedTime >= shot2MinimumChargeTime && isCharging)
            return mahouForm.projectile2;
        return mahouForm.projectile1;
    }

    void ChangeFormNormal(InputAction.CallbackContext context)
    {
        if (isMovementBlocked) return;
        SetPlayerForm(Form.Normal);
    }

    void ChangeFormMahou(InputAction.CallbackContext context)
    {
        if (isMovementBlocked) return;
        SetPlayerForm(Form.Mahou);
    }

    void ChangeFormDark(InputAction.CallbackContext context)
    {
        if (isMovementBlocked) return;
        SetPlayerForm(Form.Dark);
    }

    public void GetFood(int quantity)
    {
        sushiStock += quantity;
        gameManager.uiManager.UpdateUIValues();
    }

    public void SetSushiStock(int quantity)
    {
        sushiStock = quantity;
        gameManager.uiManager.UpdateUIValues();
    }

    bool CheckSushiStock(int quantityToCheck)
    {
        if (sushiStock < quantityToCheck)
            return false;
        return true;
    }

    bool ConsumeFood(int quantity)
    {
        if (!CheckSushiStock(quantity)) return false;
        if (!ConsumeMp(currentPlayerForm.specialMPCost)) return false;
        sushiStock--;
        RegenHp(3, 0.5f);
        RegenHungry(3);
        return true;
    }

    void ConsumeHp(float quantity)
    {
        currentHP = Mathf.Max(currentHP - quantity, 0);
        uiManager.UpdateUIValues();
        uiManager.UpdateFormUI();
    }

    void RegenHp(float quantity, float delay = 0)
    {
        StartCoroutine(HealHP(quantity, delay));
    }

    IEnumerator HealHP(float quantity, float delay = 0)
    {
        yield return new WaitForSeconds(delay);

        for (int i = 0; i < quantity && currentHP < MAX_HP;  i++)
        {
            if (currentHP == 0) break;
            currentHP++;
            GameManager.Instance.audioManager.PlaySFX(heal);
            uiManager.UpdateUIValues();
            uiManager.UpdateFormUI();
            yield return new WaitForSeconds(0.2f);
        }
    }

    bool ConsumeMp(float expendedManaValue)
    {
        if (currentMP < expendedManaValue)
            return false;
        currentMP -= expendedManaValue;
        uiManager.UpdateUIValues();
        uiManager.UpdateFormUI();
        return true;
    }

    void RegenMp(float quantity)
    {
        currentMP = Mathf.Min(currentMP + quantity, MAX_MP);
        uiManager.UpdateUIValues();
        uiManager.UpdateFormUI();
    }

    void ConsumeHungry(float quantity)
    {
        currentHungry = Mathf.Max(currentHungry - quantity, 0);
        uiManager.UpdateUIValues();
        uiManager.UpdateFormUI();
    }

    void RegenHungry(float quantity)
    {
        currentHungry = Mathf.Min(currentHungry + quantity, MAX_HUNGRY);
        uiManager.UpdateUIValues();
        uiManager.UpdateFormUI();
    }

    public void SetFullStats()
    {
        RegenHp(MAX_HP);
        RegenMp(MAX_MP);
        RegenHungry(MAX_HUNGRY);
        uiManager.UpdateUIValues();
    }

    void Starve()
    {
        SetPlayerForm(Form.Hungry);
        isStarving = true;
    }

    void OnDisable()
    {
        playerControlls.Disable();
    }

    public void SetPlayerForm(Form form)
    {
        chargingVFX.Stop();
        GameManager.Instance.audioManager.StopChargeAudioSource();
        if (currentForm == form || isStarving) return;
        StartCoroutine(BlockMovementForTime(1.1f));

        currentForm = form;
        switch (form)
        {
            case Form.Normal:
                StartCoroutine(SetNewAnimatorAfterTime(0.5f, normalForm.animator));
                animator.Play("NormalTransform");
                StartCoroutine(BlinkShadowColor(transformationNormalColor, 0.4f, 0.2f));
                hungryWalk.gameObject.SetActive(false);
                StartCoroutine(PlayDelayedVFX(0.4f, transformationNormalVFX));
                GameManager.Instance.audioManager.PlayDelayedSFX(normalTransformation, 0.4f);
                currentPlayerForm = normalForm;
                break;
            case Form.Mahou:
                StartCoroutine(SetNewAnimatorAfterTime(0.5f, mahouForm.animator));
                animator.Play("NormalTransform");
                StartCoroutine(BlinkShadowColor(transformationMahouColor, 0.4f, 0.2f));
                hungryWalk.gameObject.SetActive(false);
                StartCoroutine(PlayDelayedVFX(0f, transformationMahouVFX));
                StartCoroutine(StopDelayedVFX(0.8f, transformationMahouVFX));
                GameManager.Instance.audioManager.PlayDelayedSFX(mahouTransformation, 0.4f);
                currentPlayerForm = mahouForm;
                break;
            case Form.Dark:
                StartCoroutine(SetNewAnimatorAfterTime(0.5f, darkForm.animator));
                animator.Play("NormalTransform");
                StartCoroutine(BlinkShadowColor(transformationDarkColor, 0.4f, 0.2f));
                hungryWalk.gameObject.SetActive(false);
                StartCoroutine(PlayDelayedVFX(0.4f, transformationDarkVFX));
                GameManager.Instance.audioManager.PlayDelayedSFX(darkTransformation, 0.4f);
                currentPlayerForm = darkForm;
                break;
            case Form.Hungry:
                StartCoroutine(SetNewAnimatorAfterTime(0.5f, hungryForm.animator));
                animator.Play("NormalTransform");
                StartCoroutine(BlinkShadowColor(transformationHungryColor, 0.4f, 0.2f));
                hungryWalk.gameObject.SetActive(true);
                StartCoroutine(PlayDelayedVFX(0.4f, transformationHungryVFX));
                GameManager.Instance.audioManager.PlayDelayedSFX(hungryTransformation, 0.4f);
                currentPlayerForm = hungryForm;
                break;
        }
        jumpForce = currentPlayerForm.jumpForce;
        horizontalBaseAcceleration = currentPlayerForm.baseAcceleration;
        maxSpeed = currentPlayerForm.maxSpeed;

        StartCoroutine(SetUIForm(0.5f));
    }

    IEnumerator SetNewAnimatorAfterTime(float delay, RuntimeAnimatorController newAnimator) {
        yield return new WaitForSeconds(delay);
        UpdateFormVision();
        animator.runtimeAnimatorController = newAnimator;
    }

    IEnumerator PlayDelayedVFX(float delay, VisualEffect vfx)
    {
        yield return new WaitForSeconds(delay);
        vfx.Play();
    }

    IEnumerator StopDelayedVFX(float delay, VisualEffect vfx)
    {
        yield return new WaitForSeconds(delay);
        vfx.Stop();
    }

    IEnumerator BlockMovementForTime(float duration)
    {
        SetPlayerBlockedMovement(true);
        yield return new WaitForSeconds(duration);
        SetPlayerBlockedMovement(false);
    }

    IEnumerator SetUIForm(float delay)
    {
        yield return new WaitForSeconds(delay);
        uiManager.DisableAllFormUI();
        uiManager.EnableFormUI(currentForm);
    }

    void FixedUpdate()
    {
        if (currentForm == Form.Hungry)
            SetHungryWalkVfx();

        if (isMovementBlocked) { return; }

        float movement = move.ReadValue<float>();
        rigidBody.velocity = (new Vector2(Mathf.Clamp(rigidBody.velocity.x + (movement * horizontalBaseAcceleration), -maxSpeed, maxSpeed), rigidBody.velocity.y));
        SetSpriteFlipState(movement);
        walkNormalVFX.SetGradient("GroundDustColor", dustRunGradient);
    }

    void SetHungryWalkVfx()
    {
        if (IsGrounded() && Mathf.Abs(rigidBody.velocity.x) > 0.3f)
            hungryWalk.Play();
        else
            hungryWalk.Stop();
        hungryWalk.SetBool("isFlipped", sprite.flipX);
    }

    bool IsGrounded()
    {
        var resultCenter = Physics2D.Raycast(transform.position + new Vector3(0, -characterCollider.size.y / 2 - 0.1f), Vector2.down, 0.4f, groundLayer);
        var resultLeft = Physics2D.Raycast(transform.position + new Vector3(0.28f, -characterCollider.size.y / 2 - 0.1f), Vector2.down, 0.4f, groundLayer);
        var resultRight = Physics2D.Raycast(transform.position + new Vector3(-0.28f, -characterCollider.size.y / 2 - 0.1f), Vector2.down, 0.4f, groundLayer);
        bool isGroundedNewValue = (resultCenter.transform != null || resultLeft.transform != null || resultRight.transform != null) && Mathf.Abs(rigidBody.velocity.y) <= 0.01;

        if (!isGroundedLastValue && isGroundedNewValue) landingNormalVFX.Play();
        isGroundedLastValue = isGroundedNewValue;
        return isGroundedNewValue;
    }

    void SetSpriteFlipState(float speed)
    {
        if (speed == 0) return;
        sprite.flipX = speed < 0 ? true : false;
        walkNormalVFX.SetBool("FlipSprite", sprite.flipX);
    }

    public void TakeDamage(int lostLifeValue)
    {
        if (isInvencible || !IsAlive) return;
        ConsumeHp(lostLifeValue);

        if (currentHP <= 0)
            StartCoroutine(Die());
        else
        {
            StartCoroutine(InvencibilityAfterDamage());
            StartCoroutine(BlockMovementForTime(0.7f));
            animator.Play("NormalTakeDamage");
        }
    }

    public IEnumerator Die()
    {
        currentHP = 0;
        print("died");
        SetPlayerBlockedMovement(true);
        rigidBody.velocity = Vector2.zero;
        SetPlayerGravityScale(0);
        animator.enabled = false;
        StartCoroutine(LerpShadowColor(receiveDamageColor, 0.3f));
        yield return new WaitForSeconds(0.2f);
        SetPlayerSpriteColor(Color.clear);
        StartCoroutine(PlayDieAnimation());
        StartCoroutine(LerpShadowColor(Color.clear, 2)); ;
        StartCoroutine(GameManager.Instance.OnDeath());
    }

    public void SetPlayerSpriteColor(Color color) => sprite.color = color;
    public void SetPlayerGravityScale(float gravityScale) => rigidBody.gravityScale = gravityScale;
    public void SetAnimatorState(bool state) => animator.enabled = state;
    public void SetPlayerBlockedMovement(bool block) => isMovementBlocked = block;

    IEnumerator PlayDieAnimation()
    {
        switch (CurrentForm)
        {
            case Form.Normal:
                deathNormalVFX.Play();
                yield return null;
                break;
            case Form.Mahou:
                deathMahouVFX.Play();
                yield return null;
                break;
            case Form.Dark:
                StartCoroutine(GameManager.Instance.uiManager.ShowDiedImage(2));
                deathDarkVFX.Play();
                yield return null;
                break;
            case Form.Hungry:
                deathHungryVFX.Play();
                yield return new WaitForSeconds(2);
                deathHungryVFX.Stop();
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if ((1 << collision.collider.gameObject.layer & enemyLayer) != 0)
            TakeDamage(1);
    }

    IEnumerator InvencibilityAfterDamage()
    {
        isInvencible = true;
        StartCoroutine(BlinkShadowColor(receiveDamageColor, 0.1f));
        OnHitKnockBack();
        yield return new WaitForSeconds(0.2f);
        SetPlayerSpriteColor(Color.clear);
        for (int i = 0; i < 8; i++)
        {
            SetPlayerSpriteColor(Color.clear);
            yield return new WaitForSeconds(0.05f);
            SetPlayerSpriteColor(Color.white);
            yield return new WaitForSeconds(0.05f);
        }
        isInvencible = false;
    }

    private IEnumerator BlinkShadowColor(Color targetColor, float duration, float stayTime = 0)
    {
        Color startColor = upperShadowSprite.color;
        StartCoroutine(LerpShadowColor(targetColor, duration));

        yield return new WaitForSeconds(duration);
        yield return new WaitForSeconds(stayTime);

        StartCoroutine(LerpShadowColor(startColor, duration));
    }

    private IEnumerator LerpShadowColor(Color targetColor, float duration)
    {
        float elapsedTime = 0f;
        Color startColor = upperShadowSprite.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            upperShadowSprite.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }
        upperShadowSprite.color = targetColor;
     yield return null;
    }

    void OnHitKnockBack()
    {
        if (sprite.flipX)
            rigidBody.AddForce(new Vector2(10f, 5f), ForceMode2D.Impulse);
        else
            rigidBody.AddForce(new Vector2(-10f, 5f), ForceMode2D.Impulse);
    }


    private Color GetGroundColorPoint()
    {
        RaycastHit2D raycast = Physics2D.Raycast(getGroundColorPoint.position, Vector3.forward);
        if (raycast.transform == null) return Color.gray;

        var spriteRenderer = raycast.transform.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) return Color.gray;

        if (spriteRenderer.sprite.texture != null)
        {
            var image = spriteRenderer.sprite.texture;
            return new Color(
                image.GetPixel(image.width / 2, image.height - 1).r,
                image.GetPixel(image.width / 2, image.height - 1).g,
                image.GetPixel(image.width / 2, image.height - 1).b);
        }
        return Color.gray;
    }

    void UpdateFormVision()
    {
        normalVision.SetActive(false);
        mahouVision.SetActive(false);
        darkVision.SetActive(false);
        hungryVision.SetActive(false);

        switch (CurrentForm)
        {
            case Form.Normal:
                normalVision.SetActive(true);
                break;
            case Form.Mahou:
                mahouVision.SetActive(true);
                break;
            case Form.Dark:
                darkVision.SetActive(true);
                break;
            case Form.Hungry:
                hungryVision.SetActive(true);
                break;
        }
    }
}
