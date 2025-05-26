using System;
using System.Numerics;
using Raylib_cs;
#nullable disable

namespace MyApp;
public class GameRenderer
{
    public const int ScreenWidth = 1920;
    public const int ScreenHeight = 1080;
    private static Vector2 playerPosition;
    private static Font descriptionFont;
    private static Texture2D cardTexture;
    private static Texture2D enemyTexture;
    private static int draggedCardIndex = -1; // -1 means no card is being dragged
    private static Vector2 dragOffset; // Offset from mouse position to card position
    public static Game game; // Reference to the game instance
    private static bool showMapOverlay = false;
    private static Texture2D scrollTexture;
    private static Texture2D discardPileTexture;
    private static Texture2D drawPileTexture;
    private static Texture2D playerTexture;
    private static Texture2D energyTexture;
    private static Texture2D tileTexture;
    private static Texture2D bookshelfTexture;
    private static Texture2D wallTexture;
    private static Texture2D combinedWallTexture;
    private static Texture2D buttonTexture;  // Add button texture
    // Animation properties
    private static Dictionary<int, CardAnimation> cardAnimations = new Dictionary<int, CardAnimation>();
    private const float ANIMATION_DURATION = 0.5f; // Duration in seconds

    // Add damage number animation class
    private class DamageNumberAnimation
    {
        public float StartTime;
        public Vector2 Position;
        public int Damage;
        public bool IsPlaying;
    }
    private static List<DamageNumberAnimation> damageAnimations = new List<DamageNumberAnimation>();

    private class CardAnimation
    {
        public float StartTime;
        public Vector2 StartPosition;
        public Vector2 TargetPosition;
        public float StartScale;
        public float TargetScale;
        public bool IsPlaying;
    }

    // Map node and graph structures
    internal class MapNode
    {
        public int Layer;
        public int Index;
        public string RoomType;
        public List<MapNode> Connections = new List<MapNode>();
        public int X, Y;
        public bool IsAvailable = false;
        public bool IsCurrent = false;
    }
    internal class MapGraph
    {
        public List<List<MapNode>> Layers = new List<List<MapNode>>();
    }
    internal static MapGraph mapGraph = null;
    internal static int playerLayer = 0;
    internal static int playerIndex = 0;

    public static void InitializeGame(Game gameInstance)
    {
        game = gameInstance;

        // Load the description font
        descriptionFont = Raylib.LoadFont("resources/fonts/romulus.png");
        
        // Load the card texture
        cardTexture = Raylib.LoadTexture("resources/cards/cards.png");

        // Load the enemy texture
        enemyTexture = Raylib.LoadTexture("resources/enemy/BukuAddmath.png");

        // Load the scroll texture
        scrollTexture = Raylib.LoadTexture("resources/background/scroll.png");

        // Load the discardpile texture
        discardPileTexture = Raylib.LoadTexture("resources/cards/discardpile.png");

        // Load the drawpile texture
        drawPileTexture = Raylib.LoadTexture("resources/cards/drawpile.png");

        // Load the player texture
        playerTexture = Raylib.LoadTexture("resources/player/player.png");

        // Load the energy texture
        energyTexture = Raylib.LoadTexture("resources/background/energy.png");

        // Load the tile texture for the new floor
        tileTexture = Raylib.LoadTexture("resources/background/tiles.png");

        // Load the bookshelf texture
        bookshelfTexture = Raylib.LoadTexture("resources/background/bookshelf.png");

        // Load the wall texture
        wallTexture = Raylib.LoadTexture("resources/background/wall.png");

        // Load the button texture
        buttonTexture = Raylib.LoadTexture("resources/background/button.png");

        // Combine multiple wall.png into one large wall texture
        int floorHeight = ScreenHeight / 2;
        int wallAreaHeight = ScreenHeight - floorHeight + 30;
        int wallTileW = wallTexture.Width;
        int wallTileH = wallTexture.Height;
        int tilesX = (int)Math.Ceiling((float)ScreenWidth / wallTileW);
        int tilesY = (int)Math.Ceiling((float)wallAreaHeight / wallTileH);
        RenderTexture2D wallRender = Raylib.LoadRenderTexture(ScreenWidth, wallAreaHeight);
        Raylib.BeginTextureMode(wallRender);
        Raylib.ClearBackground(new Color(0, 0, 0, 0));
        for (int y = 0; y < tilesY; y++)
        {
            for (int x = 0; x < tilesX; x++)
            {
                Raylib.DrawTexture(wallTexture, x * wallTileW, y * wallTileH, Color.White);
            }
        }
        Raylib.EndTextureMode();
        combinedWallTexture = Raylib.LoadTextureFromImage(Raylib.LoadImageFromTexture(wallRender.Texture));
        Raylib.UnloadRenderTexture(wallRender);
    }

