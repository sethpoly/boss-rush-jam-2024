using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager: MonoBehaviour 
{
    [SerializeField] private LevelLoader levelLoader;
    public void StartGame()
    {
        StartCoroutine(levelLoader.LoadScene("Game"));
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}