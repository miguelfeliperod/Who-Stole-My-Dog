using Cinemachine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class BossEvent : MonoBehaviour
{
    [SerializeField] DialogueEvent meetEvent;
    [SerializeField] DialogueEvent secondChanceEvent;
    [SerializeField] DialogueEvent afterBattleEvent;
    [SerializeField] DialogueEvent proposalEvent;
    [SerializeField] BossController boss;
    [SerializeField] CinemachineVirtualCamera originalCamera;
    [SerializeField] CinemachineVirtualCamera dialogueCamera;
    [SerializeField] CinemachineVirtualCamera battleCamera;
    [SerializeField] GameObject proposalCanvas;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] Animator propositionBox;
    [SerializeField] GameObject invisibleWall;
    [SerializeField] Animator luna;

    [SerializeField] Transform kaelInitialTransform;
    [SerializeField] Transform kaelProposalPosition;
    Collider2D eventCollider;
    bool makeProposalCoroutine = false;
    AudioManager audioManager;

    private void Awake()
    {
        if (GameManager.Instance.CurrentEventCheckpoint == EventCheckpoint.SecondChance)
            GameManager.Instance.AdvanceEventCheckpoint(EventCheckpoint.PreBoss);
        audioManager = GameManager.Instance.audioManager;
    }

    private void Start()
    {
        eventCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer & playerLayer) == 0) return;
        if (GameManager.Instance.CurrentEventCheckpoint > EventCheckpoint.SecondChance) return;

        StartEventByPhase();
        eventCollider.enabled = false;
    }

    void Update()
    {
        CheckNextEventStatus();
    }

    public void StartEventByPhase()
    {
        switch (GameManager.Instance.CurrentEventCheckpoint)
        {
            case <= EventCheckpoint.Level3:
                ChangeCamera(dialogueCamera);
                meetEvent.StartEvents();
                break;
            case EventCheckpoint.PreBoss:
            case EventCheckpoint.SecondChance:
                boss.sprite.flipX = false;
                ChangeCamera(dialogueCamera);
                secondChanceEvent.StartEvents();
                break;
            case EventCheckpoint.PosBoss:
                GameManager.Instance.playerController.SetPlayerForm(Form.Normal, true);
                GameManager.Instance.playerController.IsTransformationBlocked = true;
                ChangeCamera(dialogueCamera);
                boss.transform.position = kaelInitialTransform.position;
                boss.sprite.flipX = false;
                GameManager.Instance.playerController.Sprite.flipX = false;
                GameManager.Instance.playerController.transform.position = transform.position;
                afterBattleEvent.StartEvents();
                break;
        }
    }

    void CheckNextEventStatus()
    {
        switch (GameManager.Instance.CurrentEventCheckpoint)
        {
            case <= EventCheckpoint.PreBoss:
                if (meetEvent.AllEventsEnded)
                {
                    ChangeCamera(battleCamera);
                    boss.SetFightStatus(true);
                    gameObject.SetActive(false);
                }
                break;
            case EventCheckpoint.SecondChance:
                if (secondChanceEvent.AllEventsEnded)
                {
                    ChangeCamera(battleCamera);
                    boss.SetFightStatus(true);
                    gameObject.SetActive(false);
                }
                break;
            case EventCheckpoint.PosBoss:
                if (afterBattleEvent.AllEventsEnded && !makeProposalCoroutine)
                {
                    makeProposalCoroutine = true;
                    StartCoroutine(Proposal());
                    boss.chargeVfx.Play();
                }
                break;
        }
    }

    IEnumerator Proposal()
    {
        invisibleWall.SetActive(true);
        boss.boxCollider.gameObject.layer = LayerMask.NameToLayer("Default");
        boss.animator.Play("KaelCharge");
        boss.rigidbody2d.constraints = RigidbodyConstraints2D.FreezeAll;
        yield return new WaitForSeconds(1);
        StartCoroutine(LerpKaelPosition(3, new Vector3(0, 2)));
        AudioSource chargeAudioSource = audioManager.PlaySFX(boss.chargeSfx, loop: true);

        yield return new WaitForSeconds(0.8f);
        GameManager.Instance.fadeManager.PlayFlash(Color.white, 0.1f);
        yield return new WaitForSeconds(0.8f);
        GameManager.Instance.fadeManager.PlayFlash(Color.black, 0.1f);
        yield return new WaitForSeconds(0.6f);
        GameManager.Instance.fadeManager.PlayFlash(Color.white, 0.1f);
        yield return new WaitForSeconds(0.6f);
        GameManager.Instance.fadeManager.PlayFlash(Color.black, 0.1f);
        yield return new WaitForSeconds(0.4f);
        GameManager.Instance.fadeManager.PlayFlash(Color.white, 0.1f);
        yield return new WaitForSeconds(0.4f);
        GameManager.Instance.fadeManager.PlayFlash(Color.black, 0.1f);
        yield return new WaitForSeconds(0.3f);
        GameManager.Instance.fadeManager.PlayFlash(Color.white, 0.1f);
        yield return new WaitForSeconds(0.3f);
        GameManager.Instance.fadeManager.PlayFlash(Color.black, 0.1f);
        yield return new WaitForSeconds(0.2f);
        GameManager.Instance.fadeManager.PlayFlash(Color.white, 0.1f);
        yield return new WaitForSeconds(0.2f);
        GameManager.Instance.fadeManager.PlayFlash(Color.black, 0.1f);
        yield return new WaitForSeconds(0.1f);
        GameManager.Instance.fadeManager.PlayFlash(Color.black, 0.1f);
        yield return new WaitForSeconds(0.1f);
        GameManager.Instance.fadeManager.PlayFlash(Color.black, 0.08f);
        yield return new WaitForSeconds(0.1f);
        GameManager.Instance.fadeManager.PlayFadeOut(Color.white, 2f);
        yield return new WaitForSeconds(2);
        
        boss.chargeVfx.Stop();
        chargeAudioSource.Stop();
        boss.sprite.flipX = false;
        boss.animator.Play(BossController.kaelDefeated);
        boss.transform.position = kaelProposalPosition.position;
        luna.GetComponentInParent<Transform>().position = kaelProposalPosition.position + new Vector3(2,0);
        luna.Play("LunaSitted");

        GameManager.Instance.playerController.Sprite.flipX = false;
        boss.rigidbody2d.constraints = RigidbodyConstraints2D.FreezePositionX;
        GameManager.Instance.playerController.IsMovementBlocked = true;
        GameManager.Instance.playerController.transform.position = transform.position;
        audioManager.FadeOutMusic(2f);
        yield return new WaitForSeconds(2f);
        GameManager.Instance.fadeManager.PlayFadeIn(2f);
        yield return new WaitForSeconds(4);
        propositionBox.gameObject.SetActive(true);
        yield return new WaitForSeconds(4);
        propositionBox.Play("RingBoxOpening");
        yield return new WaitForSeconds(2);
        audioManager.FadeInMusic(audioManager.audioPool.Proposal, 2f);
        yield return new WaitForSeconds(6);
        proposalEvent.StartEvents();
    }

    IEnumerator LerpKaelPosition(float duration, Vector3 yDistance)
    {
        float deltaTime = 0f;
        Vector3 currentPosition = boss.transform.position;
        Vector3 targetPosition = currentPosition + yDistance;

        while (deltaTime < duration)
        {
            deltaTime += Time.deltaTime;
            boss.transform.position = Vector3.Lerp(currentPosition, targetPosition, deltaTime / duration);
            yield return null;
        }
    }

    void ChangeCamera(CinemachineVirtualCamera currentCamera)
    {
        originalCamera.Priority = 1;
        dialogueCamera.Priority = 1;
        battleCamera.Priority = 1;

        currentCamera.Priority = 2;
    }
}