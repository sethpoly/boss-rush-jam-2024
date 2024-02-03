using UnityEngine;

public class PistolCard: Card
{
    public PistolCard(string name, int cost) : base(name, cost)
    {}
    
    public override void ApplyEffect(Player player)
    {
        if(isActive) return;
        base.ApplyEffect(player);

        player.ChangeGun(GunType.pistol);
    }

    public override void ResetEffect(Player player)
    {
        if(!isActive) return;
        base.ResetEffect(player);
    }

    public static PistolCard Default(Sprite sprite)
    {
        var card = new PistolCard("Pistol", 2);
        card.sprite = sprite;
        return card;
    }
}
