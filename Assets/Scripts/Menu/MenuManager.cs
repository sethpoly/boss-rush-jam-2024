using UnityEngine;

public class MenuManager: MonoBehaviour 
{
    [SerializeField] private LevelLoader levelLoader;
    [SerializeField] private MusicManager musicManager;

    public void StartGame()
    {
        musicManager.PlayButtonClick();
        StartCoroutine(levelLoader.LoadScene("Game"));
    }

    public void QuitGame()
    {
        musicManager.PlayButtonClick();
        Application.Quit();
    }
}