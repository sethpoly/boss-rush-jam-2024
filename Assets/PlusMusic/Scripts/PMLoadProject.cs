
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlusMusic;


namespace PlusMusic
{
public class PMLoadProject : MonoBehaviour
{
    [Tooltip("Unique Project ID from the PlusMusic Project Manager")][Min(0)]
    public Int64 projectId = 0;
    [Tooltip("Unique Project Key from the PlusMusic Project Manager")]
    public string apiKey = "";
    [Tooltip("Load Project at Scene Start")]
    public bool loadOnStart = false;
    [Tooltip("Play Song after loading")]
    public bool playAfterLoad = false;
    [Tooltip("Player Object for Collider Trigger")]
    public GameObject playerRootObject;
    [Tooltip("Load Project at Trigger Enter")]
    public bool triggerOnEnter = false;
    [Tooltip("Load Project at Trigger Exit")]
    public bool triggerOnExit = false;

    private string playerName = "";


    // Start is called before the first frame update
    void Start()
    {
        if (null == PlusMusic_DJ.Instance)
        {
            Debug.LogError("PMLoadProject.Start(): There is no DJ in the scene!");
            return;
        }

        if (null != playerRootObject)
            playerName = playerRootObject.name;
        else
            if (triggerOnEnter || triggerOnExit)
                Debug.LogWarning("PMLoadProject: Without a PlayerRootObject object this script will trigger off any collider!");

        if (loadOnStart)
            LoadProject();
    }

    public void LoadProject()
    {
        Debug.LogFormat("PMLoadProject.LoadProject(): root = {0}", transform.root.gameObject.name);
        Debug.LogFormat("PMLoadProject.LoadProject(): {0}", projectId);

        if (playAfterLoad)
            PlusMusic_DJ.Instance.LoadProject(projectId, apiKey, playAfterLoad);
        else
            PlusMusic_DJ.Instance.SetCurrentProject(projectId, apiKey, false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerOnEnter)
        {
            if (!String.IsNullOrWhiteSpace(playerName))
                if (playerName != other.gameObject.name)
                    return;

            //Debug.LogWarningFormat("PMLoadProject.OnTriggerEnter(): Tigger! {0}", other.gameObject.name);
            LoadProject();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (triggerOnExit)
        {
            if (!String.IsNullOrWhiteSpace(playerName))
                if (playerName != other.gameObject.name)
                    return;

            //Debug.LogWarningFormat("PMLoadProject.OnTriggerExit(): Tigger! {0}", other.gameObject.name);
            LoadProject();
        }
    }

}   // End Class
}   // End Namespace

