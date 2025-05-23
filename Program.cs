﻿using System;
using System.Numerics;
using Raylib_cs;
namespace MyApp;
#nullable disable

internal class Program
{
    private const int ScreenWidth = 1920;
    private const int ScreenHeight = 1080;
    private const string GameTitle = "12 Hours Before Final";
    
    public enum GameScreen
    {
        TitleScreen,
        MapSelection,
        ShowReward,
        Gameplay
    }
    
    private static Game game;
    private static Player player;
    public static GameScreen currentScreen = GameScreen.TitleScreen;
    private static List<Enemy> enemies = new List<Enemy>();
    
    static void Main(string[] args)
    {
        Raylib.InitWindow(ScreenWidth, ScreenHeight, GameTitle);
        Raylib.SetTargetFPS(60);
        
        // Initialize the book colors once
        GameRenderer.InitializeBookColors();
        
        // Initialize all enemies
        enemies.Add(new Enemy("Math Textbook", 60, 60, 0, new List<Effect>(), EnemyType.Basic));
        enemies.Add(new Enemy("Physics Formula Book", 70, 70, 0, new List<Effect>(), EnemyType.Basic));
        enemies.Add(new Enemy("Chemistry Lab Manual", 80, 80, 0, new List<Effect>(), EnemyType.Elite));
        enemies.Add(new Enemy("Biology Study Guide", 90, 90, 0, new List<Effect>(), EnemyType.Elite));
        enemies.Add(new Enemy("History Textbook", 100, 100, 0, new List<Effect>(), EnemyType.Elite));
        enemies.Add(new Enemy("Final Exam", 150, 150, 0, new List<Effect>(), EnemyType.Boss));
        
        // Button state
        Rectangle startButtonRec = new Rectangle(ScreenWidth / 2 - 100, ScreenHeight / 2 + 20, 200, 50);
        bool startButtonHover = false;

        Action strikeAction = new(ActionType.Attack, 50, null, false);
        Action defendAction = new(ActionType.Block, 5, null, false);
        Action bashAttack = new(ActionType.Attack, 8, null, false);
        Action bashEffect = new(ActionType.Effect, 0, EffectType.Vulnerable, false);

        while (!Raylib.WindowShouldClose())
        {
            // Update
            switch (currentScreen)
            {
                case GameScreen.TitleScreen:
                    Vector2 mousePoint = Raylib.GetMousePosition();
                    startButtonHover = Raylib.CheckCollisionPointRec(mousePoint, startButtonRec);
                    
                    if (startButtonHover && Raylib.IsMouseButtonPressed(MouseButton.Left))
                    {
                        currentScreen = GameScreen.MapSelection;
                        var starterDeck = new List<Card>
                        {
                            new Card("Strike", "Deal 6 damage.", 1, new List<Action> { strikeAction }, CardLocation.DrawPile, 1, AffinityType.None, false),
                            new Card("Strike", "Deal 6 damage.", 1, new List<Action> { strikeAction }, CardLocation.DrawPile, 1, AffinityType.None, false),
                            new Card("Strike", "Deal 6 damage.", 1, new List<Action> { strikeAction }, CardLocation.DrawPile, 1, AffinityType.None, false),
                            new Card("Strike", "Deal 6 damage.", 1, new List<Action> { strikeAction }, CardLocation.DrawPile, 1, AffinityType.None, false),
                            new Card("Defend", "Gain 5 block.", 1, new List<Action> {defendAction}, CardLocation.DrawPile, 1, AffinityType.None, false),
                            new Card("Defend", "Gain 5 block.", 1, new List<Action> {defendAction}, CardLocation.DrawPile, 1, AffinityType.None, false),
                            new Card("Defend", "Gain 5 block.", 1, new List<Action> {defendAction}, CardLocation.DrawPile, 1, AffinityType.None, false),
                            new Card("Defend", "Gain 5 block.", 1, new List<Action> {defendAction}, CardLocation.DrawPile, 1, AffinityType.None, false),
                            new Card("Bash", "Deal 8 damage. Apply 2 Vulnerable.", 2, new List<Action> {bashAttack, bashEffect}, CardLocation.DrawPile, 2, AffinityType.None, false),
                            new Card("Bash", "Deal 8 damage. Apply 2 Vulnerable.", 2, new List<Action> {bashAttack, bashEffect}, CardLocation.DrawPile, 2, AffinityType.None, false),
                        };
                        player = new Player("Player", 100, 100, 0, 3, new List<Effect>(), 0, new List<Charm>(), starterDeck, 0.0);
                        
                        // Create a basic enemy
                        var enemy = new Enemy("Slime", 50, 50, 0, new List<Effect>(), EnemyType.Basic);
                        
                        // Initialize game first
                        game = new Game(DateTime.Now.Millisecond, player);
                        GameRenderer.InitializeGame(game); // Set the game reference in GameRenderer
                        
                        // Create a combat room with the enemy
                        var combatRoom = new Combat(false, true, true, enemy, EnemyType.Basic, TurnPhase.PlayerStart, 3);
                        game.Map.Rooms = new List<Room> { combatRoom }; // Add the combat room to the game
                    }
                    break;
                
                case GameScreen.MapSelection:
                    // Handle map selection and node click
                    int selectedNode = GameRenderer.DrawMapSelectionScreen();
                    if (selectedNode >= 0)
                    {
                        // Get the selected node's room type
                        var selectedRoomType = GameRenderer.mapGraph.Layers[GameRenderer.playerLayer][selectedNode].RoomType;
                        
                        // Get the appropriate enemy
                        Enemy enemy;
                        if (selectedRoomType == "Boss")
                        {
                            enemy = enemies[5]; // Final Exam
                        }
                        else
                        {
                            // Randomly select an enemy from the first 5 enemies (excluding boss)
                            Random random = new Random();
                            int randomIndex = random.Next(0, 5); // 0 to 4
                            enemy = enemies[randomIndex];
                        }
                        
                        // Reset enemy HP to full
                        enemy.Health = enemy.MaxHealth;
                        
                        // Create a combat room with the enemy
                        var combatRoom = new Combat(false, true, true, enemy, enemy.EnemyType, TurnPhase.PlayerStart, 3);
                        game.Map.Rooms = new List<Room> { combatRoom }; // Add the combat room to the game
                        
                        // Node selected, start gameplay
                        currentScreen = GameScreen.Gameplay;
                    }
                    break;
                case GameScreen.Gameplay:
                    // Game logic will go here
                    break;
            }
            
            // Draw
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.RayWhite);
            
            switch (currentScreen)
            {
                case GameScreen.TitleScreen:
                    // Draw title
                    Raylib.DrawText(GameTitle, ScreenWidth / 2 - Raylib.MeasureText(GameTitle, 40) / 2, 
                        ScreenHeight / 3, 40, Color.DarkBlue);
                    
                    // Draw start button
                    Raylib.DrawRectangleRec(startButtonRec, startButtonHover ? Color.SkyBlue : Color.Blue);
                    Raylib.DrawRectangleLinesEx(startButtonRec, 2, Color.DarkBlue);
                    
                    // Draw button text
                    string startText = "START";
                    Raylib.DrawText(startText, 
                        (int)(startButtonRec.X + startButtonRec.Width / 2 - Raylib.MeasureText(startText, 20) / 2),
                        (int)(startButtonRec.Y + startButtonRec.Height / 2 - 10),
                        20, Color.White);
                    
                    // Draw subtitle
                    Raylib.DrawText("Click START to begin your final exam adventure!",
                        ScreenWidth / 2 - Raylib.MeasureText("Click START to begin your final exam adventure!", 20) / 2,
                        ScreenHeight - 50, 20, Color.Gray);
                    break;
                
                case GameScreen.MapSelection:
                    GameRenderer.DrawMapOverlay();
                    break;
                case GameScreen.Gameplay:
                    GameRenderer.DrawGameplayScreen(game);
                    break;
                case GameScreen.ShowReward:
                    GameRenderer.DrawRewardScreen();
                    break;
            }
            
            Raylib.EndDrawing();
        }
        
        Raylib.CloseWindow();
    }
}
