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

    Ray ray;    
    RaycastHit2D hit;

    void Start()
    {
        InitializeTitle();
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        OnHover(ray, hit); 
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

    public void OnHover(Ray ray, RaycastHit2D hit)
    {
        // hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
        // if(hit.collider == GetComponent<Collider2D>())
        // {
        //     frontRenderer.color = Color.green;
        // }         
        // else
        // {
        //     frontRenderer.color = Color.white;
        // }
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
