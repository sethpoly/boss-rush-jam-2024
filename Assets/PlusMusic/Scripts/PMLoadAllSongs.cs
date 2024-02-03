
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlusMusic;


namespace PlusMusic
{
public class PMLoadAllSongs : MonoBehaviour
{

    [HideInInspector] public enum loadChoice
    {
        loadAllSongs,
        loadBySongId,
        loadByProjectArrayIndex,
    };

    [TextArea(5, 10)]
    public string developerComments = 
        "Use this script to preload (cache) songs that you are using in your game to improve in-game loading times.\n" +
        "It is best to attach this script to an initial loading/menu scene at the beginning of your game.\n" +
        "\n" +
        "NOTE: While playing a scene in-editor," +
        " the song cache may result in a delay of audio when loading a song." +
        " We're aware of this issue and working on a fix. This does not affect in-game play.\n" +
        "\n" +
        "- 'Load All Songs'\nIgnores the two optional lists and preloads all songs in your project\n" +
        "- 'Load By Song Id'\nAdd the unique song IDs of each song you want to preload\n" +
        "- 'Load By Project Array Index'\nAdd the array indecies of your project songs you want to preload\n" +
        "\n"
        ;

    [Header("Song Load Settings")]
    [Tooltip("Select type of loading/caching")]
    public loadChoice selectLoadType;

    [Header("Project Songs Filters")]
    [Tooltip("Optional list of Song IDs")]
    public List<Int64> loadSongsBySongId;
    [Tooltip("Optional list of Project Array Indecies")]
    public List<int> loadSongsByProjectArrayIndex;

    public bool HasFinishedLoading { get => hasFinished; }

    private bool firstRun = true;
    private bool isSongLoaded = true;
    private bool didPlayArrangement = false;
    private bool hasProjectLoaded = false;
    private bool hasFinished = false;

    private float timeUntilDj = 0.0f;
    private int logIntervalDj = 5;
    private int lastLogDj = 0;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("PMLoadAllSongs.Start()");

        if (null == PlusMusic_DJ.Instance)
            Debug.LogError("PMLoadAllSongs.Start(): There is no DJ in the scene!");
    }

    private void Update()
    {
        if (null == PlusMusic_DJ.Instance)
        { 
            timeUntilDj += Time.deltaTime;

            int interVal = (int)(timeUntilDj / logIntervalDj);
            if (interVal > lastLogDj)
            {
                Debug.LogWarningFormat("PMLoadAllSongs.Update(): No DJ for {0} seconds!", (interVal * logIntervalDj));
                lastLogDj = interVal;
            }

            return;
        }

        if (firstRun)
        {
            firstRun = false;
            PlusMusic_DJ.Instance.LoadingProgress += SongLoadingProgress;
            PlusMusic_DJ.Instance.OnArrangementChanged += ArrangementChanged;
        }

        if (!hasProjectLoaded)
        {
            if (PlusMusic_DJ.Instance.HasProjectLoaded)
            {
                hasProjectLoaded = true;
                StartCoroutine(LoadSongsToCache());
            }
        }
    }

    private void OnDestroy()
    {
        if (null != PlusMusic_DJ.Instance)
        {
            PlusMusic_DJ.Instance.LoadingProgress -= SongLoadingProgress;
            PlusMusic_DJ.Instance.OnArrangementChanged -= ArrangementChanged;
        }
    }

    public void ArrangementChanged(PlusMusic_DJ.PMTags tag)
    {
        Debug.Log("PMLoadAllSongs.ArrangementChanged()");

        didPlayArrangement = true;
    }

    public void SongLoadingProgress(float value)
    {
        if (value < 1.0f)
            isSongLoaded = false;
        else
            isSongLoaded = true;
    }

