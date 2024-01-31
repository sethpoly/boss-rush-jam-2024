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

        // Apply movement speed increase to the player
        player.IncreaseDamageRate(damageRateIncrease);
    }

    public override void ResetEffect(Player player)
    {
        if(!isActive) return;
        base.ResetEffect(player);
        player.ResetFireRate();
    }

    public static DamageRateCard Default()
    {
        return new DamageRateCard("Damage Rate++", 1, 5f);
    }
}
