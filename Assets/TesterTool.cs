
using UnityEngine;

public class TesterTool : MonoBehaviour
{
    [SerializeField] GameObject gameManager;

    private void Awake()
    {
        if(FindAnyObjectByType(typeof(GameManager)) == null)
        {
            gameManager.SetActive(true);
        }
    }
}
