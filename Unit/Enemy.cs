using Raylib_cs;
#nullable disable
namespace MyApp;

public class Enemy: Unit
{
    private Intent _intent;
    private AffinityType _affinityType;
    private EnemyType _enemyType;
    public Enemy(string name, int health, int maxHealth, int block, List<Effect> effects, EnemyType enemyType) : base(name, health, maxHealth, block, effects)
    {
        _enemyType = enemyType;
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

    public override void TakeDamage(int damage)
    {
        // First reduce damage by block
        if (Block > 0)
        {
            if (Block >= damage)
            {
                Block -= damage;
                damage = 0;
            }
            else
            {
                damage -= Block;
                Block = 0;
            }
        }

        // Then apply remaining damage to health
        if (damage > 0)
        {
            Health = Math.Max(0, Health - damage);
            
            // Check if enemy is defeated
            if (Health <= 0 && GameRenderer.game?.Map?.Rooms?.Count > 0 && 
                GameRenderer.game.Map.Rooms[0] is Combat combatRoom)
            {
                combatRoom.EndCombat();
            }
        }
    }
}
