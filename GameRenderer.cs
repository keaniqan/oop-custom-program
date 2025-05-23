using System;
using System.Numerics;
using Raylib_cs;
#nullable disable

namespace MyApp;
public class GameRenderer
{
    private const int ScreenWidth = 1920;
    private const int ScreenHeight = 1080;
    private static Color[,,] bookColors;
    private static Vector2 playerPosition;
    private static Font descriptionFont;
    private static Texture2D cardTexture;
    private static int draggedCardIndex = -1; // -1 means no card is being dragged
    private static Vector2 dragOffset; // Offset from mouse position to card position
    private static Game game; // Reference to the game instance

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

        // Load the description font
        descriptionFont = Raylib.LoadFont("resources/fonts/romulus.png");
        
        // Load the card texture
        cardTexture = Raylib.LoadTexture("resources/cards/cards.png");
    }

    public static void InitializeGame(Game gameInstance)
    {
        game = gameInstance;
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

    public static void DrawGameplayScreen(Game game)
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
            ScreenHeight - floorHeight + 200, 
            ScreenWidth, 
            500, 
            Color.Gray);

        Raylib.DrawRectangle(
            0, 
            ScreenHeight - floorHeight - 515, 
            ScreenWidth, 
            50, 
            Color.Gray);

        // Draw cards in player's hand
        var cardsInHand = game.Map.Player.Cards.Where(c => c.CardLocation == CardLocation.Hand).ToList();
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            var card = cardsInHand[i];
            DrawCard(i, cardsInHand.Count, card.Name, card.Description, card.CardCost);
        }
    }

    private static void DrawLightEffect(int x, int y, int radius, Color color)
    {
        // Draw multiple layers of semi-transparent circles to create a light effect
        for (int r = radius; r > 0; r -= 100)
        {
            Raylib.DrawCircle(x, y, r, new Color(color.R, color.G, color.B, (byte)(color.A * r / radius)));
        }
    }

    private static Vector2 CalculateCardPosition(int cardIndex, int totalCards)
    {
        const int cardWidth = 140;
        const int cardHeight = 209;
        const int cardSpacing = 22;
        const int bottomMargin = 55;
        const int stackOffset = 30; // How much each card overlaps
        const int verticalOffset = 0; // Move cards down by 50 pixels
        const int horizontalOffset = 100; // Move cards right by 50 pixels

        // Safety check for single card
        if (totalCards <= 1)
        {
            return new Vector2(
                (ScreenWidth - cardWidth) / 2 + horizontalOffset,
                ScreenHeight - cardHeight - bottomMargin + verticalOffset
            );
        }

        // Calculate total width of all cards including spacing
        float totalWidth = (cardWidth * totalCards) + (cardSpacing * (totalCards - 1));
        
        // Calculate starting X position to center all cards
        float startX = (ScreenWidth - totalWidth) / 2 + horizontalOffset;
        
        // Calculate base Y position (bottom of screen minus card height and margin)
        float baseY = ScreenHeight - cardHeight - bottomMargin + verticalOffset;
        
        // Calculate X position for this specific card with stack offset
        float x = startX + (cardIndex * (cardWidth + cardSpacing - stackOffset));
        
        // Calculate center elevation effect (cards in center are higher)
        float progress = (float)cardIndex / (totalCards - 1);
        float centerProgress = Math.Abs(progress - 0.5f) * 2; // 0 at center, 1 at edges
        float elevationOffset = (1 - centerProgress); // Higher in center
        
        // Add extra lowering for outer cards
        float outerOffset = centerProgress; // More lowering at edges
        
        float y = baseY - elevationOffset + outerOffset;

        return new Vector2(x, y);
    }

    public static void DrawCard(int cardIndex, int totalCards, string cardName, string description, int cost)
    {
        // Safety check for invalid card index
        if (cardIndex < 0 || cardIndex >= totalCards)
        {
            return;
        }

        Vector2 position = CalculateCardPosition(cardIndex, totalCards);
        const int cardWidth = 140;
        const int cardHeight = 209;
        const int shadowWidth = 18; // Width of the shadow overlay
        const int gradientSteps = 6; // Number of steps in the gradient
        const int hoverElevation = 20; // How much the card elevates on hover
        const int padding = 10;
        const int costBoxSize = 28;

        // Check if mouse is hovering over this card
        Vector2 mousePos = Raylib.GetMousePosition();
        bool isHovering = mousePos.X >= position.X && 
                         mousePos.X <= position.X + cardWidth &&
                         mousePos.Y >= position.Y && 
                         mousePos.Y <= position.Y + cardHeight;

        // Handle drag start
        if (isHovering && Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            draggedCardIndex = cardIndex;
            dragOffset = new Vector2(position.X - mousePos.X, position.Y - mousePos.Y);
        }

        // Update position if this card is being dragged
        if (draggedCardIndex == cardIndex)
        {
            position = new Vector2(mousePos.X + dragOffset.X, mousePos.Y + dragOffset.Y);
            
            // Stop dragging if mouse button is released
            if (Raylib.IsMouseButtonReleased(MouseButton.Left))
            {
                // Check if card was released in upper half of screen
                if (mousePos.Y < ScreenHeight / 2)
                {
                    // Play the card
                    var cardsInHand = game.Map.Player.Cards.Where(c => c.CardLocation == CardLocation.Hand).ToList();
                    if (cardIndex < cardsInHand.Count)
                    {
                        game.Map.Player.PlayCard(cardsInHand[cardIndex]);
                    }
                }
                draggedCardIndex = -1;
            }
        }
        // Apply hover effect if not being dragged
        else if (isHovering)
        {
            position.Y -= hoverElevation;
        }

        // Draw the card texture
        Raylib.DrawTexturePro(
            cardTexture,
            new Rectangle(0, 0, cardTexture.Width, cardTexture.Height),
            new Rectangle(position.X, position.Y, cardWidth, cardHeight),
            new Vector2(0, 0),
            0,
            Color.White
        );

        // Draw cost box
        Raylib.DrawRectangle(
            (int)position.X + padding,
            (int)position.Y + padding,
            costBoxSize,
            costBoxSize,
            Color.Red
        );

        // Draw cost number
        Raylib.DrawTextPro(
            descriptionFont,
            cost.ToString(),
            new Vector2(position.X + padding + costBoxSize/2 - 6, position.Y + padding + costBoxSize/2 - 9),
            new Vector2(0, 0),
            0,
            19,
            1,
            Color.White
        );

        // Draw card name
        Raylib.DrawTextPro(
            descriptionFont,
            cardName,
            new Vector2(position.X + costBoxSize + padding * 2, position.Y + padding),
            new Vector2(0, 0),
            0,
            20,
            1,
            Color.Black
        );

        // Draw description
        string[] words = description.Split(' ');
        string currentLine = "";
        int lineY = (int)position.Y + 46;
        int maxWidth = cardWidth - (padding * 2);

        foreach (string word in words)
        {
            string testLine = currentLine + word + " ";
            int textWidth = (int)Raylib.MeasureTextEx(descriptionFont, testLine, 20, 1).X;
            
            if (textWidth > maxWidth)
            {
                Raylib.DrawTextPro(
                    descriptionFont,
                    currentLine,
                    new Vector2(position.X + padding, lineY),
                    new Vector2(0, 0),
                    0,
                    20,
                    1,
                    Color.Black
                );
                currentLine = word + " ";
                lineY += 18;
            }
            else
            {
                currentLine = testLine;
            }
        }
        
        // Draw the last line
        Raylib.DrawTextPro(
            descriptionFont,
            currentLine,
            new Vector2(position.X + padding, lineY),
            new Vector2(0, 0),
            0,
            20,
            1,
            Color.Black
        );
        
        // Draw gradient shadow overlay (skip for rightmost card, hovered card, or dragged card)
        if (cardIndex < totalCards - 1 && !isHovering && draggedCardIndex != cardIndex)
        {
            int stepWidth = shadowWidth / gradientSteps;
            for (int i = 0; i < gradientSteps; i++)
            {
                byte alpha = (byte)(100 - (i * (100 / gradientSteps))); // Decrease opacity for each step
                Raylib.DrawRectangle(
                    (int)position.X + cardWidth - shadowWidth + (i * stepWidth),
                    (int)position.Y,
                    stepWidth,
                    cardHeight,
                    new Color((byte)0, (byte)0, (byte)0, alpha)
                );
            }
        }
    }
}
