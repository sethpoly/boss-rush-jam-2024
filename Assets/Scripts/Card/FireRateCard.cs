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

    public override string Description()
    {
        return "Shoot 20% faster with all weapons (stacks)";
    }

    public static FireRateCard Default(Sprite sprite)
    {
        var card = new FireRateCard("Fire Rate++", 2, .2f);
        card.sprite = sprite;
        return card;
    }
}
