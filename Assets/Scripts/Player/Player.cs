using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public MovementController movementController;
    public PlayerHealth playerHealth;
    public PlayerGun playerGun;

    public void IncreaseMovementSpeed(float multiplier)
    {
        Debug.Log("Increasing speed by " + multiplier);
        movementController.SetSpeed(movementController.speed + multiplier);
    }

    public void IncreaseFireRate(float fireRate)
    {
        Debug.Log("Increasing fire rate by " + fireRate);
        playerGun.SetFireRate(playerGun.fireRate - fireRate);
    }

    public void IncreaseDamageRate(float damageRate)
    {
        Debug.Log("Increasing damage rate by " + damageRate);
        playerGun.SetDamageRate(playerGun.damageRate + damageRate);
    }

    public void IncreaseDefenseBuff(float defenseBuff)
    {
        Debug.Log("Increasing defense buff by " + defenseBuff);
        playerHealth.SetDefenseBuff(playerHealth.defenseBuffMultiplier + defenseBuff);
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