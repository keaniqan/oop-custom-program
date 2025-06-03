using Raylib_cs;
using System.Numerics;
#nullable disable
namespace MyApp;

public class Enemy: Unit
{
    private Intent _intent;
    private AffinityType _affinityType;
    private EnemyType _enemyType;
    private int _textureIndex;
    private Random _random;
    public Enemy(string name, int health, int maxHealth, int block, List<Effect> effects, EnemyType enemyType, int textureIndex) : base(name, health, maxHealth, block, effects)
    {
        _enemyType = enemyType;
        _textureIndex = textureIndex;
        _random = new Random();
        // Initialize all possible effects with 0 stacks except for Buffer, Logos, Momentos, and Literas
        foreach (EffectType effectType in Enum.GetValues(typeof(EffectType)))
        {
            string description = effectType switch
            {
                EffectType.StrengthUp => "Increases attack by Strength.",
                EffectType.DexterityUp => "Increases block by Dexterity.",
                EffectType.Weak => "Deal 25% less attack damage.",
                EffectType.Thorn => "Deals Thorn damage back when hit.",
                EffectType.Frail => "Gain 25% less block from cards.",
                EffectType.Vulnerable => "Receive 50% more damage from attacks.",
                EffectType.Buffer => "Negate the next damage instance.",
                _ => effectType.ToString()
            };
            _effects.Add(new Effect(effectType, description, 0, true));
        }
    }
    public Intent Intent
    {
        get { return _intent; }
        set { _intent = value; }
    }
    public AffinityType AffinityType
    {
        get { return _affinityType; }
        set { _affinityType = value; }
    }
    public EnemyType EnemyType
    {
        get { return _enemyType; }
        set { _enemyType = value; }
    }
    public int TextureIndex
    {
        get { return _textureIndex; }
        set { _textureIndex = value; }
    }

    public void Attack(Player player, int damage)
    {
        var bufferEffect = player.Effects.FirstOrDefault(e => e.EffectType == EffectType.Buffer);
        if (bufferEffect != null && bufferEffect.Stacks > 0)
        {
            bufferEffect.Stacks--;
        }
        else
        {
            var strengthEffect = this.Effects.FirstOrDefault(e => e.EffectType == EffectType.StrengthUp);
            if (strengthEffect != null && strengthEffect.Stacks > 0)
            {
            damage = strengthEffect.ApplyStrength(damage);
            }
            var vulnerableEffect = player.Effects.FirstOrDefault(e => e.EffectType == EffectType.Vulnerable);
            if (vulnerableEffect != null && vulnerableEffect.Stacks > 0)
            {
                damage = vulnerableEffect.ApplyVulnerable(damage);
            }
            var frailEffect = this.Effects.FirstOrDefault(e => e.EffectType == EffectType.Frail);
            if (frailEffect != null && frailEffect.Stacks > 0)
            {
                damage = frailEffect.ApplyWeak(damage);
            }
            player.TakeDamage(damage);
        }
    }
    public void AddEffect(Effect effect)
    {
        this.Effects.Add(effect);
    }
        private int GetAttackValue()
    {
        switch (_enemyType)
        {
            case EnemyType.Basic:
                return _random.Next(5, 9); // 5-8 damage
            case EnemyType.Elite:
                return _random.Next(8, 13); // 8-12 damage
            case EnemyType.Boss:
                return _random.Next(12, 18); // 12-17 damage
            default:
                return 5;
        }
    }

    private int GetBlockValue()
    {
        switch (_enemyType)
        {
            case EnemyType.Basic:
                return _random.Next(4, 7); // 4-6 block
            case EnemyType.Elite:
                return _random.Next(7, 11); // 7-10 block
            case EnemyType.Boss:
                return _random.Next(10, 15); // 10-14 block
            default:
                return 4;
        }
    }

    private EffectType GetRandomEffectType()
    {
        Array values = Enum.GetValues(typeof(EffectType));
        return (EffectType)values.GetValue(_random.Next(values.Length));
    }

