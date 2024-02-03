using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth: MonoBehaviour {
    [SerializeField] GameManager gameManager;
    [SerializeField] SpriteRenderer sr;
    public float maxHitPoints;
    public float currentHitPoints;
    public Image healthBar;
    public float defenseBuffMultiplier = 0;
    public BossController bossController;
    public bool invincibleActive = false;
    private Color originalSpriteColor;

    void Awake()
    {
        currentHitPoints = maxHitPoints;
        originalSpriteColor = sr.color;
    }

    void OnEnable() {
        invincibleActive = false;
        sr.color = originalSpriteColor;
    }

    public void TakeDamage(float hitPoints) 
    {
        if(invincibleActive) return;
        float ignoredDamage = hitPoints * defenseBuffMultiplier;
        float damageToTake = hitPoints - ignoredDamage;
        currentHitPoints -= damageToTake;
        healthBar.fillAmount = currentHitPoints / maxHitPoints;
        gameManager.ScreenShake();

        // Check if player died
        if(currentHitPoints <= 0)
        {
            currentHitPoints = 0;
            gameManager.PlayExplosion(this.transform);
            gameManager.musicManager.PlayExplosion();
            Debug.Log("Game over");
             Destroy(transform.parent.gameObject);
            // Go to menu
            gameManager.LoadScene("Menu");
        } else {
            StartCoroutine(Flash());
            gameManager.musicManager.PlayHit();
        }
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

    private IEnumerator Flash() 
    {
        invincibleActive = true;
        var tmpColor = originalSpriteColor;
        tmpColor.a = 0.5f;
        sr.color = tmpColor;
        yield return new WaitForSeconds(1.5f);
        sr.color = originalSpriteColor;
        invincibleActive = false;
    }
}