
using UnityEngine;
using PlusMusic;
//using static PlusMusic_DJ;


namespace PlusMusic
{
public class CurveInDownTriggerSoundtrackSwitch : MonoBehaviour
{
    [SerializeField] private TransitionInfo transition;
    [SerializeField] private PlusMusic_DJ theDJ;

    void Start()
    {
        //if (!theDJ) theDJ = Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.LogWarning("CurveInDownTriggerSoundtrackSwitch.OnTriggerEnter(): Deprecated! Use PMPlayArrangement instead");

        //theDJ.PlaySoundPM(transition);

        //theDJ.PlayArrangement(transition);
    }

}   // End Class
}   // End Namespace
