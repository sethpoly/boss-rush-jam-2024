using TMPro;
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

    void Start()
    {
        //InitializeTitle();
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
        Debug.Log("Mouse over" + card.cardName);
        frontRenderer.color = Color.green;
    }

    void OnMouseExit()
    {
        Debug.Log("Mouse exit" + card.cardName);
        frontRenderer.color = Color.white;
    }
}
