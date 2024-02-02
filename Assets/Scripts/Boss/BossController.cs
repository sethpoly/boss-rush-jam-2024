using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    public float maxHitPoints;
    public float currentHitPoints;
    public Image healthBar;
    public float damageOutput = 1f;
    public PlayerGun playerGun;
    public BulletPatternGenerator patternGeneratorMain;
    public BulletPatternGenerator patternGeneratorSecondary;
    public SpriteRenderer spriteRenderer;
    private Tuple<BulletPatternConfig, BulletPatternConfig?> currentPattern;

    [SerializeField] private Animator animator;
    [SerializeField] private bool facingRight = true;

    void Awake()
    {
        currentHitPoints = maxHitPoints;
    }

    public void TakeDamage(float hitPoints)
    {
        currentHitPoints -= hitPoints;
        healthBar.fillAmount = currentHitPoints / maxHitPoints;
        gameManager.ScreenShake(duration: .1f, magnitude: .05f);

        if (currentHitPoints <= 0)
        {
            // TODO: Game over - explode boss
        }
    }

    public void SetPattern(Tuple<BulletPatternConfig, BulletPatternConfig?> pattern)
    {
        currentPattern = pattern;
        patternGeneratorMain.SetConfig(pattern.Item1);

        // Set pattern two
        if (pattern.Item2 != null)
        {
            patternGeneratorSecondary.SetConfig(pattern.Item2.Value);
        }
    }

    public void RestartCurrentPattern()
    {
        patternGeneratorMain.Cancel();

        bool restartSecondary = currentPattern.Item2 != null;
        patternGeneratorSecondary.Cancel(restart: restartSecondary);

        FlipSprite();
    }

    public void EndPatternGenerators()
    {
        patternGeneratorMain.Cancel(false);
        patternGeneratorSecondary.Cancel(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("bullet"))
        {
            float damageToTake = playerGun.baseDamageRate + playerGun.damageRate;
            TakeDamage(damageToTake);
            Destroy(other.gameObject);
            // TODO: Play SFX
        }
    }

    private void FlipSprite()
    {
        if(facingRight)
        {
            animator.SetTrigger("FlipLeft");
        }
        else 
        {
            animator.SetTrigger("FlipRight");
        }
        facingRight = !facingRight;
    }
}
