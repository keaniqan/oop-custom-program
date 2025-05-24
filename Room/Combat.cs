using Raylib_cs;
using System;
#nullable disable  
namespace MyApp;

public class Combat: Room
{
    private Enemy _enemy;
    private EnemyType _enemyType;
    private TurnPhase _turnPhase;
    private int _currentEnergy;
    private Random _random;
    private int _turnCount;
    private Game _game;

    public Combat(bool isCleared, bool isAvailable, bool isCurrent, Enemy enemy, EnemyType enemyType, TurnPhase turnPhase, int currentEnergy) : base(isCleared, isAvailable, isCurrent)
    {
        _enemy = enemy;
        _enemyType = enemyType;
        _turnPhase = turnPhase;
        _currentEnergy = currentEnergy;
        _random = new Random();
        _turnCount = 0;
        _game = GameRenderer.game;
        SetEnemyIntent();
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

    public TurnPhase TurnPhase
    {
        get { return _turnPhase; }
        set { _turnPhase = value; }
    }

    public int TurnCount
    {
        get { return _turnCount; }
        set { _turnCount = value; }
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
        _turnCount++;
        
        // Pattern-based intents for different enemy types
        switch (_enemyType)
        {
            case EnemyType.Basic:
                // Basic enemies alternate between attack and block
                bool shouldAttack = _turnCount % 2 == 1;
                _enemy.Intent = new Intent
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
                int pattern = _turnCount % 3;
                _enemy.Intent = new Intent
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
                pattern = _turnCount % 4;
                bool isIntenseTurn = _turnCount % 8 >= 4; // Every 4 turns, increase intensity
                _enemy.Intent = new Intent
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

    public void StartEnemyTurn()
    {
        _turnPhase = TurnPhase.EnemyStart;
        SetEnemyIntent();
    }

    public void EndEnemyTurn()
    {
        if (_game?.Map?.Player == null) return; // Safety check

        _turnPhase = TurnPhase.EnemyEnd;
        
        // Execute enemy intent
        if (_enemy.Intent._attack)
        {
            _game.Map.Player.TakeDamage(_enemy.Intent._attackValue);
        }
        else if (_enemy.Intent._block)
        {
            _enemy.Block += _enemy.Intent._blockValue;
        }
        else if (_enemy.Intent._applyBuff)
        {
            _enemy.AddEffect(new Effect(_enemy.Intent._buffType, "Buff", _enemy.Intent._buffValue, true));
        }
        else if (_enemy.Intent._debuff)
        {
            _game.Map.Player.AddEffect(new Effect(_enemy.Intent._debuffType, "Debuff", _enemy.Intent._debuffValue, false));
        }

        // Reset block at end of turn
        _game.Map.Player.Block = 0;
        
        // Set turn phase back to player's turn
        _turnPhase = TurnPhase.PlayerStart;
    }

    public override void Reward()
    {
        // TODO: Implement reward
    }
}
