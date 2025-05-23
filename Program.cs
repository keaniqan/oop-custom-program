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
    
    // Store book colors to prevent regenerating them every frame
    private static Color[,,] bookColors;
 
    public static void GamePlayScreen()
    {
        int floorHeight = ScreenHeight / 2;
        
        // Draw wall background
        Raylib.DrawRectangle(0, 0, ScreenWidth, ScreenHeight - floorHeight + 30, Color.Beige);
        
        // Draw wooden floor - cover half the screen 
        Raylib.DrawRectangle(0, ScreenHeight - floorHeight + 30, ScreenWidth, floorHeight, Color.DarkBrown);
        
        // Draw bookshelves - with specific spacing to fit 5 bookshelves
        int bookshelfWidth = 250;
        int spacing = (ScreenWidth - (5 * bookshelfWidth)) / 6; // Equal spacing between 5 bookshelves
        
        int shelfIndex = 0;
        for (int i = 0; i < 5; i++) // Draw exactly 5 bookshelves
        {
            int x = spacing + i * (bookshelfWidth + spacing);
            
            // Bookshelf frame
            int shelfY = 50 ; // Moved up to adjust for higher floor
            int shelfHeight = ScreenHeight - floorHeight - shelfY + 30;
            Raylib.DrawRectangle(x, shelfY, bookshelfWidth, shelfHeight, Color.Brown);
            
            // Draw shelf lines
            for (int y = shelfY + 50; y < ScreenHeight - floorHeight - 50; y += 100)
            {
                Raylib.DrawRectangle(x, y, bookshelfWidth, 20, Color.DarkBrown);
            }
            
            // Draw books on each shelf
            int rowIndex = 0;
            for (int y = shelfY + 60; y < ScreenHeight - floorHeight - 50; y += 100)
            {
                int bookIndex = 0;
                for (int bookX = x + 10; bookX < x + bookshelfWidth - 10; bookX += 20)
                {
                    // Use the pre-generated book colors
                    if (shelfIndex < bookColors.GetLength(0) && 
                        rowIndex < bookColors.GetLength(1) && 
                        bookIndex < bookColors.GetLength(2))
                    {
                        Color bookColor = bookColors[shelfIndex, rowIndex, bookIndex];
                        Raylib.DrawRectangle(bookX, y - 80, 15, 80, bookColor);
                        bookIndex++;
                    }
                }
                rowIndex++;
            }
            shelfIndex++;
        }
        
        // Draw some ceiling lamps
        for (int x = 200; x < ScreenWidth; x += 400)
        {
            Raylib.DrawRectangle(x - 30, 0, 60, 0, Color.LightGray);
            Raylib.DrawRectangle(x - 5, 40, 10, 30, Color.DarkGray);
        }
        
        // Add dark filter overlay to create ambient darkness
        Raylib.DrawRectangle(0, 0, ScreenWidth, ScreenHeight, new Color(0, 0, 0, 100));
        
        // Draw light sources to break the darkness
        for (int x = 200; x < ScreenWidth; x += 400)
        {
            // Draw lamp light effect (circles of light)
            DrawLightEffect(x, 70, 300, new Color(255, 255, 200, 40));
        }
        
        // Draw player character in foreground (only showing upper torso)
        GameRenderer.DrawPlayer();
        
        // Draw a visible gray rectangle (gameplay interface)
        Raylib.DrawRectangle(
            0, 
            ScreenHeight - floorHeight + 120, 
            ScreenWidth, 
            500, 
            Color.Gray);

        Raylib.DrawRectangle(
            0, 
            ScreenHeight - floorHeight - 450, 
            ScreenWidth, 
            50, 
            Color.Gray);

    }
    
    private static void DrawLightEffect(int x, int y, int radius, Color color)
    {
        // Draw multiple layers of semi-transparent circles to create a light effect
        for (int r = radius; r > 0; r -= 100)
        {
            Raylib.DrawCircle(x, y, r, new Color(color.R, color.G, color.B, (byte)(color.A * r / radius)));
        }
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
