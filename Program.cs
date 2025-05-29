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
        CharmSelection,
        MapSelection,
        Gameplay,
        Reward,
        Shop
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
        Rectangle startButtonRec = new Rectangle(ScreenWidth / 2 - 100, ScreenHeight / 2, 200, 50);
        bool startButtonHover = false;

        while (!Raylib.WindowShouldClose())
        {
            // Update
            switch (currentScreen)
            {
                case GameScreen.TitleScreen:
                    Vector2 mousePoint = Raylib.GetMousePosition();
                    mousePoint = new Vector2(mousePoint.X, mousePoint.Y + 25);
                    startButtonHover = Raylib.CheckCollisionPointRec(mousePoint, startButtonRec);
                    
                    if (startButtonHover && Raylib.IsMouseButtonPressed(MouseButton.Left))
                    {
                        currentScreen = GameScreen.CharmSelection;
                    }
                    break;
                
                case GameScreen.CharmSelection:
                    int selectedCharm = GameRenderer.DrawCharmSelectionScreen();
                    if (selectedCharm != -1)
                    {
                        // Add the selected charm to the player
                        var charm = CreateStarterCharm(selectedCharm);
                        player.AddCharm(charm);
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
                            game.Map.CurrentRoom = selectedRoom;
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
                
                case GameScreen.CharmSelection:
                    GameRenderer.DrawCharmSelectionScreen();
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
                case GameScreen.Shop:
                    GameRenderer.DrawShopScreen();
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
        player = new Player("Player", 100, 100, 0, 3, new List<Effect>(), 100, new List<Charm>(), starterDeck, 0.0);
        
        // Create game instance
        game = new Game(DateTime.Now.Millisecond, player);
        
        // Initialize game renderer
        GameRenderer.InitializeGame(game);
        
        // Create map with multiple rooms
        CreateMap();
    }

    private static List<Card> CreateStarterDeck()
    {
        Action strikeAction = new(ActionType.Attack, 100, null, false);
        Action bashAttack = new(ActionType.Attack, 8, null, false);
        Action addVulnerable = new(ActionType.Effect, 1, EffectType.Vulnerable, false);
        Action addWeak = new(ActionType.Effect, 1, EffectType.Weak, false);
        Action drawCards = new(ActionType.Draw, 2, null, true);
        Action gainEnergy = new(ActionType.Energy, 1, null, true);
        Action defendAction = new(ActionType.Block, 5, null, true);
        Action addStrength = new(ActionType.Effect, 1, EffectType.StrengthUp, true);

        return new List<Card>
        {
            new Card("Study", "Deal 6 damage.", 1, new List<Action> { strikeAction }, CardLocation.DrawPile, 1, AffinityType.None, false),
            new Card("Study", "Deal 6 damage.", 1, new List<Action> { strikeAction }, CardLocation.DrawPile, 1, AffinityType.None, false),
            new Card("Study", "Deal 6 damage.", 1, new List<Action> { strikeAction }, CardLocation.DrawPile, 1, AffinityType.None, false),
            new Card("Study", "Deal 6 damage.", 1, new List<Action> { strikeAction }, CardLocation.DrawPile, 1, AffinityType.None, false),
            new Card("Memory", "Gain 5 block.", 1, new List<Action> {defendAction}, CardLocation.DrawPile, 1, AffinityType.None, false),
            new Card("Memory", "Gain 5 block.", 1, new List<Action> {defendAction}, CardLocation.DrawPile, 1, AffinityType.None, false),
            new Card("Memory", "Gain 5 block.", 1, new List<Action> {defendAction}, CardLocation.DrawPile, 1, AffinityType.None, false),
            new Card("Memory", "Gain 5 block.", 1, new List<Action> {defendAction}, CardLocation.DrawPile, 1, AffinityType.None, false),
            new Card("Coffee Break", "Gain 1 Energy. Draw 1 card.", 100, new List<Action> { gainEnergy, drawCards }, CardLocation.DrawPile, 0, AffinityType.None, true),
            new Card("Study Guide", "Draw 2 cards. Gain 1 Energy.", 80, new List<Action> { drawCards, gainEnergy }, CardLocation.DrawPile, 1, AffinityType.None, false),
            new Card("Crit Thinking", "Deal 8 damage. Apply 2 Vulnerable.", 2, new List<Action> {bashAttack, addVulnerable, addVulnerable}, CardLocation.DrawPile, 2, AffinityType.None, false),     
        };
    }

    private static List<Card> CreateRewardCardPool()
    {
        Action strikeAction = new(ActionType.Attack, 6, null, false);
        Action bashAttack = new(ActionType.Attack, 8, null, false);
        Action addVulnerable = new(ActionType.Effect, 1, EffectType.Vulnerable, false);
        Action addWeak = new(ActionType.Effect, 1, EffectType.Weak, false);
        Action defendAction = new(ActionType.Block, 5, null, true);
        Action addStrength = new(ActionType.Effect, 1, EffectType.StrengthUp, true);
        Action addDexterity = new(ActionType.Effect, 1, EffectType.DexterityUp, true);
        Action addThorn = new(ActionType.Effect, 1, EffectType.Thorn, true);
        Action addBuffer = new(ActionType.Effect, 1, EffectType.Buffer, true);
        Action drawCards = new(ActionType.Draw, 2, null, true);
        Action gainEnergy = new(ActionType.Energy, 1, null, true);
        Action healAction = new(ActionType.Heal, 4, null, true);


        return new List<Card>
        {
            // Attack Cards
            new Card("Quick Study", "Deal 6 damage. Draw 1 card.", 70, new List<Action> { strikeAction, drawCards }, CardLocation.DrawPile, 1, AffinityType.None, false),
            new Card("Deep Analysis", "Deal 8 damage. Apply 2 Vulnerable.", 70, new List<Action> { bashAttack, addVulnerable, addVulnerable }, CardLocation.DrawPile, 2, AffinityType.None, false),
            new Card("Critical Review", "Deal 12 damage. Apply 1 Weak.", 85, new List<Action> { new Action(ActionType.Attack, 12, null, false), addWeak }, CardLocation.DrawPile, 2, AffinityType.None, false),
            new Card("Unthinking", "Apply 2 Weak.", 80, new List<Action> {addWeak, addWeak}, CardLocation.DrawPile, 1, AffinityType.None, false),

            // Defense Cards
            new Card("Mental Block", "Gain 7 block. Gain 1 Dexterity.", 90, new List<Action> { new Action(ActionType.Block, 7, null, true), addDexterity }, CardLocation.DrawPile, 1, AffinityType.None, false),
            new Card("Study Break", "Gain 8 block. Draw 1 card.", 90, new List<Action> { new Action(ActionType.Block, 8, null, true), drawCards }, CardLocation.DrawPile, 1, AffinityType.None, false),
            new Card("Last Minute Cram", "Gain 12 block. Gain 1 Frail.", 77, new List<Action> { new Action(ActionType.Block, 12, null, true), new Action(ActionType.Effect, 1, EffectType.Frail, true) }, CardLocation.DrawPile, 2, AffinityType.None, false),
            
            // Power Cards
            new Card("Study Group", "Gain 1 Strength. Draw 1 card.", 71, new List<Action> { addStrength, drawCards }, CardLocation.DrawPile, 1, AffinityType.None, true),
            new Card("Coffee Break", "Gain 1 Energy. Draw 1 card.", 100, new List<Action> { gainEnergy, drawCards }, CardLocation.DrawPile, 0, AffinityType.None, true),
            new Card("All-Nighter", "Gain 2 Strength. Gain 2 Dexterity.", 82, new List<Action> { addStrength, addStrength, addDexterity, addDexterity }, CardLocation.DrawPile, 2, AffinityType.None, true),
            new Card("New Lesson", "Gain 1 Strength.", 82, new List<Action> {addStrength}, CardLocation.DrawPile, 1, AffinityType.None, false),


            // Utility Cards
            new Card("Power Nap", "Heal 4 HP. Gain 1 Energy.", 104, new List<Action> { healAction, gainEnergy }, CardLocation.DrawPile, 1, AffinityType.None, false),
            new Card("Study Guide", "Draw 2 cards. Gain 1 Energy.", 80, new List<Action> { drawCards, gainEnergy }, CardLocation.DrawPile, 1, AffinityType.None, false),
            new Card("Defend Notes", "Gain 5 block. Gain 1 Thorn.", 91, new List<Action> { defendAction, addThorn }, CardLocation.DrawPile, 1, AffinityType.None, false),
        };
    }

    public static List<Card> GenerateRewardCards()
    {
        var rewardPool = CreateRewardCardPool();
        var random = new Random();
        var selectedCards = new List<Card>();
        
        // Select 3 random unique cards from the pool
        while (selectedCards.Count < 3 && rewardPool.Count > 0)
        {
            int index = random.Next(rewardPool.Count);
            selectedCards.Add(rewardPool[index]);
            rewardPool.RemoveAt(index);
        }
        
        return selectedCards;
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
            new EnemyTemplate("Math Textbook", 30, 30, EnemyType.Basic),
            new EnemyTemplate("Physics Formula Book", 35, 35, EnemyType.Basic),
            new EnemyTemplate("English Grammar Notes", 27, 27, EnemyType.Basic),
            new EnemyTemplate("Geography Atlas", 32, 32, EnemyType.Basic),
            new EnemyTemplate("Spelling Test Sheet", 25, 25, EnemyType.Basic),
            new EnemyTemplate("Computer Science Pseudocode", 36, 36, EnemyType.Basic),
            new EnemyTemplate("Economics Diagram Packet", 30, 30, EnemyType.Basic),

            // Elite Enemies
            new EnemyTemplate("Chemistry Lab Manual", 80, 80, EnemyType.Elite),
            new EnemyTemplate("Biology Study Guide", 90, 90, EnemyType.Elite),
            new EnemyTemplate("History Textbook", 100, 100, EnemyType.Elite),
            new EnemyTemplate("Calculus Workbook", 95, 95, EnemyType.Elite),
            new EnemyTemplate("Shakespeare Anthology", 95, 95, EnemyType.Elite),
            new EnemyTemplate("Philosophy Debate Outline", 97, 97, EnemyType.Elite),
            new EnemyTemplate("Statistics Exam Key", 93, 93, EnemyType.Elite),
        };

        // Pick a random basic enemy for the first room
        var basicEnemies = enemyTemplates.Where(t => t.Type == EnemyType.Basic).ToList();
        var rng = new Random();
        var firstRoomTemplate = basicEnemies[rng.Next(basicEnemies.Count)];
        enemyTemplates.Remove(firstRoomTemplate); // Optional: remove to avoid duplicate

        // Remove boss from shuffle pool if present
        var bossTemplate = new EnemyTemplate("Final Exam", 300, 300, EnemyType.Boss);

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

    private static Charm CreateStarterCharm(int index)
    {
        switch (index)
        {
            case 0:
                return new Charm(
                    "Study Guide",
                    "Start each combat with 1 extra energy",
                    CharmType.StudyGuide,
                    new List<Action> { new Action(ActionType.Energy, 1, null, true) },
                    0,
                    100
                );
            case 1:
                return new Charm(
                    "Coffee Mug",
                    "Start each combat with 1 extra card draw",
                    CharmType.CoffeeMug,
                    new List<Action> { new Action(ActionType.Draw, 1, null, true) },
                    0,
                    100
                );
            case 2:
                return new Charm(
                    "Lucky Pen",
                    "10% chance to draw an extra card when you draw cards",
                    CharmType.LuckyPen,
                    new List<Action> { new Action(ActionType.Draw, 1, null, true) },
                    0,
                    100
                );
            default:
                return new Charm(
                    "Study Guide",
                    "Start each combat with 1 extra energy",
                    CharmType.StudyGuide,
                    new List<Action> { new Action(ActionType.Energy, 1, null, true) },
                    0,
                    100
                );
        }
    }

    private static List<Charm> CreateShopCharmPool()
    {
        var charmPool = new List<Charm>();

        // Common Charms
        charmPool.Add(new Charm(
            "Bookmark",
            "Draw 1 extra card at the start of your turn",
            CharmType.Bookmark,
            new List<Action> { new Action(ActionType.Draw, 1, null, true) },
            0,
            100
        ));

        charmPool.Add(new Charm(
            "Highlighter",
            "Cards that cost 0 deal 2 more damage",
            CharmType.Highlighter,
            new List<Action> { new Action(ActionType.Effect, 2, EffectType.StrengthUp, true) },
            0,
            100
        ));

        charmPool.Add(new Charm(
            "Sticky Notes",
            "When you play a card, gain 1 block",
            CharmType.StickyNotes,
            new List<Action> { new Action(ActionType.Block, 1, null, true) },
            0,
            100
        ));

        // Uncommon Charms
        charmPool.Add(new Charm(
            "Study Timer",
            "Every 3 turns, gain 1 energy",
            CharmType.StudyTimer,
            new List<Action> { new Action(ActionType.Energy, 1, null, true) },
            0,
            100
        ));

        charmPool.Add(new Charm(
            "Flash Cards",
            "When you shuffle your draw pile, draw 1 card",
            CharmType.FlashCards,
            new List<Action> { new Action(ActionType.Draw, 1, null, true) },
            0,
            100
        ));

        charmPool.Add(new Charm(
            "Text Book",
            "Start each combat with 2 Strength",
            CharmType.TextBook,
            new List<Action> { new Action(ActionType.Effect, 2, EffectType.StrengthUp, true) },
            0,
            100
        ));

        charmPool.Add(new Charm(
            "Notebook",
            "Start each combat with 2 Dexterity",
            CharmType.Notebook,
            new List<Action> { new Action(ActionType.Effect, 2, EffectType.DexterityUp, true) },
            0,
            100
        ));

        // Rare Charms
        charmPool.Add(new Charm(
            "Smart Watch",
            "At the start of your turn, gain 1 energy and draw 1 card",
            CharmType.SmartWatch,
            new List<Action> { 
                new Action(ActionType.Energy, 1, null, true),
                new Action(ActionType.Draw, 1, null, true)
            },
            0,
            100
        ));

        charmPool.Add(new Charm(
            "Study Group",
            "Your first card each turn costs 0",
            CharmType.StudyGroup,
            new List<Action> { new Action(ActionType.Effect, 1, EffectType.Buffer, true) },
            0,
            100
        ));

        charmPool.Add(new Charm(
            "All-Nighter",
            "Start each combat with 2 extra energy",
            CharmType.AllNighter,
            new List<Action> { new Action(ActionType.Energy, 2, null, true) },
            0,
            100
        ));

        charmPool.Add(new Charm(
            "Genius Idea",
            "When you play a card, there's a 25% chance to play it again",
            CharmType.GeniusIdea,
            new List<Action> { new Action(ActionType.Effect, 1, EffectType.Buffer, true) },
            0,
            100
        ));

        return charmPool;
    }

    public static List<Charm> GenerateShopCharms()
    {
        var charmPool = CreateShopCharmPool();
        var random = new Random();
        var selectedCharms = new List<Charm>();
        
        // Select 3 random unique charms from the pool
        while (selectedCharms.Count < 3 && charmPool.Count > 0)
        {
            int index = random.Next(charmPool.Count);
            selectedCharms.Add(charmPool[index]);
            charmPool.RemoveAt(index);
        }
        
        return selectedCharms;
    }
}
