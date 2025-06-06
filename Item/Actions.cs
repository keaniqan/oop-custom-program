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
        Unit attacker, defender;
        if (target == EffectTarget.Player)
        {
            attacker = enemy;
            defender = player;
        }
        else
        {
            attacker = player;
            defender = enemy;
        }

        if (HasAndConsumeBuffer(defender)) return;

        int calculatedDamage = CalculateDamage(attacker, defender, damage);

        ApplyThornEffect(defender, attacker);

        defender.TakeDamage(calculatedDamage);

        if (defender.isDead() && game.CurrentRoom is Combat combatRoom)
        {
            combatRoom.EndCombat();
            if (defender is Enemy enemyUnit && enemyUnit.EnemyType == EnemyType.Boss)
            {
                Program.currentScreen = Program.GameScreen.Win;
            }
            if (defender is Player)
            {
                Program.currentScreen = Program.GameScreen.Lose;
            }
        }
    }

    private bool HasAndConsumeBuffer(Unit target)
    {
        var buffer = target.Effects.FirstOrDefault(e => e.EffectType == EffectType.Buffer);
        if (buffer != null && buffer.Stacks > 0)
        {
            buffer.Stacks--;
            return true;
        }
        return false;
    }

    private int CalculateDamage(Unit attacker, Unit defender, int baseDamage)
    {
        int dmg = baseDamage;
        var strength = attacker.Effects.FirstOrDefault(e => e.EffectType == EffectType.StrengthUp);
        if (strength != null && strength.Stacks != 0)
            dmg = strength.ApplyStrength(dmg);

        var weak = attacker.Effects.FirstOrDefault(e => e.EffectType == EffectType.Weak);
        if (weak != null && weak.Stacks > 0)
            dmg = weak.ApplyWeak(dmg);

        var vulnerable = defender.Effects.FirstOrDefault(e => e.EffectType == EffectType.Vulnerable);
        if (vulnerable != null && vulnerable.Stacks > 0)
            dmg = vulnerable.ApplyVulnerable(dmg);

        return dmg;
    }

    private void ApplyThornEffect(Unit defender, Unit attacker)
    {
        var thorn = defender.Effects.FirstOrDefault(e => e.EffectType == EffectType.Thorn);
        if (thorn != null && thorn.Stacks > 0)
            attacker.TakeDamage(thorn.Stacks);
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
        Unit unit = player;
        var dexterityEffect = unit.Effects.FirstOrDefault(e => e.EffectType == EffectType.DexterityUp);
        int blockAmount = amount;
        if (dexterityEffect != null && dexterityEffect.Stacks != 0)
        {
            blockAmount += dexterityEffect.Stacks;
        }
        var frailEffect = unit.Effects.FirstOrDefault(e => e.EffectType == EffectType.Frail);
        if (frailEffect != null && frailEffect.Stacks > 0)
        {
            blockAmount = (int)(blockAmount * 0.75);
        }
        player.AddBlock(blockAmount);
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
        Unit targetUnit = (target == EffectTarget.Player) ? (Unit)player : (Unit)enemy;
        targetUnit.AddEffectStack(effectType, stacks);
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

