public class DefenseBuffCard: Card
{
    public float defenseBuffIncrease;

    public DefenseBuffCard(string name, int cost, float defenseBuffIncrease) : base(name, cost)
    {
        this.defenseBuffIncrease = defenseBuffIncrease;
    }
    
    public override void ApplyEffect(Player player)
    {
        if(isActive) return;
        base.ApplyEffect(player);

        player.IncreaseDefenseBuff(defenseBuffIncrease);
    }

    public override void ResetEffect(Player player)
    {
        if(!isActive) return;
        base.ResetEffect(player);
    }

    public static DefenseBuffCard Default()
    {
        return new DefenseBuffCard("Defense Buff++", 1, .25f);
    }
}
