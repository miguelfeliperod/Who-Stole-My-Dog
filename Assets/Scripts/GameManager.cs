using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    public static GameManager Instance {  get { return instance; } }
    public PlayerController playerController;
    public UIManager uiManager;
    public DialogueManager dialogueManager;
    public FadeManager fadeManager;

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

    void Start()
    {
        DontDestroyOnLoad(this);
    }

    void SetLastCheckpointPosition(Vector2 checkpointPosition)
    {
        this.lastCheckpointPosition = checkpointPosition;
    }
}
