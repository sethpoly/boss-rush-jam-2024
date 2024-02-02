using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;

public class BossPatternController: MonoBehaviour {
    [SerializeField] BossController boss;
    private Tuple<BulletPatternConfig, BulletPatternConfig?> patternEasyOne;
    private Tuple<BulletPatternConfig, BulletPatternConfig?> patternEasyTwo;
    private Tuple<BulletPatternConfig, BulletPatternConfig?> patternMediumOne;

    // Basic spread with full auto down middle secondary
    private Tuple<BulletPatternConfig, BulletPatternConfig?> patternMediumTwo;
    private Tuple<BulletPatternConfig, BulletPatternConfig?> patternMediumThree;

    // Spinning flower
    private Tuple<BulletPatternConfig, BulletPatternConfig?> patternHardOne;
    private Tuple<BulletPatternConfig, BulletPatternConfig?> patternHardTwo;


    private List<Tuple<BulletPatternConfig, BulletPatternConfig?>> easyPatterns = new();
    private List<Tuple<BulletPatternConfig, BulletPatternConfig?>> mediumPatterns = new();
    private List<Tuple<BulletPatternConfig, BulletPatternConfig?>> hardPatterns = new();
    private List<Tuple<BulletPatternConfig, BulletPatternConfig?>> easyMediumPatterns = new();
    private List<Tuple<BulletPatternConfig, BulletPatternConfig?>> mediumHardPatterns = new();
    private List<Tuple<BulletPatternConfig, BulletPatternConfig?>> allPatterns = new();


    public int patternChangeCooldown;

    private Random random = new Random();

    void Awake()
    {
        InitializeAllPatterns();
    }

    private void InitializeAllPatterns()
    {
        InitializeEasyPatterns();  
        InitializeMediumPatterns();
        InitializeHardPatterns(); 
        easyMediumPatterns.AddRange(easyPatterns);
        easyMediumPatterns.AddRange(mediumPatterns);
        mediumHardPatterns.AddRange(mediumPatterns);
        mediumHardPatterns.AddRange(hardPatterns);
        allPatterns.AddRange(easyMediumPatterns);
        allPatterns.AddRange(hardPatterns);
    }

