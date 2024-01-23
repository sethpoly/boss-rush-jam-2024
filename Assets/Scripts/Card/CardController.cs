using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Controller for Card prefab (for runtime instantiation)
/// </summary>
public class CardController : MonoBehaviour
{
    public Card card;
    public TextMeshProUGUI title;
    public Canvas canvas;
    public SpriteRenderer frontRenderer;
    public SpriteRenderer backRenderer;
    public event Action<string> MouseClickOccuredOnDrawnCardWithId;
    public event Action<string> MouseClickOccuredOnSelectedCardWithId;
    private Vector2 startingPosition;
    public CardState cardState = CardState.decked;

    void Start()
    {
        GameObject hand = GameObject.FindGameObjectWithTag("hand");
        InitializeTitle();
        SetStartingPosition(new Vector2(transform.position.x, hand.transform.position.y));
    }

    private void InitializeTitle()
    {
        title.text = card.cardName;
    }

    /// <summary>
    /// Set the sort order of this card so text doesnt overlap
    /// </summary>
    /// <param name="order"></param>
    public void SetSortOrder(int order)
    {
        frontRenderer.sortingOrder = order;
        backRenderer.sortingOrder = order;
        canvas.sortingOrder = order;
    }

    public void SetCardState(CardState cardState)
    {
        this.cardState = cardState;
    }

    private void SetStartingPosition(Vector2 startingPosition)
    {
        this.startingPosition = startingPosition;
    }

    void OnMouseDown()
    {
        Debug.Log("Player clicked " + card.cardName);
        switch(cardState)
        {
            case CardState.drawn:
                MouseClickOccuredOnDrawnCardWithId.Invoke(card.id);
                break;
            case CardState.selected:
                MouseClickOccuredOnSelectedCardWithId.Invoke(card.id);
                break;
            default: break;
        }
    }

    void OnMouseEnter()
    {
        frontRenderer.color = Color.green;
        MoveCardUpAnimation();
    }

    void OnMouseExit()
    {
        frontRenderer.color = Color.white;
        MoveCardDownAnimation();
    }

    public void OnDidFinishRefreshing()
    {
        cardState = CardState.drawn;
    }

    public void DidStartRefreshing()
    {
        cardState = CardState.refreshing;
    }

    private void MoveCardUpAnimation()
    {
        if(cardState != CardState.drawn) return;
        var yOffset = startingPosition.y + .5f;
        iTween.MoveTo(gameObject, iTween.Hash("y", yOffset, "time", 1, "islocal", true));    
    }

    private void MoveCardDownAnimation()
    {
        if(cardState != CardState.drawn) return;
        var yOffset = startingPosition.y;
        iTween.MoveTo(gameObject, iTween.Hash("y", yOffset, "time", 1, "islocal", true));
    }
}
