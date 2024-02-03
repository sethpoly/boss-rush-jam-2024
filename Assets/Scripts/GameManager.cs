using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    public MusicManager musicManager;
    [SerializeField] private ParticleSystem ps;
    [SerializeField] private LevelLoader levelLoader;

    public void ScreenShake(float duration = .1f, float magnitude = .3f) 
    {
        var shaker = mainCamera.GetComponent<CameraShake>();
        StartCoroutine(shaker.Shake(duration: duration, magnitude: magnitude));
    }

    public void PlayExplosion(Transform transform)
    {
        ps.transform.position = transform.position;
        ps.Play();
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(levelLoader.LoadScene(sceneName, 1f));
    }
}
