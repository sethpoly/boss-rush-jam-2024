using UnityEngine;

public class RocketCard: Card
{
    public RocketCard(string name, int cost) : base(name, cost)
    {}
    
    public override void ApplyEffect(Player player)
    {
        if(isActive) return;
        base.ApplyEffect(player);

        player.ChangeGun(GunType.rocket);
    }

    public override void ResetEffect(Player player)
    {
        if(!isActive) return;
        base.ResetEffect(player);
    }

    public override string Description()
    {
        return "Equip an additional gun that shoots very slow, but deals a whole lot of damage (stacks)";
    }

    public static RocketCard Default(Sprite sprite)
    {
        var card = new RocketCard("Rocket Launcher", 3);
        card.sprite = sprite;
        return card;
    }
}
