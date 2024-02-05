using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    public MusicManager musicManager;
    [SerializeField] private ParticleSystem ps;
    [SerializeField] private LevelLoader levelLoader;

    static public string playerLoadout = "";

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

    static public void SetPlayerLoadout(List<GameObject> cardControllerObjects)
    {
        var text = "";
        foreach(GameObject obj in cardControllerObjects)
        {
            CardController controller = obj.GetComponent<CardController>();
            text += controller.card.cardName + "\n";
        }
        playerLoadout = text;
    }

    static public void ResetPlayerLoadout()
    {
        playerLoadout = "";
    }
}
