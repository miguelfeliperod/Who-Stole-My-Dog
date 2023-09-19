using System.Collections;
using UnityEditor.SearchService;
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

    public Vector2 LastCheckpointPosition => lastCheckpointPosition;
    Vector2 lastCheckpointPosition;

    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
        fadeManager = FindObjectOfType<FadeManager>();
        audioManager = FindObjectOfType<AudioManager>();
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
        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();
        playerController.transform.position = GetInitialPositionPerLevel();
        lastCheckpointPosition = playerController.transform.position;
    }

    Vector2 GetInitialPositionPerLevel() {
        switch (SceneManager.GetActiveScene().name.ToLower())
        {
            case "level1":
                return new Vector2(-6, 0);
            case "level2":
            case "level3":
            default:
                return Vector2.zero;
        }
    }

    void Start()
    {
        lastCheckpointPosition = playerController.transform.position;
        DontDestroyOnLoad(this);
        audioManager.FadeInMusic(audioManager.GetCurrentLevelMusic(SceneManager.GetActiveScene().name),1);
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
        playerController.SetFullStats(false);
        playerController.SetPlayerSpriteState(true);
        if (playerController.SushiStock < 3) playerController.SetSushiStock(3);
        playerController.SetPlayerSpriteColor(Color.white);
        StartCoroutine(uiManager.HideDiedImage(0.1f));

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        fadeManager.PlayFadeIn(1);
        yield return new WaitForSeconds(1);
        
        playerController.SetPlayerBlockedMovement(false);
        audioManager.PlayMusic(audioManager.GetCurrentLevelMusic(SceneManager.GetActiveScene().name));
    }
}