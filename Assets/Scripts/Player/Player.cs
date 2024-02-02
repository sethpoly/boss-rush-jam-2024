using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public MovementController movementController;
    public PlayerHealth playerHealth;
    public PlayerGunManager playerGun;

    public void IncreaseMovementSpeed(float multiplier)
    {
        Debug.Log("Increasing speed by " + multiplier);
        movementController.SetSpeed(movementController.speed + multiplier);
    }

    public void IncreaseFireRate(float fireRate)
    {
        Debug.Log("Increasing fire rate by " + fireRate);
        playerGun.SetFireRate(fireRate);
    }

    public void IncreaseDamageRate(float damageRate)
    {
        Debug.Log("Increasing damage rate by " + damageRate);
        playerGun.SetDamageRate(damageRate);
    }

    public void IncreaseDefenseBuff(float defenseBuff)
    {
        Debug.Log("Increasing defense buff by " + defenseBuff);
        playerHealth.SetDefenseBuff(playerHealth.defenseBuffMultiplier + defenseBuff);
    }

    public void ApplyPotion(float potionAmount)
    {
        Debug.Log("Applying potion: " + potionAmount);
        playerHealth.Heal(potionAmount);
    }

    public void ChangeGun(GunType gunType)
    {
        Debug.Log("Applying new gun: " + gunType);
        playerGun.ApplyNewGun(gunType);
    }

    public void ResetMovementSpeed()
    {
        Debug.Log("Reset movement speed effect");
        movementController.ResetSpeed();
    }

    public void ResetFireRate()
    {
        Debug.Log("Reset fire rate");
        playerGun.ResetFireRate();
    }
}