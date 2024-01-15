using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    // Cooldown between shots
    public float cooldown = .5f;

    public GameObject bulletPrefab;

    // Auto-locked target
    private Vector2 currentTarget;

    const String targetTag = "Boss";

    private bool canShoot = false;
    private float lastShotTime;

    // Update is called once per frame
    void Update()
    {
        SearchForTarget();

        // Attempt shoot if cooldown is finished
        if(Time.time > lastShotTime)
        {
            lastShotTime = Time.time + cooldown;
            Shoot();
        }
    }

    private void SearchForTarget()
    {
        var target = GameObject.FindGameObjectWithTag(targetTag);
        currentTarget = target.transform.position;
    }

    private void Shoot()
    {
        Debug.Log("Shooting...");
    }
}
