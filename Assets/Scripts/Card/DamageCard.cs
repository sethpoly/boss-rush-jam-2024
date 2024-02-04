using UnityEngine;

public class DamageRateCard: Card
{
    public float damageRateIncrease;

    public DamageRateCard(string name, int cost, float damageRateIncrease) : base(name, cost)
    {
        this.damageRateIncrease = damageRateIncrease;
    }
    
    public override void ApplyEffect(Player player)
    {
        if(isActive) return;
        base.ApplyEffect(player);

        player.IncreaseDamageRate(damageRateIncrease);
    }

    public override void ResetEffect(Player player)
    {
        if(!isActive) return;
        base.ResetEffect(player);
        player.ResetFireRate();
    }

    public override string Description()
    {
        return "Deal 50% more damage with all weapons (stacks)";
    }

    public static DamageRateCard Default(Sprite sprite)
    {
        var card = new DamageRateCard("Damage++", 2, .5f)
        {
            sprite = sprite
        };
        return card;
    }
}
