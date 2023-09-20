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
    [SerializeField] BossController boss;
    [SerializeField] CinemachineVirtualCamera originalCamera;
    [SerializeField] CinemachineVirtualCamera dialogueCamera;
    [SerializeField] CinemachineVirtualCamera battleCamera;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] GameObject invisibleWall;

    [SerializeField] Transform kaelInitialTransform;
    Collider2D eventCollider;
    bool makeProposalCoroutine = false;

    private void Awake()
    {
        if (GameManager.Instance.CurrentEventCheckpoint == EventCheckpoint.SecondChance)
            GameManager.Instance.AdvanceEventCheckpoint(EventCheckpoint.PreBoss);
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
        GameManager.Instance.audioManager.PlaySFX(boss.chargeSfx, loop: true);
        yield return new WaitForSeconds(1);
        GameManager.Instance.fadeManager.PlayFlash(Color.white, 0.05f);
        yield return new WaitForSeconds(2);
        GameManager.Instance.fadeManager.PlayFlash(Color.black, 0.05f);
        yield return new WaitForSeconds(1);
        GameManager.Instance.fadeManager.PlayFlash(Color.white, 0.05f);
        yield return new WaitForSeconds(1.5f);
        GameManager.Instance.fadeManager.PlayFlash(Color.white, 0.05f);
        yield return new WaitForSeconds(2);
        GameManager.Instance.fadeManager.PlayFlash(Color.black, 0.05f);
        yield return new WaitForSeconds(1);
        GameManager.Instance.fadeManager.PlayFlash(Color.black, 0.05f);
        yield return new WaitForSeconds(1.5f);
        GameManager.Instance.fadeManager.PlayFlash(Color.white, 0.05f);
        yield return new WaitForSeconds(1.5f);
        GameManager.Instance.fadeManager.PlayFlash(Color.black, 0.05f);
        yield return new WaitForSeconds(1.5f);
        GameManager.Instance.fadeManager.PlayFlash(Color.white, 0.05f);
        yield return new WaitForSeconds(1);
        GameManager.Instance.fadeManager.PlayFlash(Color.white, 0.05f);
        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.fadeManager.PlayFlash(Color.black, 0.05f);
        yield return new WaitForSeconds(2);
        GameManager.Instance.fadeManager.PlayFadeOut(Color.white, 1f);
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