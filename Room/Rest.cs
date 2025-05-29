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
    private Game _game;

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
                this.IsCleared = true;
                if (GameRenderer.game != null)
                {
                    // Update current node and make next nodes available
                    var currentNode = GameRenderer.mapGraph.Layers[GameRenderer.playerLayer][GameRenderer.playerIndex];
                    currentNode.IsCurrent = false;
                    currentNode.IsCleared = true;
                    
                    // Make all connected nodes available
                    foreach (var nextNode in currentNode.Connections)
                    {
                        nextNode.IsAvailable = true;
                    }
                }
                Program.currentScreen = Program.GameScreen.MapSelection;
            }
            
            if (choice.HealthChange != 0)
            {
                // Calculate heal amount based on max health
                int healAmount = (_game.Player.MaxHealth * choice.HealthChange) / 100;
                _game.Player.AddHealth(healAmount);
                this.IsCleared = true;
                if (GameRenderer.game != null)
                {
                    // Update current node and make next nodes available
                    var currentNode = GameRenderer.mapGraph.Layers[GameRenderer.playerLayer][GameRenderer.playerIndex];
                    currentNode.IsCurrent = false;
                    currentNode.IsCleared = true;
                    
                    // Make all connected nodes available
                    foreach (var nextNode in currentNode.Connections)
                    {
                        nextNode.IsAvailable = true;
                    }
                }
                Program.currentScreen = Program.GameScreen.MapSelection;
            }
        }
    }

    public static void DrawRestScreen()
    {
        // Draw rest background
        Raylib.DrawRectangle(0, 0, GameRenderer.ScreenWidth, GameRenderer.ScreenHeight, new Color(0, 0, 0, 180));

        // Draw dialog box
        int dialogBoxWidth = 1200;
        int dialogBoxHeight = 600;
        int dialogBoxX = (GameRenderer.ScreenWidth - dialogBoxWidth) / 2;
        int dialogBoxY = (GameRenderer.ScreenHeight - dialogBoxHeight) / 2;

        // Draw dialog box background
        Raylib.DrawRectangle(dialogBoxX, dialogBoxY, dialogBoxWidth, dialogBoxHeight, Color.White);
        Raylib.DrawRectangleLinesEx(
            new Rectangle(dialogBoxX, dialogBoxY, dialogBoxWidth, dialogBoxHeight),
            2,
            Color.DarkGray
        );

        // Draw dialog text
        string dialogText = "You find a peaceful resting spot. What would you like to do?";
        int dialogWidth = Raylib.MeasureText(dialogText, 30);
        Raylib.DrawText(
            dialogText,
            GameRenderer.ScreenWidth/2 - dialogWidth/2,
            dialogBoxY + 50,
            30,
            Color.Black
        );

        // Draw choices
        Vector2 mousePos = Raylib.GetMousePosition();
        int choiceY = dialogBoxY + 150;

        if (GameRenderer.game?.CurrentRoom is Rest restRoom)
        {
            foreach (var choice in restRoom.Choices)
            {
                // Calculate button dimensions
                const int buttonWidth = 400;
                const int buttonHeight = 60;
                const int buttonSpacing = 20;
                
                // Create button rectangle
                Rectangle buttonRect = new Rectangle(
                    GameRenderer.ScreenWidth/2 - buttonWidth/2,
                    choiceY,
                    buttonWidth,
                    buttonHeight
                );

                // Check if mouse is hovering over button
                bool isHovering = Raylib.CheckCollisionPointRec(mousePos, buttonRect);

                // Draw button background
                Color buttonColor = isHovering ? new Color(200, 200, 200, 255) : Color.White;
                Raylib.DrawRectangleRec(buttonRect, buttonColor);
                Raylib.DrawRectangleLinesEx(buttonRect, 2, Color.DarkGray);

                // Draw choice text
                string choiceText = choice.Text;
                int choiceWidth = Raylib.MeasureText(choiceText, 20);
                Raylib.DrawText(
                    choiceText,
                    (int)(buttonRect.X + (buttonWidth - choiceWidth) / 2),
                    (int)(buttonRect.Y + (buttonHeight - 20) / 2),
                    20,
                    Color.Black
                );

                // Handle button click
                if (isHovering && Raylib.IsMouseButtonPressed(MouseButton.Left))
                {
                    restRoom.MakeChoice(choice);
                }

                choiceY += buttonHeight + buttonSpacing;
            }
        }
    }
}
