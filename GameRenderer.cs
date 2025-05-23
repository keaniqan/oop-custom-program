using System;
using System.Numerics;
using Raylib_cs;
#nullable disable

namespace MyApp;
public class GameRenderer
{
    private const int ScreenWidth = 1400;
    private const int ScreenHeight = 900;
    private static Color[,,] bookColors;
    private static Vector2 playerPosition;

    public static void InitializeBookColors()
    {
        // Initialize the book colors array based on shelf layout
        int shelves = 10; // Ensure enough shelves for height
        int bookshelves = 6; // Ensure we account for 5+ bookshelves
        int booksPerShelf = 12;
        
        bookColors = new Color[bookshelves, shelves, booksPerShelf];
        
        // Generate random colors once
        for (int shelf = 0; shelf < bookshelves; shelf++)
        {
            for (int row = 0; row < shelves; row++)
            {
                for (int book = 0; book < booksPerShelf; book++)
                {
                    bookColors[shelf, row, book] = new Color(
                        Raylib.GetRandomValue(50, 200),
                        Raylib.GetRandomValue(50, 200),
                        Raylib.GetRandomValue(50, 200),
                        255
                    );
                }
            }
        }
    }

    public static void DrawPlayer()
    {
        // Position at bottom of screen, center of screen
        playerPosition = new Vector2(
            ScreenWidth / 2 - 350, 
            ScreenHeight - 350); // Moved higher up to be more visible
        
        // Head (from behind)
        Raylib.DrawCircle(
            (int)playerPosition.X,
            (int)playerPosition.Y - 150,
            60,
            Color.Beige);

        // Hair as a circle on top
        Raylib.DrawCircle(
            (int)playerPosition.X - 10,
            (int)playerPosition.Y - 160,
            60,
            new Color(70, 40, 20, 255)); // Dark brown hair

        // Rectangle on top of head
        Raylib.DrawRectangle(
            (int)playerPosition.X - 20,
            (int)playerPosition.Y - 218,
            80,
            40,
            new Color(70, 40, 20, 255)); // Dark brown hair
            
        // Shoulders and upper back
        Raylib.DrawRectangle(
            (int)playerPosition.X - 90,
            (int)playerPosition.Y - 90,
            180,
            120,
            new Color(30, 100, 180, 255)); // Blue hoodie

        // Left arm
        Raylib.DrawRectangle(
            (int)playerPosition.X - 100,
            (int)playerPosition.Y - 80,
            40,
            120,
            new Color(30, 100, 180, 255)); // blue hoodie

        // Right arm
        Raylib.DrawRectangle(
            (int)playerPosition.X + 70,
            (int)playerPosition.Y - 80,
            40,
            120,
            new Color(30, 100, 180, 255)); // blue hoodie
            
        // Hoodie details
        
        // Hood outline
        Raylib.DrawRectangle(
            (int)playerPosition.X - 65,
            (int)playerPosition.Y - 95,
            130,
            20,
            new Color(20, 80, 160, 255)); // Darker blue
            
        // Shoulder details
        Raylib.DrawLine(
            (int)playerPosition.X - 90,
            (int)playerPosition.Y - 40,
            (int)playerPosition.X + 90,
            (int)playerPosition.Y - 40,
            new Color(20, 80, 160, 255)); // Seam line
    }

    public static void DrawGameplayScreen()
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
            int shelfY = 50; // Moved up to adjust for higher floor
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
            Raylib.DrawRectangle(x - 30, 0, 60, 100, Color.LightGray);
            Raylib.DrawRectangle(x - 5, 80, 10, 30, Color.DarkGray);
        }
        
        // Add dark filter overlay to create ambient darkness
        Raylib.DrawRectangle(0, 0, ScreenWidth, ScreenHeight, new Color(0, 0, 0, 100));
        
        // Draw light sources to break the darkness
        for (int x = 200; x < ScreenWidth; x += 400)
        {
            // Draw lamp light effect (circles of light)
            DrawLightEffect(x, 70, 300, new Color(255, 255, 200, 40));
        }
        
        // Draw player character in foreground
        DrawPlayer();

        // Draw gameplay interface
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
}
