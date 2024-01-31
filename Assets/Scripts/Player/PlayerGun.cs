using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    const string targetTag = "Boss";

    public GameObject bulletPrefab;

    // Auto-locked target
    private Vector2? currentTarget;

    private bool canShoot = false;
    private float lastShotTime;

    public float startingFireRate = 2f;

    public float fireRate;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        fireRate = startingFireRate;
    }

    // Update is called once per frame
    void Update()
    {
        SearchForTarget();
        ShootTimer();

        if (canShoot && currentTarget.HasValue)
        {
            Shoot();
        }
    }

    // Lock on to the boss if it exists in the world
    private void SearchForTarget()
    {
        try {
            var target = GameObject.FindGameObjectWithTag(targetTag);
            currentTarget = target.transform.position;
        } catch {
            currentTarget = null;
        }
    }

    // Cooldown timer for bullet fire rate
    private void ShootTimer()
    {
        if(Time.time > lastShotTime)
        {
            lastShotTime = Time.time + fireRate;
            canShoot = true;
        } 
        else 
        {
            canShoot = false;
        }
    }

    private void Shoot()
    {
        GameObject.Instantiate(bulletPrefab, new Vector3(transform.position.x, transform.position.y, 0f), transform.rotation);
    }

    public void SetFireRate(float fireRate)
    {
        this.fireRate = fireRate;
    }

    public void ResetFireRate()
    {
        this.fireRate = startingFireRate;
    }
}
