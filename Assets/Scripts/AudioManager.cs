using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }


    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one AudioManager in the scene");
            Destroy(gameObject);
        }  
        instance = this;
    }
}
