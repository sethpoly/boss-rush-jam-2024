using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public MovementController movementController;

    public void IncreaseMovementSpeed(float multiplier)
    {
        Debug.Log("Increasing speed by " + multiplier);
        movementController.SetSpeed(movementController.speed * multiplier);
    }

    public void ResetMovementSpeed()
    {
        Debug.Log("Reset movement speed effect");
        movementController.ResetSpeed();
    }
}