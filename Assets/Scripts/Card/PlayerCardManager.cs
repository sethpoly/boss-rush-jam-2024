using System.Collections.Generic;
using UnityEngine;

class PlayerCardManager: MonoBehaviour
{
    public int drawCount = 4;
    public List<Card> cardsInDeck;
    public List<Card> selectedCards;
    public List<Card> cardsInHand;

    public Player player;

    /// <summary>
    /// Deactivate selected cards, discard selected cards, draw X new cards
    /// </summary>
    public void StartNewRound()
    {
        DeactivateAllSelectedCards();
        DiscardAllSelectedCards();
        DiscardAllCardsInHand();

        for(int i = 0; i < drawCount; i++)
        {
            DrawCard();
        }
    }

    /// <summary>
    /// Draw a card from cardsInDeck list and add it to the player's cardsInHand list
    /// </summary>
    public void DrawCard()
    {
        if (cardsInDeck.Count > 0) 
        {
            int index = cardsInDeck.Count - 1;
            cardsInHand.Add(cardsInDeck[index]);
            cardsInDeck.RemoveAt(index);
            Debug.Log("Player drew card: " + cardsInHand[^1].cardName);
        }
    }

    /// <summary>
    /// Select a given card from the current cards in the Player's hand.
    /// This card will be added the selectedCards list so it may be activated later on.
    /// This card will be removed from the cardsInHand list.
    /// </summary>
    /// <param name="cardInHandIndex"></param>
    public void SelectCard(int cardInHandIndex)
    {
        if(cardsInHand.Count - 1 >= cardInHandIndex)
        {
            selectedCards.Add(cardsInHand[cardInHandIndex]);
            cardsInHand.RemoveAt(cardInHandIndex);
            Debug.Log("Player selected card from hand: " + selectedCards[^1].cardName);
        }
        else
        {
            Debug.LogError("Card with index: " + cardInHandIndex + " does not exist in cardsinHand list");
        }
    }

    public void ActivateAllSelectedCards()
    {
        if(player == null) 
        {
            Debug.LogError("Cannot activated selected cards. Player is null");
            return;
        }

        for(int i = 0; i < selectedCards.Count; i++)
        {
            selectedCards[i].ApplyEffect(player);
        }
        Debug.Log("Activated all selected cards");
    }

    public void DeactivateAllSelectedCards()
    {
        if(player == null) 
        {
            Debug.LogError("Cannot reset selected cards. Player is null");
            return;
        }

        for(int i = 0; i < selectedCards.Count; i++)
        {
            selectedCards[i].ResetEffect(player);
        }
        Debug.Log("Deactivated all selected cards");
    }

    public void DiscardAllSelectedCards()
    {
        selectedCards.Clear();
        Debug.Log("Discarding all selected cards");
    }
    public void DiscardAllCardsInHand()
    {
        cardsInHand.Clear();
        Debug.Log("Discarding all cards in hand");
    }
}