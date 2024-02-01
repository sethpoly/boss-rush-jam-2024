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
    private BulletPatternConfig patternEasyOne;
    private BulletPatternConfig patternEasyTwo;

    void Awake()
    {
        currentHitPoints = maxHitPoints;
        InitializeBulletPatterns();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            patternGeneratorMain.SetConfig(patternEasyOne);
            patternGeneratorMain.Restart();
        }
        if (Input.GetKeyDown(KeyCode.J)) 
        {
            patternGeneratorMain.SetConfig(patternEasyTwo);
            patternGeneratorMain.Restart();
        }
    }

    private void InitializeBulletPatterns()
    {
        patternEasyOne = new()
        {
            columnNumber = 10,
            baseAngle = 180f,
            speed = 1.5f,
            color = Color.white,
            lifetime = 5f,
            firerate = 1.5f,
            size = .2f,
            shouldSpin = false,
            spinSpeed = 0f,
            direction = 0f
        };
        patternEasyTwo = new()
        {
            columnNumber = 10,
            baseAngle = 180f,
            speed = 2f,
            color = Color.red,
            lifetime = 5f,
            firerate = 1f,
            size = .2f,
            shouldSpin = false,
            spinSpeed = 0f,
            direction = 0f
        };
    }

    public void TakeDamage(float hitPoints)
    {
        currentHitPoints -= hitPoints;
        healthBar.fillAmount = currentHitPoints / maxHitPoints;
        gameManager.ScreenShake(duration: .1f, magnitude: .05f);
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
}
