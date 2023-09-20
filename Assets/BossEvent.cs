using Cinemachine;
using UnityEngine;

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
    Collider2D eventCollider;

    private void Awake()
    {
        if (GameManager.Instance.CurrentEventCheckpoint >= EventCheckpoint.SecondChance)
            GameManager.Instance.AdvanceEventCheckpoint(EventCheckpoint.PreBoss);
    }

    private void Start()
    {
        eventCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer & playerLayer) == 0) return;

        StartEventByPhase();
        eventCollider.enabled = false;
    }

    void Update()
    {
        CheckNextEventStatus();
    }

    private void StartEventByPhase()
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