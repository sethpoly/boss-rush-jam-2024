using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerGunManager : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject laserPrefab;
    public GameObject machineGunBulletPrefab;
    public GameObject rocketPrefab;
    public float fireRateBuff = 0f;
    public float damageRateBuff = 0f;
    private bool isReady = false;

    void Awake()
    {
        ApplyPistol();
    }

    void Update()
    {
        if(isReady) Shoot();
    }

    void OnEnable() 
    {
        StartCoroutine(PrepareForBattle());
    }

    void OnDisable()
    {
        isReady = false;
    }

    private IEnumerator PrepareForBattle()
    {
        yield return new WaitForSeconds(1f);
        isReady = true;
    }

    private void Shoot()
    {
        foreach(Gun gun in GetComponents<Gun>())
        {
            gun.Shoot(fireRateBuff: fireRateBuff);
        }
    }

    public void SetFireRate(float fireRate)
    {
        this.fireRateBuff = fireRate;
    }

    public void SetDamageRate(float damageRate)
    {
        this.damageRateBuff = damageRate;
    }

    public void ResetFireRate()
    {
        this.fireRateBuff = 0f;
    }

    public void ApplyNewGun(GunType gunType)
    {
        switch(gunType)
        {
            case GunType.tommyGun:
            ApplyTommyGun();
            break;
            case GunType.pistol:
            ApplyPistol();
            break;
            case GunType.laser:
            ApplyLaser();
            break;
            case GunType.rocket:
            ApplyRocketLauncher();
            break;
        }
    }

    private void ApplyTommyGun()
    {
        var gun = gameObject.AddComponent<Gun>();
        gun.Setup(bulletPrefab: machineGunBulletPrefab, fireRate: .4f, GunType.tommyGun);
    }

    private void ApplyPistol()
    {
        var gun = gameObject.AddComponent<Gun>();
        gun.Setup(bulletPrefab: bulletPrefab, fireRate: 1f, GunType.pistol);
    }

    private void ApplyLaser()
    {
        var gun = gameObject.AddComponent<Gun>();
        gun.Setup(bulletPrefab: laserPrefab, fireRate: 3f, GunType.laser);
    }

    private void ApplyRocketLauncher()
    {
        var gun = gameObject.AddComponent<Gun>();
        gun.Setup(bulletPrefab: rocketPrefab, fireRate: 7f, GunType.rocket);
    }
}
