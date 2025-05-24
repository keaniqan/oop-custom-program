using Raylib_cs;
#nullable disable  
namespace MyApp;

public class Combat: Room
{
    private Enemy _enemy;
    private EnemyType _enemyType;
    private TurnPhase _turnPhase;
    private int _currentEnergy;
    public Combat(bool isCleared, bool isAvailable, bool isCurrent, Enemy enemy, EnemyType enemyType, TurnPhase turnPhase, int currentEnergy) : base(isCleared, isAvailable, isCurrent)
    {
        _enemy = enemy;
        _enemyType = enemyType;
        _turnPhase = turnPhase;
        _currentEnergy = currentEnergy;
    }

    public Enemy Enemy
    {
        get { return _enemy; }
        set { _enemy = value; }
    }

    public int CurrentEnergy
    {
        get { return _currentEnergy; }
        set { _currentEnergy = value; }
    }

    public override void Reward()
    {
        // TODO: Implement reward
    }
}
