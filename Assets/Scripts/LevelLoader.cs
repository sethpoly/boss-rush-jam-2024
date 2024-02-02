using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = .25f;
    public GameObject currentPhase;
    public GameObject nextPhase;

    public void LoadNextPhase()
    {
        StartCoroutine(LoadPhase(nextPhase));
    }

    private IEnumerator LoadPhase(GameObject phase)
    {
        // Play animation
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        // Disable current phase
        currentPhase.SetActive(false);

        // Enable wanted scene
        phase.SetActive(true);
    }

    public IEnumerator LoadScene(string sceneName, float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneName);
    }
}
