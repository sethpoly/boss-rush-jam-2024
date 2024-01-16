using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public MovementController movementController;
    public List<Card> activeCards;
    public List<Card> selectedCards;
    public List<Card> cardsInHand;

    void Start()
    {
        ActivateAllSelectedCards();
    }

    public void ActivateAllSelectedCards()
    {
        for(int i = 0; i < selectedCards.Count; i++)
        {
            selectedCards[i].ApplyEffect(this);
        }
    }

    public void ResetAllSelectedCards()
    {
        for(int i = 0; i < selectedCards.Count; i++)
        {
            selectedCards[i].ResetEffect(this);
        }
    }

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