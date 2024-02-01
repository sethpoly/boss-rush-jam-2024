using UnityEngine;

public class MovementSpeedCard: Card
{
    public float speedIncrease;

    public MovementSpeedCard(string name, int cost, float speedIncrease) : base(name, cost)
    {
        this.speedIncrease = speedIncrease;
    }
    
    public override void ApplyEffect(Player player)
    {
        if(isActive) return;
        base.ApplyEffect(player);

        // Apply movement speed increase to the player
        player.IncreaseMovementSpeed(speedIncrease);
    }

    public override void ResetEffect(Player player)
    {
        if(!isActive) return;
        base.ResetEffect(player);
        player.ResetMovementSpeed();
    }

    public static MovementSpeedCard Default(Sprite sprite)
    {
        var card = new MovementSpeedCard("Speed++", 1, 15f);
        card.sprite = sprite;
        return card;
    }
}
