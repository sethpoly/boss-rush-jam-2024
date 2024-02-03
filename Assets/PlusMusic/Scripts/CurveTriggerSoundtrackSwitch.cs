
using System;
using UnityEngine;
using PlusMusic;
//using static PlusMusic_DJ;


namespace PlusMusic
{
public class CurveTriggerSoundtrackSwitch : MonoBehaviour
{
    [SerializeField] private PlusMusic_DJ theDJ;
    [SerializeField] private TransitionInfo theTransition;
    void Start()
    {
        //if (!theDJ) theDJ = Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.LogWarning("CurveTriggerSoundtrackSwitch.OnTriggerEnter(): Deprecated! Use PMPlayArrangement instead");

        //theDJ.PlaySoundPM(switchSoundtrackTag.ToString(), curve, _duration, stingerSoundtrack, useStinger, false, timing);
        //theDJ.PlaySoundPM(theTransition);

        //theDJ.PlayArrangement(theTransition);
    }

}   // End Class
}   // End Namespace
