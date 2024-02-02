using UnityEngine;

public class FireRateCard: Card
{
    public float fireRateIncrease;

    public FireRateCard(string name, int cost, float fireRateIncrease) : base(name, cost)
    {
        this.fireRateIncrease = fireRateIncrease;
    }
    
    public override void ApplyEffect(Player player)
    {
        if(isActive) return;
        base.ApplyEffect(player);

        player.IncreaseFireRate(fireRateIncrease);
    }

    public override void ResetEffect(Player player)
    {
        if(!isActive) return;
        base.ResetEffect(player);
        player.ResetFireRate();
    }

    public static FireRateCard Default(Sprite sprite)
    {
        var card = new FireRateCard("Fire Rate++", 1, .2f);
        card.sprite = sprite;
        return card;
    }
}
