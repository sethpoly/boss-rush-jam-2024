[System.Serializable]
public class Card 
{
    public string cardName = "Card";
    public int cardCost = 0;
    public bool isActive = false;

    public Card(string name, int cost)
    {
        this.cardName = name;
        this.cardCost = cost;
    }

    public virtual void ApplyEffect(Player player)
    {
        isActive = true;
    }

    public virtual void ResetEffect(Player player)
    {
        isActive = false;
    }
}
