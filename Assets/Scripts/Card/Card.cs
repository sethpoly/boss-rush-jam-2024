using UnityEngine;

[System.Serializable]
abstract public class Card: MonoBehaviour
{
    public string cardName = "Card";
    public int cardCost = 0;

    public virtual void ApplyEffect(Player player)
    {
        // Base implementation for card effect
    }

    public virtual void ResetEffect(Player player)
    {
        
    }
}