    public static void DrawPlayer()
    {
        // Position at bottom of screen, center of screen
        playerPosition = new Vector2(
            ScreenWidth / 2 - 350, 
            ScreenHeight - 350); // Moved higher up to be more visible
        
        // Draw the player texture
        Raylib.DrawTexturePro(
            playerTexture,
            new Rectangle(0, 0, playerTexture.Width, playerTexture.Height),
            new Rectangle(playerPosition.X - 280, playerPosition.Y - 400, 800, 750),
            new Vector2(0, 0),
            0,
            Color.White
        );
    }

    public static void DrawGameplayScreen(Game game)
    {
        int floorHeight = ScreenHeight / 2;
        
        // Draw simple wall
        Raylib.DrawRectangle(0, 0, ScreenWidth, floorHeight + 30, Color.DarkGray);

        // Add decorative patterns to the wall
        int patternSize = 100;  // Size of each pattern block
        Color patternColor = new Color(60, 60, 60, 255);  // Slightly lighter than wall

        // Draw vertical lines
        for (int x = patternSize; x < ScreenWidth; x += patternSize)
        {
            Raylib.DrawLine(x, 0, x, floorHeight + 30, patternColor);
        }

        // Draw horizontal lines
        for (int y = patternSize; y < floorHeight + 30; y += patternSize)
        {
            Raylib.DrawLine(0, y, ScreenWidth, y, patternColor);
        }
        
        // Draw new tiled floor using tileTexture, transformed to 45-degree angle
        int tileW = tileTexture.Width;
        int tileH = tileTexture.Height;
        float scaleX = 0.5f;
        float scaleY = 0.35f; // Compress vertically for 45-degree look
        int drawTileW = (int)(tileW * scaleX);
        int drawTileH = (int)(tileH * scaleY);
        int floorY = ScreenHeight - floorHeight + 30;
        for (int y = floorY; y < ScreenHeight; y += drawTileH)
        {
            for (int x = 0; x < ScreenWidth; x += drawTileW)
            {
                Raylib.DrawTexturePro(
                    tileTexture,
                    new Rectangle(0, 0, tileW, tileH),
                    new Rectangle(x, y, drawTileW, drawTileH),
                    new Vector2(0, 0),
                    0,
                    Color.White
                );
            }
        }
        
        // Draw bookshelves - with specific spacing to fit 5 bookshelves
        int bookshelfWidth = 250;
        int spacing = (ScreenWidth - (5 * bookshelfWidth)) / 6; // Equal spacing between 5 bookshelves
        
        for (int i = 0; i < 5; i++) // Draw exactly 5 bookshelves
        {
            int x = spacing + i * (bookshelfWidth + spacing);
            
            // Draw bookshelf using texture
            int shelfY = floorHeight - 300; // Moved up to adjust for higher floor
            int shelfHeight = 350;
            Raylib.DrawTexturePro(
                bookshelfTexture,
                new Rectangle(0, 0, bookshelfTexture.Width, bookshelfTexture.Height),
                new Rectangle(x, shelfY, bookshelfWidth, shelfHeight),
                new Vector2(0, 0),
                0,
                Color.White
            );
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
            ScreenHeight - floorHeight - 515, 
            ScreenWidth, 
            50, 
            Color.DarkBrown);

        // Draw HP bars
        if (game.Map.Rooms.Count > 0 && game.Map.Rooms[0] is Combat combatRoom)
        {
            DrawEnemy(combatRoom);
            DrawHPBar(combatRoom.Enemy, new Vector2(ScreenWidth - 700, ScreenHeight / 2 + 100), false);
            DrawEnergyCounter(game.Map.Player.MaxEnergy, combatRoom.CurrentEnergy);
            DrawEndTurnButton(combatRoom);
        }
        
        // Draw player HP bar above the player
        DrawHPBar(game.Map.Player, new Vector2(playerPosition.X - 100, playerPosition.Y - 380), true);

        // Add Scroll Image behind the cards
        Raylib.DrawTexturePro(
            scrollTexture,
            new Rectangle(0, 0, scrollTexture.Width, scrollTexture.Height),
            new Rectangle(270, 700, scrollTexture.Width * 1.8f, scrollTexture.Height * 1.35f),
            new Vector2(0, 0),
            0,
            Color.White
        );

        // Draw the discard pile
        Raylib.DrawTexture(discardPileTexture, 1680, 800, Color.White);

        // Draw discard pile count
        int discardCount = game.Map.Player.Cards.Count(c => c.CardLocation == CardLocation.DiscardPile);
        string discardText = discardCount.ToString();
        Vector2 discardTextSize = Raylib.MeasureTextEx(descriptionFont, discardText, 30, 1);
        Raylib.DrawTextPro(
            descriptionFont,
            discardText,
            new Vector2(1680 + discardPileTexture.Width/2 - discardTextSize.X/2, 860 + discardPileTexture.Height/2 - discardTextSize.Y/2),
            new Vector2(0, 0),
            0,
            30,
            1,
            Color.White
        );

        // Draw the draw pile
        Raylib.DrawTexture(drawPileTexture, 100, 800, Color.White);

        // Draw draw pile count
        int drawCount = game.Map.Player.Cards.Count(c => c.CardLocation == CardLocation.DrawPile);
        string drawText = drawCount.ToString();
        Vector2 drawTextSize = Raylib.MeasureTextEx(descriptionFont, drawText, 30, 1);
        Raylib.DrawTextPro(
            descriptionFont,
            drawText,
            new Vector2(100 + drawPileTexture.Width/2 - drawTextSize.X/2, 860 + drawPileTexture.Height/2 - drawTextSize.Y/2),
            new Vector2(0, 0),
            0,
            30,
            1,
            Color.White
        );

        // Draw cards in player's hand
        var cardsInHand = game.Map.Player.Cards.Where(c => c.CardLocation == CardLocation.Hand).ToList();
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            var card = cardsInHand[i];
            DrawCard(i, cardsInHand.Count, card.Name, card.Description, card.CardCost);
        }

