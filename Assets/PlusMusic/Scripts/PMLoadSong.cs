
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlusMusic;


namespace PlusMusic
{
public class PMLoadSong:MonoBehaviour
{
    [TextArea(5, 10)]
    public string developerComments =
        "Use this script to load songs from your PlusMusic project.\n" +
        "\n" +
        "NOTE: While playing a scene in-editor," +
        " the song cache may result in a delay of audio when loading a song." +
        " We're aware of this issue and working on a fix. This does not affect in-game play.\n" +
        "\n" +
        "- 'Song Id'\nLoad song using the unique Song ID found in your PlusMusic project\n" +
        "- 'Song Array Index'\nLoad song using the song's array index in your PlusMusic project\n" +
        "- 'Load On Start'\nLoad song at scene start (disable if you use triggers)\n" +
        "- 'Play After Load'\nPlays song immediately after it is loaded (disable if you are scripting playing behavior)\n" +
        "- 'Player Root Object'\nReference to your player object, used as trigger collider." +
        " NOTE: This does not have to be the player, any other collider will do. If omitted, any collider will trigger.\n" +
        "- 'Trigger On Enter' and 'Trigger On Exit'\nSelect what action(s) you want to trigger this script\n" +
        "- 'Song Transition'\nAllows you to customize the transition from the current audio to this song\n" +
        "\n"
        ;

    [Header("Song Load Settings")]
    [Tooltip("Unique Song ID from the PlusMusic Project Manager")]
    public Int64 songId = 0;
    [Tooltip("Array index in the Project Song Array (Zero based)")]
    public int songArrayIndex = 0;
    [Tooltip("Load Song at Scene Start")]
    public bool loadOnStart = false;
    [Tooltip("Play Song after loading")]
    public bool playAfterLoad = false;
    [Tooltip("Player Object for Collider Trigger")]
    public GameObject playerRootObject;
    [Tooltip("Load Song at Trigger Enter")]
    public bool triggerOnEnter = false;
    [Tooltip("Load Song at Trigger Exit")]
    public bool triggerOnExit = false;
    [Tooltip("Transition to use if PlayAfterLoad is true")]
    public TransitionInfo songTransition;

    private string playerName = "";
    private bool hasProjectLoaded = false;

    private float timeUntilDj = 0.0f;
    private int logIntervalDj = 5;
    private int lastLogDj = 0;


    // Start is called before the first frame update
    void Start()
    {
        if (null == PlusMusic_DJ.Instance)
            Debug.LogError("PMLoadSong.Start(): There is no DJ in the scene!");

        if (null != playerRootObject)
            playerName = playerRootObject.name;
        else
            if (triggerOnEnter || triggerOnExit)
            Debug.LogWarning("PMLoadSong: Without a PlayerRootObject object this script will trigger off any collider!");
    }

    private void Update()
    {
        if (null == PlusMusic_DJ.Instance)
        {
            timeUntilDj += Time.deltaTime;

            int interVal = (int)(timeUntilDj / logIntervalDj);
            if (interVal > lastLogDj)
            {
                Debug.LogWarningFormat("PMLoadSong.Update(): No DJ for {0} seconds!", (interVal * logIntervalDj));
                lastLogDj = interVal;
            }

            return;
        }

        if (!hasProjectLoaded)
        {
            if (PlusMusic_DJ.Instance.HasProjectLoaded)
            {
                hasProjectLoaded = true;
                if (loadOnStart)
                    LoadSong();
            }
        }
    }

    public void LoadSong()
    {
        Debug.LogFormat("PMLoadSong.LoadSong(): root = {0}", transform.root.gameObject.name);
        Debug.LogFormat("PMLoadSong.LoadSong(): [{0}] - {1}", songArrayIndex, songId);

        if (songId < 1)
        {
            SoundtrackOptionData[] songs = PlusMusic_DJ.Instance.TheSoundtrackOptions;
            if (null != songs)
                if (songs.Length > songArrayIndex)
                    songId = Int64.Parse(songs[songArrayIndex].id);
                else
                    Debug.LogWarningFormat("PMLoadSong.Start(): Index out of range! {0}/{1}", songArrayIndex, songs.Length);
            else
                Debug.LogWarning("PMLoadSong.Start(): songs is null!");
        }

        if (songId > 0)
            PlusMusic_DJ.Instance.LoadSoundtrack(songId, songTransition, playAfterLoad);
        else
            Debug.LogWarning("PMLoadSong.Start(): songId = 0!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerOnEnter)
        {
            if (!String.IsNullOrWhiteSpace(playerName))
                if (playerName != other.gameObject.name)
                    return;

            //Debug.LogWarningFormat("PMLoadSong.OnTriggerEnter(): Tigger! {0}", other.gameObject.name);
            LoadSong();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (triggerOnExit)
        {
            if (!String.IsNullOrWhiteSpace(playerName))
                if (playerName != other.gameObject.name)
                    return;

            //Debug.LogWarningFormat("PMLoadSong.OnTriggerExit(): Tigger! {0}", other.gameObject.name);
            LoadSong();
        }
    }

}   // End Class
}   // End Namespace
