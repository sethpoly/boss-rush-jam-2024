using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth: MonoBehaviour {
    public float maxHitPoints;
    public float currentHitPoints;
    public Image healthBar;
    public float defenseBuffMultiplier = 0;
    public BossController bossController;

    void Awake()
    {
        currentHitPoints = maxHitPoints;
    }

    public void TakeDamage(float hitPoints) 
    {
        float ignoredDamage = hitPoints * defenseBuffMultiplier;
        float damageToTake = hitPoints - ignoredDamage;
        currentHitPoints -= damageToTake;
        healthBar.fillAmount = currentHitPoints / maxHitPoints;

        // Check if player died
        if(currentHitPoints <= 0)
        {
            currentHitPoints = 0;
            Debug.Log("Game over");
            // TODO: Go to menu
        }_
    }

    public void Heal(float healAmount)
    {
        if(currentHitPoints + healAmount > maxHitPoints) 
        {
            currentHitPoints = maxHitPoints;
        } 
        else
        {
            currentHitPoints += healAmount;
        }
        healthBar.fillAmount = currentHitPoints / maxHitPoints;
    }

    public void SetDefenseBuff(float defenseBuff)
    {
        defenseBuffMultiplier = defenseBuff;
    }

    /// <summary>
    /// OnParticleCollision is called when a particle hits a collider.
    /// </summary>
    /// <param name="other">The GameObject hit by the particle.</param>
    void OnParticleCollision(GameObject other)
    {
        TakeDamage(bossController.damageOutput);
    }
}