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
    public event Action<string> MouseClickOccuredOnCardWithId;
    private Vector2 startingPosition;

    void Start()
    {
        InitializeTitle();
        SetStartingPosition(new Vector2(transform.position.x, -4.5f));
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

    private void SetStartingPosition(Vector2 startingPosition)
    {
        this.startingPosition = startingPosition;
    }

    void OnMouseDown()
    {
        Debug.Log("Player clicked " + card.cardName);
        MouseClickOccuredOnCardWithId.Invoke(card.id);
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

    private void MoveCardUpAnimation()
    {
        var yOffset = startingPosition.y + .5f;
        iTween.MoveTo(gameObject, iTween.Hash("y", yOffset, "time", 1, "islocal", true));    
    }

    private void MoveCardDownAnimation()
    {
        var yOffset = startingPosition.y;
        iTween.MoveTo(gameObject, iTween.Hash("y", yOffset, "time", 1, "islocal", true));
    }
}