        // Handle map overlay toggle
        if (Raylib.IsKeyPressed(KeyboardKey.M))
        {
            showMapOverlay = !showMapOverlay;
        }
        if (showMapOverlay)
        {
            DrawMapOverlay();
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
        const int shadowWidth = 18;
        const int gradientSteps = 6;
        const int hoverElevation = 20;
        const int padding = 10;
        const int costBoxSize = 28;

        // Check if this card was just drawn (moved from draw pile to hand)
        var cardsInHand = game.Map.Player.Cards.Where(c => c.CardLocation == CardLocation.Hand).ToList();
        if (cardIndex < cardsInHand.Count)
        {
            var card = cardsInHand[cardIndex];
            // Only create animation if the card was just drawn (check if it's a new card in hand)
            if (!cardAnimations.ContainsKey(cardIndex) && card.CardLocation == CardLocation.Hand && card.JustDrawn)
            {
                // Create draw animation
                var animation = new CardAnimation
                {
                    StartTime = (float)Raylib.GetTime() + (cardIndex * 0.1f), // Stagger the animations
                    StartPosition = new Vector2(100, 800), // Draw pile position
                    TargetPosition = position,
                    StartScale = 0.5f,
                    TargetScale = 1.0f,
                    IsPlaying = true
                };
                cardAnimations[cardIndex] = animation;
                card.JustDrawn = false; // Mark the card as no longer newly drawn
            }
        }

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
                    // Start play animation
                    var handCards = game.Map.Player.Cards.Where(c => c.CardLocation == CardLocation.Hand).ToList();
                    if (cardIndex < handCards.Count)
                    {
                        // Create animation
                        var animation = new CardAnimation
                        {
                            StartTime = (float)Raylib.GetTime(),
                            StartPosition = position,
                            TargetPosition = new Vector2(1680, 800), // Discard pile position
                            StartScale = 1.0f,
                            TargetScale = 0.5f,
                            IsPlaying = true
                        };
                        cardAnimations[cardIndex] = animation;

                        // Play the card after animation
                        game.Map.Player.PlayCard(handCards[cardIndex]);
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

        // Handle animation
        if (cardAnimations.TryGetValue(cardIndex, out var anim) && anim.IsPlaying)
        {
            float currentTime = (float)Raylib.GetTime();
            float elapsed = currentTime - anim.StartTime;
            float progress = Math.Min(elapsed / ANIMATION_DURATION, 1.0f);

            // Ease out cubic function for smoother animation
            float easedProgress = 1 - (float)Math.Pow(1 - progress, 3);

            // Update position
            position = Vector2.Lerp(anim.StartPosition, anim.TargetPosition, easedProgress);

            // Update scale
            float scale = anim.StartScale + (anim.TargetScale - anim.StartScale) * easedProgress;

            // Draw the card texture with animation
            Raylib.DrawTexturePro(
                cardTexture,
                new Rectangle(0, 0, cardTexture.Width, cardTexture.Height),
                new Rectangle(position.X, position.Y, cardWidth * scale, cardHeight * scale),
                new Vector2(cardWidth * scale / 2, cardHeight * scale / 2),
                0,
                new Color((byte)255, (byte)255, (byte)255, (byte)(255 * (anim.TargetScale > anim.StartScale ? easedProgress : (1 - easedProgress)))) // Fade in for draw, fade out for discard
            );

            // Remove animation if complete
            if (progress >= 1.0f)
            {
                cardAnimations.Remove(cardIndex);
            }
            return; // Skip normal card drawing during animation
        }

        // Draw the card texture (normal drawing)
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
        string costText = cost.ToString();
        Vector2 costTextSize = Raylib.MeasureTextEx(descriptionFont, costText, 19, 1);
        Raylib.DrawTextPro(
            descriptionFont,
            costText,
            new Vector2(
                position.X + padding + (costBoxSize - costTextSize.X) / 2,
                position.Y + padding + (costBoxSize - costTextSize.Y) / 2
            ),
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

    public static void DrawEnemy(Combat combatRoom)
    {
        const int bookWidth = 220;
        const int bookHeight = 300;
        const int bookThickness = 40;
        Vector2 bookPos = new Vector2(ScreenWidth - 600, ScreenHeight / 2 - 100);

        // Draw book shadow
        Raylib.DrawRectangle(
            (int)bookPos.X - bookWidth/2 - bookThickness + 10,
            (int)bookPos.Y - bookHeight/2 + 10,
            bookWidth + bookThickness,
            bookHeight,
            new Color((byte)0, (byte)0, (byte)0, (byte)50)
        );

        // Draw book spine (back) with gradient
        for (int i = 0; i < bookThickness; i++)
        {
            byte shade = (byte)(100 + (i * 55 / bookThickness));
            Raylib.DrawRectangle(
                (int)bookPos.X - bookWidth/2 - bookThickness + i,
                (int)bookPos.Y - bookHeight/2,
                1,
                bookHeight,
                new Color(shade, shade, shade, (byte)255)
            );
        }

        // Draw book cover (front) with slight tilt
        Raylib.DrawTexturePro(
            enemyTexture,
            new Rectangle(0, 0, enemyTexture.Width, enemyTexture.Height),
            new Rectangle(bookPos.X - bookWidth/2, bookPos.Y - bookHeight/2, bookWidth, bookHeight),
            new Vector2(0, 0),
            0,
            Color.White
        );

        // Draw book edges with 3D effect
        // Top edge
        Raylib.DrawRectangle(
            (int)bookPos.X - bookWidth/2,
            (int)bookPos.Y - bookHeight/2,
            bookWidth,
            2,
            new Color(200, 200, 200, 255)
        );
        // Bottom edge
        Raylib.DrawRectangle(
            (int)bookPos.X - bookWidth/2,
            (int)bookPos.Y + bookHeight/2 - 2,
            bookWidth,
            2,
            new Color(50, 50, 50, 255)
        );
        // Right edge
        Raylib.DrawRectangle(
            (int)bookPos.X + bookWidth/2 - 2,
            (int)bookPos.Y - bookHeight/2,
            2,
            bookHeight,
            new Color(50, 50, 50, 255)
        );

        // Draw book spine edge
        Raylib.DrawRectangleLinesEx(
            new Rectangle(bookPos.X - bookWidth/2 - bookThickness, bookPos.Y - bookHeight/2, bookThickness, bookHeight),
            2,
            Color.Black
        );

        // Draw book pages edge with gradient
        for (int i = 0; i < 4; i++)
        {
            byte shade = (byte)(200 - (i * 40));
            Raylib.DrawRectangle(
                (int)bookPos.X - bookWidth/2 + i,
                (int)bookPos.Y - bookHeight/2,
                1,
                bookHeight,
                new Color(shade, shade, shade, (byte)255)
            );
        }

        // Draw damage numbers
        float currentTime = (float)Raylib.GetTime();
        for (int i = damageAnimations.Count - 1; i >= 0; i--)
        {
            var anim = damageAnimations[i];
            if (anim.IsPlaying)
            {
                float elapsed = currentTime - anim.StartTime;
                float progress = Math.Min(elapsed / ANIMATION_DURATION, 1.0f);
                
                // Calculate position (move up and fade out)
                float y = anim.Position.Y - (progress * 50); // Move up 50 pixels
                float alpha = 1.0f - progress; // Fade out
                
                // Draw damage number
                string damageText = anim.Damage.ToString();
                Vector2 textSize = Raylib.MeasureTextEx(descriptionFont, damageText, 40, 1);
                Raylib.DrawTextPro(
                    descriptionFont,
                    damageText,
                    new Vector2(anim.Position.X - textSize.X/2, y),
                    new Vector2(0, 0),
                    0,
                    40,
                    1,
                    new Color((byte)255, (byte)0, (byte)0, (byte)(255 * alpha))
                );

                // Remove animation if complete
                if (progress >= 1.0f)
                {
                    damageAnimations.RemoveAt(i);
                }
            }
        }

        // Draw enemy intent
        if (combatRoom.TurnPhase == TurnPhase.PlayerStart || combatRoom.TurnPhase == TurnPhase.EnemyStart || combatRoom.TurnPhase == TurnPhase.EnemyEnd)
        {
            string intentText = "";
            if (combatRoom.Enemy.Intent._attack)
                intentText = $"Attack {combatRoom.Enemy.Intent._attackValue}";
            else if (combatRoom.Enemy.Intent._block)
                intentText = $"Block {combatRoom.Enemy.Intent._blockValue}";
            else if (combatRoom.Enemy.Intent._applyBuff)
                intentText = $"Buff {combatRoom.Enemy.Intent._buffType} {combatRoom.Enemy.Intent._buffValue}";
            else if (combatRoom.Enemy.Intent._debuff)
                intentText = $"Debuff {combatRoom.Enemy.Intent._debuffType} {combatRoom.Enemy.Intent._debuffValue}";

            // Draw turn count
            string turnText = $"Turn {combatRoom.TurnCount}";
            int turnTextWidth = Raylib.MeasureText(turnText, 16);
            Raylib.DrawText(
                turnText,
                (int)ScreenWidth/2 - turnTextWidth/2 + 350,
                (int)ScreenHeight/2 - 500,
                30,
                Color.White
            );

            // Draw intent background
            int textWidth = Raylib.MeasureText(intentText, 20);
            Raylib.DrawRectangle(
                (int)bookPos.X - textWidth/2 - 10,
                (int)bookPos.Y + bookHeight/2 - 340,
                textWidth + 20,
                30,
                new Color(0, 0, 0, 150)
            );

            // Draw intent text
            Raylib.DrawText(
                intentText,
                (int)bookPos.X - textWidth/2,
                (int)bookPos.Y + bookHeight/2 - 330,
                20,
                Color.White
            );
        }
    }

    private static void DrawHPBar(Unit unit, Vector2 position, bool isPlayer)
    {
        const int barWidth = 200;
        const int barHeight = 20;
        const int padding = 5;

        // Draw background
        Raylib.DrawRectangle(
            (int)position.X,
            (int)position.Y,
            barWidth,
            barHeight,
            Color.DarkGray
        );

        // Draw health bar
        float healthPercentage = (float)unit.Health / unit.MaxHealth;
        Raylib.DrawRectangle(
            (int)position.X + padding,
            (int)position.Y + padding,
            (int)((barWidth - padding * 2) * healthPercentage),
            barHeight - padding * 2,
            Color.Red
        );

        // Draw health text
        string healthText = $"{unit.Health}/{unit.MaxHealth}";
        Raylib.DrawText(
            healthText,
            (int)position.X + barWidth + 10,
            (int)position.Y,
            20,
            Color.White
        );

        // Draw block counter if block > 0
        if (unit.Block > 0)
        {
            // Draw block icon (shield shape)
            Raylib.DrawCircle(
                (int)position.X + barWidth + 100,
                (int)position.Y + barHeight/2,
                10,
                Color.Blue
            );
            
            // Draw block value
            string blockText = unit.Block.ToString();
            Raylib.DrawText(
                blockText,
                (int)position.X + barWidth + 120,
                (int)position.Y,
                20,
                Color.White
            );
        }

        // Draw name
        string nameText = isPlayer ? "Player" : unit.Name;
        Raylib.DrawText(
            nameText,
            (int)position.X,
            (int)position.Y - 25,
            20,
            Color.White
        );

        // Draw effect stacks
        DrawEffectStacks(unit, new Vector2(position.X, position.Y + barHeight + 10));
    }

    private static void DrawEffectStacks(Unit unit, Vector2 startPosition)
    {
        const int circleRadius = 15;
        const int circleSpacing = 35;
        int currentX = (int)startPosition.X;

        // Draw each effect that has stacks
        foreach (var effect in unit.Effects)
        {
            if (effect.Stack > 0)
            {
                Color effectColor = effect.EffectType switch
                {
                    EffectType.StrengthUp => Color.Red,
                    EffectType.DexterityUp => Color.Green,
                    EffectType.Thorn => Color.Purple,
                    EffectType.Frail => Color.Orange,
                    EffectType.Vulnerable => Color.Yellow,
                    EffectType.Buffer => new Color(0, 255, 255, 255),  // Cyan
                    EffectType.Logos => Color.Magenta,
                    EffectType.Momentos => Color.Pink,
                    EffectType.Literas => new Color(173, 216, 230, 255),  // LightBlue
                    _ => Color.White
                };

                DrawEffectCircle(currentX, (int)startPosition.Y, circleRadius, effectColor, effect.Stack.ToString());
                currentX += circleSpacing;
            }
        }
    }

    private static void DrawEffectCircle(int x, int y, int radius, Color color, string text)
    {
        // Draw circle background
        Raylib.DrawCircle(x, y, radius, color);
        
        // Draw circle border
        Raylib.DrawCircleLines(x, y, radius, Color.White);
        
        // Draw text
        Vector2 textSize = Raylib.MeasureTextEx(descriptionFont, text, 16, 1);
        Raylib.DrawTextPro(
            descriptionFont,
            text,
            new Vector2(x - textSize.X/2, y - textSize.Y/2),
            new Vector2(0, 0),
            0,
            16,
            1,
            Color.White
        );
    }

    private static void DrawEnergyCounter(int maxEnergy, int currentEnergy)
    {
        // Sectioned bar settings
        const int sectionWidth = 50;
        const int sectionHeight = 50;
        const int sectionSpacing = 8;
        const int startX = 370;
        const int startY = ScreenHeight - 410;

        for (int i = 0; i < maxEnergy; i++)
        {
            int x = startX + i * (sectionWidth + sectionSpacing);
            Color fill = i < currentEnergy ? Color.White : new Color(180, 180, 180, 120); // Dimmed for empty
            Raylib.DrawTexturePro(
                energyTexture,
                new Rectangle(0, 0, energyTexture.Width, energyTexture.Height),
                new Rectangle(x, startY, sectionWidth, sectionHeight),
                new Vector2(0, 0),
                0,
                fill
            );
        }
    }

    private static void DrawEndTurnButton(Combat combatRoom)
    {
        if (game?.Map?.Player == null || combatRoom == null) return; // Safety check

        const int buttonWidth = 150;
        const int buttonHeight = 50;
        const int buttonX = ScreenWidth - buttonWidth - 200;
        const int buttonY = ScreenHeight - buttonHeight - 400;

        // Check if mouse is hovering over button
        Vector2 mousePos = Raylib.GetMousePosition();
        bool isHovering = mousePos.X >= buttonX && 
                         mousePos.X <= buttonX + buttonWidth &&
                         mousePos.Y >= buttonY - 30 && 
                         mousePos.Y <= buttonY + buttonHeight - 30;

        // Draw button using texture
        Raylib.DrawTexturePro(
            buttonTexture,
            new Rectangle(0, 0, buttonTexture.Width, buttonTexture.Height),
            new Rectangle(buttonX, buttonY, buttonWidth, buttonHeight),
            new Vector2(0, 0),
            0,
            isHovering ? Color.DarkGray : new Color(200, 200, 200, 255)  // Slightly dim when not hovering
        );

        // Draw button text
        string buttonText = "End Turn";
        int textWidth = Raylib.MeasureText(buttonText, 20);
        Raylib.DrawText(
            buttonText,
            buttonX + (buttonWidth - textWidth) / 2,
            buttonY + (buttonHeight - 20) / 2,
            20,
            Color.White
        );

        // Handle button click
        if (isHovering && Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            try
            {
                // Get all cards in hand
                var cardsInHand = game.Map.Player.Cards.Where(c => c.CardLocation == CardLocation.Hand).ToList();
                
                // Create animations for each card
                for (int i = 0; i < cardsInHand.Count; i++)
                {
                    Vector2 cardPos = CalculateCardPosition(i, cardsInHand.Count);
                    var animation = new CardAnimation
                    {
                        StartTime = (float)Raylib.GetTime() + (i * 0.1f), // Stagger the animations
                        StartPosition = new Vector2(cardPos.X + 300, cardPos.Y),
                        TargetPosition = new Vector2(1680, 800), // Discard pile position
                        StartScale = 1.0f,
                        TargetScale = 0.5f,
                        IsPlaying = true
                    };
                    cardAnimations[i] = animation;
                }

                // End turn after starting animations
                game.Map.Player.EndTurn();
                combatRoom.StartEnemyTurn();
                combatRoom.EndEnemyTurn();
                combatRoom.CurrentEnergy = game.Map.Player.MaxEnergy;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error during turn transition: {e.Message}");
            }
        }
    }

    private static void GenerateMapGraph()
    {
        int totalLayers = 12;
        int[] nodesPerLayer = new int[totalLayers];
        for (int i = 0; i < totalLayers; i++)
        {
            nodesPerLayer[i] = (i % 2 == 0) ? 2 : 3; // alternate 2,3,2,3...
        }
        nodesPerLayer[0] = 1; // Start with 1 node
        nodesPerLayer[totalLayers-1] = 1; // End with 1 node (boss)

        mapGraph = new MapGraph();
        int verticalSpacing = 70;
        int horizontalSpacing = 120;
        int startY = 120;
        int centerX = ScreenWidth / 2;

        // Create nodes
        for (int layer = 0; layer < totalLayers; layer++)
        {
            int nodes = nodesPerLayer[layer];
            int y = startY + layer * verticalSpacing;
            int totalWidth = (nodes - 1) * horizontalSpacing;
            var layerNodes = new List<MapNode>();
            for (int n = 0; n < nodes; n++)
            {
                int x = centerX - totalWidth / 2 + n * horizontalSpacing;
                string roomType;
                
                // Calculate the room index based on the map graph
                int roomIndex = 0;
                for (int l = 0; l < layer; l++)
                {
                    roomIndex += mapGraph.Layers[l].Count;
                }
                roomIndex += n;

                // Check if we have a room at this index
                if (game?.Map?.Rooms != null && roomIndex < game.Map.Rooms.Count)
                {
                    if (game.Map.Rooms[roomIndex] is Combat combatRoom)
                    {
                        switch (combatRoom.Enemy.EnemyType)
                        {
                            case EnemyType.Basic:
                                roomType = "Enemy";
                                break;
                            case EnemyType.Elite:
                                roomType = "Elite";
                                break;
                            case EnemyType.Boss:
                                roomType = "Boss";
                                break;
                            default:
                                roomType = "Enemy";
                                break;
                        }
                    }
                    else
                    {
                        roomType = "Start";
                    }
                }
                else
                {
                    // Default room types if no room exists yet
                    if (layer == 0)
                        roomType = "Start";
                    else if (layer == totalLayers-1)
                        roomType = "Boss";
                    else if (layer % 3 == 0 && n == nodes - 1)
                        roomType = "Elite";
                    else
                        roomType = "Enemy";
                }
                
                layerNodes.Add(new MapNode { Layer = layer, Index = n, RoomType = roomType, X = x, Y = y });
            }
            mapGraph.Layers.Add(layerNodes);
        }
        // Connect nodes with improved branching logic
        for (int layer = 1; layer < totalLayers; layer++)
        {
            var prevLayer = mapGraph.Layers[layer-1];
            var currLayer = mapGraph.Layers[layer];
            int prevCount = prevLayer.Count;
            int currCount = currLayer.Count;

            // Special handling for 2-to-3 and 3-to-2 transitions
            if (prevCount == 2 && currCount == 3)
            {
                // Left node connects to left and center
                prevLayer[0].Connections.Add(currLayer[0]);
                prevLayer[0].Connections.Add(currLayer[1]);
                // Right node connects to center and right
                prevLayer[1].Connections.Add(currLayer[1]);
                prevLayer[1].Connections.Add(currLayer[2]);
            }
            else if (prevCount == 3 && currCount == 2)
            {
                // Left connects to left
                prevLayer[0].Connections.Add(currLayer[0]);
                // Center connects to both
                prevLayer[1].Connections.Add(currLayer[0]);
                prevLayer[1].Connections.Add(currLayer[1]);
                // Right connects to right
                prevLayer[2].Connections.Add(currLayer[1]);
            }
            else
            {
                // For each node in prevLayer, connect to its two nearest neighbors in currLayer
                for (int p = 0; p < prevCount; p++)
                {
                    float proportional = (float)p / (prevCount - 1) * (currCount - 1);
                    int left = (int)Math.Floor(proportional);
                    int right = (int)Math.Ceiling(proportional);
                    prevLayer[p].Connections.Add(currLayer[left]);
                    if (right != left) prevLayer[p].Connections.Add(currLayer[right]);
                }
            }
            // Ensure every node in currLayer has at least one incoming connection
            foreach (var node in currLayer)
            {
                bool hasIncoming = prevLayer.Any(prev => prev.Connections.Contains(node));
                if (!hasIncoming)
                {
                    var closest = prevLayer.OrderBy(prev => Math.Abs(prev.X - node.X)).First();
                    closest.Connections.Add(node);
                }
            }
        }
        // Set starting node as available/current
        mapGraph.Layers[0][0].IsAvailable = true;
        mapGraph.Layers[0][0].IsCurrent = true;
        playerLayer = 0;
        playerIndex = 0;
    }

    public static int DrawMapSelectionScreen()
    {
        if (mapGraph == null) GenerateMapGraph();
        DrawMapOverlay();
        
        // Check all layers for available nodes
        Vector2 mouse = Raylib.GetMousePosition();
        int nodeRadius = 30;
        
        // Check each layer
        for (int layer = 0; layer < mapGraph.Layers.Count; layer++)
        {
            var currentLayer = mapGraph.Layers[layer];
            for (int i = 0; i < currentLayer.Count; i++)
            {
                var node = currentLayer[i];
                if (node.IsAvailable)
                {
                    float dx = mouse.X - node.X;
                    float dy = mouse.Y - node.Y;
                    if (dx * dx + dy * dy <= nodeRadius * nodeRadius)
                    {
                        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                        {
                            // Update current node
                            var oldCurrentNode = mapGraph.Layers[playerLayer][playerIndex];
                            oldCurrentNode.IsCurrent = false;
                            
                            // Set new current node
                            node.IsCurrent = true;
                            playerLayer = layer;
                            playerIndex = i;
                            
                            // Calculate the actual room index based on the map graph
                            int roomIndex = 0;
                            for (int l = 0; l < layer; l++)
                            {
                                roomIndex += mapGraph.Layers[l].Count;
                            }
                            roomIndex += i;
                            
                            return roomIndex;
                        }
                    }
                }
            }
        }
        return -1;
    }

    public static void DrawMapOverlay()
    {
        if (mapGraph == null) GenerateMapGraph();
        Raylib.DrawRectangle(0, 0, ScreenWidth, ScreenHeight, new Color(0, 0, 0, 180));
        int nodeRadius = 30;
        // Draw connections
        for (int layer = 0; layer < mapGraph.Layers.Count-1; layer++)
        {
            foreach (var node in mapGraph.Layers[layer])
            {
                foreach (var next in node.Connections)
                {
                    Raylib.DrawLine(node.X, node.Y, next.X, next.Y, Color.Gray);
                }
            }
        }
        // Draw nodes
        for (int layer = 0; layer < mapGraph.Layers.Count; layer++)
        {
            foreach (var node in mapGraph.Layers[layer])
            {
                Color fill = node.IsCurrent ? Color.Yellow : node.IsAvailable ? Color.LightGray : new Color(80,80,80,255);
                Raylib.DrawCircle(node.X, node.Y, nodeRadius, fill);
                Raylib.DrawCircleLines(node.X, node.Y, nodeRadius, Color.DarkGray);
                Raylib.DrawText(node.RoomType, node.X-24, node.Y-10, 18, Color.Black);
            }
        }
        // Draw close instruction
        string closeText = "Click a node to start";
        int textWidth = Raylib.MeasureText(closeText, 24);
        Raylib.DrawText(closeText, ScreenWidth/2 - textWidth/2, ScreenHeight - 60, 24, Color.White);
    }

    public static void DrawRewardScreen()
    {
        Raylib.DrawRectangle(0, 0, ScreenWidth, ScreenHeight, new Color(0, 0, 0, 180));
        Raylib.DrawText("Reward", ScreenWidth/2 - 100, ScreenHeight/2 - 50, 40, Color.White);
        Raylib.DrawText("Click anywhere to continue", ScreenWidth/2 - 200, ScreenHeight/2 + 50, 20, Color.White);
        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            Program.currentScreen = Program.GameScreen.MapSelection;
        }
    }

    // Add method to create damage number animation
    public static void CreateDamageNumber(int damage, Vector2 position)
    {
        var animation = new DamageNumberAnimation
        {
            StartTime = (float)Raylib.GetTime(),
            Position = position,
            Damage = damage,
            IsPlaying = true
        };
        damageAnimations.Add(animation);
    }
}
