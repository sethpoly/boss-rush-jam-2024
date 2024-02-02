using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BossController : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    public float maxHitPoints;
    public float currentHitPoints;
    public Image healthBar;
    public float damageOutput = 1f;
    public PlayerGunManager playerGun;
    public BulletPatternGenerator patternGeneratorMain;
    public BulletPatternGenerator patternGeneratorSecondary;
    private Tuple<BulletPatternConfig, BulletPatternConfig?> currentPattern;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool facingRight = true;
    [SerializeField] private Transform locationLeft;
    [SerializeField] private Transform locationRight;
    [SerializeField] private Transform locationMiddle;
    private List<Transform> possibleLocations = new();
    private Transform currentPosition;
    private float movementSpeed = 1f;

    private Color originalColor;
    private bool flashing = false;

    void Awake()
    {
        currentHitPoints = maxHitPoints;
        possibleLocations.Add(locationLeft);
        possibleLocations.Add(locationRight);
        possibleLocations.Add(locationMiddle);
        originalColor = spriteRenderer.color;
    }

    void Update() 
    {
        if(currentPosition != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, currentPosition.position, movementSpeed * Time.deltaTime);
        }
    }

    public void TakeDamage(float hitPoints)
    {
        currentHitPoints -= hitPoints;
        healthBar.fillAmount = currentHitPoints / maxHitPoints;
        gameManager.ScreenShake(duration: .1f, magnitude: .05f);

        if (currentHitPoints <= 0)
        {
            gameManager.PlayExplosion(transform);
            Destroy(transform.parent.gameObject);
            Debug.Log("Game Over");
            // TODO: Show credits
        } else 
        {
            // Toggle flashing
            if(!flashing)
            {
                StartCoroutine(Flash());
            }
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
        MoveToRandomLocation();
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
            var bullet = other.gameObject.GetComponent<Bullet>();
            float damageToTake = playerGun.damageRateBuff + bullet.bulletDamage;
            TakeDamage(damageToTake);
            if(bullet.gunType != GunType.laser) Destroy(other.gameObject);
            
            // TODO: Play hit SFX
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

    private void MoveToRandomLocation()
    {
        int randIndex = Random.Range(0, possibleLocations.Count);
        
        // Apply random movement speed
        movementSpeed = Random.Range(1f, 4f);
        currentPosition = possibleLocations[randIndex];
    }

    private IEnumerator Flash() 
    {
        flashing = true;
        var tmpColor = originalColor;
        tmpColor.a = 0.7f;
        spriteRenderer.color = tmpColor;
        yield return new WaitForSeconds(.1f);
        spriteRenderer.color = originalColor;
        flashing = false;
    }
}
