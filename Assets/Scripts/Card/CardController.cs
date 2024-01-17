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
    public float speed = 2f;

    private Vector2 startingPosition;

    void Start()
    {
        InitializeTitle();
        startingPosition = transform.position;
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

    void OnMouseEnter()
    {
        frontRenderer.color = Color.green;
        MoveCardUp();
    }

    void OnMouseExit()
    {
        frontRenderer.color = Color.white;
        MoveCardDown();
    }

    private void MoveCardUp()
    {
        var yOffset = startingPosition.y + .5f;
        iTween.MoveTo(gameObject, iTween.Hash("y", yOffset, "time", 1, "islocal", true));    
    }

    private void MoveCardDown()
    {
        var yOffset = startingPosition.y;
        iTween.MoveTo(gameObject, iTween.Hash("y", yOffset, "time", 1, "islocal", true));
    }
}
