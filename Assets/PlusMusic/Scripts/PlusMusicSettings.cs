
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace PlusMusic
{
public class PlusMusicSettings : MonoBehaviour
{

    public Slider volumeSlider;
    public Dropdown theDropdownMenu;
    public Slider loadingBar;
    public TMP_Text loadingText;
    public Action<bool> SetPause;
    public bool isPauseMenu = false;
    public bool autoPlay = false;
    public GameObject settingsRoot;
    public GameObject debugText;
    public GameObject closeButton;
    public Toggle playButton;

    private SoundtrackOptionData[] theSoundtrackOptions;
    private bool firstRun = true;
    private bool hasSongLoaded = false;
    private bool hasProjectLoaded = false;
    private bool hideDebugText = false;

    private float timeUntilDj = 0.0f;
    private int logIntervalDj = 5;
    private int lastLogDj = 0;
    private bool debugMode = false;

    public bool HasLoaded { get => hasSongLoaded; set => hasSongLoaded = value; }

        
    private void Start()
    {
        if (null == PlusMusic_DJ.Instance)
            Debug.LogError("PlusMusicSettings.Start(): There is no DJ in the scene!");
        else
            debugMode = PlusMusic_DJ.Instance.debugMode;

        if (debugMode)
            Debug.Log("PlusMusicSettings.Start()");
        
        OpenSettings(true);

        theDropdownMenu.ClearOptions();
        playButton.isOn = autoPlay;
        volumeSlider.value = 0.0f;
        closeButton.SetActive(isPauseMenu);
    }

    private void Update()
    {
        if (null == PlusMusic_DJ.Instance)
        {
            timeUntilDj += Time.deltaTime;

            int interVal = (int)(timeUntilDj / logIntervalDj);
            if (interVal > lastLogDj)
            {
                Debug.LogWarningFormat("PlusMusicSettings.Update(): No DJ for {0} seconds!", (interVal * logIntervalDj));
                lastLogDj = interVal;
            }

            return;
        }

        if (firstRun)
        {
            firstRun = false;
            PlusMusic_DJ.Instance.LoadingProgress += SetLoadingBarProgress;
            volumeSlider.value = PlusMusic_DJ.Instance.GetVolume();
        }

        if (!hasProjectLoaded)
        { 
            if (PlusMusic_DJ.Instance.HasProjectLoaded)
            {
                hasProjectLoaded = true;
                SetSoundtrackOptions(PlusMusic_DJ.Instance.TheSoundtrackOptions);
            }
        }

        if (isPauseMenu && (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape)))
        {
            OpenSettings(!settingsRoot.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (!hideDebugText)
            {
                hideDebugText = true;
                if (debugText != null) debugText.SetActive(false);
            }
            else
            {
                hideDebugText = false;
                if (debugText != null) debugText.SetActive(true);
            }
        }
    }

    private void OnDestroy()
    {
        if (null == PlusMusic_DJ.Instance) { return; }

        PlusMusic_DJ.Instance.LoadingProgress -= SetLoadingBarProgress;
    }

    public void SetSoundtrackOptions(SoundtrackOptionData[] soundtrackOptions)
    {
        if (debugMode)
            Debug.Log("PlusMusicSettings.SetSoundtrackOptions()");

        theSoundtrackOptions = soundtrackOptions;
        theDropdownMenu.ClearOptions();

        List<string> dropOptions = new List<string>();

        foreach (SoundtrackOptionData option in theSoundtrackOptions)
            dropOptions.Add(option.name);

        theDropdownMenu.AddOptions(dropOptions);

        if (!PlusMusic_DJ.Instance.settings.auto_play)
            SetSoundtrackOnChange();
    }

    public void SetSoundtrackOnChange()
    {
        Int32 index = theDropdownMenu.value;
        Int64 songId = Int64.Parse(theSoundtrackOptions[index].id);

        if (debugMode)
            Debug.LogFormat("PlusMusicSettings.SetSoundtrackOnChange(): [{0}] = {1}, {2}",
                index, songId, theSoundtrackOptions[index].name);

        TransitionInfo transition = PlusMusic_DJ.Instance.GetCurrentTransition;
        volumeSlider.value = transition.volume;

        PlusMusic_DJ.Instance.WindDownMainAudio();
        PlusMusic_DJ.Instance.currentAudioSource.Stop();
        PlusMusic_DJ.Instance.LoadSoundtrack(songId, transition, autoPlay);
    }

    public void TogglePlay(bool value)
    {
        if (null == PlusMusic_DJ.Instance) { return; }

        if (debugMode)
            Debug.Log("PlusMusicSettings.TogglePlay()");

        if (playButton.isOn)
            PlusMusic_DJ.Instance.StartAudio();
        else
            PlusMusic_DJ.Instance.StopAudio();
    }
        
    public void SetMusicVolume(float value)
    {
        //value = Mathf.Pow(value, 2); //Volume bars feel better when they are in a logaritmic
        PlusMusic_DJ.Instance.SetVolume(value);
    }
        
    public void SetLoadingBarProgress(float value)
    {
        loadingBar.value = value;
        if (value < 1)
        {
            hasSongLoaded = false;
            loadingText.text = "Loading ...";
        }
        else
        {
            loadingText.text = "Done";
            hasSongLoaded = true;
        }
    }

    public void Close()
    {
        if (isPauseMenu)
            OpenSettings(!settingsRoot.activeSelf);
    }

    public void QuitApp()
    {
        if (Application.isPlaying)
            Application.Quit();
    }

    public void OpenSettings(bool isActive)
    {
        if (settingsRoot != null)
        {
            settingsRoot.SetActive(isActive);
            SetPause?.Invoke(isActive);
        }
    }

}   // End Class
}   // End Namespace
