using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] FadeManager fadeManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnStartButtonClick()
    {
        fadeManager.PlayFadeOut(Color.black, 1);
        StartCoroutine(LoadScene(2));
    }

    IEnumerator LoadScene(float delayTime) {
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadSceneAsync("Level1");
    }

}
