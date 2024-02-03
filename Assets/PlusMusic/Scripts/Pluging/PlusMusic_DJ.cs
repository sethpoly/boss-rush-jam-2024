#region Usings
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Audio;
using PlusMusic;
//using UnityEditor.VSAtrribution;

#endregion


namespace PlusMusic
{
    public class PlusMusic_DJ : MonoBehaviour
    {
        #region Initialization
        //-----------------------------------------------
        // Private vars
        //-----------------------------------------------
        private string pluginVersion = "0.0.0";

        private List<SongCacheEntry> songDataCache;
        private int num_arrangements_to_load = 0;

        private string previousTag = "backing_track";
        private bool licenseLimited = false;
        private bool isAudioSource1Playing = true;
        private string next_tag = "backing_track";
        private string whichSoundtrack = "0";

        private int loadedRemoteFiles = 0;
        private int ignoredRemoteFiles = 0;
        private int loadedRemoteUrls = 0;
        private int ignoredRemoteUrls = 0;

        private PlusMusicSettingsSo plusMusicAccount;
        private ParentProjectData parentData;
        private Dictionary<string, AudioClip> map_StingerClip = new Dictionary<string, AudioClip>();
        private Dictionary<string, Coroutine> playingCoroutines = new Dictionary<string, Coroutine>();
        private float lastTime = 0.0f;
        private int loopCounter;
        private bool doPingbacks = true;
        private bool didStartupPingback = false;
        private bool didVSAttribution = false;

        private static int STATE_PLAYING = 1;
        private static int STATE_STOPPED = 2;
        private static int STATE_PAUSED  = 3;
        private static int STATE_MUTED   = 4;
        private static int STATE_UNMUTED = 5;
        private int audioState = STATE_STOPPED;

        private TransitionInfo currentTransition;
        private TransitionInfo afterLoadTransition;
        private float musicVolume = 1.0f;

        private bool sceneUnloadedIsHooked = false;
        private bool killQueue = false;


        //-----------------------------------------------
        // Public vars
        //-----------------------------------------------
        public enum PMTags
        {
            none = 0,
            high_backing,
            low_backing,
            backing_track,
            preview,
            victory,
            failure,
            highlight,
            lowlight,
            full_song
        };
        public enum PMTimings {
            //[Obsolete("Deprecated, use nextBeat instead", false)]
            beats,
            //[Obsolete("Deprecated, use nextBar instead", false)]
            bars, 
            now, 
            nextBeat, 
            nextBar
        };

        // Static singleton instance of this Class
        public static PlusMusic_DJ Instance { get; private set; }

        [TextArea(5, 10)]
        public string developerComments =
            "Don't add this script to a scene object as it is automatically loaded by the 'PlusMusicSceneManager'.\n" +
            "If you have used previous version of this plugin and have scenes with this script attached, " +
            "you will have to remove them all to avoid concurrency conflicts.\n" +
            "\n" +
            "- 'Persists Across Scenes'\nShould always be on to ensure proper song caching\n" +
            "- 'Debug Mode'\nAdds lots of extra debugging to the console log. Disable for production.\n" +
            "- 'Project Id' and 'Auth token'\nYour PlusMusic project credentials from the Project Manager web page\n" +
            "- 'Auto Load Project'\nIn most circumstances you'd want this to be enabled\n" +
            "- 'Audio Mixer Group'\nAllows you to redirect the plugin audio to an existing AudioMixer\n" +
            "\n"
            ;

        [Header("Plugin Settings")]
        [Tooltip("Keep PlusMusic loaded across scenes")]
        public bool persistAcrossScenes = true;
        [Tooltip("Log/Display extra debugging information")]
        public bool debugMode = false;
        [Tooltip("Log the api server requests")]
        public bool logRequestUrls = false;
        [Tooltip("Log the server api responses")]
        public bool logServerResponses = false;

        [Header("Project Settings")]
        [Tooltip("Unique Project ID from the PlusMusic Project Manager")]
        public Int64 projectId = 0;
        [Tooltip("Unique Authentication token from the PlusMusic Project Manager")]
        public string authToken = "";
        [Tooltip("Auto Load Project on Start()")]
        public bool autoLoadProject = true;
        [Tooltip("Auto Play the first song in the project on Start()")]
        public bool autoPlayProject = false;

        [Header("Audio Playback Settings")]
        [Tooltip("If true, any current audio will continue playing even after a scene is unloaded")]
        public bool playAcrossScenes = false;
        [Tooltip("Optional Audio Mixer Group for playback")]
        public AudioMixerGroup audioMixerGroup;
        public AudioSource theAudioSource;
        public AudioSource newAudio;
        public AudioSource stingerSource;
        [Tooltip("The default transition, used if Auto Play Project is enabled")]
        public TransitionInfo defaultTransition = new TransitionInfo();

        [HideInInspector] public AudioSource currentAudioSource;
        [HideInInspector] public ServerArrangementsData serverArrangementsFile;
        [HideInInspector] public SoundtrackOptionData[] theSoundtrackOptions;
        [HideInInspector] public PM_Settings settings;
        [HideInInspector] public int currentSongIndex = -1;

        [HideInInspector] public bool isProjectLoaded = false;

        public event Action<SoundtrackOptionData[]> OnSoundTrackOptionsReceived;
        public event Action<float> OnLoadingProjectProgress;
        public event Action<float> LoadingProgress;
        public event Action<string> RealTimeDebug;
        public event Action<int> OnAudioStateChanged;
        public event Action<PMTags> OnArrangementChanged;
        public event Action<bool> OnInit;

        public bool HasProjectLoaded { get => isProjectLoaded; }
        public string WhichSoundtrack { get => whichSoundtrack; set => whichSoundtrack = value; }
        public string PluginVersion { get => pluginVersion; }
        public SoundtrackOptionData[] TheSoundtrackOptions { get => theSoundtrackOptions; }
        public TransitionInfo GetCurrentTransition { get => currentTransition; }



        public delegate bool HookUnityVSA(string actionName, string partnerName, string customerUid);
        public static HookUnityVSA VSASendAttributionEvent;


        //-----------------------------------------------
        private void Init()
        {
            Debug.Log("------------------ PlusMusicDJ ----------------------");
            if (debugMode)
                Debug.Log("DJ.Init()");

            // Load the settings
            plusMusicAccount = Resources.Load<PlusMusicSettingsSo>("PlusMusicSettingsSo");
            if (null != plusMusicAccount)
            {
                // Load all stinger sound effects
                AudioClip[] stingers = Resources.LoadAll<AudioClip>("Stinger");
                for (int i = 0; i < stingers.Length; i++)
                    if (!map_StingerClip.ContainsKey(stingers[i].name))
                        map_StingerClip.Add(stingers[i].name, stingers[i]);

                // Set current to first as default so set volume doesn't break if we're called 
                // by another class before Update()
                SetVolume(musicVolume);
                newAudio.volume = 0.0f;
                currentAudioSource = theAudioSource;
                if (null != audioMixerGroup)
                    currentAudioSource.outputAudioMixerGroup = audioMixerGroup;

                // Get the saved settings
                didVSAttribution = plusMusicAccount.DidVSAttribution;

                // Set the package data
                plusMusicAccount.SetPackageData();
                if (null != plusMusicAccount.PackageData && null != plusMusicAccount.PackageData.version)
                    pluginVersion = plusMusicAccount.PackageData.version;
                else
                    Debug.LogError("DJ.Init(): package.json is missing or corrupt!");

                // Coroutine map
                playingCoroutines.Add("transitionCoroutine", null);
                playingCoroutines.Add("effectCoroutine", null);
                playingCoroutines.Add("getArragementCoroutine", null);
                playingCoroutines.Add("queueSong", null);
                licenseLimited = false;

                // Default settings
                settings = new PM_Settings();
                settings.target      = GetEnvVariable("PM_TARGET", "app");
                settings.username    = GetEnvVariable("PM_USER", "");
                settings.password    = GetEnvVariable("PM_PASS", "");
                settings.credentials = "";
                if ((!string.IsNullOrWhiteSpace(settings.username)) && (!string.IsNullOrWhiteSpace(settings.password)))
                    settings.credentials = settings.username + ":" + settings.password + "@";
                settings.base_url = "https://" + settings.credentials + settings.target + ".plusmusic.ai/api/plugin/";

                if (debugMode)
                {
                    Debug.LogFormat("> target = {0}", settings.target);
                    Debug.LogFormat("> username = {0}", settings.username);
                    Debug.LogFormat("> password = {0}", settings.password);
                    Debug.LogFormat("> credentials = {0}", settings.credentials);
                    Debug.LogFormat("> base_url = {0}", settings.base_url);
                    Debug.LogFormat("> doPingbacks = {0}", doPingbacks);
                }

                // Defaults for the Transition
                currentTransition = CopyTransition(defaultTransition);
                afterLoadTransition = CopyTransition(defaultTransition);

                // Get id and key from either the editor or the environment
                // NOTE: In production neither of these will be present so settings need to be loaded from a save file
                string defProjectId = projectId.ToString();
                string defProjectKey = authToken;
                if (0 == projectId)
                {
                    defProjectId  = GetEnvVariable("PM_PROJECT", defProjectId);
                    defProjectKey = GetEnvVariable("PM_API_KEY", defProjectKey);
                }

                // If auto load is enabled and we have supplied credentials
                // we try to load the specified project data
                if (autoLoadProject)
                {
                    doPingbacks = true;
                    LoadProject(Int64.Parse(defProjectId), defProjectKey, autoPlayProject);
                }
                else
                { 
                    doPingbacks = false;
                    SetCurrentProject(Int64.Parse(defProjectId), defProjectKey, false);
                }
            }
            else
            {
                string err_str = "DJ.Init(): PlusMusic account is not configured!";
                Debug.LogError(err_str);
                RealTimeDebug?.Invoke(err_str);
            }
        }

