using UnityEngine;

public class LaserCard: Card
{
    public LaserCard(string name, int cost) : base(name, cost)
    {}
    
    public override void ApplyEffect(Player player)
    {
        if(isActive) return;
        base.ApplyEffect(player);

        player.ChangeGun(GunType.laser);
    }

    public override void ResetEffect(Player player)
    {
        if(!isActive) return;
        base.ResetEffect(player);
    }

    public override string Description()
    {
        return "Equip an additional gun that shoots slow, but deals more damage (stacks)";
    }

    public static LaserCard Default(Sprite sprite)
    {
        var card = new LaserCard("Laser Gun", 3);
        card.sprite = sprite;
        return card;
    }
}
