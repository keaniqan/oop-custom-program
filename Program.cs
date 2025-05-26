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
        Action bashAttack = new(ActionType.Attack, 8, null, false);
        Action addVulnerable = new(ActionType.Effect, 1, EffectType.Vulnerable, false);
        Action addWeak = new(ActionType.Effect, 1, EffectType.Weak, false);

        Action defendAction = new(ActionType.Block, 5, null, true);
        Action addStrength = new(ActionType.Effect, 1, EffectType.StrengthUp, true);
        
        return new List<Card>
        {
            new Card("Study", "Deal 6 damage.", 1, new List<Action> { strikeAction }, CardLocation.DrawPile, 1, AffinityType.None, false),
            new Card("Study", "Deal 6 damage.", 1, new List<Action> { strikeAction }, CardLocation.DrawPile, 1, AffinityType.None, false),
            new Card("Study", "Deal 6 damage.", 1, new List<Action> { strikeAction }, CardLocation.DrawPile, 1, AffinityType.None, false),
            new Card("Memory", "Gain 5 block.", 1, new List<Action> {defendAction}, CardLocation.DrawPile, 1, AffinityType.None, false),
            new Card("Memory", "Gain 5 block.", 1, new List<Action> {defendAction}, CardLocation.DrawPile, 1, AffinityType.None, false),
            new Card("Memory", "Gain 5 block.", 1, new List<Action> {defendAction}, CardLocation.DrawPile, 1, AffinityType.None, false),
            new Card("Unthinking", "Apply 2 Weak.", 1, new List<Action> {addWeak, addWeak}, CardLocation.DrawPile, 1, AffinityType.None, false),
            new Card("Crit Thinking", "Deal 8 damage. Apply 2 Vulnerable.", 2, new List<Action> {bashAttack, addVulnerable, addStrength}, CardLocation.DrawPile, 2, AffinityType.None, false),
            new Card("New Lesson", "Gain 1 Strength.", 2, new List<Action> {addStrength}, CardLocation.DrawPile, 1, AffinityType.None, false),

        };
    }

    private class EnemyTemplate
    {
        public string Name { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public EnemyType Type { get; set; }

        public EnemyTemplate(string name, int health, int maxHealth, EnemyType type)
        {
            Name = name;
            Health = health;
            MaxHealth = maxHealth;
            Type = type;
        }

        public Enemy CreateEnemy()
        {
            return new Enemy(Name, Health, MaxHealth, 0, new List<Effect>(), Type);
        }
    }

    private static void CreateMap()
    {
        // Create enemy templates
        var enemyTemplates = new List<EnemyTemplate>
        {
            // Basic Enemies
            new EnemyTemplate("Math Textbook", 60, 60, EnemyType.Basic),
            new EnemyTemplate("Physics Formula Book", 70, 70, EnemyType.Basic),
            new EnemyTemplate("English Grammar Notes", 50, 55, EnemyType.Basic),
            new EnemyTemplate("Geography Atlas", 65, 65, EnemyType.Basic),
            new EnemyTemplate("Spelling Test Sheet", 55, 50, EnemyType.Basic),
            new EnemyTemplate("Computer Science Pseudocode", 68, 72, EnemyType.Basic),
            new EnemyTemplate("Economics Diagram Packet", 60, 58, EnemyType.Basic),

            // Elite Enemies
            new EnemyTemplate("Chemistry Lab Manual", 80, 80, EnemyType.Elite),
            new EnemyTemplate("Biology Study Guide", 90, 90, EnemyType.Elite),
            new EnemyTemplate("History Textbook", 100, 100, EnemyType.Elite),
            new EnemyTemplate("Calculus Workbook", 95, 105, EnemyType.Elite),
            new EnemyTemplate("Shakespeare Anthology", 85, 95, EnemyType.Elite),
            new EnemyTemplate("Philosophy Debate Outline", 92, 97, EnemyType.Elite),
            new EnemyTemplate("Statistics Exam Key", 88, 93, EnemyType.Elite),
        };

        // Pick a random basic enemy for the first room
        var basicEnemies = enemyTemplates.Where(t => t.Type == EnemyType.Basic).ToList();
        var rng = new Random();
        var firstRoomTemplate = basicEnemies[rng.Next(basicEnemies.Count)];
        enemyTemplates.Remove(firstRoomTemplate); // Optional: remove to avoid duplicate

        // Remove boss from shuffle pool if present
        var bossTemplate = new EnemyTemplate("Final Exam", 150, 150, EnemyType.Boss);

        // Create a list for the rest of the rooms (excluding the first and boss)
        var shufflePool = enemyTemplates.ToList();
        // Shuffle
        int n = shufflePool.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            var temp = shufflePool[k];
            shufflePool[k] = shufflePool[n];
            shufflePool[n] = temp;
        }

        // Add the boss as the last room
        shufflePool.Add(bossTemplate);
        // Insert the first room at the start
        shufflePool.Insert(0, firstRoomTemplate);

        // Create rooms from templates
        var rooms = new List<Room>();
        foreach (var template in shufflePool)
        {
            rooms.Add(new Combat(false, true, true, template.CreateEnemy(), template.Type, TurnPhase.PlayerStart, 3));
        }

        // Set the rooms in the game's map
        game.Map.Rooms = rooms;
    }
}
