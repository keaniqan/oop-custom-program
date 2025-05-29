using System.Collections.Generic;
using Raylib_cs;
#nullable disable
namespace MyApp;

public class Event : Room
{
    private string _dialog;
    private List<EventChoice> _choices;
    private bool _isChoiceMade;
    private Game _game;

    public Event(bool isCleared, bool isAvailable, bool isCurrent, string dialog, List<EventChoice> choices) 
        : base(isCleared, isAvailable, isCurrent)
    {
        _dialog = dialog;
        _choices = choices;
        _isChoiceMade = false;
        _game = GameRenderer.game;
    }

    public string Dialog
    {
        get { return _dialog; }
        set { _dialog = value; }
    }

    public List<EventChoice> Choices
    {
        get { return _choices; }
        set { _choices = value; }
    }

    public bool IsChoiceMade
    {
        get { return _isChoiceMade; }
        set { _isChoiceMade = value; }
    }

    public override void EnterRoom()
    {
        base.EnterRoom();
        _isChoiceMade = false;
    }

    public override void Reward()
    {
        // Rewards are handled through the choices
        this.IsCleared = true;
        if (GameRenderer.game != null)
        {
            // Update current node and make next nodes available
            var currentNode = GameRenderer.mapGraph.Layers[GameRenderer.playerLayer][GameRenderer.playerIndex];
            currentNode.IsCurrent = false;
            
            // Make all connected nodes available
            foreach (var nextNode in currentNode.Connections)
            {
                nextNode.IsAvailable = true;
            }
            
            // Return to map selection
            Program.currentScreen = Program.GameScreen.MapSelection;
        }
    }

    public void MakeChoice(EventChoice choice)
    {
        if (!_isChoiceMade && _game?.Player != null)
        {
            _isChoiceMade = true;
            
            // Apply the choice's effects
            if (choice.GoldReward != 0)
            {
                _game.Player.AddGold(choice.GoldReward);
            }
            
            if (choice.HealthChange != 0)
            {
                _game.Player.AddHealth(choice.HealthChange);
            }

            if (choice.CardReward != null)
            {
                _game.Player.AddCard(choice.CardReward);
            }

            if (choice.CharmReward != null)
            {
                _game.Player.AddCharm(choice.CharmReward);
            }

            // Mark the room as cleared but don't return to map yet
            
        }
    }
}

public class EventChoice
{
    private string _text;
    private int _goldReward;
    private int _healthChange;
    private Card _cardReward;
    private Charm _charmReward;

    public EventChoice(string text, int goldReward = 0, int healthChange = 0, Card cardReward = null, Charm charmReward = null)
    {
        _text = text;
        _goldReward = goldReward;
        _healthChange = healthChange;
        _cardReward = cardReward;
        _charmReward = charmReward;
    }

    public string Text
    {
        get { return _text; }
        set { _text = value; }
    }

    public int GoldReward
    {
        get { return _goldReward; }
        set { _goldReward = value; }
    }

    public int HealthChange
    {
        get { return _healthChange; }
        set { _healthChange = value; }
    }

    public Card CardReward
    {
        get { return _cardReward; }
        set { _cardReward = value; }
    }

    public Charm CharmReward
    {
        get { return _charmReward; }
        set { _charmReward = value; }
    }
}
