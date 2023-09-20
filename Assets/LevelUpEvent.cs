using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelUpEvent : MonoBehaviour
{
    GameManager gameManager;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] string nextSceneName;
    [SerializeField] AudioClip moveSound;
    [SerializeField] AudioClip applauseSound;
    [SerializeField] AudioClip cheerSound;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer & playerLayer) == 0) return;
        GoToNextLevel();
    }

    public void GoToNextLevel()
    {
        gameManager.playerController.IsGameplayBlocked = true;
        gameManager.playerController.IsMovementBlocked = true;
        StartCoroutine(PrepareToNextLevel());
    }

    IEnumerator PrepareToNextLevel()
    {
        if (GameManager.Instance.CurrentEventCheckpoint > EventCheckpoint.Level3) 
        {
            yield return new WaitForSeconds(2);
            gameManager.audioManager.FadeOutMusic(3f);
            yield return new WaitForSeconds(4);
        }
        else
        {
            gameManager.audioManager.FadeOutMusic(0.5f);
            yield return new WaitForSeconds(1);
        }
        
        if (gameManager.playerController.CurrentForm != Form.Normal)
        {
            gameManager.playerController.SetPlayerForm(Form.Normal);
            yield return new WaitForSeconds(1);
        }
        gameManager.playerController.SetFullStats(true);
        gameManager.audioManager.PlaySFX(GameManager.Instance.audioManager.audioPool.Win);
        gameManager.audioManager.PlaySFX(applauseSound, volume: 0.2f);
        gameManager.audioManager.PlaySFX(cheerSound, volume: 0.12f);
        StartCoroutine(gameManager.uiManager.ShowVictoryImage(1, 3));
        yield return new WaitForSeconds(5);
        gameManager.fadeManager.PlayFadeOut(Color.black,1);
        yield return new WaitForSeconds(1);
        if(GameManager.Instance.CurrentEventCheckpoint < EventCheckpoint.Level3)
            gameManager.audioManager.PlaySFX(moveSound);
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(nextSceneName);
    }
}
