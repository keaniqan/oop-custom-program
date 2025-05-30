using Raylib_cs;
#nullable disable
namespace MyApp;

public class Enemy: Unit
{
    private Intent _intent;
    private AffinityType _affinityType;
    private EnemyType _enemyType;
    private int _textureIndex;
    public Enemy(string name, int health, int maxHealth, int block, List<Effect> effects, EnemyType enemyType, int textureIndex) : base(name, health, maxHealth, block, effects)
    {
        _enemyType = enemyType;
        _textureIndex = textureIndex;
        // Initialize all possible effects with 0 stacks except for Buffer, Logos, Momentos, and Literas
        foreach (EffectType effectType in Enum.GetValues(typeof(EffectType)))
        {
            if (effectType != EffectType.Buffer && 
                effectType != EffectType.Logos && 
                effectType != EffectType.Momentos && 
                effectType != EffectType.Literas)
            {
                _effects.Add(new Effect(effectType, effectType.ToString(), 0, true));
            }
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
        player.TakeDamage(damage);
    }
    public void AddEffect(Effect effect)
    {
        this.Effects.Add(effect);
    }
    public void Buff(Effect effect)
    {
        this.AddEffect(effect);
    }
    public void Debuff(Effect effect)
    {
        this.AddEffect(effect);
    }

    public override void EndTurn()
    {
        // Reduce stacks of vulnerable, frail, and weak effects by one
        var vulnerableEffect = _effects.FirstOrDefault(e => e.EffectType == EffectType.Vulnerable);
        if (vulnerableEffect != null && vulnerableEffect.Stack > 0)
        {
            vulnerableEffect.Stack--;
        }

        var frailEffect = _effects.FirstOrDefault(e => e.EffectType == EffectType.Frail);
        if (frailEffect != null && frailEffect.Stack > 0)
        {
            frailEffect.Stack--;
        }

        var weakEffect = _effects.FirstOrDefault(e => e.EffectType == EffectType.Weak);
        if (weakEffect != null && weakEffect.Stack > 0)
        {
            weakEffect.Stack--;
        }
    }
}
