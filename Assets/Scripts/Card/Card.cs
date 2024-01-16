using UnityEngine;

[System.Serializable]
abstract public class Card: MonoBehaviour
{
    public string cardName = "Card";
    public int cardCost = 0;
    public bool isActive = false;

    public virtual void ApplyEffect(Player player)
    {
        isActive = true;
    }

    public virtual void ResetEffect(Player player)
    {
        isActive = false;
    }
}
