
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using PlusMusic;


namespace PlusMusic
{
public class PMPlayArrangement : MonoBehaviour
{
    [Tooltip("Play Arrangement at Scene Start")]
    public bool playOnStart = false;
    [Tooltip("Player Object for Collider Trigger")]
    public GameObject playerRootObject;
    [Tooltip("Play Arrangement at Trigger Enter")]
    public bool triggerOnEnter = true;
    [Tooltip("Play Arrangement at Trigger Exit")]
    public bool triggerOnExit = false;
    [Tooltip("Transition to use")]
    public TransitionInfo arrangementTransition;

    private string playerName = "";
    private bool hasProjectLoaded = false;


    // Start is called before the first frame update
    void Start()
    {
        if (null == PlusMusic_DJ.Instance)
        {
            Debug.LogError("PMPlayArrangement.Start(): There is no DJ in the scene!");
            return;
        }

        if (null != playerRootObject)
            playerName = playerRootObject.name;
        else
            if (triggerOnEnter || triggerOnExit)
                Debug.LogWarning("PMPlayArrangement: Without a PlayerRootObject object this script will trigger off any collider!");

        //Debug.LogFormat("PMPlayArrangement(): root = {0}", transform.root.gameObject.name);
    }

    private void Update()
    {
        if (!hasProjectLoaded)
        {
            if (PlusMusic_DJ.Instance.HasProjectLoaded)
            {
                hasProjectLoaded = true;
                if (playOnStart)
                    PlayArrangement();
            }
        }
    }

    public void PlayArrangement()
    {
        Debug.LogFormat("PMPlayArrangement.PlayArrangement(): root = {0}", transform.root.gameObject.name);
        Debug.LogFormat("PMPlayArrangement.PlayArrangement(): {0}", arrangementTransition.tag);

        PlusMusic_DJ.Instance.PlayArrangement(arrangementTransition);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerOnEnter)
        {
            if (!String.IsNullOrWhiteSpace(playerName))
                if (playerName != other.gameObject.name)
                    return;

            //Debug.LogWarningFormat("PMPlayArrangement.OnTriggerEnter(): Tigger! {0}", other.gameObject.name);
            PlayArrangement();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (triggerOnExit)
        {
            if (!String.IsNullOrWhiteSpace(playerName))
                if (playerName != other.gameObject.name)
                    return;

            //Debug.LogWarningFormat("PMPlayArrangement.OnTriggerExit(): Tigger! {0}", other.gameObject.name);
            PlayArrangement();
        }
    }

}   // End Class
}   // End Namespace