        //-----------------------------------------------
        private void Awake()
        {
            // Check if we are an imposter
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;    // Nope, we're the real deal

            if (debugMode)
                Debug.Log("DJ.Awake()");

            // Moved DontDestroyOnLoad() to Awake() to make sure we have a valid reference
            // for other classes in their Start() functions
            if (debugMode)
                Debug.LogFormat("DJ.Awake(): persistAcrossScenes = {0}", persistAcrossScenes);

            if (persistAcrossScenes)
            {
                if (debugMode)
                    Debug.LogFormat("DJ.Awake(): Application.isPlaying = {0}", Application.isPlaying);

                if (Application.isPlaying)
                    DontDestroyOnLoad(Instance);
            }

            OnInit?.Invoke(false);
            Init();
            OnInit?.Invoke(true);
        }

        //-----------------------------------------------
        private void Start()
        {
            if (debugMode)
                Debug.Log("DJ.Start()");

            SceneManager.sceneUnloaded += OnSceneUnloaded;
            sceneUnloadedIsHooked = true;
        }

        //-----------------------------------------------
        private void OnDestroy()
        {
            if (debugMode)
                Debug.LogFormat("DJ.OnDestroy(): sceneUnloadedIsHooked = {0}", sceneUnloadedIsHooked);

            plusMusicAccount.SaveData(
                settings.project_id, settings.api_key, SystemInfo.deviceUniqueIdentifier,
                musicVolume, autoLoadProject, autoPlayProject, debugMode, logServerResponses,
                didVSAttribution
            );

            if (sceneUnloadedIsHooked)
                SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        //-----------------------------------------------
        private void OnSceneUnloaded(Scene current)
        {
            Debug.LogFormat("DJ.OnSceneUnloaded(): {0}", current.name);

            if (!playAcrossScenes)
            { 
                killQueue = true;
                StopAudio();
            }
        }

        /**
        * @brief Make the supplied project the current
        * @param projectId
        * @param projectKey
        */
        public void SetCurrentProject(Int64 projectId, string projectKey, bool autoPlay)
        {
            if (debugMode)
                Debug.LogFormat("DJ.SetCurrentProject(): autoPlay = {0}", autoPlay);

            // Set the project settings
            settings.project_id  = projectId;
            settings.api_key     = projectKey;
            settings.auto_play   = autoPlay;
            
            if (debugMode)
            {
                Debug.LogFormat("> project_id = {0}", settings.project_id);
                Debug.LogFormat("> api_key = {0}", settings.api_key);
                Debug.LogFormat("> auto_play = {0}", settings.auto_play);
            }
        }

        #endregion


        #region Basic Methods
        void Update()
        {
            if (isProjectLoaded)
            { 
                if (currentAudioSource.clip != null)
                {
                    RealTimeDebug?.Invoke("soundtrack " + WhichSoundtrack + "'s " + next_tag + " (length: " + currentAudioSource.clip.length + ") playing for " + currentAudioSource.time.ToString("0.00"));
                }

                // First frame of the loop
                if (lastTime > currentAudioSource.time)
                {
                    loopCounter++;
                }
                lastTime = currentAudioSource.time;
            }
        }

        private void LateUpdate()
        {
            if (licenseLimited)
            {
                RealTimeDebug?.Invoke("SOUNDTRACK UNAVAILABLE");
                theAudioSource.Stop();
                newAudio.Stop();
            }
        }
    
        #endregion
    
        #region Transition Methods
    
        private float GetNextClosestTime(float[] countsToUse)
        {
            float sendThis = 0.0f;

            if (null != countsToUse)
            { 
                foreach (float count in countsToUse)
                {
                    if (currentAudioSource.time < count)
                    {
                        sendThis = count - currentAudioSource.time;
                        break;
                    }
                }
            }
            else
            {
                Debug.LogError("GetNextClosestTime(): countsToUse is null!");
            }

            return sendThis;
        }
        #endregion


        #region Web Calls

        /**
        * @brief It storage the data of the song, events and device
        * @param eventText - The type of pingback
        * @param pingProject - The project which is playing
        * @param pingSoundtrack - The sountrack which is playing
        * @param pingTag - The segment of the sountrack which is playing
        * @param pingTransitionType - The type of transition
        * @param pingTransitionTiming - The if it was useed a transition by timing
        * @param pingTransitionDelay - The delay that the transition requires
        * @param isUsingStinger - If it is using a stinger in the transition
        */
        private bool SetPingBackInfo(
            string eventText, Int64 pingProject, string pingSoundtrack, string pingTag, 
            string pingTransitionType, string pingTransitionTiming, float pingTransitionDelay, bool isUsingStinger)
        {
            if (!doPingbacks) return false;

            if (debugMode)
                Debug.Log("DJ.SetPingBackInfo()");

            if (0 == pingProject)
            {
                Debug.Log("DJ.SetPingBackInfo(): No project configured, skipping ...");
                return false;
            }

            PingBack pingBackData = new PingBack()
            {
                os = SystemInfo.operatingSystem,
                event_text = eventText,
                device_id = SystemInfo.deviceUniqueIdentifier,
                in_editor = Application.isEditor,
                platform = "Unity",
                title = Application.productName,
                connected = Application.internetReachability.ToString(),
                is_using_stinger = isUsingStinger,
                project_id = ((0 == pingProject) ? -1 : pingProject),
                arrangement_id = (string.IsNullOrWhiteSpace(pingSoundtrack) ? -1 : Int64.Parse(pingSoundtrack)),
                arrangement_type = pingTag,
                transition_type = pingTransitionType,
                transition_timing = pingTransitionTiming,
                transition_delay = pingTransitionDelay,
                time = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                web_url = "",
                plugin_version = PluginVersion,
                play_id = ""
            };

            string sendingData = JsonUtility.ToJson(pingBackData);

            // NOTE: 'event' is a reserved keyword and can't be used in the PingBack data class
            // We use 'event_text' instead and replace it with 'event' in the data string
            // We also replace "" and ":-1" with null to indicate missing string and number values
            sendingData = "{\"ping_backs\":[" +
                sendingData.Replace("event_text", "event")
                    .Replace("\"\"", "null")
                    .Replace(": -1", ": null")
                    .Replace(":-1", ":null") +
                "]}";

            StartCoroutine(UploadPingBack(sendingData));

            return true;
        }

        IEnumerator LoadDefaultProject(bool autoPlay)
        {
            Debug.Log("DJ.LoadDefaultProject()");

            string finalURL = String.Format(
                "{0}default-project?plugin_version={1}",
                settings.base_url, PluginVersion);

            if (debugMode)
                Debug.LogFormat("DJ.LoadDefaultProject(): finalURL = {0}", finalURL);

            UnityWebRequest webRequest = UnityWebRequest.Get(finalURL);
            webRequest.SetRequestHeader("Accept", "application/json");

            yield return webRequest.SendWebRequest();
            string jsonString = webRequest.downloadHandler.text;

            if (logServerResponses)
                Debug.LogFormat("> response = {0}", Regex.Replace(jsonString, @"\r\n?|\n", ""));

            if (webRequest.responseCode == 200 || webRequest.responseCode == 201)
            {
                DefaultProjectInfo defaultProject = JsonUtility.FromJson<DefaultProjectInfo>(jsonString);
                DefaultProjectData projectData;

                if (null != defaultProject)
                {
                    projectData = defaultProject.default_project;

                    if (debugMode)
                    { 
                        Debug.Log($"defaultProject.id = {projectData.id}");
                        Debug.Log($"defaultProject.plugin_api_key = {projectData.plugin_api_key}");
                    }

                    if (projectData.id > 0)
                        LoadProject(projectData.id, projectData.plugin_api_key, autoPlay);
                    else
                        Debug.LogError("DJ.LoadDefaultProject() failed: No project data in response!");
                }
                else
                    Debug.LogError("DJ.LoadDefaultProject() failed: No project info in response!");
            }
            else
            {
                Debug.LogError("DJ.LoadDefaultProject() failed: " + webRequest.error);
            }
        }

        IEnumerator UploadPingBack(string dataToSend)
        {
            string finalURL = settings.base_url + "ping-backs";

            if (logRequestUrls)
                Debug.LogFormat("DJ.UploadPingBack(): finalURL = {0}", finalURL);
            if (debugMode)
                Debug.LogFormat("> dataToSend = {0}", dataToSend);

            var webRequest = new UnityWebRequest(finalURL, "POST");
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(dataToSend);
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Accept", "application/json");
            webRequest.SetRequestHeader("x-api-key", settings.api_key);

            yield return webRequest.SendWebRequest();
            string jsonString = webRequest.downloadHandler.text;

            if (debugMode)
                Debug.LogFormat("> responseCode = {0}, result = {1}", webRequest.responseCode, webRequest.result);

            if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                RealTimeDebug?.Invoke("PingBack failed");
                Debug.LogError("DJ.PingBack failed: " + webRequest.error);
                if (debugMode)
                    Debug.LogFormat("> response = {0}", Regex.Replace(jsonString, @"\r\n?|\n", ""));

                // Cleanup ...
                webRequest.Dispose();
                jsonToSend = null;
                jsonString = null;
                yield break;
            }
            else
            {
                if (webRequest.responseCode == 200 || webRequest.responseCode == 201)
                {
                    if (debugMode)
                        Debug.Log("DJ.PingBackInfo successfully sent");
                    if (logServerResponses)
                        Debug.LogFormat("> response = {0}", Regex.Replace(jsonString, @"\r\n?|\n", ""));
                }
                else if (webRequest.responseCode == 403)
                {
                    licenseLimited = true;
                }
                else
                {
                    Debug.LogError("DJ.PingBack failed!");
                    if (debugMode)
                        Debug.LogFormat("> response = {0}", Regex.Replace(jsonString, @"\r\n?|\n", ""));
                }

                // Cleanup ...
                webRequest.Dispose();
                jsonToSend = null;
                jsonString = null;
                yield break;
            }
        }
    
