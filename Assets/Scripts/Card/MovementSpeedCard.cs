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

    public override string Description()
    {
        return "Move 40% faster (stacks)";
    }

    public static MovementSpeedCard Default(Sprite sprite)
    {
        var card = new MovementSpeedCard("Agility++", 1, 40f);
        card.sprite = sprite;
        return card;
    }
}
