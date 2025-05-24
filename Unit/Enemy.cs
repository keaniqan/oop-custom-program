using Raylib_cs;

namespace MyApp;

public class Enemy: Unit
{
    private Intent _intent;
    private AffinityType _affinityType;
    private EnemyType _enemyType;
    public Enemy(string name, int health, int maxHealth, int block, List<Effect> effects, EnemyType enemyType) : base(name, health, maxHealth, block, effects)
    {
        _enemyType = enemyType;
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

}
