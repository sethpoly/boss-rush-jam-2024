using TMPro;
using UnityEngine;

class EnergyController: MonoBehaviour {
    public int startingEnergy;
    private int _currentEnergy;
    public int currentEnergy {
        get {
            return _currentEnergy;
        }
        set { 
            _currentEnergy = value;
            RefreshEnergyLabel();
        }
    }

    public TextMeshProUGUI energyLabel;

    void Start()
    {
        ResetEnergy();
    }

    private void ResetEnergy()
    {
        currentEnergy = startingEnergy;
    }

    private void RefreshEnergyLabel()
    {
        energyLabel.text = _currentEnergy.ToString();
    }
}