    public void SetEnemyIntent()
    {
        // Get turn count safely, defaulting to 1 if game or current room is not initialized
        int turnCount = 1;
        if (GameRenderer.game?.CurrentRoom is Combat combatRoom)
        {
            turnCount = combatRoom.TurnCount;
        }

        // Pattern-based intents for different enemy types
        switch (_enemyType)
        {
            case EnemyType.Basic:
                // Basic enemies alternate between attack and block
                bool shouldAttack = turnCount % 2 == 1;
                Intent = new Intent
                {
                    _attack = shouldAttack,
                    _block = !shouldAttack,
                    _applyBuff = false,
                    _debuff = false,
                    _attackValue = shouldAttack ? GetAttackValue() : 0,
                    _blockValue = !shouldAttack ? GetBlockValue() : 0
                };
                break;

            case EnemyType.Elite:
                // Elites follow a 3-turn pattern: Attack -> Block -> Buff
                int pattern = turnCount % 3;
                Intent = new Intent
                {
                    _attack = pattern == 0,
                    _block = pattern == 1,
                    _applyBuff = pattern == 2,
                    _debuff = false,
                    _attackValue = pattern == 0 ? GetAttackValue() : 0,
                    _blockValue = pattern == 1 ? GetBlockValue() : 0,
                    _buffType = pattern == 2 ? GetRandomEffectType() : EffectType.StrengthUp,
                    _buffValue = pattern == 2 ? 2 : 0
                };
                break;

            case EnemyType.Boss:
                // Bosses follow a 4-turn pattern with increasing intensity
                pattern = turnCount % 4;
                bool isIntenseTurn = turnCount % 8 >= 4; // Every 4 turns, increase intensity
                Intent = new Intent
                {
                    _attack = pattern == 0,
                    _block = pattern == 1,
                    _applyBuff = pattern == 2,
                    _debuff = pattern == 3,
                    _attackValue = pattern == 0 ? GetAttackValue() + (isIntenseTurn ? 5 : 0) : 0,
                    _blockValue = pattern == 1 ? GetBlockValue() + (isIntenseTurn ? 3 : 0) : 0,
                    _buffType = pattern == 2 ? GetRandomEffectType() : EffectType.StrengthUp,
                    _buffValue = pattern == 2 ? (isIntenseTurn ? 3 : 2) : 0,
                    _debuffType = pattern == 3 ? GetRandomEffectType() : EffectType.Vulnerable,
                    _debuffValue = pattern == 3 ? (isIntenseTurn ? 3 : 2) : 0
                };
                break;
        }
    }
    public override void EndTurn()
    {
        // Reduce stacks of vulnerable, frail, and weak effects by one
        var vulnerableEffect = _effects.FirstOrDefault(e => e.EffectType == EffectType.Vulnerable);
        if (vulnerableEffect != null && vulnerableEffect.Stacks > 0)
        {
            vulnerableEffect.Stacks--;
        }

        var frailEffect = _effects.FirstOrDefault(e => e.EffectType == EffectType.Frail);
        if (frailEffect != null && frailEffect.Stacks > 0)
        {
            frailEffect.Stacks--;
        }

        var weakEffect = _effects.FirstOrDefault(e => e.EffectType == EffectType.Weak);
        if (weakEffect != null && weakEffect.Stacks > 0)
        {
            weakEffect.Stacks--;
        }
    }

    public void ExecuteIntent(Player player)
    {
        // Execute current intent first
        if (GameRenderer.game?.Player == null) return; // Safety check
        
        if (Intent._attack)
        {
            Attack(player, Intent._attackValue);
        }
        else if (Intent._block)
        {
            AddBlock(Intent._blockValue);
        }
        else if (Intent._applyBuff)
        {
            AddEffect(new Effect(Intent._buffType, "Buff", Intent._buffValue, true));
        }
        else if (Intent._debuff)
        {
            player.AddEffect(new Effect(Intent._debuffType, "Debuff", Intent._debuffValue, false));
        }
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        GameRenderer.CreateDamageNumber(damage, new Vector2(ScreenWidth / 2 + 400, ScreenHeight - 650));
    }
}
