using System;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
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
        Shop,
        Event,
        Rest
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
                        var charm = Game.CreateStarterCharm(selectedCharm);
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
                        if (game?.Rooms != null && selectedNode < game.Rooms.Count)
                        {
                            // Move the selected room to the front of the list
                            var selectedRoom = game.Rooms[selectedNode];
                            game.Rooms.RemoveAt(selectedNode);
                            game.Rooms.Insert(0, selectedRoom);
                            game.CurrentRoom = selectedRoom;
                            selectedRoom.EnterRoom(); // This will set IsCurrent to true
                        }
                        if (game.CurrentRoom is Combat)
                        {
                            currentScreen = GameScreen.Gameplay;
                        }
                        if (game.CurrentRoom is Event)
                        {
                            currentScreen = GameScreen.Event;
                        }
                        if (game.CurrentRoom is Rest)
                        {
                            currentScreen = GameScreen.Rest;
                        }
                    }
                    break;
            }
            
            // Draw
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.RayWhite);
            
            switch (currentScreen)
            {
                case GameScreen.TitleScreen:
                    GameRenderer.DrawTitleScreen();
                    break;
                case GameScreen.CharmSelection:
                    GameRenderer.DrawCharmSelectionScreen();
                    break;
                case GameScreen.MapSelection:
                    GameRenderer.DrawMapSelectionScreen();
                    break;
                case GameScreen.Gameplay:
                    GameRenderer.DrawGameplayScreen(game);
                    break;
                case GameScreen.Reward:
                    GameRenderer.DrawRewardScreen();
                    break;
                case GameScreen.Shop:
                    GameRenderer.DrawShopScreen();
                    break;
                case GameScreen.Event:
                    GameRenderer.DrawEventScreen(game);
                    break;
                case GameScreen.Rest:
                    GameRenderer.DrawRestScreen();
                    break;
            }
            
            Raylib.EndDrawing();
        }
        
        Raylib.CloseWindow();
    }

    private static void InitializeGame()
    {
        // Create starter deck
        var starterDeck = Game.CreateStarterDeck();
        
        // Create player
        player = new Player("Player", 100, 100, 0, 3, new List<Effect>(), 1000, new List<Charm>(), starterDeck, 0.0);
        
        // Create game instance
        game = new Game(player);
        
        // Create the map before initializing the renderer
        game.CreateMap();
        
        // Initialize game renderer
        GameRenderer.InitializeGame(game);
        
        // Create the map after renderer is initialized
        GameRenderer.GenerateMapGraph();
    }
}