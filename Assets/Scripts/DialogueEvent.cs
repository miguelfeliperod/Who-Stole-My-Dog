using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueEvent : MonoBehaviour
{
    Image dialogueBox;
    Image characterFace;
    TextMeshProUGUI dialogueText;
    
    InputAction nextDialogue;
    DialogueControlls dialogueControlls;

    int currentDialogueIndex = 0;
    bool isWritting = false;
    bool isEventFinished = false;
    GameManager gameManager;
    public bool AllEventsEnded => allEventsEndend;
    bool allEventsEndend = false;

    [SerializeField] EventCheckpoint eventCheckpoint;
    [SerializeField] public List<GameEvent> gameEvents;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] float startAwaitTime;
    [SerializeField] float finishAwaitTime;

    void Start()
    {
        if ((int)GameManager.Instance.CurrentEventCheckpoint >= (int)eventCheckpoint)
            gameObject.SetActive(false);
        gameManager = GameManager.Instance;

        dialogueControlls = new DialogueControlls();

        dialogueBox = gameManager.dialogueManager.dialogueBox;
        characterFace = gameManager.dialogueManager.characterFace;
        dialogueText = gameManager.dialogueManager.dialogueText;

        SetDialogueCommand();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer & playerLayer) == 0) return;
        gameManager.playerController.IsGameplayBlocked = true;
        StartCoroutine(ContinuousBlockStateMovement(2));
        EventHandler();
    }

    public void StartEvents()
    {
        gameManager.playerController.IsGameplayBlocked = true;
        StartCoroutine(ContinuousBlockStateMovement(2));
        EventHandler();
    }

    void EventHandler()
    {
        isEventFinished = false;
        if (currentDialogueIndex < gameEvents.Count)
            switch (gameEvents[currentDialogueIndex].gameEventType)
            {
                case GameEventType.Dialogue:
                    StartCoroutine(PlayDialogueEvent());
                    break;
                case GameEventType.Audio:
                    StartCoroutine(PlayAudioEvent());
                    break;
                case GameEventType.GameObject:
                    StartCoroutine(PlayMovementEvent());
                    break;
                case GameEventType.WaitEvent:
                    StartCoroutine(PlayWaitEvent());
                    break;
                case GameEventType.Fade:
                    StartCoroutine(PlayFadeEvent());
                    break;
                case GameEventType.Animation:
                    StartCoroutine(PlayAnimationEvent());
                    break;
                case GameEventType.Flash:
                    PlayFlashEvent();
                    break;
                case GameEventType.Sprite:
                    PlaySpriteEvent();
                    break;
                case GameEventType.CrossFade:
                    StartCoroutine(PlayCrossFadeEvent());
                    break;
            }
        else
        {
            StartCoroutine(FinishEvent());
            return;
        }
    }


    // ----------------- Flash --------------------//
    private void PlayFlashEvent()
    {
        GameManager.Instance.fadeManager.PlayFlash(gameEvents[currentDialogueIndex].color, gameEvents[currentDialogueIndex].TimeToWait);
        OnFinishEvent();
    }

    // ----------------- Sprite --------------------//
    private void PlaySpriteEvent()
    {
        gameEvents[currentDialogueIndex].sprite.flipX = gameEvents[currentDialogueIndex].flipX;
        OnFinishEvent();
    }

    // ----------------- Fade --------------------//
    private IEnumerator PlayFadeEvent()
    {
        if (gameEvents[currentDialogueIndex].isFadeIn)
            GameManager.Instance.fadeManager.PlayFadeIn(gameEvents[currentDialogueIndex].TimeToWait);
        else
            GameManager.Instance.fadeManager.PlayFadeOut(Color.black, gameEvents[currentDialogueIndex].TimeToWait);

        yield return new WaitForSeconds(gameEvents[currentDialogueIndex].TimeToWait);

        OnFinishEvent();
    }

    // ----------------- Animation --------------------//
    private IEnumerator PlayAnimationEvent()
    {
        gameEvents[currentDialogueIndex].animator.Play(gameEvents[currentDialogueIndex].text);
        yield return new WaitForSeconds(gameEvents[currentDialogueIndex].TimeToWait);

        OnFinishEvent();
    }

    // ----------------- Wait --------------------//
    private IEnumerator PlayWaitEvent()
    {
        yield return new WaitForSeconds(gameEvents[currentDialogueIndex].TimeToWait);

        OnFinishEvent();
    }

    // ----------------- Game Object --------------------//
    private IEnumerator PlayMovementEvent()
    {
        yield return new WaitForSeconds(gameEvents[currentDialogueIndex].TimeToWait/2);

        if (gameEvents[currentDialogueIndex].isMovement)
            gameEvents[currentDialogueIndex].eventObject.transform.position = gameEvents[currentDialogueIndex].destinyPlace;
        else if (gameEvents[currentDialogueIndex].isEnable)
            gameEvents[currentDialogueIndex].eventObject.SetActive(true);
        else
            gameEvents[currentDialogueIndex].eventObject.SetActive(false);

        yield return new WaitForSeconds(gameEvents[currentDialogueIndex].TimeToWait / 2);

        OnFinishEvent();
    }
    // ----------------- Audio --------------------//

    private IEnumerator PlayAudioEvent()
    {
        yield return new WaitForSeconds(gameEvents[currentDialogueIndex].TimeToWait/2); 

        if (gameEvents[currentDialogueIndex].stopMusic)
            GameManager.Instance.audioManager.FadeOutMusic(1);
        else if(gameEvents[currentDialogueIndex].isSFX)
            GameManager.Instance.audioManager.PlaySFX(gameEvents[currentDialogueIndex].audioSound);
        else
            GameManager.Instance.audioManager.FadeInMusic(gameEvents[currentDialogueIndex].audioSound);

        yield return new WaitForSeconds(gameEvents[currentDialogueIndex].TimeToWait / 2);

        OnFinishEvent();
    }

    // ----------------- CrossFade --------------------//

    private IEnumerator PlayCrossFadeEvent()
    {
        StartCoroutine(GameManager.Instance.audioManager.CrossFadeMusic(gameEvents[currentDialogueIndex].audioSound, gameEvents[currentDialogueIndex].TimeToWait));
        OnFinishEvent();
        yield return null;
    }

    // ----------------- DIALOGUE --------------------//

    IEnumerator PlayDialogueEvent()
    {
        if (!gameManager.dialogueManager.gameObject.activeSelf)
        {
            StartCoroutine(ShowDialogueBox());
            yield return new WaitForSeconds(startAwaitTime + 1);
        }

        SetCurrentDialogue(gameEvents[currentDialogueIndex]);
        yield return null;
    }


    IEnumerator ShowDialogueBox()
    {
        yield return new WaitForSeconds(startAwaitTime);
        gameManager.dialogueManager.gameObject.SetActive(true);
        characterFace.gameObject.SetActive(false);

        float duration = 0.5f;
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            dialogueBox.rectTransform.localScale = Vector2.Lerp(Vector2.zero, Vector2.one, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
        characterFace.sprite = gameEvents[currentDialogueIndex].characterImage;
        characterFace.gameObject.SetActive(true);
    }

    private void SetCurrentDialogue(GameEvent dialogue)
    {
        isWritting = true;
        characterFace.sprite = dialogue.characterImage;
        StartCoroutine(WriteDialogue(dialogue));
    }

    IEnumerator WriteDialogue(GameEvent dialogue)
    {
        int textIndex = 0;
        while(isWritting && textIndex < dialogue.text.Length)
        {
            dialogueText.text = dialogue.text[..textIndex];
            yield return new WaitForSeconds(0.05f);
            if( textIndex % 2 == 0 )
                GameManager.Instance.audioManager.PlaySFX(dialogue.audioSound);

            if (textIndex % 2 == 0)
            {
                characterFace.sprite = textIndex % 4 == 0
                    ? dialogue.characterImage
                    : dialogue.characterImageTalking;
            }
            textIndex++;
        }

        isWritting = false;
        dialogueText.text = dialogue.text;
        characterFace.sprite = dialogue.characterImage;
        isEventFinished = true;
        yield return null;
    }

    private void OnPressNextDialogue(InputAction.CallbackContext context)
    {
        if (isWritting)
        {
            dialogueText.text = gameEvents[currentDialogueIndex].text;
            characterFace.sprite = gameEvents[currentDialogueIndex].characterImage;
            isWritting = false;
        }
        else if (!isEventFinished) return;
        else
        {
            OnFinishEvent();
        }
    }

    void SetDialogueCommand()
    {
        nextDialogue = dialogueControlls.Dialogue.NextDialogue;
        nextDialogue.Enable();
        nextDialogue.performed += OnPressNextDialogue;
    }

    // ----------------- EXTRA --------------------//

    IEnumerator FinishEvent()
    {
        float duration = 0.5f;
        float elapsedTime = 0;
        while (elapsedTime < Time.deltaTime)
        {
            elapsedTime += Time.deltaTime;
            dialogueBox.transform.localScale = Vector2.Lerp(Vector2.one, Vector2.zero, elapsedTime / duration);
            yield return null;
        }
        gameManager.dialogueManager.gameObject.SetActive(false);
        yield return new WaitForSeconds(finishAwaitTime);
        StartCoroutine(ContinuousBlockStateMovement(0.5f, false));
        characterFace.gameObject.SetActive(false);
        gameManager.playerController.IsGameplayBlocked = false;
        nextDialogue.Dispose();
        allEventsEndend = true;
        GameManager.Instance.AdvanceEventCheckpoint(eventCheckpoint);
        Destroy(gameObject, 0.5f);
        yield return null;
    }


    IEnumerator ContinuousBlockStateMovement(float duration, bool state = true)
    {
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            gameManager.playerController.IsMovementBlocked = state;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yield return null;
    }

    void OnFinishEvent()
    {
        currentDialogueIndex++;
        isEventFinished = true;
        EventHandler();
    }
}