using System;
using UnityEngine;

public class BulletPatternGenerator : MonoBehaviour
{
    public bool reset;
    public ParticleSystem system;
    public int columnNumber;
    public float baseAngle;
    public float speed;
    public Color color;
    public float lifetime;
    public float firerate;
    public float size;
    public Material material;
    public bool shouldSpin;
    public float spinSpeed;
    public float direction;
    public LayerMask collisionLayerMask;
    private float angle;
    private float time;
    private bool _reset;
    private float startingDirection;

    void Awake()
    {   
        startingDirection = transform.rotation.z;
        _reset = reset;
    }

    void Update()
    {
        if(_reset != reset)
        {
            _reset = reset;
            Cancel();
        }
    }

    /// <summary>
    /// Updates all properties of this config. Does NOT restart
    /// </summary>
    /// <param name="config"></param>
    public void SetConfig(BulletPatternConfig config)
    {
        columnNumber = config.columnNumber;
        baseAngle = config.baseAngle;
        speed = config.speed;
        color = config.color;
        lifetime = config.lifetime;
        firerate = config.firerate;
        size = config.size;
        shouldSpin = config.shouldSpin;
        spinSpeed = config.spinSpeed;
        direction = config.direction;
    }

    /// <summary>
    /// Restart the generator with the current properties set. Note: Call SetConfig to configure the params
    /// before calling this function
    /// </summary>
    public void Cancel(Boolean restart = true)
    {
        CancelInvoke();
        RemoveAllParticleSystems();
        transform.rotation = Quaternion.Euler(0, 0, direction);


        if(restart) Summon();
    }

    void FixedUpdate()
    {
        time += Time.fixedDeltaTime;
        
        if(shouldSpin)
        {
            transform.rotation = Quaternion.Euler(0, 0, time * spinSpeed);
        } else 
        {
            transform.rotation = Quaternion.Euler(0, 0, direction);
        }
    }

    void Summon()
    {
        angle = baseAngle / columnNumber;
        for(int i = 0; i < columnNumber; i++)
        {
            // A simple particle material with no texture.
            Material particleMaterial = material;

            // Create a Particle System.
            var go = new GameObject("Particle System");
            go.transform.Rotate(angle * i, 90, 0); // Rotate so the system emits upwards.
            go.transform.parent = transform;
            go.transform.position = transform.position;
            system = go.AddComponent<ParticleSystem>();
            go.GetComponent<ParticleSystemRenderer>().material = particleMaterial;
            var mainModule = system.main;
            mainModule.startColor = Color.white;
            mainModule.startSize = 0.5f;
            mainModule.startSpeed = speed;
            mainModule.maxParticles = 100000;
            mainModule.simulationSpace = ParticleSystemSimulationSpace.World;

            var collision = system.collision;
            collision.type = ParticleSystemCollisionType.World;
            collision.mode = ParticleSystemCollisionMode.Collision2D;
            collision.lifetimeLoss = 1; //(if this is at 1, the paricle will "die" after colliding)
            collision.collidesWith = collisionLayerMask; //(the layer you want collision happening on)
            collision.sendCollisionMessages = true;
            collision.enabled = true;

            var emission = system.emission;
            emission.enabled = false;

            var forma = system.shape;
            forma.enabled = true;
            forma.shapeType = ParticleSystemShapeType.Sprite;
            forma.sprite = null;
        }

        // Every 2 secs we will emit.
        InvokeRepeating(nameof(DoEmit), 0f, firerate);
    }

    void DoEmit()
    {
        foreach(Transform child in transform)
        {
            system = child.GetComponent<ParticleSystem>();
            var emitParams = new ParticleSystem.EmitParams
            {
                startColor = color,
                startSize = size,
                startLifetime = lifetime
            };
            system.Emit(emitParams, 10);
            system.Play(); // Continue normal emissions
        }
    }

    private void RemoveAllParticleSystems()
    {
        foreach(Transform child in transform)
        {
            DetachParticles(child.GetComponent<ParticleSystem>());
            Destroy(child.gameObject, 5.0f);
        }
    }

    private void DetachParticles(ParticleSystem emit)
    {
        emit.Stop();
        emit.transform.parent = null;
    }
}