        IEnumerator LoadArrangements(Int64 soundtrackId)
        {
            if (debugMode)
                Debug.LogFormat("DJ.LoadArrangements({0})", soundtrackId);

            num_arrangements_to_load = 0;
        
            string finalURL = String.Format("{0}projects/{1}?plugin_version={2}", settings.base_url, soundtrackId, PluginVersion);
            if (logRequestUrls)
                Debug.LogFormat("DJ.LoadArrangements(): finalURL = {0}", finalURL);

            UnityWebRequest webRequest = UnityWebRequest.Get(finalURL);
            webRequest.SetRequestHeader("Accept", "application/json");
            webRequest.SetRequestHeader("x-api-key", settings.api_key);
            yield return webRequest.SendWebRequest();
            string jsonString = webRequest.downloadHandler.text;

            if (debugMode)
                Debug.LogFormat("> responseCode = {0}, result = {1}", webRequest.responseCode, webRequest.result);

            if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError("DJ.LoadArrangements(): Failed getting arragements: " + webRequest.error);
                if (debugMode)
                    Debug.LogFormat("> response = {0}", Regex.Replace(jsonString, @"\r\n?|\n", ""));
            }
            else if (webRequest.responseCode == 403)
            {
                licenseLimited = true;
                if (logServerResponses)
                    Debug.LogFormat("> response = {0}", Regex.Replace(jsonString, @"\r\n?|\n", ""));
            }
            else
            {
                if (logServerResponses)
                    Debug.LogFormat("> response = {0}", Regex.Replace(jsonString, @"\r\n?|\n", ""));

                songDataCache[currentSongIndex].sad = JsonUtility.FromJson<ServerArrangementsData>(jsonString);
                serverArrangementsFile = songDataCache[currentSongIndex].sad;

                if (debugMode)
                    Debug.Log("DJ.LoadArrangements(): Loading arrangements for soundtrack: " + serverArrangementsFile.name);

                RealTimeDebug?.Invoke(
                    "Loading soundtrack arrangements for: " + serverArrangementsFile.name + ", last modified " + serverArrangementsFile.updated_at);

                SetAllTrackTimings(soundtrackId.ToString());
            }
        }
    
        private void SetAllTrackTimings(string soundtrack)
        {
            if (debugMode)
                Debug.LogFormat("DJ.SetAllTrackTimings({0})", soundtrack);

            // Set number of arrangements to download
            num_arrangements_to_load = serverArrangementsFile.arrangements.Length;

            if (debugMode)
                Debug.LogFormat("> num_arrangements_to_load = {0}", num_arrangements_to_load);

            TrackTimingsData trackTimingsDataHold = songDataCache[currentSongIndex].ttd;

            foreach (Arrangements arrangement in serverArrangementsFile.arrangements)
            {
                switch (arrangement.container.type_id)
                {
                    case 1:
                        {
                            trackTimingsDataHold.low_backing_bars  = arrangement.bars;
                            trackTimingsDataHold.low_backing_beats = arrangement.beats;
                        }
                        break;
                    case 2:
                        {
                            trackTimingsDataHold.high_backing_bars  = arrangement.bars;
                            trackTimingsDataHold.high_backing_beats = arrangement.beats;
                        }
                        break;
                    case 3:
                        {
                            trackTimingsDataHold.backing_track_bars  = arrangement.bars;
                            trackTimingsDataHold.backing_track_beats = arrangement.beats;
                        }
                        break;
                    case 4:
                        {
                            trackTimingsDataHold.preview_bars  = arrangement.bars;
                            trackTimingsDataHold.preview_beats = arrangement.beats;
                        }
                        break;
                    case 5:
                        {
                            trackTimingsDataHold.victory_bars  = arrangement.bars;
                            trackTimingsDataHold.victory_beats = arrangement.beats;
                        }
                        break;
                    case 6:
                        {
                            trackTimingsDataHold.failure_bars  = arrangement.bars;
                            trackTimingsDataHold.failure_beats = arrangement.beats;
                        }
                        break;
                    case 7:
                        {
                            trackTimingsDataHold.highlight_bars  = arrangement.bars;
                            trackTimingsDataHold.highlight_beats = arrangement.beats;
                        }
                        break;
                    case 8:
                        {
                            trackTimingsDataHold.lowlight_bars  = arrangement.bars;
                            trackTimingsDataHold.lowlight_beats = arrangement.beats;
                        }
                        break;
                    case 9:
                        {
                            trackTimingsDataHold.full_song_bars  = arrangement.bars;
                            trackTimingsDataHold.full_song_beats = arrangement.beats;
                        }
                        break;
                    default:
                        Debug.LogError("DJ.SetAllTrackTimings(): Invalid Arrangement Type! "
                            + arrangement.container.type_id.ToString());
                        break;
                }
            }

            GetRemoteAvailableArrangements(soundtrack);
        }

        private void GetRemoteAvailableArrangements(string theSoundtrack)
        {
            if (debugMode)
                Debug.LogFormat("DJ.GetRemoteAvailableArrangements({0})", theSoundtrack);

            StartCoroutine(CheckForAllURLsLoaded());

            foreach (Arrangements arrangement in serverArrangementsFile.arrangements)
            {            
                string streamURL = settings.base_url + "projects/" + theSoundtrack + "/arrangements/" +
                    arrangement.id.ToString() + "/stream?format=ogg";

                PMTags tagid = (PMTags)arrangement.container.type_id;

                StartCoroutine(GetRemoteAudioClipURL(streamURL, tagid));
            }
        }
    
