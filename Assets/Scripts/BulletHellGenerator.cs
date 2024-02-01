using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHellGenerator : MonoBehaviour
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
    public Sprite sprite;
    public bool shouldSpin;
    public float spinSpeed;
    public float direction;
    public LayerMask collisionLayerMask;
    private float angle;
    private float time;
    private bool _reset;

    void Awake()
    {   
        _reset = reset;
        Summon();
    }

    void Update()
    {
        if(_reset != reset)
        {
            _reset = reset;
            CancelInvoke();
            RemoveAllParticleSystems();
            Summon();
        }
    }

    void FixedUpdate()
    {
        time += Time.fixedDeltaTime;
        
        if(shouldSpin)
        {
            transform.rotation = Quaternion.Euler(0, 0, time * spinSpeed);
        } else 
        {
            transform.rotation = Quaternion.Euler(0, 0, transform.rotation.z + direction);
        }
    }

    void Summon()
    {
        angle = baseAngle / columnNumber;
        for(int i = 0; i < columnNumber; i++)
        {
            // A simple particle material with no texture.
            Material particleMaterial = material;

            // Create a green Particle System.
            var go = new GameObject("Particle System");
            go.transform.Rotate(angle * i, 90, 0); // Rotate so the system emits upwards.
            go.transform.parent = transform;
            go.transform.position = transform.position;
            system = go.AddComponent<ParticleSystem>();
            go.GetComponent<ParticleSystemRenderer>().material = particleMaterial;
            var mainModule = system.main;
            mainModule.startColor = Color.green;
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

            var texture = system.textureSheetAnimation;
            texture.mode = ParticleSystemAnimationMode.Sprites;
            texture.AddSprite(sprite);
            texture.enabled = true;
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
            Destroy(child.gameObject);
        }
    }
}
