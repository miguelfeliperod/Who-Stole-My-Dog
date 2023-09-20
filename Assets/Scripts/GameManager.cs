using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    public static GameManager Instance {  get { return instance; } }
    public PlayerController playerController;
    public UIManager uiManager;
    public DialogueManager dialogueManager;
    public FadeManager fadeManager;
    public AudioManager audioManager;
    [SerializeField] AudioSource audioSource;
    public static string[] levelNames = { "Menu", "Level1", "Level2", "Level3" };

    ResetControlls resetControlls;

    public Vector2 LastCheckpointPosition => lastCheckpointPosition;
    Vector2 lastCheckpointPosition;
    public EventCheckpoint CurrentEventCheckpoint => currentEventCheckpoint;
    EventCheckpoint currentEventCheckpoint = EventCheckpoint.None;

    void Awake()
    {
        SingletonCheck();

        fadeManager = FindObjectOfType<FadeManager>();
        audioManager = FindObjectOfType<AudioManager>();

        resetControlls = new ResetControlls();
        InputAction reset;
        reset = resetControlls.Reset.Reload;
        reset.Enable();
        reset.performed += ReloadLevel;
    }

    void Start()
    {
        DontDestroyOnLoad(this);
        audioManager.FadeInMusic(audioManager.GetCurrentLevelMusic(SceneManager.GetActiveScene().name), 1);
    }

    void SingletonCheck()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
    }

    private void OnLevelWasLoaded(int level)
    {
        if (SceneManager.GetActiveScene().name == "Credits") {
            audioManager.FadeInMusic(audioManager.GetCurrentLevelMusic(SceneManager.GetActiveScene().name), 1);
            return;
        } 
        SingletonCheck();
        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();
        if (lastCheckpointPosition == null)
            lastCheckpointPosition = GetInitialPositionPerLevel();
        else
            playerController.transform.position = lastCheckpointPosition;

        audioManager.FadeInMusic(audioManager.GetCurrentLevelMusic(SceneManager.GetActiveScene().name), 1);
    }

    public void AdvanceEventCheckpoint(EventCheckpoint eventCheckpoint)
    {
        currentEventCheckpoint = eventCheckpoint;
    }

    void ReloadLevel(InputAction.CallbackContext context)
    {
        StartCoroutine(ReloadLevel());
    }

    public void Forfeit()
    {
        PauseGame();
        StartCoroutine(playerController.Die());
    }

    IEnumerator ReloadLevel()
    {
        fadeManager.PlayFadeOut(Color.black, 1);
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        fadeManager.PlayFadeIn(1);
        yield return new WaitForSeconds(1);
    }

    public void PauseGame(InputAction.CallbackContext context)
    {
        PauseGame();
    }

    void PauseGame()
    {
        if (Time.timeScale > 0)
        {
            if (playerController.IsMovementBlocked || playerController.IsGameplayBlocked) return;
            Time.timeScale = 0;
            playerController.IsMovementBlocked = true;
            playerController.IsGameplayBlocked = true;
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


    Vector2 GetInitialPositionPerLevel() {
        switch (SceneManager.GetActiveScene().name.ToLower())
        {
            case "level1":
                return new Vector2(0, 0);
            case "level2":
            case "level3":
            default:
                return Vector2.zero;
        }
    }

    public void SetLastCheckpointPosition(Vector2 checkpointPosition)
    {
        lastCheckpointPosition = checkpointPosition;
    }

    public IEnumerator OnDeath()
    {
        audioManager.PlayMusic(audioManager.audioPool.Lose);

        yield return new WaitForSeconds(3);
        fadeManager.PlayFadeOut(Color.black, 2);
        yield return new WaitForSeconds(2);
        StartCoroutine(uiManager.HideDiedImage());
        StartCoroutine(ReloadLevel());
    }
}

public enum EventCheckpoint
{
    None, Level1, Level2, Level3, PreBoss, SecondChance, PosBoss
}