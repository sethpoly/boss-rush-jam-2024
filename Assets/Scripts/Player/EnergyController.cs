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

    public void ResetEnergy()
    {
        currentEnergy = startingEnergy;
    }

    private void RefreshEnergyLabel()
    {
        energyLabel.text = currentEnergy.ToString();
    }

    /// <summary>
    /// Attempt to use energy
    /// </summary>
    /// <param name="amount"></param>
    /// <returns>false if not enough energy, true otherwise</returns>
    public bool UseEnergy(int amount)
    {
        if (amount <= currentEnergy)
        {
            currentEnergy -= amount;
            return true;
        } 
        else 
        {
            Debug.LogError("Not enough energy");
            return false;
        }
    }

    public bool ReplaceEnergy(int amount)
    {
        if(amount + currentEnergy <= startingEnergy)
        {
            currentEnergy += amount;
            return true;
        }
        else 
        {
            Debug.LogError("Too much energy to add");
            return false;
        }
    }
}