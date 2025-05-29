using Raylib_cs;
using System;
using System.Collections.Generic;
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
    private int _goldReward;
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

        // Ensure all cards are in draw pile first
        if (_game?.Player != null)
        {
            foreach (var card in _game.Player.Cards)
            {
                card.CardLocation = CardLocation.DrawPile;
            }
            // Shuffle the deck
            _game.Player.ShuffleDeck();
            // Draw initial hand
            _game.Player.DrawCards(5);
        }

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

    public int GoldReward
    {
        get { return _goldReward; }
        set { _goldReward = value; }
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
        _enemy.Block = 0;
        _turnPhase = TurnPhase.EnemyStart;
        
        // Execute current intent first
        if (_game?.Player == null) return; // Safety check
        
        if (_enemy.Intent._attack)
        {
            _game.Player.TakeDamage(_enemy.Intent._attackValue);
        }
        else if (_enemy.Intent._block)
        {
            _enemy.AddBlock(_enemy.Intent._blockValue);
        }
        else if (_enemy.Intent._applyBuff)
        {
            _enemy.AddEffect(new Effect(_enemy.Intent._buffType, "Buff", _enemy.Intent._buffValue, true));
        }
        else if (_enemy.Intent._debuff)
        {
            _game.Player.AddEffect(new Effect(_enemy.Intent._debuffType, "Debuff", _enemy.Intent._debuffValue, false));
        }
    }

    public void EndEnemyTurn()
    {
        if (_game?.Player == null) return; // Safety check

        _turnPhase = TurnPhase.EnemyEnd;
        
        // Reset both player and enemy block at end of turn
        _game.Player.Block = 0;
        
        // Reduce enemy effect stacks
        _enemy.EndTurn();
        
        // Set intent for next turn
        SetEnemyIntent();
        
        // Set turn phase back to player's turn
        _turnPhase = TurnPhase.PlayerStart;
        // Draw 5 cards at the start of player's turn
        _game.Player.DrawCards(5);
    }

    public override void Reward()
    {
        if (_enemy.EnemyType == EnemyType.Basic)
        {
            _goldReward = _random.Next(75, 125);
        }
        else if (_enemy.EnemyType == EnemyType.Elite)
        {
            _goldReward = _random.Next(125, 225);
        }
        else if (_enemy.EnemyType == EnemyType.Boss)
        {
            _goldReward = _random.Next(225, 325);
        }
        _game.Player.AddGold(_goldReward);
        Program.currentScreen = Program.GameScreen.Reward;
    }

    public void EndCombat()
    {
        this.IsCleared = true;
        if (GameRenderer.game != null)
        {
            // Update current node and make next nodes available
            var currentNode = GameRenderer.mapGraph.Layers[GameRenderer.playerLayer][GameRenderer.playerIndex];
            currentNode.IsCurrent = false;
            currentNode.IsCleared = true;  // Mark the node as cleared
            
            // Make only directly connected nodes available
            foreach (var nextNode in currentNode.Connections)
            {
                nextNode.IsAvailable = true;
            }
            
            // Show reward screen first
            Reward();
        }
    }

    public void AddEnergy(int energy)
    {
        _currentEnergy += energy;
    }

    public override void EnterRoom()
    {
        base.EnterRoom();
        
        // Reset turn count and energy first
        _turnCount = 0;
        _currentEnergy = _game?.Player?.MaxEnergy ?? 3;
        
        // Ensure all cards are in draw pile first
        if (_game?.Player != null)
        {
            foreach (var card in _game.Player.Cards)
            {
                card.CardLocation = CardLocation.DrawPile;
            }
            // Shuffle the deck
            _game.Player.ShuffleDeck();
            // Draw initial hand
            _game.Player.DrawCards(5);

            // Trigger start of combat effects for charms
            _game.Player.TriggerStartOfCombat();
        }
        
        // Set initial enemy intent
        SetEnemyIntent();
    }
}
