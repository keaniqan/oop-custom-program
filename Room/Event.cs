using System.Collections.Generic;
using Raylib_cs;
#nullable disable
namespace MyApp;

public class Event : Room
{
    private string _dialog;
    private List<EventChoice> _choices;
    private bool _isChoiceMade;

    public Event(bool isCleared, bool isAvailable, bool isCurrent, string dialog, List<EventChoice> choices) 
        : base(isCleared, isAvailable, isCurrent)
    {
        _dialog = dialog;
        _choices = choices;
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
            currentNode.IsCleared = true;  // Update node state
            currentNode.IsCurrent = false;
                
            
            // Make all connected nodes available
            foreach (var nextNode in currentNode.Connections)
            {
                nextNode.IsAvailable = true;
            }
            
            // Show the event screen
            Program.currentScreen = Program.GameScreen.Event;
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
                game.Player.Heal(choice.HealthChange);
            }

            if (choice.CardReward != null)
            {
                game.Player.AddCard(choice.CardReward);
            }

            if (choice.CharmReward != null)
            {
                game.Player.AddCharm(choice.CharmReward);
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
                
                // Show the event screen (which will show the reward screen)
                Program.currentScreen = Program.GameScreen.Event;
            }
        }
    }
}
