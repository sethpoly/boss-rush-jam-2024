using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

class DraftPhaseManager: MonoBehaviour
{
    public int drawCount = 4;
    public List<GameObject> cardsInDeck;
    public List<GameObject> selectedCards;
    public List<GameObject> cardsInHand;
    public List<GameObject> playerCards; // Current cards that the player selected and are active

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

    public EnergyController energyController;
    public LevelLoader levelLoader;
    public BattlePhaseManager battlePhaseManager;
    public TextMeshProUGUI cardListText;

    [Space]
    [Header("Card Sprites")]
    public Sprite pistolSprite;
    public Sprite rocketSprite;
    public Sprite laserSprite;
    public Sprite machineGunSprite;
    public Sprite damageSprite;
    public Sprite defenseSprite;
    public Sprite movementSprite;
    public Sprite firerateSprite;
    public Sprite potionSprite;

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        OnDraftPhaseResume();
    }

    private void OnDraftPhaseResume()
    {
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
 
        AddCardToDeck(MovementSpeedCard.Default(sprite: movementSprite));
        AddCardToDeck(FireRateCard.Default(sprite: firerateSprite));
        AddCardToDeck(DamageRateCard.Default(sprite: damageSprite));
        AddCardToDeck(DefenseBuffCard.Default(sprite: defenseSprite));
        AddCardToDeck(PotionCard.Default(sprite: potionSprite));
        AddCardToDeck(TommyGunCard.Default(sprite: machineGunSprite));
        AddCardToDeck(LaserCard.Default(sprite: laserSprite));
        AddCardToDeck(PistolCard.Default(sprite: pistolSprite));
        AddCardToDeck(RocketCard.Default(sprite: rocketSprite));
        Debug.Log("Deck created with " + cardsInDeck.Count + " cards");
        ShuffleDeck();
    }

    private void ShuffleDeck()
    {
        // Knuth shuffle algorithm :: courtesy of Wikipedia :)
        for (int t = 0; t < cardsInDeck.Count; t++ )
        {
            var tmp = cardsInDeck[t];
            int r = Random.Range(t, cardsInDeck.Count);
            cardsInDeck[t] = cardsInDeck[r];
            cardsInDeck[r] = tmp;
        }
        Debug.Log("Shuffling deck...");
    }

    private void AddCardToDeck(Card card)
    {
        var cardPrefab = Instantiate(this.cardPrefab, deck.transform.position, Quaternion.identity, this.transform);
        var controller = cardPrefab.GetComponent<CardController>();
        controller.card = card;
        controller.MouseClickOccuredOnDrawnCardWithId += OnDrawnCardClicked;
        cardsInDeck.Add(cardPrefab);
    }

    /// <summary>
    /// Draw a card from cardsInDeck list and add it to the player's cardsInHand list
    /// </summary>
    public void DrawCard()
    {
        if (cardsInDeck.Count > 0) 
        {
            int index = cardsInDeck.Count - 1;
            var controller = cardsInDeck[index].GetComponent<CardController>();
            var cardToMove = cardsInDeck[index];

            Transform newPosition = PositionForNextDrawnCard(cardsInHand.Count);
            controller.DidStartRefreshing();
            iTween.MoveTo(cardToMove, iTween.Hash("y", newPosition.position.y, "x", newPosition.position.x, "time", 1.5, "islocal", true, "onComplete", "OnDidFinishRefreshing"));  

            controller.SetSortOrder( cardsInHand.Count + 1 % 10);
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
            var controller = cardsInHand[existingCardIndex].GetComponent<CardController>();

            // Attempt to use energy
            bool enoughEnergy = energyController.UseEnergy(amount: controller.card.cardCost);

            if(!enoughEnergy) 
            {
                // TODO: Screenshake?
                return;
            }

            selectedCards.Add(cardsInHand[existingCardIndex]);
            cardsInHand.RemoveAt(existingCardIndex);
            controller.SetCardState(CardState.selected);

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
            // Tween scale
            Vector3 desiredSize = new(.15f, .15f, .15f);
            float time = 1f;           
            iTween.ScaleTo(selectedCards[existingCardIndex], desiredSize, time);

            var pos = PositionForNextDrawnCard(cardsInHand.Count);
            Debug.Log("MOVING TO POS = " + pos);
            iTween.MoveTo(selectedCards[existingCardIndex], iTween.Hash("y", pos.position.y, "x", pos.position.x, "time", 1, "islocal", true, "onComplete", "OnDidFinishRefreshing"));

            var controller = selectedCards[existingCardIndex].GetComponent<CardController>();
            controller.SetSortOrder(cardsInHand.Count + 1 % 10);
            cardsInHand.Add(selectedCards[existingCardIndex]);
            selectedCards.RemoveAt(existingCardIndex);

            // Replace energy
            energyController.ReplaceEnergy(amount: controller.card.cardCost);

            Debug.Log("Player deselected card: " + GetController(cardsInHand[^1]).card.cardName);
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

    private void DestroyCardsInHand()
    {
        for (int i = cardsInHand.Count - 1; i >= 0; i--)
        {
            Destroy(cardsInHand[i]);
            cardsInHand.RemoveAt(i);
            Debug.Log("Destroyed card in hand");
        }
    }

    private void MoveSelectedCardsToPlayerCardsList()
    {
        for (int i = selectedCards.Count - 1; i >= 0; i--)
        {
            if(!playerCards.Contains(selectedCards[i]))
            {
                var controller = selectedCards[i].GetComponent<CardController>();
                playerCards.Add(selectedCards[i]);
                controller.SetCardState(CardState.active);
                selectedCards.RemoveAt(i);
                Debug.Log("Added card: " + controller.card.cardName + " to playerCards");
            }
        }
    }

    private void UpdateCardListUi()
    {
        var cardList = "";
        for(int i = 0; i < playerCards.Count; i++)
        {
            var controller = playerCards[i].GetComponent<CardController>();
            cardList += controller.card.cardName + "\n";
        }
        cardListText.text = cardList;
    }

    /// <summary>
    /// End the current draft phase and transition to Battle phase
    /// </summary>
    public void EndDraftPhase()
    {
        // Call levelLoader.loadNextPhase
        levelLoader.LoadNextPhase();

        // Activate player selected cards
        ActivateAllSelectedCards();

        // Move SelectedCards to PlayerCards list
        MoveSelectedCardsToPlayerCardsList();

        // Update card list ui
        UpdateCardListUi();

        // Discard cards in hand
        DestroyCardsInHand();

        // Replenish energy
        energyController.ResetEnergy();
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

    /// <summary>
    /// Move cards over in hand once a card leaves the hand
    /// </summary>
    /// <param name="cardInHandIndexRemoved"></param>
    private void RefreshCardsInHandPositions(int cardInHandIndexRemoved)
    {
        if(cardInHandIndexRemoved >= 3) return; // Max cards is 4
        int startCardIndex = cardInHandIndexRemoved++;
        for(int i = startCardIndex; i < 4; i++)
        {
            if(i >= cardsInHand.Count) continue;

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