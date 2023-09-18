using Cinemachine;
using UnityEngine;

public class BossEvent : MonoBehaviour
{
    [SerializeField] DialogueEvent meetEvent;
    [SerializeField] DialogueEvent AfterBattleEvent;
    [SerializeField] BossController boss;
    [SerializeField] CinemachineVirtualCamera originalCamera;
    [SerializeField] CinemachineVirtualCamera dialogueCamera;
    [SerializeField] CinemachineVirtualCamera battleCamera;
    [SerializeField] LayerMask playerLayer;
    FinalBattlePhases CurrentPhase = FinalBattlePhases.Meet;
    Collider2D eventCollider;

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

    void CheckNextEventStatus()
    {
        switch (CurrentPhase)
        {
            case FinalBattlePhases.Meet:
                if (meetEvent.AllEventsEnded)
                {
                    ChangeCamera(battleCamera);
                }
                break;
            case FinalBattlePhases.AfterBattle:
                break;
            case FinalBattlePhases.Proposal:
                break;
            case FinalBattlePhases.End:
                break;
        }
    }

    public void ChangePhase(FinalBattlePhases phase)
    {
        CurrentPhase = phase;
    }

    private void StartEventByPhase()
    {
        switch (CurrentPhase)
        {
            case FinalBattlePhases.Meet:
                ChangeCamera(dialogueCamera);
                meetEvent.StartEvents();
                break;
            case FinalBattlePhases.AfterBattle:
                break;
            case FinalBattlePhases.Proposal:
                break;
            case FinalBattlePhases.End:
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

public enum FinalBattlePhases
{
    Meet, AfterBattle, Proposal, End
}