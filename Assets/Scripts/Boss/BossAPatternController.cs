using System;
using System.Collections.Generic;
using UnityEngine;

public class BossAPatternController: MonoBehaviour {
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


    private List<Tuple<BulletPatternConfig, BulletPatternConfig?>> easyPatterns;
    private List<Tuple<BulletPatternConfig, BulletPatternConfig?>> mediumPatterns;
    private List<Tuple<BulletPatternConfig, BulletPatternConfig?>> hardPatterns;

    public Transform bottomReference;

    void Awake()
    {
        InitializeEasyPatterns();  
        InitializeMediumPatterns();
        InitializeHardPatterns(); 
    }

    // Update is called once per frame
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
            color = Color.red,
            lifetime = 5f,
            firerate = 1f,
            size = .2f,
            shouldSpin = false,
            spinSpeed = 0f,
            direction = 0f
        }, null);
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
    }

    private void InitializeHardPatterns()
    {
        patternHardOne = new Tuple<BulletPatternConfig, BulletPatternConfig?>(new()
        {
            columnNumber = 20,
            baseAngle = 360f,
            speed = 2f,
            color = Color.green,
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
    }
}