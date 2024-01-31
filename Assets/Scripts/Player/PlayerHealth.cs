using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth: MonoBehaviour {
    public float maxHitPoints;
    public float currentHitPoints;
    public Image healthBar;

    void Awake()
    {
        currentHitPoints = maxHitPoints;
    }

    public void TakeDamage(float hitPoints) 
    {
        currentHitPoints -= hitPoints;
        healthBar.fillAmount = currentHitPoints / maxHitPoints;
    }

    public void Heal(float healAmount)
    {
        currentHitPoints += healAmount;
        healthBar.fillAmount = currentHitPoints / maxHitPoints;
    }
}