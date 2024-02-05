using TMPro;
using UnityEngine;

public class StatTracker: MonoBehaviour {
    [SerializeField] private TextMeshProUGUI playerLoadoutLabel;

    void Start()
    {
        playerLoadoutLabel.text = GameManager.playerLoadout;
    }

    void OnDestroy()
    {
        GameManager.ResetPlayerLoadout();
    }
}