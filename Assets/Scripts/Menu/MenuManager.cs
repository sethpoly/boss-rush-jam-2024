using UnityEngine;

public class MenuManager: MonoBehaviour 
{
    [SerializeField] private LevelLoader levelLoader;
    [SerializeField] private MusicManager musicManager;

    public void StartGame()
    {
        musicManager.PlayButtonClick();
        musicManager.StopMenuTheme();
        StartCoroutine(levelLoader.LoadScene("Game"));
    }

    public void RestartGame()
    {
        StartCoroutine(levelLoader.LoadScene("Menu"));
    }
}