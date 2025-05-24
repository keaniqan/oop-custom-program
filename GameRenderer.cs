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
    private static Texture2D enemyTexture;
    private static int draggedCardIndex = -1; // -1 means no card is being dragged
    private static Vector2 dragOffset; // Offset from mouse position to card position
    public static Game game; // Reference to the game instance
    private static bool showMapOverlay = false;

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

        // Load the enemy texture
        enemyTexture = Raylib.LoadTexture("resources/enemy/BukuAddmath.png");
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

        // Draw HP bars
        DrawHPBar(game.Map.Player, new Vector2(100, 100), true);
        if (game.Map.Rooms.Count > 0 && game.Map.Rooms[0] is Combat combatRoom)
        {
            DrawEnemy(combatRoom);
            DrawHPBar(combatRoom.Enemy, new Vector2(ScreenWidth - 700, ScreenHeight / 2 + 100), false);
            DrawEnergyCounter(game.Map.Player.MaxEnergy, combatRoom.CurrentEnergy);
            DrawEndTurnButton(combatRoom);
        }

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

    private static void DrawEnemy(Combat combatRoom)
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

        // Draw name
        string nameText = isPlayer ? "Player" : unit.Name;
        Raylib.DrawText(
            nameText,
            (int)position.X,
            (int)position.Y - 25,
            20,
            Color.White
        );
    }

    private static void DrawEnergyCounter(int maxEnergy, int currentEnergy)
    {
        const int orbSize = 120;
        const int startX = 200;
        const int startY = ScreenHeight - 450;

        // Draw outer ring
        Raylib.DrawCircleLines(
            startX + orbSize/2,
            startY,
            orbSize/2 + 5,
            Color.Gold
        );

        // Draw inner glow
        Raylib.DrawCircle(
            startX + orbSize/2,
            startY,
            orbSize/2 - 10,
            new Color(255, 255, 200, 100)
        );

        // Draw main energy orb
        Raylib.DrawCircle(
            startX + orbSize/2,
            startY,
            orbSize/2,
            Color.Yellow
        );

        // Draw decorative lines
        for (int i = 0; i < 8; i++)
        {
            float angle = i * (float)(Math.PI / 4);
            float x1 = startX + orbSize/2 + (float)Math.Cos(angle) * (orbSize/2 - 15);
            float y1 = startY + (float)Math.Sin(angle) * (orbSize/2 - 15);
            float x2 = startX + orbSize/2 + (float)Math.Cos(angle) * (orbSize/2 + 5);
            float y2 = startY + (float)Math.Sin(angle) * (orbSize/2 + 5);
            Raylib.DrawLine((int)x1, (int)y1, (int)x2, (int)y2, Color.Gold);
        }

        // Draw energy text
        string energyText = $"{currentEnergy}/{maxEnergy}";
        int textWidth = Raylib.MeasureText(energyText, 40);
        Raylib.DrawText(
            energyText,
            startX + orbSize/2 - textWidth/2,
            startY - 20,
            40,
            Color.Black
        );
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
                         mousePos.Y >= buttonY && 
                         mousePos.Y <= buttonY + buttonHeight;

        // Draw button background
        Raylib.DrawRectangle(
            buttonX,
            buttonY,
            buttonWidth,
            buttonHeight,
            isHovering ? Color.SkyBlue : Color.Blue
        );

        // Draw button border
        Raylib.DrawRectangleLinesEx(
            new Rectangle(buttonX, buttonY, buttonWidth, buttonHeight),
            2,
            Color.DarkBlue
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
        //int nodeRadius = 30;
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
                string roomType = (layer == 0) ? "Start" : (layer == totalLayers-1) ? "Boss" : (n % 3 == 0 ? "Elite" : (n % 2 == 0 ? "Combat" : "Event"));
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
            // 1. Proportional connections (prev to curr)
            for (int n = 0; n < currCount; n++)
            {
                float prevPos = (float)n / (currCount - 1) * (prevCount - 1);
                int left = (int)Math.Floor(prevPos);
                int right = (int)Math.Ceiling(prevPos);
                prevLayer[left].Connections.Add(currLayer[n]);
                if (right != left) prevLayer[right].Connections.Add(currLayer[n]);
            }
            // 2. Ensure every node in currLayer has at least one incoming connection
            foreach (var node in currLayer)
            {
                bool hasIncoming = prevLayer.Any(prev => prev.Connections.Contains(node));
                if (!hasIncoming)
                {
                    // Connect to the closest node in prevLayer
                    var closest = prevLayer.OrderBy(prev => Math.Abs(prev.X - node.X)).First();
                    closest.Connections.Add(node);
                }
            }
            // 3. Ensure every node in prevLayer has at least one outgoing connection
            foreach (var prev in prevLayer)
            {
                if (!prev.Connections.Any())
                {
                    // Connect to the closest node in currLayer
                    var closest = currLayer.OrderBy(curr => Math.Abs(curr.X - prev.X)).First();
                    prev.Connections.Add(closest);
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
        // Only allow selection of available nodes in the first layer (start)
        var layer = mapGraph.Layers[0];
        Vector2 mouse = Raylib.GetMousePosition();
        int nodeRadius = 30;
        for (int i = 0; i < layer.Count; i++)
        {
            var node = layer[i];
            if (node.IsAvailable)
            {
                float dx = mouse.X - node.X;
                float dy = mouse.Y - node.Y;
                if (dx * dx + dy * dy <= nodeRadius * nodeRadius)
                {
                    if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                    {
                        node.IsCurrent = true;
                        return i;
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

    public static void DrawEndCombatScreen()
    {
        Raylib.DrawRectangle(0, 0, ScreenWidth, ScreenHeight, new Color(0, 0, 0, 180));
        Raylib.DrawText("Combat Ended", ScreenWidth/2 - 100, ScreenHeight/2 - 50, 40, Color.White);
    }
}
