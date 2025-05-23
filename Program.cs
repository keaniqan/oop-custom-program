using System;
using System.Numerics;
using Raylib_cs;
namespace MyApp;
#nullable disable

internal class Program
{
    private const int ScreenWidth = 1400;
    private const int ScreenHeight = 900;
    private const string GameTitle = "12 Hours Before Final";
    
    private enum GameScreen
    {
        TitleScreen,
        Gameplay
    }
    
    static void Main(string[] args)
    {
        Raylib.InitWindow(ScreenWidth, ScreenHeight, GameTitle);
        Raylib.SetTargetFPS(60);
        
        // Initialize the book colors once
        GameRenderer.InitializeBookColors();
        
        GameScreen currentScreen = GameScreen.TitleScreen;
        
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
                
                case GameScreen.Gameplay:
                    GameRenderer.DrawGameplayScreen();
                    break;
            }
            
            Raylib.EndDrawing();
        }
        
        Raylib.CloseWindow();
    }
}
