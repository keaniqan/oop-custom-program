using MyApp;

public abstract class ActionCommand
{
    public abstract void Execute(Player player, Enemy enemy, Game game);
}

public class AttackCommand : ActionCommand
{
    private int damage;

    public AttackCommand(int damage)
    {
        this.damage = damage;
    }

    public override void Execute(Player player, Enemy enemy, Game game)
    {
        enemy.TakeDamage(damage);
    }
}

public class BlockCommand : ActionCommand
{
    private int amount;

    public BlockCommand(int amount)
    {
        this.amount = amount;
    }

    public override void Execute(Player player, Enemy enemy, Game game)
    {
        player.AddBlock(amount);
    }
}

public class ApplyEffectCommand : ActionCommand
{
    private EffectType effectType;
    private int stacks;

    public ApplyEffectCommand(EffectType effectType, int stacks)
    {
        this.effectType = effectType;
        this.stacks = stacks;
    }

    public override void Execute(Player player, Enemy enemy, Game game)
    {
        enemy.AddEffectStack(effectType, stacks);
    }
}   


public class DrawCommand : ActionCommand
{
    private int amount;

    public DrawCommand(int amount)
    {
        this.amount = amount;
    }

    public override void Execute(Player player, Enemy enemy, Game game)
    {
        player.DrawCards(amount);
    }
}

public class GainEnergyCommand : ActionCommand
{
    private int amount;

    public GainEnergyCommand(int amount)
    {
        this.amount = amount;
    }

    public override void Execute(Player player, Enemy enemy, Game game)
    {
        if (game.CurrentRoom is Combat combatRoom)
        {
            combatRoom.AddEnergy(amount);
        }
    }
}

public class HealCommand : ActionCommand
{
    private int amount;

    public HealCommand(int amount)
    {
        this.amount = amount;
    }

    public override void Execute(Player player, Enemy enemy, Game game)
    {
        player.AddHealth(amount);
    }
}
