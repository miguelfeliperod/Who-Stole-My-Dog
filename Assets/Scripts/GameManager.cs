using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    public static GameManager Instance {  get { return instance; } }
    public PlayerController playerController;
    public UIManager uiManager;
    public DialogueManager dialogueManager;

    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    void Start()
    {
        DontDestroyOnLoad(this);
    }
}
