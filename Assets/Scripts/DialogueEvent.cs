using System.Collections;
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
    BoxCollider2D boxCollider;
    int currentDialogueIndex = 0;
    bool isWritting = false;
    GameManager gameManager;

    [SerializeField] DialogueSequence dialogueSequence;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] float startAwaitTime;
    [SerializeField] float finishAwaitTime;

    void Start()
    {
        gameManager = GameManager.Instance;

        boxCollider = GetComponent<BoxCollider2D>();

        dialogueControlls = new DialogueControlls();

        dialogueBox = gameManager.dialogueManager.dialogueBox;
        characterFace = gameManager.dialogueManager.characterFace;
        dialogueText = gameManager.dialogueManager.dialogueText;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer & playerLayer) == 0) return;

        StartCoroutine(ShowDialogueBox());
    }

    void SetDialogueCommand()
    {
        nextDialogue = dialogueControlls.Dialogue.NextDialogue;
        nextDialogue.Enable();
        nextDialogue.performed += OnPressNextDialogue;
    }
    
    void SetNextDialogue()
    {
        if (currentDialogueIndex >= dialogueSequence.dialogueList.Count) {
            StartCoroutine(HideDialogueBox());
            return;
        }
        SetCurrentDialogue(dialogueSequence.dialogueList[currentDialogueIndex]);
    }

    private void SetCurrentDialogue(Dialogue dialogue)
    {
        isWritting = true;
        characterFace.sprite = dialogue.characterImage;
        StartCoroutine(WriteDialogue(dialogue));
    }

    IEnumerator WriteDialogue(Dialogue dialogue)
    {
        int textIndex = 0;
        while(isWritting && textIndex < dialogue.text.Length)
        {
            dialogueText.text = dialogue.text[..textIndex];
            yield return new WaitForSeconds(0.05f);
            if (textIndex % 3 == 0)
            {
                characterFace.sprite = textIndex % 2 == 0
                    ? dialogue.characterImage
                    : dialogue.characterImageTalking;
            }
            textIndex++;
        }

        isWritting = false;
        dialogueText.text = dialogue.text;
        characterFace.sprite = dialogue.characterImage;
        yield return null;
    }

    IEnumerator ShowDialogueBox()
    {
        StartCoroutine(ContinuousBlockMovement(startAwaitTime));
        yield return new WaitForSeconds(startAwaitTime);
        gameManager.dialogueManager.gameObject.SetActive(true);
        characterFace.gameObject.SetActive(false);

        float duration = 0.5f;
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            gameManager.playerController.IsMovementBlocked = true;
            dialogueBox.rectTransform.localScale = Vector2.Lerp(Vector2.zero, Vector2.one, elapsedTime/duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
        characterFace.gameObject.SetActive(true);
        SetDialogueCommand();
        SetNextDialogue();
        yield return null;
    }

    IEnumerator ContinuousBlockMovement(float duration)
    {
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            gameManager.playerController.IsMovementBlocked = true;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yield return null;
    }

    IEnumerator HideDialogueBox()
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
        gameManager.playerController.IsMovementBlocked = false;
        characterFace.gameObject.SetActive(false);
        nextDialogue.Dispose();
        Destroy(gameObject, 0.5f);
        yield return null;
    }

    private void OnPressNextDialogue(InputAction.CallbackContext context)
    {
        if (isWritting)
            isWritting = false;
        else
        {
            currentDialogueIndex++;
            SetNextDialogue();
        }
    }
}