    // TODO: The WaitUntil() functions have the possibility to deadlock!
    // AS: For now, we're simply using a timeout ...
    protected IEnumerator LoadSongsToCache()
    {
        Debug.Log("PMLoadAllSongs.LoadSongsToCache()");

        float loadTimeout = 30.0f;
        float playTimeout = 5.0f;
        hasFinished = false;

        // If there is a current song load, we wait until it is loaded
        Debug.Log("PMLoadAllSongs.LoadSongsToCache(): Waiting for pending song loads ...");
        yield return new WaitUntil(() => isSongLoaded || (loadTimeout -= Time.deltaTime) <= 0.0f);
        if (loadTimeout > 0.0f)
            Debug.Log("PMLoadAllSongs.LoadSongsToCache(): OK, moving on");
        else
            Debug.LogWarning("PMLoadAllSongs.LoadSongsToCache(): Timeout waiting for pending song loads!");

        // If auto play is on, we wait until the song starts playing
        if (PlusMusic_DJ.Instance.settings.auto_play)
        {
            Debug.Log("PMLoadAllSongs.LoadSongsToCache(): Waiting for pending song play ...");
            yield return new WaitUntil(() => didPlayArrangement || (playTimeout -= Time.deltaTime) <= 0.0f);
            if (loadTimeout > 0.0f)
                Debug.Log("PMLoadAllSongs.LoadSongsToCache(): OK, moving on");
            else
                Debug.LogWarning("PMLoadAllSongs.LoadSongsToCache(): Timeout waiting for pending song play!");
        }

        // Now we loop over the song array and load any songs that arent in memory yet
        Debug.Log("PMLoadAllSongs.LoadSongsToCache(): Loading/Caching project songs ...");
        bool auto_play = PlusMusic_DJ.Instance.settings.auto_play;
        PlusMusic_DJ.Instance.settings.auto_play = false;
        int currentSongIndex = PlusMusic_DJ.Instance.currentSongIndex;
        int songsLoaded = 0;
        int songsAlreadyLoaded = 0;
        int songsInProject = 0;

        SoundtrackOptionData[] PMSongs = PlusMusic_DJ.Instance.TheSoundtrackOptions;
        if (null != PMSongs)
        {
            songsInProject = PMSongs.Length;
            for (int so = 0; so<songsInProject; so++)
            {
                Debug.LogFormat("> Song[{0}].is_loaded = {1}", so, PMSongs[so].is_loaded);

                if (!PMSongs[so].is_loaded)
                {
                    bool loadSong = false;
                    Int64 soundtrackId = Int64.Parse(PMSongs[so].id);

                    switch (selectLoadType)
                    {
                        case loadChoice.loadAllSongs:
                            loadSong = true;
                            break;
                        case loadChoice.loadBySongId:
                            if (loadSongsBySongId.Contains(soundtrackId))
                                loadSong = true;
                            break;
                        case loadChoice.loadByProjectArrayIndex:
                            if (loadSongsByProjectArrayIndex.Contains(so))
                                loadSong = true;
                            break;
                    }

                    if (loadSong)
                    {
                        Debug.LogFormat("> Loading Song[{0}] ...", so);

                        isSongLoaded = false;
                        loadTimeout = 30.0f;
                        PlusMusic_DJ.Instance.LoadSoundtrack(soundtrackId);
                        yield return new WaitUntil(() => isSongLoaded || (loadTimeout -= Time.deltaTime) <= 0.0f);
                        if (loadTimeout > 0.0f)
                        { 
                            Debug.LogFormat("PMLoadAllSongs.LoadSongsToCache(): Song[{0}] loaded OK", so);
                            songsLoaded++;
                        }
                        else
                            Debug.LogWarning("PMLoadAllSongs.LoadSongsToCache(): Timeout loading song!");
                    }
                }
                else
                    songsAlreadyLoaded++;
            }
        }

        if ((songsAlreadyLoaded + songsLoaded) > 0)
            Debug.LogFormat(
                "PMLoadAllSongs.LoadSongsToCache(): songsAlreadyLoaded/songsLoaded/songsInProject = {0}/{1}/{2}",
                songsAlreadyLoaded, songsLoaded, songsInProject);
        else
            Debug.LogWarning("PMLoadAllSongs.LoadSongsToCache(): No songs were loaded/cached!");

        PlusMusic_DJ.Instance.currentSongIndex = currentSongIndex;
        PlusMusic_DJ.Instance.settings.auto_play = auto_play;
        hasFinished = true;
    }

}   // End Class
}   // End Namespace
