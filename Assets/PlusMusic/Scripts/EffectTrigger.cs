using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PlusMusic
{
public class EffectTrigger : MonoBehaviour
{
    public string effect = "CutOff";
    public float value;
    public float duration;

    private void OnTriggerEnter(Collider other)
    {
        Debug.LogWarning("EffectTrigger.OnTriggerEnter(): SetMixerSetting() is deprecated! Use an AudioMixer instead");
        //PlusMusic_DJ.Instance.SetMixerSetting(effect, value, duration);
    }

}   // End Class
}   // End Namespace