        IEnumerator GetRemoteAudioClipURL(string arrangementURL, PMTags arrangementType)
        {
            if (logRequestUrls)
                Debug.LogFormat(
                    "DJ.GetRemoteAudioClipURL(): arrangementURL = {0}, arrangementType = {1}", 
                    arrangementURL, arrangementType);

            UnityWebRequest webRequest = UnityWebRequest.Get(arrangementURL);
            webRequest.SetRequestHeader("Accept", "application/json");
            webRequest.SetRequestHeader("x-api-key", settings.api_key);

            yield return webRequest.SendWebRequest();
            string jsonString = webRequest.downloadHandler.text;

            if (debugMode)
                Debug.LogFormat("> responseCode = {0}, result = {1}", webRequest.responseCode, webRequest.result);

            if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                ignoredRemoteUrls++;
                string errorMsg = String.Format(
                    "'{0}' url not found! {1}", 
                    arrangementType, Regex.Replace(jsonString, @"\r\n?|\n", ""));

                Debug.LogError(errorMsg);
                RealTimeDebug.Invoke(errorMsg);
            }
            else
            {
                if (logServerResponses)
                    Debug.LogFormat("> response = {0}", Regex.Replace(jsonString, @"\r\n?|\n", ""));

                songDataCache[currentSongIndex].aur = JsonUtility.FromJson<ArrangementURL>(jsonString);

                // Sanity check, try to match returning type with requested type
                if ((int)arrangementType == songDataCache[currentSongIndex].aur.arrangement_type_id)
                { 
                    string arrangementStr = arrangementType.ToString();
                    Dictionary<string, string> map_AudioURLs = songDataCache[currentSongIndex].mau;

                    if (!map_AudioURLs.ContainsKey(arrangementStr))
                    {
                        loadedRemoteUrls++;
                        map_AudioURLs.Add(arrangementStr, songDataCache[currentSongIndex].aur.arrangement_url);
                    }
                    else
                    {
                        ignoredRemoteUrls++;
                        string warningMsg = String.Format(
                            "Duplicate '{0}' audioclip! Ignoring ...", arrangementType);
                        Debug.LogWarning(warningMsg);
                        RealTimeDebug.Invoke(warningMsg);
                    }
                }
                else
                {
                    ignoredRemoteUrls++;
                    string errorMsg = String.Format(
                        "Arrangement Type mismatch! {0} != {1}", 
                        (int)arrangementType, songDataCache[currentSongIndex].aur.arrangement_type_id);
                    Debug.LogError(errorMsg);
                    RealTimeDebug.Invoke(errorMsg);
                }
            }
        }

        private string GetFileType()
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                return "wav";
            }
            else
            {
                return "ogg";
            }
        }
    
        private AudioType GetAudioType()
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                return AudioType.WAV;
            }
            else
            {
                return AudioType.OGGVORBIS;
            }
        }

        IEnumerator CheckForAllURLsLoaded()
        {
            while ((loadedRemoteUrls + ignoredRemoteUrls) < num_arrangements_to_load)
            {
                yield return null;
            }

            if (debugMode)
                Debug.LogFormat("All {0} arrangement URLs loaded, proceeding to load arrangements audio ...", num_arrangements_to_load);

            if (playingCoroutines.ContainsKey("transitionCoroutine") && playingCoroutines["transitionCoroutine"] != null) 
            {
                StopCoroutine(playingCoroutines["transitionCoroutine"]); 
            }

            StartCoroutine(CheckForAllFilesLoaded());
            LoadAllArrangements();
        }

        IEnumerator CheckForAllFilesLoaded()
        {
            while ((ignoredRemoteUrls + loadedRemoteFiles + ignoredRemoteFiles) < num_arrangements_to_load)
            {
                LoadingProgress?.Invoke((float)(ignoredRemoteUrls + loadedRemoteFiles + ignoredRemoteFiles) / num_arrangements_to_load);
                yield return null;
            }

            if (debugMode)
                Debug.LogFormat("All {0} arrangements loaded", num_arrangements_to_load);

            songDataCache[currentSongIndex].isLoaded = true;
            theSoundtrackOptions[currentSongIndex].is_loaded = true;

            LoadingProgress?.Invoke(1.0f);

            RealTimeDebug?.Invoke("all arrangements loaded");
            if (playingCoroutines.ContainsKey("transitionCoroutine") && playingCoroutines["transitionCoroutine"] != null) 
                StopCoroutine(playingCoroutines["transitionCoroutine"]); 

            if (settings.auto_play)
            {
                if (debugMode)
                    Debug.LogFormat("> autoPlayArrangement = {0}", afterLoadTransition.tag);

                PlayArrangement(afterLoadTransition);
            }
        }
    
        private void LoadAllArrangements()
        {
            Dictionary<string, string> map_AudioURLs = songDataCache[currentSongIndex].mau;

            foreach (string thekey in map_AudioURLs.Keys)
            {
                string arrangementURL = map_AudioURLs[thekey];

                if (debugMode)
                    Debug.Log("arrangementName = " + thekey);
                StartCoroutine(GetRemoteAudioClip(arrangementURL, thekey));
            }
        }

        IEnumerator GetRemoteAudioClip(string songURL, string arrangementClip)
        {
            if (logRequestUrls)
                Debug.LogFormat("DJ.GetRemoteAudioClip(): songURL = {0}", songURL);

            UnityWebRequest webRequest = UnityWebRequestMultimedia.GetAudioClip(songURL, GetAudioType());
            yield return webRequest.SendWebRequest();

            if (debugMode)
                Debug.LogFormat("> responseCode = {0}, result = {1}", webRequest.responseCode, webRequest.result);

            if (webRequest.isDone)
            {
                if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.LogError(
                        $"[{webRequest.responseCode}] Error Downloading '{arrangementClip}' - {webRequest.result}" +
                        " - Please check to make sure the arrangement exists and is not empty!"
                    );

                    ignoredRemoteFiles++;
                }
                else
                {
                    AudioClip myClip = DownloadHandlerAudioClip.GetContent(webRequest);
                    SetAudioClip(myClip, arrangementClip);
                }
            }
        }
    
        private void SetAudioClip(AudioClip audioClip, string name)
        {
            Dictionary<string, AudioClip> map_AudioClip = songDataCache[currentSongIndex].mac;

            if (!map_AudioClip.ContainsKey(name))
            { 
                map_AudioClip.Add(name, audioClip);
                loadedRemoteFiles++;

                RealTimeDebug?.Invoke($"Loaded {name}");
                if (debugMode)
                    Debug.Log($"Loaded '{name}'");
            }
            else
                ignoredRemoteFiles++;
        }

        IEnumerator RunSetupSoundtrackOptions(string finalURL, Int64 projectId, string projectKey, bool autoPlay)
        {
            RealTimeDebug?.Invoke("Setting up soundtrack options for menu");

            UnityWebRequest webRequest = UnityWebRequest.Get(finalURL);
            webRequest.SetRequestHeader("Accept", "application/json");
            webRequest.SetRequestHeader("x-api-key", settings.api_key);

            yield return webRequest.SendWebRequest();
            string jsonString = webRequest.downloadHandler.text;

            OnLoadingProjectProgress?.Invoke(0.5f);

            if (debugMode)
                Debug.LogFormat("> responseCode = {0}, result = {1}", webRequest.responseCode, webRequest.result);

            if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogErrorFormat("Failed soundtrack options setup: {0}", webRequest.error);
                Debug.LogWarning("Please check your Project ID and Auth Token in Assets/PlusMusic/Resources/PlusMusicPlugin ..."); 
                RealTimeDebug?.Invoke("Failed soundtrack options setup");

                if (debugMode)
                    Debug.LogFormat("> response = {0}", Regex.Replace(jsonString, @"\r\n?|\n", ""));
            }
            else
            {
                if (logServerResponses)
                    Debug.LogFormat("> response = {0}", Regex.Replace(jsonString, @"\r\n?|\n", ""));

                parentData = JsonUtility.FromJson<ParentProjectData>(jsonString);
                if (parentData.parent_id != null)
                {
                    theSoundtrackOptions = SoundtrackOptionsForMenu();

                    // Create a new song cache
                    songDataCache = new List<SongCacheEntry>();
                    for (int so = 0; so<theSoundtrackOptions.Length; so++)
                    { 
                        songDataCache.Add(new SongCacheEntry { isLoaded = false });
                        theSoundtrackOptions[so].is_loaded = false;
                    }

                    OnSoundTrackOptionsReceived?.Invoke(TheSoundtrackOptions);
                    isProjectLoaded = true;

                    // Do we need to Auto Load a Song?
                    if (autoPlay)
                    {
                        Int64 songId = 0;

                        if (theSoundtrackOptions.Length > 0)
                            songId = Int64.Parse(TheSoundtrackOptions[0].id);
                        else
                            Debug.LogWarningFormat("RunSetupSoundtrackOptions(): No Songs in project for auto play!");

                        if (songId > 0)
                            LoadSoundtrack(songId);
                        else
                            Debug.LogWarning("RunSetupSoundtrackOptions(): songId = 0!");
                    }

                    RealTimeDebug?.Invoke("Successfully setup soundtrack options");
                    DisplaySoundtrackOptions();
                }
            }

            OnLoadingProjectProgress?.Invoke(1.0f);

            // Cleanup ...
            webRequest.Dispose();
            jsonString = null;
        }

        private void DisplaySoundtrackOptions()
        {
            foreach (SoundtrackOptionData option in TheSoundtrackOptions)
            {
                if (debugMode)
                    Debug.LogFormat(
                        "soundtrack option id, name, is_licensed = {0}, {1}, {2}",
                        option.id, option.name, option.is_licensed);
            }
        }
 
        private SoundtrackOptionData[] SoundtrackOptionsForMenu()
        {
            SoundtrackOptionData[] theOptions = new SoundtrackOptionData[parentData.children_projects.Length];
            int loopIndexCounter = 0;
            foreach (ChildProjectData children in parentData.children_projects)
            {
                SoundtrackOptionData thisOption = new SoundtrackOptionData();
                thisOption.name = children.name;
                thisOption.id = children.id;
                thisOption.is_licensed = children.is_licensed;
                theOptions[loopIndexCounter] = thisOption;
                loopIndexCounter++;
            }
            return theOptions;
        }
        #endregion
    

        private IEnumerator PlaySoundPM_curve(TransitionInfo transition)
        {
            if (debugMode)
                Debug.LogFormat("DJ.PlaySoundPM_curve({0}, {1})", transition.tag, transition.volume);

            float delay = GetTimeForTransition(transition.timing);
            if (delay < 0.0f)
            {
                Debug.LogWarning($"DJ.PlaySoundPM_curve(): Could not get timing value, aborting sound play ...");
                yield break;
            }

            if (doPingbacks)
                SetPingBackInfo(
                    "transition", Int64.Parse(WhichSoundtrack), "", transition.tag.ToString(), 
                    "CurveTransition", transition.timing.ToString(), delay, !string.IsNullOrEmpty(transition.stingerId)
                );
        
            if (playingCoroutines.ContainsKey("queueSong") && playingCoroutines["queueSong"] != null) 
                StopCoroutine(playingCoroutines["queueSong"]);

            if (debugMode)
                Debug.LogFormat("DJ.PlaySoundPM_curve(): delay = {0}, duration = {1}", delay, transition.durationTransition);

            yield return new WaitForSeconds(delay);

            if (!string.IsNullOrEmpty(transition.stingerId)) 
                PlayStinger(transition.stingerId);

            if (transition.returnToPrevious || transition.timeToLive > 0.0f)
            { 
                playingCoroutines["queueSong"] = StartCoroutine(
                    QueueTransition(currentTransition, transition)

                    //ReturnToPreviousArrangement(
                    //    currentTransition.tag, transition.tag, transition.durationTransition, transition.timeToLive)
                );
            }

            AudioClip nextAudioClip = null;
            Dictionary<string, AudioClip> map_AudioClip = songDataCache[currentSongIndex].mac;
            if (map_AudioClip.ContainsKey(transition.tag.ToString())) 
                nextAudioClip = map_AudioClip[transition.tag.ToString()]; 

            if (null != nextAudioClip)
            { 
                if (isAudioSource1Playing)
                {
                    StartCoroutine(CurveTransitionFade(transition, nextAudioClip, newAudio, theAudioSource));
                    currentAudioSource = newAudio;
                }
                else
                {
                    StartCoroutine(CurveTransitionFade(transition, nextAudioClip, theAudioSource, newAudio));
                    currentAudioSource = theAudioSource;
                }
                isAudioSource1Playing = !isAudioSource1Playing;
            }
            else
            {
                Debug.LogWarning($"DJ.PlaySoundPM_curve(): Missing '{transition.tag}' Arrangement in Soundtrack!");
            }

            if (null != audioMixerGroup)
                currentAudioSource.outputAudioMixerGroup = audioMixerGroup;

            OnArrangementChanged?.Invoke(transition.tag);
        }

        private IEnumerator QueueTransition(TransitionInfo previousTransition, TransitionInfo currentTransition)
        {
            if (PMTags.none == previousTransition.tag)
                yield break;

            Dictionary<string, AudioClip> map_AudioClip = songDataCache[currentSongIndex].mac;
            float clipLength = map_AudioClip[currentTransition.tag.ToString()].length;
            float timeToWait = clipLength - Mathf.Min(currentTransition.durationTransition, clipLength);

            if (currentTransition.timeToLive > 0.0f)
                timeToWait = Mathf.Min(currentTransition.timeToLive, clipLength);

            if (timeToWait < 0.0f)
                timeToWait = 0.0f;

            yield return new WaitForSeconds(timeToWait);

            TransitionInfo transition;
            if (currentTransition.returnToPrevious)
                transition = CopyTransition(previousTransition);
            else
                transition = CopyTransition(currentTransition);

            if (!killQueue)
            {
                if (debugMode)
                    Debug.Log("ReturnToPreviousArrangement(): " + transition.tag);
                PlayArrangement(transition);
            }
            else
            {
                if (debugMode)
                    Debug.Log("QueueTransition(): Abort requested ...");
                killQueue = false;
            }
        }

        private float GetTimeForTransition(PMTimings theTiming)
        {
            return theTiming switch
            {
                PMTimings.nextBeat => TimeNextBeat(),
                PMTimings.nextBar => TimeNextBar(),
                PMTimings.now => 0.0f,
                PMTimings.beats => TimeNextBeat(),  // Deprecated
                PMTimings.bars => TimeNextBar(),    // Deprecated
                _ => 0.0f,
            };
        }
    
        IEnumerator CurveTransitionFade(
            TransitionInfo transition, AudioClip passedNextTrack, AudioSource audioSourceIn, AudioSource audioSourceDown)
        {
            float journey = 0.0f;
            float volumeOut = musicVolume;
            float volumeIn = (transition.useVolume ? transition.volume : musicVolume);

            audioSourceIn.volume = 0.001f;
            audioSourceIn.clip = passedNextTrack;
            audioSourceIn.Play();

            while (journey <= transition.durationTransition)
            {
                journey = journey + Time.deltaTime;
                float percent = Mathf.Clamp01(journey / transition.durationTransition);
                float curvePercent = percent;
                if (transition.curve != null && transition.curve.keys.Length >= 1) { 
                    curvePercent = transition.curve.Evaluate(percent); 
                }
                audioSourceIn.volume = curvePercent * volumeIn;
                audioSourceDown.volume = volumeOut * (1 - curvePercent);
                yield return null;
            }

            musicVolume = volumeIn;
            if (!transition.returnToPrevious && 0.0f == transition.timeToLive)
                currentTransition = CopyTransition(transition);
        }
    
        IEnumerator CurveTransitionFadeAny(
            TransitionInfo transition, AudioClip passedNextTrack, AudioSource audioSourceIn)
        {
            float journey = 0.0f;
            float volumeIn = (transition.useVolume ? transition.volume : musicVolume);

            audioSourceIn.volume = 0.001f;
            audioSourceIn.clip = passedNextTrack;
            audioSourceIn.Play();

            while (journey <= transition.durationTransition)
            {
                journey = journey + Time.deltaTime;
                float percent = Mathf.Clamp01(journey / transition.durationTransition);
                float curvePercent = transition.curve.Evaluate(percent);
                audioSourceIn.volume = curvePercent * volumeIn;
                yield return null;
            }

            musicVolume = volumeIn;
            if (!transition.returnToPrevious && 0.0f == transition.timeToLive)
                currentTransition = CopyTransition(transition);
        }

        public void PlayStinger(string stingerId)
        {
            if (!map_StingerClip.ContainsKey(stingerId)) return;

            stingerSource.clip = map_StingerClip[stingerId];

            stingerSource.Play();
        }

        public void WindDownMainAudio()
        {
            if (debugMode)
                Debug.Log("DJ.WindDownMainAudio()");

            currentAudioSource.volume = musicVolume / 1.3333f;
            currentAudioSource.volume = musicVolume / 2.0f;
            currentAudioSource.volume = musicVolume / 4.0f;
            currentAudioSource.volume = 0.0f;
        }

        public void TurnUpMainAudio()
        {
            if (debugMode)
                Debug.Log("DJ.TurnUpMainAudio()");

            currentAudioSource.volume = musicVolume / 4.0f;
            currentAudioSource.volume = musicVolume / 2.0f;
            currentAudioSource.volume = musicVolume / 1.3333f;
            currentAudioSource.volume = musicVolume;
        }


        #region Public Plugin API functions
        //-----------------------------------------------
        // Public Plugin API functions
        //-----------------------------------------------

        /**
        * @brief Set a AudioMixerGroup for audio playback
        * @param mixerGroup
        */
        public void SetAudioMixerGroup(AudioMixerGroup mixerGroup)
        {
            audioMixerGroup = mixerGroup;
            if (null != audioMixerGroup)
                currentAudioSource.outputAudioMixerGroup = audioMixerGroup;
        }

        /**
        * @brief Make a deep copy of a transition object
        * @param sourceTransition
        * @return The new TransitionInfo object
        */
        public TransitionInfo CopyTransition(TransitionInfo sourceTransition)
        {
            return new TransitionInfo(
                sourceTransition.tag, sourceTransition.durationTransition, sourceTransition.timing,
                sourceTransition.useVolume, sourceTransition.volume, sourceTransition.stingerId, 
                sourceTransition.canTransitionToItself, sourceTransition.returnToPrevious,
                sourceTransition.timeToLive, sourceTransition.curve
            );
        }

        /**
        * @brief [asynchronous] Get the project details json from the PlusMusic Web Api and set the global project variables
        * @param projectId
        * @param projectKey
        * @param autoPlay
        */
        public void LoadProject(Int64 projectId, string projectKey, bool autoPlay)
        {
            Debug.LogFormat("DJ.LoadProject({0}, {1})", projectId, autoPlay);

            // Check if the project ID is 0 (default)
            // If yes, we try to download the PlusMusic default project and use that instead
            if (0 == projectId)
            {
                StartCoroutine(LoadDefaultProject(autoPlay));
                return;
            }

            isProjectLoaded = false;
            OnLoadingProjectProgress?.Invoke(0.0f);

            SetCurrentProject(projectId, projectKey, autoPlay);

            // Send Unity VSA event
            if (Application.isEditor && plusMusicAccount.IsFromUnityStore)
            {
                // Check if we need to reset the attribution flag
                if ((plusMusicAccount.DeviceId != SystemInfo.deviceUniqueIdentifier) ||
                    (plusMusicAccount.AuthToken != projectKey))
                {
                    didVSAttribution = false;
                }

                if (!didVSAttribution)
                { 
                    if (null != VSASendAttributionEvent)
                    {
                        string customerUid =
                            SystemInfo.deviceUniqueIdentifier + "|" +
                            projectKey + "|" + pluginVersion;
                        didVSAttribution = VSASendAttributionEvent("Initial Project Load", "PlusMusic", customerUid);
                    }
                    else
                        Debug.LogError("DJ.LoadProject(): VSASendAttributionEvent is null!");
                }
            }

            // Call home with some useful info
            string eventStr = "Start of Game";
            if (!didStartupPingback)
            {
                didStartupPingback = true;
                eventStr = "Load Project";
            }
            SetPingBackInfo(
                eventStr, settings.project_id, "", "", "", "", 0.0f, false);

            // Kick off async loading
            string finalURL = String.Format("{0}projects/{1}/hierarchy?plugin_version={2}", settings.base_url, projectId, PluginVersion);
            if (logRequestUrls)
                Debug.LogFormat("DJ.LoadProject(): finalURL = {0}", finalURL);
            StartCoroutine(RunSetupSoundtrackOptions(finalURL, projectId, projectKey, autoPlay));
        }

        /**
        * @brief [asynchronous] Load the soundtrack and all arrangements from the PlusMusic Web Api
        * @param soundtrackName
        */
        public void LoadSoundtrackByName(string soundtrackName)
        {
            Int64 soundtrackId = 0;

            if (debugMode)
                Debug.LogFormat("DJ.LoadSoundtrackByName({0})", soundtrackName);

            for (int so = 0; so<theSoundtrackOptions.Length; so++)
            {
                if (theSoundtrackOptions[so].name == soundtrackName)
                {
                    soundtrackId = Int64.Parse(theSoundtrackOptions[so].id);
                    break;
                }
            }

            if (soundtrackId > 0)
                LoadSoundtrack(soundtrackId);
            else
                Debug.LogError("DJ.LoadSoundtrackByName(): Invalid Song Name!");
        }

        /**
        * @brief [asynchronous] Load the soundtrack and all arrangements from the PlusMusic Web Api
        * @param soundtrackId
        */
        public void LoadSoundtrack(Int64 soundtrackId)
        {
            if (debugMode)
                Debug.LogFormat("> autoPlayArrangement = {0}", currentTransition.tag);

            LoadSoundtrack(soundtrackId, currentTransition, settings.auto_play);
        }

        /**
        * @brief [asynchronous] Load the soundtrack and all arrangements from the PlusMusic Web Api
        * @param soundtrackId
        */
        public void LoadSoundtrack(Int64 soundtrackId, TransitionInfo transition, bool autoPlay)
        {
            Debug.LogFormat("DJ.LoadSoundtrack({0}, {1}, {2}, {3}, {4})", 
                soundtrackId, transition.tag, transition.volume, transition.useVolume, autoPlay);

            settings.auto_play = autoPlay;

            if (playingCoroutines.ContainsKey("getArragementCoroutine") && playingCoroutines["getArragementCoroutine"] != null)
            {
                //Debug.Log("DJ.LoadSoundtrack(): Already loading another track! Stopping previous load ...");
                StopCoroutine(playingCoroutines["getArragementCoroutine"]);
            }

            // Get song index from id
            int songIndex = -1;
            for (int so = 0; so<theSoundtrackOptions.Length; so++)
            {
                if (soundtrackId == Int64.Parse(theSoundtrackOptions[so].id))
                {
                    songIndex = so;
                    break;
                }
            }

            if (-1 == songIndex)
            {
                Debug.LogError("DJ.LoadSoundtrack(): Invalid Song ID!");
                return;
            }

            LoadingProgress?.Invoke(0.0f);
            RealTimeDebug?.Invoke("Setting soundtrack " + soundtrackId);

            currentSongIndex = songIndex;
            WhichSoundtrack = soundtrackId.ToString();
            loadedRemoteFiles = 0;
            ignoredRemoteFiles = 0;
            loadedRemoteUrls = 0;
            ignoredRemoteUrls = 0;
            num_arrangements_to_load = 0;

            if (debugMode)
                Debug.LogFormat("> whichSoundtrack = {0}", WhichSoundtrack);

            // Check if the song needs to be loaded
            if (!songDataCache[currentSongIndex].isLoaded)
            {
                // Update the cache object
                songDataCache[currentSongIndex].songId = soundtrackId;
                songDataCache[currentSongIndex].isLoaded = false;
                songDataCache[currentSongIndex].mac = new Dictionary<string, AudioClip>();
                songDataCache[currentSongIndex].mau = new Dictionary<string, string>();
                songDataCache[currentSongIndex].sad = null;
                songDataCache[currentSongIndex].aur = null;
                songDataCache[currentSongIndex].ttd = new TrackTimingsData();

                afterLoadTransition = CopyTransition(transition);

                if (!licenseLimited)
                {
                    playingCoroutines["getArragementCoroutine"] = StartCoroutine(LoadArrangements(soundtrackId));
                }
            }

            // Otherwise see if we need to play it
            else
            {
                serverArrangementsFile = songDataCache[currentSongIndex].sad;
                loadedRemoteFiles = serverArrangementsFile.arrangements.Length;
                num_arrangements_to_load = serverArrangementsFile.arrangements.Length;
                LoadingProgress?.Invoke(1.0f);

                if (settings.auto_play)
                {
                    if (debugMode)
                        Debug.LogFormat("> autoPlayArrangement = {0}", transition.tag);

                    PlayArrangement(transition);
                }
            }
        }

        /**
        * @brief [asynchronous] Play an arrangement from a loaded soundtrack
        * @param transition
        */
        public void PlayArrangement(TransitionInfo transition)
        {
            if (debugMode)
                Debug.LogFormat("DJ.PlayArrangement({0}, {1}, {2})", 
                    transition.tag, transition.volume, transition.useVolume);

            if (!AllFilesLoaded())
            {
                Debug.LogWarning("DJ.PlayArrangement(): Arrangements are still loading!");
                return;
            }

            if (PMTags.none == transition.tag)
            {
                Debug.LogWarning("DJ.PlayArrangement(): No arrangement type selected!");
                return;
            }

            if (!transition.canTransitionToItself && transition.tag.ToString() == previousTag) {
                Debug.LogWarningFormat("DJ.PlayArrangement(): Arrangement '{0}' is already selected!", transition.tag);
                return;
            }
            next_tag = transition.tag.ToString();

            loopCounter = 0;
            previousTag = next_tag;
            if (playingCoroutines.ContainsKey("transitionCoroutine") && playingCoroutines["transitionCoroutine"] != null) 
                StopCoroutine(playingCoroutines["transitionCoroutine"]); 

            playingCoroutines["transitionCoroutine"] = StartCoroutine(PlaySoundPM_curve(transition));
        }

        /**
        * @brief Return the volume of the current soundtrack playback
        * @return The volume (0.0 - 1.0)
        */
        public float GetVolume()
        {
            return musicVolume;
        }

        /**
        * @brief Set the volume for the soundtrack playback (0.0 - 1.0)
        * @param value
        */
        public void SetVolume(float volume)
        {
            // Clamp volume
            if (volume <= 0.0f)
                volume = 0.001f;
            if (volume > 1.0f)
                volume = 1.0f;

            musicVolume = volume;

            // Added null checks
            if (null != stingerSource)
                stingerSource.volume = volume;

            if (null != currentAudioSource)
                currentAudioSource.volume = volume;
        }

        /**
        * @brief Set mute true/false
        * @param value
        */
        public void SetMute(bool value)
        {
            if (debugMode)
                Debug.LogFormat("DJ.SetMute({0})", value);

            float muteVal = (value ? 0.001f : musicVolume);

            // Added null checks
            if (null != theAudioSource)
                theAudioSource.mute = value;

            if (null != newAudio)
                newAudio.mute = value;

            if (null != stingerSource)
                stingerSource.mute = value;

            if (value)
                audioState = STATE_MUTED;
            else
                audioState = STATE_UNMUTED;
            OnAudioStateChanged?.Invoke(audioState);
        }

        /**
        * @brief Return true if the arrangement is loopable
        * @param arrangementType
        * @return true/false
        */
        public bool GetIsLoopable(PMTags arrangementType)
        {
            if (debugMode)
                Debug.LogFormat("DJ.GetIsLoopable({0})", arrangementType);

            switch (arrangementType)
            {
                case PMTags.highlight:
                    return false;
                case PMTags.failure:
                    return false;
                case PMTags.victory:
                    return false;
            }

            return true;
        }

        /**
        * @brief Returns the time of the next bar of the song
        * @return The next bar time
        */
        public float TimeNextBar()
        {
            float[] barCountsToUse = null;
            float valueToSend = 0.0f;

            TrackTimingsData trackTimingsDataHold = songDataCache[currentSongIndex].ttd;
            if (null != trackTimingsDataHold)
            {
                switch (previousTag)
                {
                    case "high_backing": barCountsToUse  = trackTimingsDataHold.high_backing_bars; break;
                    case "low_backing": barCountsToUse   = trackTimingsDataHold.low_backing_bars; break;
                    case "backing_track": barCountsToUse = trackTimingsDataHold.backing_track_bars; break;
                    case "preview": barCountsToUse       = trackTimingsDataHold.preview_bars; break;
                    case "victory": barCountsToUse       = trackTimingsDataHold.victory_bars; break;
                    case "failure": barCountsToUse       = trackTimingsDataHold.failure_bars; break;
                    case "highlight": barCountsToUse     = trackTimingsDataHold.highlight_bars; break;
                    case "lowlight": barCountsToUse      = trackTimingsDataHold.lowlight_bars; break;
                    case "full_song": barCountsToUse     = trackTimingsDataHold.full_song_bars; break;
                }
            }

            if (null != barCountsToUse)
               valueToSend = GetNextClosestTime(barCountsToUse);
            else
            { 
                string errMsg = "DJ.TimeNextBar(): barCountsToUse is null! " +
                    "Your Song is either missing arrangements or hasn't been fully loaded yet. " +
                    "LoadSoundtrack() is asynchronous, use the 'LoadingProgress' message callback to determine if a Soundtrack has been fully loaded. " +
                    "See 'PlusMusicSettings.cs' for an example.";
                Debug.LogError(errMsg);
                return -1.0f;
            }

            return valueToSend;
        }

        /**
        * @brief Returns the time of the next beat of the song
        * @return The next beat time
        */
        public float TimeNextBeat()
        {
            float[] beatCountsToUse = null;
            float valueToSend = 0.0f;

            TrackTimingsData trackTimingsDataHold = songDataCache[currentSongIndex].ttd;
            if (null != trackTimingsDataHold)
            {
                switch (previousTag)
                {
                    case "high_backing": beatCountsToUse  = trackTimingsDataHold.high_backing_beats; break;
                    case "low_backing": beatCountsToUse   = trackTimingsDataHold.low_backing_beats; break;
                    case "backing_track": beatCountsToUse = trackTimingsDataHold.backing_track_beats; break;
                    case "preview": beatCountsToUse       = trackTimingsDataHold.preview_beats; break;
                    case "victory": beatCountsToUse       = trackTimingsDataHold.victory_beats; break;
                    case "failure": beatCountsToUse       = trackTimingsDataHold.failure_beats; break;
                    case "highlight": beatCountsToUse     = trackTimingsDataHold.highlight_beats; break;
                    case "lowlight": beatCountsToUse      = trackTimingsDataHold.lowlight_beats; break;
                    case "full_song": beatCountsToUse     = trackTimingsDataHold.full_song_beats; break;
                }
            }

            if (null != beatCountsToUse)
                valueToSend = GetNextClosestTime(beatCountsToUse);
            else
            {
                string errMsg = "DJ.TimeNextBeat(): beatCountsToUse is null! " +
                    "Your Song is either missing arrangements or hasn't been fully loaded yet. " +
                    "LoadSoundtrack() is asynchronous, use the 'LoadingProgress' message callback to determine if a Soundtrack has been fully loaded. " +
                    "See 'PlusMusicSettings.cs' for an example.";
                Debug.LogError(errMsg);
                return -1.0f;
            }

            return valueToSend;
        }

        /**
        * @brief Load an environment variable into a String
        * @param var_name
        * @param def_val
        * @return Return the env variable String or def_val("") if not found
        */
        public string GetEnvVariable(string var_name, string def_val)
        {
            string envVariable = Environment.GetEnvironmentVariable(var_name);

            if (!string.IsNullOrWhiteSpace(envVariable))
                return envVariable.Trim();

            return def_val;
        }

        /**
        * @brief Check if all arrangements have been loaded
        * @return true/false
        */
        public bool AllFilesLoaded()
        {
            if (debugMode)
                Debug.LogFormat(
                    "ignoredRemoteUrls/loadedRemoteFiles/ignoredRemoteFiles/num_arrangements_to_load = {0}/{1}/{2}/{3}",
                    ignoredRemoteUrls, loadedRemoteFiles, ignoredRemoteFiles, num_arrangements_to_load);

            if ((ignoredRemoteUrls + loadedRemoteFiles + ignoredRemoteFiles) >= num_arrangements_to_load)
                return true;
            else
            { 
                if (licenseLimited)
                    return true;
                else
                    return false;
            }
        }

        /**
        * @brief Pause audio playback
        */
        public void PauseAudio()
        {
            if (debugMode)
                Debug.LogFormat("DJ.PauseAudio(): musicVolume = {0}", musicVolume);

            currentAudioSource.Pause();

            audioState = STATE_PAUSED;
            OnAudioStateChanged?.Invoke(audioState);
        }

        /**
        * @brief UnPause audio playback
        */
        public void UnPauseAudio()
        {
            if (debugMode)
                Debug.LogFormat("DJ.UnPauseAudio(): musicVolume = {0}", musicVolume);

            currentAudioSource.UnPause();

            audioState = STATE_PLAYING;
            OnAudioStateChanged?.Invoke(audioState);
        }

        /**
        * @brief Stop audio playback
        */
        public void StopAudio()
        {
            if (debugMode)
                Debug.Log("DJ.StopAudio()");

            WindDownMainAudio();
            currentAudioSource.Stop();

            audioState = STATE_STOPPED;
            OnAudioStateChanged?.Invoke(audioState);
        }

        /**
        * @brief Start audio playback
        */
        public void StartAudio()
        {
            if (debugMode)
                Debug.Log("DJ.StartAudio()");

            currentAudioSource.volume = 0.0f;
            currentAudioSource.Play();
            TurnUpMainAudio();

            audioState = STATE_PLAYING;
            OnAudioStateChanged?.Invoke(audioState);
        }

        #endregion


        #region DEPRECATED Legacy functions
        //-----------------------------------------------
        // DEPRECATED Legacy functions
        // 
        // NOTE: The current demos have hardcoded references to these old functions
        //-----------------------------------------------


        /**
        * @deprecated Use filters on an AudioMixer instead
        * @brief Set low pass filter
        * @param value
        */
        [Obsolete("Deprecated, use filters on an AudioMixer instead", false)]
        public void SetLowPassFilter(bool value)
        {
            Debug.LogWarning("[deprecated] DJ.SetLowPassFilter(): Use filters on an AudioMixer instead");
        }

        /**
        * @deprecated Use an AudioMixer instead
        * @brief Set Mixer settings
        * @param name
        * @param value
        * @param duration = 0.0f
        */
        [Obsolete("Deprecated, use an AudioMixer instead", false)]
        public void SetMixerSetting(string name, float value, float duration = 0.0f)
        {
            Debug.LogWarning("[deprecated] DJ.SetMixerSetting(): Use an AudioMixer instead");
        }

        /**
        * @deprecated Use LoadSoundtrack() instead
        * @brief Change the soundtrack
        * @param theSoundtrack
        */
        [Obsolete("Deprecated, use LoadSoundtrack() instead", false)]
        public void ChangeSoundtrack(string theSoundtrack)
        {
            Debug.LogWarning("[deprecated] DJ.ChangeSoundtrack(): Use LoadSoundtrack() instead");

            LoadSoundtrack(Int64.Parse(theSoundtrack));
        }

        /**
        * @deprecated Use LoadSoundtrack() instead
        * @brief Change the soundtrack
        * @param theSoundtrack
        */
        [Obsolete("Deprecated, use LoadSoundtrack() instead", false)]
        public void SelectRemoteSoundtrackByID(string theSoundtrack)
        {
            Debug.LogWarning("[deprecated] DJ.SelectRemoteSoundtrackByID(): Use LoadSoundtrack() instead");

            LoadSoundtrack(Int64.Parse(theSoundtrack));
        }

        /**
        * @deprecated Use PlayArrangement() instead
        * @brief Play sound by type
        * @param transitionInfo
        */
        [Obsolete("Deprecated, use PlayArrangement() instead", false)]
        public void PlaySoundPM(TransitionInfo transitionInfo)
        {
            Debug.LogWarning("[deprecated] DJ.PlaySoundPM(): Use PlayArrangement() instead");

            PlayArrangement(transitionInfo);
        }

        /**
        * @deprecated Use SetVolume() instead
        * @brief Set the music volume
        * @param value
        */
        [Obsolete("Deprecated, use SetVolume() instead", false)]
        public void SetMusicVolume(float value)
        {
            Debug.LogWarning("[deprecated] DJ.SetMusicVolume(): Use SetVolume() instead");

            SetVolume(value);
        }

        #endregion



        //----------------------------------------------------------
        /**
        * @brief Save the sample data from an AudioClip to a WAV file
        * @param clip
        * @param filename
        * @param absolute - If false (default), the filename is prepended with Application.persistentDataPath
        */
        private void SaveClipAsWav(AudioClip clip, string filename, bool absolute = false)
        {
            string filepath = filename;
            if (!absolute)
                filepath = Path.Combine(Application.persistentDataPath, filename);
            Debug.LogFormat("DJ.SaveClipAsWav({0})", filepath);

            try
            {
                FileInfo fi = new FileInfo(filepath);
                if (fi.Extension.ToLower() != ".wav")
                {
                    Debug.LogError("DJ.SaveClipAsWav(): File Extension is not .wav!");
                    return;
                }

                // Make sure the target folder exists, if not, we create it
                string targetFolder = Path.GetDirectoryName(filepath);
                if (!Directory.Exists(targetFolder))
                {
                    Debug.LogWarning("> Target Folder does not exist! Creating ...");
                    Directory.CreateDirectory(targetFolder);
                }

                // Get the float samples from the AudioClip
                int num_floats = clip.samples * clip.channels;
                float[] float_samples = new float[num_floats];
                clip.GetData(float_samples, 0);

                Debug.LogFormat("> channels, frequency, length, samples = {0}, {1}, {2}, {3}",
                    clip.channels, clip.frequency, clip.length, clip.samples);

                int dataLength = num_floats * sizeof(UInt16);       // PCM = 2 bytes
                byte[] buffer = new byte[dataLength];
                int fileSize = 44 + dataLength - 8;               // Header + data - 8
                int blockSize = clip.channels * sizeof(UInt16);
                int bytesPerSecond = clip.frequency * blockSize;

                float rescaleFactor = 32767.0f; // To convert float to Int16
                int p = 0;
                for (int i = 0; i<num_floats; i++)
                {
                    Int16 value = (Int16)(float_samples[i] * rescaleFactor);
                    buffer[p++] = (byte)(value >> 0);
                    buffer[p++] = (byte)(value >> 8);
                }

                // Write out the WAV file
                FileStream stream = new FileStream(filepath, FileMode.Create);
                BinaryWriter writer = new BinaryWriter(stream);

                // Write header
                PMFileHeaders.WAV header = new PMFileHeaders.WAV();
                header.size      = fileSize;
                header.channels  = (Int16)clip.channels;
                header.frequency = clip.frequency;
                header.bytesPerSecond = bytesPerSecond;
                header.blockSize  = (Int16)blockSize;
                header.dataLength = dataLength;
                foreach (byte[] field in header)
                    writer.Write(field);

                // Write byte[] data
                writer.Write(buffer, 0, header.dataLength);

                writer.Close();

            } catch (Exception e)
            {
                Debug.LogErrorFormat("DJ.SaveClipAsWav(): {0}", e.ToString());
            }
        }

    }   // End Class


    #region Data Classes

    public class PMFileHeaders
    {
        // WAV (Waveform Audio File Format)
        public class WAV
        {
            public UInt32 signature = 0x46464952;   // 'RIFF'
            public Int32 size = 0;                  // Size of file - 8
            public UInt32 content = 0x45564157;     // 'WAVE'
            public UInt32 formatChunk = 0x20746d66; // chunk 'fmt '
            public Int32 formatLength = 16;         // chunk length (in bytes) following this field
            public Int16 type = 1;                  // 1 = Uncompressed PCM, 2 Bytes per sample
            public Int16 channels = 2;              // Number of channels
            public Int32 frequency = 44100;         // Sample frequency
            public Int32 bytesPerSecond = 0;        // Bytes per second
            public Int16 blockSize = 4;             // Block size
            public Int16 bitsPerSample = 16;        // Bits per Sample
            public UInt32 dataChunk = 0x61746164;   // chunk 'data'
            public Int32 dataLength = 0;            // chunk length (in bytes) following this field
            // DATA
            // Number of data bytes = (channels * samples * type_size)

            // Custom enumerator that can be used with a BinaryWriter
            public IEnumerator<byte[]> GetEnumerator()
            {
                yield return BitConverter.GetBytes(signature);
                yield return BitConverter.GetBytes(size);
                yield return BitConverter.GetBytes(content);
                yield return BitConverter.GetBytes(formatChunk);
                yield return BitConverter.GetBytes(formatLength);
                yield return BitConverter.GetBytes(type);
                yield return BitConverter.GetBytes(channels);
                yield return BitConverter.GetBytes(frequency);
                yield return BitConverter.GetBytes(bytesPerSecond);
                yield return BitConverter.GetBytes(blockSize);
                yield return BitConverter.GetBytes(bitsPerSample);
                yield return BitConverter.GetBytes(dataChunk);
                yield return BitConverter.GetBytes(dataLength);
            }
        }

    }   // End Class


    [System.Serializable]
    public class TransitionInfo
    {
        [Tooltip("Which Arrangement type to play")]
        public PlusMusic_DJ.PMTags tag;
        [Tooltip("Duration of the transition")][Min(0.0f)]
        public float durationTransition;
        [Tooltip("When to start the transition")]
        public PlusMusic_DJ.PMTimings timing;
        [Tooltip("If checked, the volume below is used as the traget volume.\nIf unchecked, the volume is unchanged.")]
        public bool useVolume;
        [Tooltip("The volume to transtition to")][Range(0.0f, 1.0f)]
        public float volume;
        [Tooltip("Optional Stinger to play")]
        public string stingerId;
        [Tooltip("Allow the arrangement to transition to itself (Reset/Restart when played)")]
        public bool canTransitionToItself;
        [Tooltip("If enabled, the arrangement will not loop but instead revert back to the previous arrangement after it has finished playing")]
        public bool returnToPrevious;
        [Tooltip("If > 0, the arrangement will stop playing at the cutoff time instead of the length of the audio")]
        [Min(0.0f)]
        public float timeToLive;
        [Tooltip("Which curve to use to transition")]
        public AnimationCurve curve;

        public TransitionInfo()
        {
            tag = PlusMusic_DJ.PMTags.backing_track;
            durationTransition = 1.0f;
            timing = PlusMusic_DJ.PMTimings.now;
            useVolume = true;
            volume = 1.0f;
            stingerId = "";
            canTransitionToItself = true;
            returnToPrevious = false;
            timeToLive = 0.0f;
            curve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
        }

        public TransitionInfo(
            PlusMusic_DJ.PMTags tag, float durationTransition, PlusMusic_DJ.PMTimings timing,
            bool useVolume, float volume, string stingerId, bool canTransitionToItself, 
            bool returnToPrevious, float timeToLive, AnimationCurve curve
        ) : this()
        {
            this.tag = tag;
            this.durationTransition = durationTransition;
            this.timing = timing;
            this.useVolume = useVolume;
            this.volume = volume;
            this.stingerId = stingerId;
            this.canTransitionToItself = canTransitionToItself;
            this.returnToPrevious = returnToPrevious;
            this.timeToLive = timeToLive;
            this.curve = new AnimationCurve(curve.keys);
        }
    }   // End Class

    [System.Serializable]
    public class SongCacheEntry
    {
        public Int64 songId;
        public bool isLoaded;
        public int numArrangements;
        public Dictionary<string, AudioClip> mac;
        public Dictionary<string, string> mau;
        public ServerArrangementsData sad;
        public ArrangementURL aur;
        public TrackTimingsData ttd;
    }

    [System.Serializable]
    public class TrackTimingsData
    {
        public float[] victory_beats;
        public float[] victory_bars;
        public float[] low_backing_beats;
        public float[] low_backing_bars;
        public float[] high_backing_beats;
        public float[] high_backing_bars;
        public float[] backing_track_beats;
        public float[] backing_track_bars;
        public float[] failure_beats;
        public float[] failure_bars;
        public float[] highlight_beats;
        public float[] highlight_bars;
        public float[] lowlight_beats;
        public float[] lowlight_bars;
        public float[] preview_bars;
        public float[] preview_beats;
        public float[] full_song_bars;
        public float[] full_song_beats;
    }
    
    [System.Serializable]
    class SavePlayInfo
    {
        public string savedData;
    }

    [System.Serializable]
    class DefaultProjectData
    {
        public Int64 id;
        public string plugin_api_key;
    }

    [System.Serializable]
    class DefaultProjectInfo
    {
        public DefaultProjectData default_project;
    }

    [System.Serializable]
    public class ServerArrangementsData
    {
        public Int64  id;
        public string name;
        public string type_id;
        public Int64  parent_id;
        public Int64  creator_id;
        public bool   is_licensed;
        public string licensed_by;
        public string licensed_at;
        public string deleted_at;
        public string created_at;
        public string updated_at;
        public Arrangements[] arrangements;
        public string message;
    }
    
    [System.Serializable]
    public class Arrangements
    {
        public Int64   id;
        public Int64   container_id;
        public Int64   created_by_user_id;
        public int     version;
        public Int64[] song_clips;
        public Int64   pipeline_run_id;
        public bool    is_instrumental;
        public string  created_at;
        public string  updated_at;
        public string  deleted_at;
        public float[] beats;
        public float[] bars;
        public Pivot   pivot;
        public Container container;
    }
    
    [System.Serializable]
    public class Pivot
    {
        public int    project_id;
        public int    arrangement_id;
        public string created_at;
        public string updated_at;
        public int    is_active;
    }
    
    [System.Serializable]
    public class Container
    {
        public int    id;
        public string name;
        public int    type_id;
        public int    based_on_container_id;
        public int    is_template;
        public string created_at;
        public string updated_at;
    }
    
    [System.Serializable]
    public class ArrangementURL
    {
        public string message;
        public string arrangement_type;
        public int    arrangement_type_id;
        public string arrangement_url;
    }
    
    [System.Serializable]
    public class AuthenticationData
    {
        public string access_token;
        public string expires_in;
        public string first_name;
        public string last_name;
        public string email;
    }
    
    [System.Serializable]
    public class ParentProjectData
    {
        public string id;
        public string name;
        public string type_id;
        public string parent_id;
        public string template_project_id;
        public string creator_id;
        public bool   is_licensed;
        public string licensed_by;
        public string licensed_at;
        public string deleted_at;
        public string created_at;
        public string updated_at;
        public string last_compilation_requested_at;
        public string root_project_id;
        public ChildProjectData[] children_projects;
    }
    
    [System.Serializable]
    public class ChildProjectData
    {
        public string id, name;
        public string type_id;
        public string parent_id;
        public string template_project_id;
        public string creator_id;
        public string licensed_by;
        public string licensed_at;
        public string deleted_at;
        public string created_at;
        public string updated_at;
        public string last_compilation_requested_at;
        public string root_project_id;
        public bool is_licensed;
    }
    
    [System.Serializable]
    public class SoundtrackOptionData
    {
        public string id;
        public string name;
        public bool   is_licensed;
        public bool   is_loaded;
    }
    
    [Serializable]
    public class PackageData
    {
        public string name;
        public string version;
        public string displayName;
        public string description;
        public string unity;
        public string unityRelease;
        public string documentationUrl;
        public string changelogUrl;
        public string licensesUrl;
        public string[] dependencies;
        public string[] keywords;
        public PackageAuthor author;
    }
    
    [Serializable]
    public class PackageAuthor
    {
        public string name;
        public string email;
        public string url;
    }

    [Serializable]
    public class PingBack
    {
        public string os;
        public string event_text;
        public string device_id;
        public bool   in_editor;
        public string platform;
        public string title;
        public string connected;
        public bool   is_using_stinger;
        public Int64  project_id;
        public Int64  arrangement_id;
        public string arrangement_type;
        public string transition_type;
        public string transition_timing;
        public float  transition_delay;
        public string time;
        public string web_url;
        public string plugin_version;
        public string play_id;
    }

    // PlusMusic settings
    [Serializable]
    public class PM_Settings
    {
        public string target;
        public string username;
        public string password;
        public Int64  project_id;
        public string api_key;
        public bool   auto_play;
        public string credentials;
        public string base_url;
    }

    #endregion

}   // End Namespace
