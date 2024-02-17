using System.Collections;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(LoadScene("MainMenu"));
    }
    IEnumerator LoadScene(string sceneName)
    {
        yield return new WaitUntil(() => NetworkManager.Singleton != null);
        SceneManager.LoadScene(sceneName);
    }
}
