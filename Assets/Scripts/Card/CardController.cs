using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Controller for Card prefab (for runtime instantiation)
/// </summary>
public class CardController : MonoBehaviour
{
    public Card card;
    public TextMeshPro title;

    void Start()
    {
        InitializeTitle();
    }

    private void InitializeTitle()
    {
        title.text = card.cardName;
    }
}
