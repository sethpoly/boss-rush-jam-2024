using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

[System.Serializable]
public class Card 
{
    public string id = System.Guid.NewGuid().ToString();
    public string cardName = "Card";
    public int cardCost = 0;
    public bool isActive = false;
    public Sprite sprite;

    public Card(string name, int cost)
    {
        this.cardName = name;
        this.cardCost = cost;
    }

    public virtual void ApplyEffect(Player player)
    {
        isActive = true;
        Debug.Log("Activating card: " + cardName);
    }

    public virtual void ResetEffect(Player player)
    {
        isActive = false;
        Debug.Log("Deactivating card: " + cardName);
    }
}
