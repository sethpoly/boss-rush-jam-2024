public class TommyGunCard: Card
{
    public TommyGunCard(string name, int cost) : base(name, cost)
    {}
    
    public override void ApplyEffect(Player player)
    {
        if(isActive) return;
        base.ApplyEffect(player);

        player.ChangeGun(GunType.tommyGun);
    }

    public override void ResetEffect(Player player)
    {
        if(!isActive) return;
        base.ResetEffect(player);
    }

    public static TommyGunCard Default()
    {
        return new TommyGunCard("Tommy Gun", 3);
    }
}
