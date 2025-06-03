using MyApp;


public abstract class ActionCommand
{
    public abstract void Execute(Player player, Enemy enemy, Game game);
}

public class AttackCommand : ActionCommand
{
    private int damage;
    private EffectTarget target;
    public AttackCommand(int damage, EffectTarget target)
    {
        this.damage = damage;
        this.target = target;
    }

    public override void Execute(Player player, Enemy enemy, Game game)
    {
        if (target == EffectTarget.Player)
        {
            var bufferEffect = player.Effects.FirstOrDefault(e => e.EffectType == EffectType.Buffer);
            if (bufferEffect != null && bufferEffect.Stacks > 0)
            {
            bufferEffect.Stacks--;
            }
            else
            {
                var strengthEffect = player.Effects.FirstOrDefault(e => e.EffectType == EffectType.StrengthUp);
                if (strengthEffect != null && strengthEffect.Stacks > 0)
                {
                    damage = strengthEffect.ApplyStrength(damage);
                }
                var weakEffect = player.Effects.FirstOrDefault(e => e.EffectType == EffectType.Weak);
                if (weakEffect != null && weakEffect.Stacks > 0)
                {
                    damage = weakEffect.ApplyWeak(damage);
                }
                var vulnerableEffect = enemy.Effects.FirstOrDefault(e => e.EffectType == EffectType.Vulnerable);
                if (vulnerableEffect != null && vulnerableEffect.Stacks > 0)
                {
                    damage = vulnerableEffect.ApplyVulnerable(damage);
                }
                var thornEffect = enemy.Effects.FirstOrDefault(e => e.EffectType == EffectType.Thorn);
                if (thornEffect != null && thornEffect.Stacks > 0)
                {
                    player.TakeDamage(thornEffect.Stacks);
                }
                player.TakeDamage(damage);
                if (player.Health <= 0)
                {
                    if (game.CurrentRoom is Combat combatRoom)
                    {
                        combatRoom.EndCombat();
                    }
                }
            }
        }
        else if (target == EffectTarget.Enemy)
        {
            var bufferEffect = enemy.Effects.FirstOrDefault(e => e.EffectType == EffectType.Buffer);
            if (bufferEffect != null && bufferEffect.Stacks > 0)
            {
                bufferEffect.Stacks--;
            }
            else
            {
                var strengthEffect = player.Effects.FirstOrDefault(e => e.EffectType == EffectType.StrengthUp);
                if (strengthEffect != null && strengthEffect.Stacks > 0)
                {
                    damage = strengthEffect.ApplyStrength(damage);
                }
                var weakEffect = player.Effects.FirstOrDefault(e => e.EffectType == EffectType.Weak);
                if (weakEffect != null && weakEffect.Stacks > 0)
                {
                    damage = weakEffect.ApplyWeak(damage);
                }
                var vulnerableEffect = enemy.Effects.FirstOrDefault(e => e.EffectType == EffectType.Vulnerable);
                if (vulnerableEffect != null && vulnerableEffect.Stacks > 0)
                {
                    damage = vulnerableEffect.ApplyVulnerable(damage);
                }
                var thornEffect = enemy.Effects.FirstOrDefault(e => e.EffectType == EffectType.Thorn);
                if (thornEffect != null && thornEffect.Stacks > 0)
                {
                    player.TakeDamage(thornEffect.Stacks);
                }
                enemy.TakeDamage(damage);
                if (enemy.Health <= 0)
                {
                    if (game.CurrentRoom is Combat combatRoom)
                    {
                        combatRoom.EndCombat();
                    }
                }
            }
        }
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
        var dexterityEffect = player.Effects.FirstOrDefault(e => e.EffectType == EffectType.DexterityUp);
        if (dexterityEffect != null && dexterityEffect.Stacks > 0)
        {
            amount += dexterityEffect.Stacks;
        }
        var frailEffect = player.Effects.FirstOrDefault(e => e.EffectType == EffectType.Frail);
        if (frailEffect != null && frailEffect.Stacks > 0)
        {
            amount = (int)(amount * 0.75);
        }
        player.AddBlock(amount);
    }
}

public class ApplyEffectCommand : ActionCommand
{
    private EffectType effectType;
    private int stacks;
    private EffectTarget target;

    public ApplyEffectCommand(EffectType effectType, int stacks, EffectTarget target)
    {
        this.effectType = effectType;
        this.stacks = stacks;
        this.target = target;
    }

    public override void Execute(Player player, Enemy enemy, Game game)
    {
        if (target == EffectTarget.Player)
        {
            player.AddEffectStack(effectType, stacks);
        }
        else if (target == EffectTarget.Enemy)
        {
            enemy.AddEffectStack(effectType, stacks);
        }
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

public class SelfDamageCommand : ActionCommand
{
    private int amount;

    public SelfDamageCommand(int amount)
    {
        this.amount = amount;
    }

    public override void Execute(Player player, Enemy enemy, Game game)
    {
        player.TakeDamage(amount);
    }
}

