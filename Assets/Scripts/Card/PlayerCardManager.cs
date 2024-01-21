using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

class PlayerCardManager: MonoBehaviour
{
    public int drawCount = 4;
    public List<GameObject> cardsInDeck;
    public List<GameObject> selectedCards;
    public List<GameObject> cardsInHand;

    public Player player;
    public GameObject cardPrefab;
    public Transform playerHand;
    public GameObject deck;
    public GameObject cardSelectionContainer;

    // Positions where cards in hand can go
    public Transform cardSlotLeft;
    public Transform cardSlotLeftMiddle;
    public Transform cardSlotRightMiddle;
    public Transform cardSlotRight;

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
        for(int i = 0; i < 5; i++)
        {
            var card = new MovementSpeedCard("Speed x" + i, 1, i);
            var cardPrefab = Instantiate(this.cardPrefab, deck.transform.position, Quaternion.identity);
            var controller = cardPrefab.GetComponent<CardController>();
            controller.card = card;
            controller.MouseClickOccuredOnDrawnCardWithId += OnDrawnCardClicked;
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

            Transform newPosition = PositionForNextDrawnCard(cardsInHand.Count);
            // Draw animation
            // float xOffset = 1f; // Adjust this value to control the spacing
            // float y = playerHand.position.y;
            // float x = playerHand.position.x - 2f + (cardsInHand.Count + 1) * xOffset;
            
            iTween.MoveTo(cardsInDeck[index], iTween.Hash("y", newPosition.position.y, "x", newPosition.position.x, "time", 1, "islocal", true));  

            var controller = cardsInDeck[index].GetComponent<CardController>();
            controller.SetSortOrder( cardsInHand.Count + 1 % 10);
            controller.SetCardState(CardState.drawn);
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
    public void SelectCard(string cardId)
    {
        int existingCardIndex = cardsInHand.FindIndex(card => card.GetComponent<CardController>().card.id == cardId);
        if (existingCardIndex != -1)
        {
            selectedCards.Add(cardsInHand[existingCardIndex]);
            cardsInHand.RemoveAt(existingCardIndex);
            var controller = selectedCards.Last().GetComponent<CardController>();
            controller.SetCardState(CardState.selected);

            // TODO: 
            //  Enable "Selected" state of CardController
            //  "Selected" state shows the name of card when it hovers
            //  "Selected" state allows deselection as well which reverses this process

            // Tween scale
            Vector3 desiredSize = new(.1f, .1f, .1f);
            float time = 1f;           
            iTween.ScaleTo(selectedCards.Last(), desiredSize, time);

            // Tween card location
            float offset = .75f;
            Vector2 position = new(cardSelectionContainer.transform.position.x + (selectedCards.Count - 1)  * offset, cardSelectionContainer.transform.position.y);
            iTween.MoveTo(selectedCards.Last(), position, time);

            controller.MouseClickOccuredOnSelectedCardWithId += OnSelectedCardClicked;
            RefreshCardsInHandPositions(existingCardIndex);
            
            Debug.Log("Player selected card from hand: " + GetController(selectedCards[^1]).card.cardName);
        }
        else
        {
            Debug.LogError("Card with index: " + existingCardIndex + " does not exist in cardsinHand list");
        }
    }

    private void DeselectCard(string cardId)
    {
        int existingCardIndex = selectedCards.FindIndex(card => card.GetComponent<CardController>().card.id == cardId);
        if (existingCardIndex != -1)
        {
            cardsInHand.Add(selectedCards[existingCardIndex]);
            selectedCards.RemoveAt(existingCardIndex);
            var controller = cardsInHand.Last().GetComponent<CardController>();
            controller.SetCardState(CardState.drawn);

            // Tween scale
            Vector3 desiredSize = new(.15f, .15f, .15f);
            float time = 1f;           
            iTween.ScaleTo(cardsInHand.Last(), desiredSize, time);

            // Tween card location
            float xOffset = 1f; 
            float y = playerHand.position.y;
            float x = playerHand.position.x - 2f + cardsInHand.Count * xOffset;
            
            iTween.MoveTo(cardsInHand.Last(), iTween.Hash("y", y, "x", x, "time", 1, "islocal", true));
            
            Debug.Log("Player deselected card: " + GetController(selectedCards[^1]).card.cardName);
        }
        else
        {
            Debug.LogError("Card with index: " + existingCardIndex + " does not exist in selectedCards list");
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

    private Transform PositionForNextDrawnCard(int cardsInHandCount)
    {
        return cardsInHandCount switch
        {
            0 => cardSlotLeft,
            1 => cardSlotLeftMiddle,
            2 => cardSlotRightMiddle,
            3 => cardSlotRight,
            _ => null,
        };
    }

    private void RefreshCardsInHandPositions(int cardInHandIndexRemoved)
    {
        if(cardInHandIndexRemoved >= 3) return; // Max cards is 4
        int startCardIndex = cardInHandIndexRemoved++;
        for(int i = startCardIndex; i < 4; i++)
        {
            Transform blankPosition = PositionForNextDrawnCard(i);
            GameObject cardToMove = cardsInHand[i];
            cardToMove.GetComponent<CardController>().DidStartRefreshing();
            iTween.MoveTo(cardToMove, iTween.Hash("y", blankPosition.position.y, "x", blankPosition.position.x, "time", 1, "islocal", true, "onComplete", "OnDidFinishRefreshing"));  
        }
    }

    private CardController GetController(GameObject obj)
    {
        return obj.GetComponent<CardController>();
    }

    private void OnDrawnCardClicked(string cardIndex) 
    {
        SelectCard(cardIndex);
    }

    private void OnSelectedCardClicked(string cardIndex)
    {
        DeselectCard(cardIndex);
    }
}