public class MovementSpeedCard: Card
{
    public float speedIncrease;

    public MovementSpeedCard(string name, int cost, float speedIncrease) : base(name, cost)
    {
        this.speedIncrease = speedIncrease;
    }
    
    public override void ApplyEffect(Player player)
    {
        base.ApplyEffect(player);

        // Apply movement speed increase to the player
        player.IncreaseMovementSpeed(speedIncrease);
    }

    public override void ResetEffect(Player player)
    {
        base.ResetEffect(player);
        player.ResetMovementSpeed();
    }
}
