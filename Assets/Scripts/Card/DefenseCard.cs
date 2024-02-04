using UnityEngine;

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

    public override string Description()
    {
        return "Take 25% less damage than usual (stacks)";
    }

    public static DefenseBuffCard Default(Sprite sprite)
    {
        var card = new DefenseBuffCard("Armor++", 2, .25f);
        card.sprite = sprite;
        return card;
    }
}
