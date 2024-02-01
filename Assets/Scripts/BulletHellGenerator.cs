using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHellGenerator : MonoBehaviour
{

    public ParticleSystem system;
    public int columnNumber;
    public float speed;
    public Color color;
    public float lifetime;
    public float firerate;
    public float size;
    public Material material;
    public Sprite sprite;
    public float spinSpeed;

    private float angle;
    private float time;

    void Awake()
    {   
        Summon();
    }

    void FixedUpdate()
    {
        time += Time.fixedDeltaTime;

        transform.rotation = Quaternion.Euler(0, 0, time * spinSpeed);
    }

    void Summon()
    {
        angle = 360f / columnNumber;
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
}
