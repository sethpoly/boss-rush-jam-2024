using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = .25f;
    public GameObject currentPhase;
    public GameObject nextPhase;

    void Update()
    {
        if(Input.GetMouseButtonDown(0)) 
        {
            LoadNextPhase();
        }
    }

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
}
