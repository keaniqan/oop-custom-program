using System;
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
        Gameplay,
        MapSelection,
        Reward
    }
    
    private static Game game;
    private static Player player;
    public static GameScreen currentScreen = GameScreen.TitleScreen;
    
    static void Main(string[] args)
    {
        Raylib.InitWindow(ScreenWidth, ScreenHeight, GameTitle);
        Raylib.SetTargetFPS(60);
        
        // Initialize game elements
        InitializeGame();
        
        // Button state
        Rectangle startButtonRec = new Rectangle(ScreenWidth / 2 - 100, ScreenHeight / 2 + 20, 200, 50);
        bool startButtonHover = false;

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
                    }
                    break;
                
                case GameScreen.Gameplay:
                    // Game logic will go here
                    break;

                case GameScreen.MapSelection:
                    int selectedNode = GameRenderer.DrawMapSelectionScreen();
                    if (selectedNode != -1)
                    {
                        // Update the current room based on the selected node
                        if (game?.Map?.Rooms != null && selectedNode < game.Map.Rooms.Count)
                        {
                            // Move the selected room to the front of the list
                            var selectedRoom = game.Map.Rooms[selectedNode];
                            game.Map.Rooms.RemoveAt(selectedNode);
                            game.Map.Rooms.Insert(0, selectedRoom);
                            game.Map.Rooms[0].EnterRoom();
                        }
                        currentScreen = GameScreen.Gameplay;
                    }
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
                
                case GameScreen.Gameplay:
                    GameRenderer.DrawGameplayScreen(game);
                    break;
                case GameScreen.MapSelection:
                    GameRenderer.DrawMapSelectionScreen();
                    break;
                case GameScreen.Reward:
                    GameRenderer.DrawRewardScreen();
                    break;
            }
            
            Raylib.EndDrawing();
        }
        
        Raylib.CloseWindow();
    }

    private static void InitializeGame()
    {
        // Create starter deck
        var starterDeck = CreateStarterDeck();
        
        // Create player
        player = new Player("Player", 100, 100, 0, 3, new List<Effect>(), 0, new List<Charm>(), starterDeck, 0.0);
        
        // Create game instance
        game = new Game(DateTime.Now.Millisecond, player);
        
        // Initialize game renderer
        GameRenderer.InitializeGame(game);
        
        // Create map with multiple rooms
        CreateMap();
    }

    private static List<Card> CreateStarterDeck()
    {
        Action strikeAction = new(ActionType.Attack, 50, null, false);
        Action defendAction = new(ActionType.Block, 5, null, false);
        Action bashAttack = new(ActionType.Attack, 8, null, false);
        Action bashEffect = new(ActionType.Effect, 0, EffectType.Vulnerable, false);

        return new List<Card>
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
    }

    private static void CreateMap()
    {
        // Create enemies for each room
        var mathEnemy = new Enemy("Math Textbook", 60, 60, 0, new List<Effect>(), EnemyType.Basic);
        var physicsEnemy = new Enemy("Physics Formula Book", 70, 70, 0, new List<Effect>(), EnemyType.Basic);
        var chemistryEnemy = new Enemy("Chemistry Lab Manual", 80, 80, 0, new List<Effect>(), EnemyType.Elite);
        var biologyEnemy = new Enemy("Biology Study Guide", 90, 90, 0, new List<Effect>(), EnemyType.Elite);
        var historyEnemy = new Enemy("History Textbook", 100, 100, 0, new List<Effect>(), EnemyType.Elite);
        var finalExamEnemy = new Enemy("Final Exam", 150, 150, 0, new List<Effect>(), EnemyType.Boss);

        // Create rooms with enemies
        var rooms = new List<Room>
        {
            new Combat(false, true, true, mathEnemy, EnemyType.Basic, TurnPhase.PlayerStart, 3),
            new Combat(false, true, true, physicsEnemy, EnemyType.Basic, TurnPhase.PlayerStart, 3),
            new Combat(false, true, true, chemistryEnemy, EnemyType.Elite, TurnPhase.PlayerStart, 3),
            new Combat(false, true, true, biologyEnemy, EnemyType.Elite, TurnPhase.PlayerStart, 3),
            new Combat(false, true, true, historyEnemy, EnemyType.Elite, TurnPhase.PlayerStart, 3),
            new Combat(false, true, true, finalExamEnemy, EnemyType.Boss, TurnPhase.PlayerStart, 3)
        };

        // Set the rooms in the game's map
        game.Map.Rooms = rooms;
    }
}
