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

        // Apply movement speed increase to the player
        player.IncreaseFireRate(fireRateIncrease);
    }

    public override void ResetEffect(Player player)
    {
        if(!isActive) return;
        base.ResetEffect(player);
        player.ResetFireRate();
    }

    public static FireRateCard Default()
    {
        return new FireRateCard("Fire Rate++", 1, .1f);
    }
}
