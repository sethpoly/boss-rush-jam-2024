using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float baseDamageRate = 1f;
    public float fireRateBuff = 0f;
    public float damageRate;

    void Awake()
    {
        ApplyPistol();
    }

    void Update()
    {
        Shoot();
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
        this.damageRate = damageRate;
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
        }
    }

    private void ApplyTommyGun()
    {
        var gun = gameObject.AddComponent<Gun>();
        gun.Setup(bulletPrefab: bulletPrefab, fireRate: .2f, GunType.tommyGun);
    }

    private void ApplyPistol()
    {
        var gun = gameObject.AddComponent<Gun>();
        gun.Setup(bulletPrefab: bulletPrefab, fireRate: 1f, GunType.pistol);
    }
}