    void OnEnable()
    {
        InvokeRepeating(nameof(ChooseNextPattern), 2f, patternChangeCooldown);
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            boss.SetPattern(patternEasyOne);
            boss.RestartCurrentPattern();
        }
        if (Input.GetKeyDown(KeyCode.J)) 
        {
            boss.SetPattern(patternEasyTwo);
            boss.RestartCurrentPattern();
        }
        if (Input.GetKeyDown(KeyCode.K)) 
        {
            boss.SetPattern(patternHardOne);
            boss.RestartCurrentPattern();
        }
        if (Input.GetKeyDown(KeyCode.L)) 
        {
            boss.SetPattern(patternMediumOne);
            boss.RestartCurrentPattern();
        }
        if (Input.GetKeyDown(KeyCode.M)) 
        {
            boss.SetPattern(patternMediumTwo);
            boss.RestartCurrentPattern();
        }
        if (Input.GetKeyDown(KeyCode.N)) 
        {
            boss.SetPattern(patternMediumThree);
            boss.RestartCurrentPattern();
        }
        if (Input.GetKeyDown(KeyCode.B)) 
        {
            boss.SetPattern(patternHardOne);
            boss.RestartCurrentPattern();
        }
    }

    private void ChooseNextPattern()
    {
        float percentageOfBossHealth = boss.currentHitPoints / boss.maxHitPoints;
        if(percentageOfBossHealth > .75f)
        {
            var nextIndex = random.Next(easyPatterns.Count);
            boss.SetPattern(easyPatterns[nextIndex]);
            boss.RestartCurrentPattern();
        } else if(percentageOfBossHealth < .75f && percentageOfBossHealth > .5f)
        {
            var nextIndex = random.Next(easyMediumPatterns.Count);
            boss.SetPattern(easyMediumPatterns[nextIndex]);
            boss.RestartCurrentPattern();
        } else if(percentageOfBossHealth < .5f && percentageOfBossHealth > .3f)
        {
            var nextIndex = random.Next(allPatterns.Count);
            boss.SetPattern(allPatterns[nextIndex]);
            boss.RestartCurrentPattern();
        } 
        else if(percentageOfBossHealth > 0 && percentageOfBossHealth < .3f)
        {
            var nextIndex = random.Next(mediumHardPatterns.Count);
            boss.SetPattern(mediumHardPatterns[nextIndex]);
            boss.RestartCurrentPattern();
        } else {
            // Stop all patterns, boss is dead
            boss.EndPatternGenerators();
        }
    }

    private void InitializeEasyPatterns()
    {
        patternEasyOne = new Tuple<BulletPatternConfig, BulletPatternConfig?>(new()
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
        }, null);

        patternEasyTwo = new Tuple<BulletPatternConfig, BulletPatternConfig?>(new()
        {
            columnNumber = 10,
            baseAngle = 180f,
            speed = 2f,
            color = Color.white,
            lifetime = 5f,
            firerate = 1f,
            size = .2f,
            shouldSpin = false,
            spinSpeed = 0f,
            direction = 0f
        }, null);
        easyPatterns.Add(patternEasyOne);
        easyPatterns.Add(patternEasyTwo);
    }

    private void InitializeMediumPatterns()
    {
        patternMediumOne = new Tuple<BulletPatternConfig, BulletPatternConfig?>(new()
        {
            columnNumber = 20,
            baseAngle = 70f,
            speed = 2f,
            color = Color.white,
            lifetime = 5f,
            firerate = 1f,
            size = .2f,
            shouldSpin = false,
            spinSpeed = 0f,
            direction = 0f
        }, null);
        
        patternMediumTwo = new Tuple<BulletPatternConfig, BulletPatternConfig?>(new()
        {
            columnNumber = 10,
            baseAngle = 180f,
            speed = 2f,
            color = Color.white,
            lifetime = 5f,
            firerate = .5f,
            size = .2f,
            shouldSpin = false,
            spinSpeed = 0f,
            direction = 0f
        }, 
        new()
        {
            columnNumber = 1,
            baseAngle = 0f,
            speed = 3f,
            color = Color.white,
            lifetime = 5f,
            firerate = .1f,
            size = .2f,
            shouldSpin = true,
            spinSpeed = 50f,
            direction = 0
        });
        patternMediumThree = new Tuple<BulletPatternConfig, BulletPatternConfig?>(new()
        {
            columnNumber = 10,
            baseAngle = 360f,
            speed = 2f,
            color = Color.white,
            lifetime = 5f,
            firerate = .4f,
            size = .2f,
            shouldSpin = true,
            spinSpeed = 15f,
            direction = 0f
        }, 
        new()
        {
            columnNumber = 1,
            baseAngle = 0f,
            speed = 3f,
            color = Color.white,
            lifetime = 5f,
            firerate = .3f,
            size = .2f,
            shouldSpin = true,
            spinSpeed = 50f,
            direction = 0
        });
        mediumPatterns.Add(patternMediumOne);
        mediumPatterns.Add(patternMediumTwo);
        mediumPatterns.Add(patternMediumThree);
    }

    private void InitializeHardPatterns()
    {
        patternHardOne = new Tuple<BulletPatternConfig, BulletPatternConfig?>(new()
        {
            columnNumber = 20,
            baseAngle = 360f,
            speed = 2f,
            color = Color.white,
            lifetime = 5f,
            firerate = .5f,
            size = .2f,
            shouldSpin = true,
            spinSpeed = 50f,
            direction = 0f
        }, null);
        
        patternHardTwo = new Tuple<BulletPatternConfig, BulletPatternConfig?>(new()
        {
            columnNumber = 20,
            baseAngle = 360f,
            speed = 2f,
            color = Color.white,
            lifetime = 5f,
            firerate = .8f,
            size = .2f,
            shouldSpin = true,
            spinSpeed = 50f,
            direction = 0f
        }, 
        new()
        {
            columnNumber = 40,
            baseAngle = 40f,
            speed = 3f,
            color = Color.white,
            lifetime = 5f,
            firerate = 1f,
            size = .2f,
            shouldSpin = true,
            spinSpeed = 75f,
            direction = 0
        });
        hardPatterns.Add(patternHardOne);
        hardPatterns.Add(patternHardTwo);
    }
}