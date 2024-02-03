using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using PlusMusic;
using System;


namespace PlusMusic
{
public class PlusMusicInit : MonoBehaviour
{
        /*
        public Dropdown theDropdownMenu;
        private SoundtrackOptionData[] theSoundtrackOptions;

        // Start is called before the first frame update
        void Start()
        {
            theDropdownMenu.ClearOptions();
            if (PlusMusic_DJ.Instance == null) { Debug.LogError("There is no DJ in scene"); return; }
            PlusMusic_DJ.Instance.OnSoundTrackOptionsReceived += SetSoundtrackOptions;

        }

        private void OnDestroy()
        {
            if (PlusMusic_DJ.Instance == null) { return; }
            PlusMusic_DJ.Instance.OnSoundTrackOptionsReceived -= SetSoundtrackOptions;
        }

        public void SetSoundtrackOptions(SoundtrackOptionData[] soundtrackOptions)
        {
            theSoundtrackOptions = soundtrackOptions;
            theDropdownMenu.ClearOptions();
            List<string> dropOptions = new List<string>();
            foreach (SoundtrackOptionData option in theSoundtrackOptions)
            {
                dropOptions.Add(option.name);
            }
            theDropdownMenu.AddOptions(dropOptions);
        }

        public void SetSoundtrackOnChange()
        {
            Int32 index = theDropdownMenu.value;
            Int64 songId = Int64.Parse(theSoundtrackOptions[index].id);

            Debug.LogFormat("SetSoundtrackOnChange(): [{0}] = {1}, {2}", 
                index, songId, theSoundtrackOptions[index].name);

            PlusMusic_DJ.Instance.currentAudioSource.volume = 0.75f;
            PlusMusic_DJ.Instance.currentAudioSource.volume = 0.5f;
            PlusMusic_DJ.Instance.currentAudioSource.volume = 0.0f;
            PlusMusic_DJ.Instance.currentAudioSource.Stop();
            PlusMusic_DJ.Instance.LoadSoundtrack(songId);
        }
        */

}   // End Class
}   // End Namespace
