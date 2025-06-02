using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
#nullable disable
namespace MyApp;

public class Rest : Room
{
    private string _dialog;
    private List<EventChoice> _choices;
    private bool _isChoiceMade;


    public Rest(bool isCleared, bool isAvailable, bool isCurrent) 
        : base(isCleared, isAvailable, isCurrent)
    {
        _dialog = "You find a peaceful resting spot. What would you like to do?";
        _choices = new List<EventChoice>
        {
            new EventChoice("Rest and recover (Heal 25% of max health)", healthChange: 25),
            new EventChoice("Search for valuables (Gain 150 gold)", goldReward: 150)
        };
        _isChoiceMade = false;
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
        var game = GetGame();
        if (game != null)
        {
            // Update current node and make next nodes available
            var currentNode = GameRenderer.game.Layers[GameRenderer.playerLayer][GameRenderer.playerIndex];
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
        var game = GetGame();
        if (!_isChoiceMade && game?.Player != null)
        {
            _isChoiceMade = true;
            
            // Apply the choice's effects
            if (choice.GoldReward != 0)
            {
                game.Player.AddGold(choice.GoldReward);
            }
            
            if (choice.HealthChange != 0)
            {
                // Calculate heal amount based on max health
                int healAmount = (game.Player.MaxHealth * choice.HealthChange) / 100;
                game.Player.AddHealth(healAmount);
            }

            // Mark the room as cleared
            this.IsCleared = true;
            if (game != null)
            {
                // Update current node and make next nodes available
                var currentNode = GameRenderer.game.Layers[GameRenderer.playerLayer][GameRenderer.playerIndex];
                currentNode.IsCleared = true;  // Update node state
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
    }
}
