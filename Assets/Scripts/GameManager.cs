using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    public static GameManager Instance {  get { return instance; } }
    public PlayerController playerController;
    public UIManager uiManager;
    public DialogueManager dialogueManager;
    public FadeManager fadeManager;
    public AudioManager audioManager;

    public Level CurrentLevel => currentLevel;
    Level currentLevel = Level.Menu;

    public Vector2 LastCheckpointPosition => lastCheckpointPosition;
    Vector2 lastCheckpointPosition;

    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
        fadeManager = FindObjectOfType<FadeManager>();
    }

    public void PauseGame(InputAction.CallbackContext context)
    {
        if (Time.timeScale > 0)
        {
            Time.timeScale = 0;
            playerController.IsMovementBlocked = true;
            playerController.IsGameplayBlocked= true;
            uiManager.ShowPauseScreen();
        }
        else
        {
            Time.timeScale = 1;
            playerController.IsMovementBlocked = false;
            playerController.IsGameplayBlocked = false;
            uiManager.HidePauseScreen();
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        lastCheckpointPosition = playerController.transform.position;
    }

    void Start()
    {
        lastCheckpointPosition = playerController.transform.position;
        DontDestroyOnLoad(this);
    }

    public void SetCurrentLevel(Level level)
    {
        currentLevel = level;
    }

    public void SetLastCheckpointPosition(Vector2 checkpointPosition)
    {
        lastCheckpointPosition = checkpointPosition;
    }

    public IEnumerator OnDeath()
    {
        audioManager.PlayMusic(audioManager.audioPool.Lose);

        yield return new WaitForSeconds(3);
        fadeManager.PlayFadeOut(Color.black,2);
        yield return new WaitForSeconds(2);
        
        playerController.transform.position = lastCheckpointPosition;
        playerController.SetPlayerGravityScale(playerController.PlayerGravityScale);
        playerController.SetAnimatorState(true);
        playerController.SetPlayerForm(Form.Normal);
        playerController.SetFullStats();
        if (playerController.SushiStock < 3) playerController.SetSushiStock(3);
        playerController.SetPlayerSpriteColor(Color.white);
        StartCoroutine(uiManager.HideDiedImage(0.1f));


        fadeManager.PlayFadeIn(1);
        yield return new WaitForSeconds(1);
        
        playerController.SetPlayerBlockedMovement(false);
        audioManager.PlayMusic(audioManager.GetCurrentLevelMusic(currentLevel));
    }
}

public enum Level
{
    Menu, Rabasa, HorizonteBonito, TemploLunar
}
