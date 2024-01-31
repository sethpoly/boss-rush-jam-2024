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

    public float baseDamageRate = 1f;
    public float baseFireRate = 2f;
    public float fireRate;
    public float damageRate;

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
            lastShotTime = Time.time + baseFireRate - fireRate;
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

    public void SetDamageRate(float damageRate)
    {
        this.damageRate = damageRate;
    }

    public void ResetFireRate()
    {
        this.fireRate = baseFireRate;
    }

    public void ApplyNewGun(GunType gunType)
    {
        switch(gunType)
        {
            case GunType.tommyGun:
            ApplyTommyGun();
            break;
        }
    }

    private void ApplyTommyGun()
    {
        baseFireRate = .2f;
        baseDamageRate = 1f;
    }
}
