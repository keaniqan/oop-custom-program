using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;
#nullable disable  
namespace MyApp;

public class Combat: Room
{
    private Enemy _enemy;
    private TurnPhase _turnPhase;
    private int _currentEnergy;
    private Random _random;
    private int _turnCount;
    private int _goldReward;
    private Game _game;

    public Combat(bool isCleared, bool isAvailable, bool isCurrent, Enemy enemy, EnemyType enemyType, TurnPhase turnPhase, int currentEnergy) : base(isCleared, isAvailable, isCurrent)
    {
        _enemy = enemy;
        _turnPhase = turnPhase;
        _currentEnergy = currentEnergy;
        _random = new Random();
        _turnCount = 0;
        _game = GameRenderer.game;
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
    
    public void StartEnemyTurn()
    {
        _enemy.Block = 0;
        _turnPhase = TurnPhase.EnemyStart;
        _enemy.ExecuteIntent(_game.Player);
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
        _turnCount++;
        _enemy.SetEnemyIntent();
        
        // Set turn phase back to player's turn
        _turnPhase = TurnPhase.PlayerStart;
        // Draw 5 cards at the start of player's turn
        _game.Player.DrawCards(5);

        _currentEnergy = _game.Player.MaxEnergy;
        // Trigger end of turn effects for charms
        foreach (var charm in _game.Player.Charms)
        {
            charm.OnTurnStart(_game.Player);
        }
    }

    public bool CheckCurrentEnergy(int cardCost)
    {
        if (_currentEnergy >= cardCost)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void DeductEnergy(int cardCost)
    {
        _currentEnergy -= cardCost;
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
            var currentNode = GameRenderer.game.Layers[GameRenderer.playerLayer][GameRenderer.playerIndex];
            currentNode.IsCleared = true;  // Update node state
            currentNode.IsCurrent = false;

            // Make only directly connected nodes available
            foreach (var nextNode in currentNode.Connections)
            {
                nextNode.IsAvailable = true;
            }
            
            // Show reward screen first
            Reward();
        }
        if (GameRenderer.game != null)
        {
            //GameRenderer.game.CurrentRoom.ExitRoom();
            //GameRenderer.game.CurrentRoom.SetAvailableRoom(GameRenderer.game);
        }
    }

    public void AddEnergy(int energy)
    {
        _currentEnergy += energy;
    }

    public override void EnterRoom()
    {
        base.EnterRoom();
        
        // Update game reference
        _game = GameRenderer.game;
        //Set the player effects to 0
        _game.Player.Effects.ForEach(effect => effect.Stacks = 0);

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
        }
        
        // Set initial enemy intent
        _enemy.SetEnemyIntent();

        // Trigger start of combat effects for charms
        foreach (var charm in _game.Player.Charms)
        {
            charm.OnStartOfCombat(_game.Player);
        }
    }
}
