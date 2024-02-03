
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace PlusMusic
{
    public class PlusMusicSettingsSo : ScriptableObject
    {
        [SerializeField] private Int64  _projectId              = 0;
        [SerializeField] private string _authToken              = "";
        [SerializeField] private string _deviceId               = "";
        [SerializeField] private float  _musicVolume            = 1.0f;
        [SerializeField] private bool   _autoLoadProject        = true;
        [SerializeField] private bool   _autoPlayProject        = true;
        [SerializeField] private bool   _debugMode              = false;
        [SerializeField] private bool   _logServerResponses     = false;
        [SerializeField] private bool   _isFromUnityStore       = false;
        [SerializeField] private bool   _didVSAttribution       = false;

        public TextAsset jsonPackage;
        [SerializeField] private PackageData packageData;

        public Int64  ProjectId          => _projectId;
        public string AuthToken          => _authToken;
        public string DeviceId           => _deviceId;
        public float  MusicVolume        => _musicVolume;
        public bool   AutoLoadProject    => _autoLoadProject;
        public bool   AutoPlayProject    => _autoPlayProject;
        public bool   DebugMode          => _debugMode;
        public bool   LogServerResponses => _logServerResponses;
        public bool   IsFromUnityStore   => _isFromUnityStore;
        public bool   DidVSAttribution   => _didVSAttribution;

        public PackageData PackageData => packageData;

        public void SaveData(
            Int64 projectId, string authToken, string deviceId,
            float musicVolume, bool autoLoadProject, bool autoPlayProject, 
            bool debugMode, bool logServerResponses, bool didVSAttribution)
        {
            _projectId          = projectId;
            _authToken          = authToken;
            _deviceId           = deviceId;
            _musicVolume        = musicVolume;
            _autoLoadProject    = autoLoadProject;
            _autoPlayProject    = autoPlayProject;
            _debugMode          = debugMode;
            _logServerResponses = logServerResponses;
            _didVSAttribution   = didVSAttribution;
        }

        public void SetPackageData()
        {
            if (jsonPackage != null)
            {
                packageData = JsonUtility.FromJson<PackageData>(jsonPackage.text);
            }
        }
    }

}
