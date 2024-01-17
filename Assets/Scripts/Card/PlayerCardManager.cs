using System.Collections.Generic;
using UnityEngine;

class PlayerCardManager: MonoBehaviour
{
    public int drawCount = 4;
    public List<GameObject> cardsInDeck;
    public List<GameObject> selectedCards;
    public List<GameObject> cardsInHand;

    public Player player;
    public GameObject cardPrefab;
    public Transform playerHand;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        StartNewRound();
    }

    /// <summary>
    /// Deactivate selected cards, discard selected cards, draw X new cards
    /// </summary>
    public void StartNewRound()
    {
        DeactivateAllSelectedCards();
        DiscardAllSelectedCards();
        DiscardAllCardsInHand();
        ResetAndCreateDeck();

        for(int i = 0; i < drawCount; i++)
        {
            DrawCard();
        }
    }

    private void ResetAndCreateDeck()
    {
        cardsInDeck.Clear();
        float? lastXLocation = null;
        for(int i = 0; i < 5; i++)
        {
            if(lastXLocation == null)
            {
                lastXLocation = playerHand.position.x - 4f;
            } 
            else 
            {
                lastXLocation -= -1.5f;
            }
            var card = new MovementSpeedCard("Speed x" + i, 1, i);
            var spawnLocation = new Vector3(lastXLocation.Value, playerHand.position.y, 0);
            var cardPrefab = Instantiate(this.cardPrefab, spawnLocation, Quaternion.identity);
            var controller = cardPrefab.GetComponent<CardController>();
            controller.card = card;
            controller.SetSortOrder( i % 10);
            cardsInDeck.Add(cardPrefab);
        }
        Debug.Log("Deck created with " + cardsInDeck.Count + " cards");
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
            Debug.Log("Player drew card: " + GetController(cardsInHand[^1]).card.cardName);
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
            Debug.Log("Player selected card from hand: " + GetController(selectedCards[^1]).card.cardName);
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
            GetController(selectedCards[i]).card.ApplyEffect(player);
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
            GetController(selectedCards[i]).card.ResetEffect(player);
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

    private CardController GetController(GameObject obj)
    {
        return obj.GetComponent<CardController>();
    }
}