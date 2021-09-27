using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{

    public void PlayGame()
    {
        SceneManager.LoadScene("Gameplay");
    }
    public void StartScene()
    {
        SceneManager.LoadScene("Start-UIScene");
    }
    public void PlayFab()
    {
        SceneManager.LoadScene("PlayFab_Scene");
    }

    public static void PlayFabLoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
