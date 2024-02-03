
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PlusMusic;


namespace PlusMusic
{
public class PlusMusicSceneManager : MonoBehaviour
{

    //-----------------------------------------------
    // Do NOT use RuntimeInitializeLoadType.SubsystemRegistration!
    // It allows for your DontDestroyOnLoad() game object to be destroyed!
    // https://forum.unity.com/threads/game-runs-fine-in-editor-but-not-in-build.1364256/

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnRuntimeInit()
    {
        Debug.Log("PlusMusicSceneManager.OnRuntimeInit()");

        if (null != PlusMusic_DJ.Instance)
            Debug.Log("PlusMusicSceneManager.OnRuntimeInit(): DJ found, nothing to see here ...");
        else
        {
            Debug.Log("PlusMusicSceneManager.OnRuntimeInit(): No DJ found, let's make one ...");

            Object pluginObject = Resources.Load("PlusMusicPlugin");
            Instantiate(pluginObject, new Vector3(0, 0, 0), Quaternion.identity);
        }
    }

        /*
        //-----------------------------------------------
        private void Awake()
        {
            Debug.Log("PlusMusicSceneManager.Awake()");

            if (null == PlusMusic_DJ.Instance)
                Debug.LogWarning("PlusMusicSceneManager.Awake(): There still is no DJ in the scene!");

            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        //-----------------------------------------------
        private void Start()
        {
            Debug.Log("PlusMusicSceneManager.Start()");

            // By now, we really should have a DJ!
            if (null == PlusMusic_DJ.Instance)
                Debug.LogError("PlusMusicSceneManager.Start(): There really is no DJ in the scene!");
        }

        //-----------------------------------------------
        private void OnDestroy()
        {
            Debug.Log("PlusMusicSceneManager.OnDestroy()");

            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        //-----------------------------------------------
        private void OnSceneUnloaded(Scene current)
        {
            Debug.Log("PlusMusicSceneManager.OnSceneUnloaded(): " + current);
        }
        */

}   // End Class
}   // End Namespace
