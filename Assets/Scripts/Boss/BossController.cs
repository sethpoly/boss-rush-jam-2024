using System.Collections;
using System.Collections.Generic;
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
    }

    private void InitializeBulletPatterns()
    {
        patternEasyOne = new()
        {
            columnNumber = 10,
            baseAngle = 180f,
            speed = .75f,
            color = Color.white,
            lifetime = 5f,
            firerate = 1.5f,
            size = .1f,
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
