using UnityEngine;

public class PotionCard: Card
{
    public float potionAmount;

    public PotionCard(string name, int cost, float potionAmount) : base(name, cost)
    {
        this.potionAmount = potionAmount;
    }
    
    public override void ApplyEffect(Player player)
    {
        if(isActive) return;
        base.ApplyEffect(player);

        player.ApplyPotion(potionAmount);
    }

    public override void ResetEffect(Player player)
    {
        if(!isActive) return;
        base.ResetEffect(player);
    }

    public override string Description()
    {
        return "Heal to max health at the start of the next battle (1 time use)";
    }

    public static PotionCard Default(Sprite sprite)
    {
        var card = new PotionCard("Potion", 3, 10f);
        card.sprite = sprite;
        return card;
    }
}
