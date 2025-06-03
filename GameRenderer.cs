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
    private static Font cardNameFont;  // Add new font field for card names
    private static Texture2D cardTexture;
    private static Texture2D enemyTexture;
    private static Texture2D enemyTexture1;
    private static Texture2D enemyTexture2;
    private static Texture2D enemyTexture3;
    private static Texture2D enemyTexture4;
    private static Texture2D enemyTexture5;
    private static Texture2D enemyTexture6;
    private static Texture2D enemyTexture7;
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
    private static Texture2D vulnerableTexture;  // Add vulnerable texture
    private static Texture2D bufferTexture;     // Add buffer texture
    private static Texture2D dexterityTexture;  // Add dexterity texture
    private static Texture2D frailTexture;      // Add frail texture
    private static Texture2D strengthTexture;   // Add strength texture
    private static Texture2D thornTexture;      // Add thorn texture
    private static Texture2D weakTexture;       // Add weak texture
    private static Texture2D rewardBackground;     // Add energy texture
    private static Texture2D shopBackground;    // Add shop background texture
    private static Texture2D studyGuideTexture;    // Add study guide charm texture
    private static Texture2D coffeeMugTexture;     // Add coffee mug charm texture
    private static Texture2D luckyPenTexture;      // Add lucky pen charm texture
    private static Texture2D bookmarkTexture;      // Add bookmark charm texture
    private static Texture2D calculatorTexture;    // Add calculator charm texture
    private static Texture2D highlighterTexture;   // Add highlighter charm texture
    private static Texture2D stickyNotesTexture;   // Add sticky notes charm texture
    private static Texture2D studyTimerTexture;    // Add study timer charm texture
    private static Texture2D flashCardsTexture;    // Add flash cards charm texture
    private static Texture2D textBookTexture;      // Add text book charm texture
    private static Texture2D notebookTexture;      // Add notebook charm texture
    private static Texture2D smartWatchTexture;    // Add smart watch charm texture
    private static Texture2D studyGroupTexture;    // Add study group charm texture
    private static Texture2D allNighterTexture;    // Add all-nighter charm texture
    private static Texture2D geniusIdeaTexture;    // Add genius idea charm texture
    private static Texture2D eventBackground;    // Add event background texture
    private static Texture2D restBackground;    // Add rest background texture
    private static Texture2D mainMenuBackground;
    private static Texture2D charmBackground;
    private static Texture2D mapScrollTexture;
    private static Texture2D mapBackground;
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
    // public class MapGraph
    // {
    //     public List<List<Room>> Layers = new List<List<Room>>();
    // }
    internal static int playerLayer = 0;
    internal static int playerIndex = 0;

    private static List<Card> rewardCards = new List<Card>();
    private static bool rewardCardsGenerated = false;

    private static bool showDrawPileOverlay = false;
    private static bool showDiscardPileOverlay = false;
    private static float lastClickTime = 0;
    private const float CLICK_DELAY = 0.2f; // 200ms delay between clicks

    // Shop-related fields
    private static List<Card> shopCards = new List<Card>();
    private static List<Charm> shopCharms = new List<Charm>();
    private static bool shopCardsGenerated = false;

    private static bool showEventRewardScreen = false;
    private static EventChoice lastEventChoice = null;

    private static bool shopVisitedThisReward = false;

    private static float lastRoomEntryTime = 0;
    private const float INPUT_BUFFER_TIME = 0.5f; // 0.5 seconds buffer

    public static void InitializeGame(Game gameInstance)
    {
        game = gameInstance;
        
        // Load the description font
        descriptionFont = Raylib.LoadFont("resources/fonts/alagard.png");
        
        // Load the card name font
        cardNameFont = Raylib.LoadFont("resources/fonts/alagard.png");
        
        // Load the card texture
        cardTexture = Raylib.LoadTexture("resources/cards/cards.png");
        // Load the Main Menu Background
        mainMenuBackground = Raylib.LoadTexture("resources/background/mainmenu.png");
        // Load the enemy texture
        enemyTexture1 = Raylib.LoadTexture("resources/enemy/book1.png");
        enemyTexture2 = Raylib.LoadTexture("resources/enemy/book2.png");
        enemyTexture3 = Raylib.LoadTexture("resources/enemy/book3.png");
        enemyTexture4 = Raylib.LoadTexture("resources/enemy/book4.png");
        enemyTexture5 = Raylib.LoadTexture("resources/enemy/book5.png");
        enemyTexture6 = Raylib.LoadTexture("resources/enemy/book6.png");
        enemyTexture7 = Raylib.LoadTexture("resources/enemy/book7.png");


        // Load the scroll texture
        scrollTexture = Raylib.LoadTexture("resources/background/scroll.png");

        // Load the event background texture
        eventBackground = Raylib.LoadTexture("resources/background/event.png");

        // Load the discardpile texture
        discardPileTexture = Raylib.LoadTexture("resources/cards/discardpile.png");

        // Load the drawpile texture
        drawPileTexture = Raylib.LoadTexture("resources/cards/drawpile.png");

        // Load the player texture
        playerTexture = Raylib.LoadTexture("resources/player/player8.png");

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

        // Load the rest background texture
        restBackground = Raylib.LoadTexture("resources/background/rest.png");

        // Load the charm background texture
        charmBackground = Raylib.LoadTexture("resources/background/charm.png");

        // Load the map scroll texture
        mapScrollTexture = Raylib.LoadTexture("resources/background/mapscroll.png");

        // Load the map background texture
        mapBackground = Raylib.LoadTexture("resources/background/map.png");

        // Load effect textures
        vulnerableTexture = Raylib.LoadTexture("resources/effects/vulnerable.png");
        bufferTexture = Raylib.LoadTexture("resources/effects/buffer.png");
        dexterityTexture = Raylib.LoadTexture("resources/effects/dexterity.png");
        frailTexture = Raylib.LoadTexture("resources/effects/frail.png");
        strengthTexture = Raylib.LoadTexture("resources/effects/strength.png");
        thornTexture = Raylib.LoadTexture("resources/effects/thorn.png");
        weakTexture = Raylib.LoadTexture("resources/effects/weak.png");
        rewardBackground = Raylib.LoadTexture("resources/background/rewardbook.png");
        shopBackground = Raylib.LoadTexture("resources/background/shop.png");

        // Load charm textures
        studyGuideTexture = Raylib.LoadTexture("resources/charms/studyguide.png");
        coffeeMugTexture = Raylib.LoadTexture("resources/charms/coffeemug.png");
        luckyPenTexture = Raylib.LoadTexture("resources/charms/luckypen.png");
        bookmarkTexture = Raylib.LoadTexture("resources/charms/bookmark.png");
        calculatorTexture = Raylib.LoadTexture("resources/charms/calculator.png");
        highlighterTexture = Raylib.LoadTexture("resources/charms/highlighter.png");
        stickyNotesTexture = Raylib.LoadTexture("resources/charms/stickynotes.png");
        studyTimerTexture = Raylib.LoadTexture("resources/charms/studytimer.png");
        flashCardsTexture = Raylib.LoadTexture("resources/charms/flashcards.png");
        textBookTexture = Raylib.LoadTexture("resources/charms/textbook.png");
        notebookTexture = Raylib.LoadTexture("resources/charms/notebook.png");
        smartWatchTexture = Raylib.LoadTexture("resources/charms/smartwatch.png");
        studyGroupTexture = Raylib.LoadTexture("resources/charms/studygroup.png");
        allNighterTexture = Raylib.LoadTexture("resources/charms/allnighter.png");
        geniusIdeaTexture = Raylib.LoadTexture("resources/charms/geniusidea.png");

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
            new Rectangle(playerPosition.X - 150, playerPosition.Y - 350, 350, 750),
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
            Color.DarkBrown
        );

        // Draw the current room
        if (game.Rooms.Count > 0)
        {
            if (game.Rooms[0] is Combat combatRoom)
            {
                DrawEnemy(combatRoom);
                DrawHPBar(combatRoom.Enemy, new Vector2(ScreenWidth - 700, ScreenHeight / 2 + 100), false);
                DrawEnergyCounter(game.Player.MaxEnergy, combatRoom.CurrentEnergy);
                DrawEndTurnButton(combatRoom);
            }
        }
        
        // Draw player HP bar above the player
        DrawHPBar(game.Player, new Vector2(playerPosition.X - 100, playerPosition.Y - 380), true);

        // Add Scroll Image behind the cards
        Raylib.DrawTexturePro(
            scrollTexture,
            new Rectangle(0, 0, scrollTexture.Width, scrollTexture.Height),
            new Rectangle(270, 700, scrollTexture.Width * 1.8f, scrollTexture.Height * 1.35f),
            new Vector2(0, 0),
            0,
            Color.White
        );

        // Check for mouse position
        Vector2 mousePos = Raylib.GetMousePosition();
        Rectangle drawPileRect = new Rectangle(100, 800, drawPileTexture.Width, drawPileTexture.Height);
        Rectangle discardPileRect = new Rectangle(1680, 800, discardPileTexture.Width, discardPileTexture.Height);

        // Check for hover states
        bool isDrawPileHovered = Raylib.CheckCollisionPointRec(mousePos, drawPileRect);
        bool isDiscardPileHovered = Raylib.CheckCollisionPointRec(mousePos, discardPileRect);

        // Draw hover effects
        if (isDrawPileHovered)
        {
            // Draw glow effect
            Raylib.DrawCircle(
                (int)(drawPileRect.X + drawPileTexture.Width/2),
                (int)(drawPileRect.Y + drawPileTexture.Height/2),
                drawPileTexture.Width/2 + 10,
                new Color(255, 255, 255, 100)
            );
        }

        if (isDiscardPileHovered)
        {
            // Draw glow effect
            Raylib.DrawCircle(
                (int)(discardPileRect.X + discardPileTexture.Width/2),
                (int)(discardPileRect.Y + discardPileTexture.Height/2),
                discardPileTexture.Width/2 + 10,
                new Color(255, 255, 255, 100)
            );
        }

        // Draw the discard pile with hover effect
        float discardScale = isDiscardPileHovered ? 1.1f : 1.0f;
        float discardX = discardPileRect.X - (discardPileTexture.Width * (discardScale - 1)) / 2;
        float discardY = discardPileRect.Y - (discardPileTexture.Height * (discardScale - 1)) / 2;
        Raylib.DrawTexturePro(
            discardPileTexture,
            new Rectangle(0, 0, discardPileTexture.Width, discardPileTexture.Height),
            new Rectangle(discardX, discardY, discardPileTexture.Width * discardScale, discardPileTexture.Height * discardScale),
            new Vector2(0, 0),
            0,
            Color.White
        );

        // Draw discard pile count
        int discardCount = game.Player.Cards.Count(c => c.CardLocation == CardLocation.DiscardPile);
        string discardText = discardCount.ToString();
        Vector2 discardTextSize = Raylib.MeasureTextEx(descriptionFont, discardText, 30, 1);
        
        // Draw black circle behind discard pile number
        float discardNumberX = discardX + (discardPileTexture.Width * discardScale)/2;
        float discardNumberY = discardY + (discardPileTexture.Height * discardScale)/2;
        float discardCircleRadius = Math.Max(discardTextSize.X, discardTextSize.Y) * 0.7f;
        Raylib.DrawCircle(
            (int)discardNumberX,
            (int)discardNumberY,
            discardCircleRadius,
            new Color(0, 0, 0, 150)
        );
        
        Raylib.DrawTextPro(
            descriptionFont,
            discardText,
            new Vector2(discardNumberX - discardTextSize.X/2, 
                       discardNumberY - discardTextSize.Y/2),
            new Vector2(0, 0),
            0,
            30,
            1,
            Color.White
        );

        // Draw the draw pile with hover effect
        float drawScale = isDrawPileHovered ? 1.1f : 1.0f;
        float drawX = drawPileRect.X - (drawPileTexture.Width * (drawScale - 1)) / 2;
        float drawY = drawPileRect.Y - (drawPileTexture.Height * (drawScale - 1)) / 2;
        Raylib.DrawTexturePro(
            drawPileTexture,
            new Rectangle(0, 0, drawPileTexture.Width, drawPileTexture.Height),
            new Rectangle(drawX, drawY, drawPileTexture.Width * drawScale, drawPileTexture.Height * drawScale),
            new Vector2(0, 0),
            0,
            Color.White
        );

        // Draw draw pile count
        int drawCount = game.Player.Cards.Count(c => c.CardLocation == CardLocation.DrawPile);
        string drawText = drawCount.ToString();
        Vector2 drawTextSize = Raylib.MeasureTextEx(descriptionFont, drawText, 30, 1);
        
        // Draw black circle behind draw pile number
        float drawNumberX = drawX + (drawPileTexture.Width * drawScale)/2;
        float drawNumberY = drawY + (drawPileTexture.Height * drawScale)/2;
        float drawCircleRadius = Math.Max(drawTextSize.X, drawTextSize.Y) * 0.7f;
        Raylib.DrawCircle(
            (int)drawNumberX,
            (int)drawNumberY,
            drawCircleRadius,
            new Color(0, 0, 0, 150)
        );
        
        Raylib.DrawTextPro(
            descriptionFont,
            drawText,
            new Vector2(drawNumberX - drawTextSize.X/2, 
                       drawNumberY - drawTextSize.Y/2),
            new Vector2(0, 0),
            0,
            30,
            1,
            Color.White
        );

        // Check for clicks on draw pile or discard pile
        float currentTime = (float)Raylib.GetTime();
        if ((Raylib.IsMouseButtonPressed(MouseButton.Left) || Raylib.IsKeyPressed(KeyboardKey.D)) && 
            currentTime - lastClickTime > CLICK_DELAY)
        {
            if (!showDrawPileOverlay && !showDiscardPileOverlay)
            {
                if (isDrawPileHovered)
                {
                    showDrawPileOverlay = true;
                    lastClickTime = currentTime;
                }
                else if (isDiscardPileHovered)
                {
                    showDiscardPileOverlay = true;
                    lastClickTime = currentTime;
                }
            }
        }

        // Draw cards in player's hand
        var cardsInHand = game.Player.Cards.Where(c => c.CardLocation == CardLocation.Hand).ToList();
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            var card = cardsInHand[i];
            DrawCard(i, cardsInHand.Count, card.Name, card.Description, card.CardCost);
        }

        // Draw charm icon in top left corner
        const int charmIconSize = 100;
        const int charmPadding = 20;
        int charmStartY = charmPadding + 60;
        int charmSpacing = 10;
        
        // Draw all charm icons the player has with hover tooltip
        if (game?.Player?.Charms != null && game.Player.Charms.Count > 0)
        {
            Vector2 mousePosition = Raylib.GetMousePosition();
            string hoveredCharmDesc = null;
            int hoveredX = 0, hoveredY = 0;
            for (int i = 0; i < game.Player.Charms.Count; i++)
            {
                var charm = game.Player.Charms[i];
                Texture2D charmTexture = GetCharmTexture(charm.CharmType);
                int iconX = charmPadding + 10;
                int iconY = charmStartY + i * (charmIconSize + charmSpacing);
                Rectangle iconRect = new Rectangle(iconX, iconY, charmIconSize - 10, charmIconSize - 10);
                // Check for hover
                if (Raylib.CheckCollisionPointRec(mousePosition, iconRect))
                {
                    hoveredCharmDesc = charm.Description;
                    hoveredX = iconX + (charmIconSize - 10) / 2;
                    hoveredY = iconY;
                }
                Raylib.DrawTexturePro(
                    charmTexture,
                    new Rectangle(0, 0, charmTexture.Width, charmTexture.Height),
                    iconRect,
                    new Vector2(0, 0),
                    0,
                    Color.White
                );
            }
            // Draw overlay if hovering a charm
            if (!string.IsNullOrEmpty(hoveredCharmDesc))
            {
                int overlayWidth = 520;
                int overlayHeight = 60;
                int overlayX = hoveredX - overlayWidth / 2 + 250;
                int overlayY = hoveredY + charmIconSize - 10 + 8;
                Raylib.DrawRectangle(overlayX, overlayY, overlayWidth, overlayHeight, new Color(30, 30, 30, 220));
                Raylib.DrawRectangleLines(overlayX, overlayY, overlayWidth, overlayHeight, Color.Gold);
                Vector2 descSize = Raylib.MeasureTextEx(descriptionFont, hoveredCharmDesc, 18, 1);
                Raylib.DrawTextPro(
                    descriptionFont,
                    hoveredCharmDesc,
                    new Vector2(overlayX + (overlayWidth - descSize.X) / 2, overlayY + (overlayHeight - descSize.Y) / 2),
                    new Vector2(0, 0),
                    0,
                    18,
                    1,
                    Color.White
                );
            }
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

        // Draw overlays if active
        if (showDrawPileOverlay)
        {
            DrawPileOverlay(game.Player.Cards.Where(c => c.CardLocation == CardLocation.DrawPile).ToList(), "Draw Pile");
        }
        else if (showDiscardPileOverlay)
        {
            DrawPileOverlay(game.Player.Cards.Where(c => c.CardLocation == CardLocation.DiscardPile).ToList(), "Discard Pile");
        }
    }

    public static void DrawEventScreen(Game game)
    {
        if (!(game.CurrentRoom is Event eventRoom)) return;

        // Set input buffer when entering a new event room
        if (lastRoomEntryTime == 0)
        {
            lastRoomEntryTime = (float)Raylib.GetTime();
        }

        // If showing reward screen, only draw that
        if (showEventRewardScreen)
        {
            DrawEventRewardScreen();
            return;
        }

        // Draw event background
        Raylib.DrawTexturePro(
            eventBackground,
            new Rectangle(0, 0, eventBackground.Width, eventBackground.Height),
            new Rectangle(0, 0, ScreenWidth, ScreenHeight),
            new Vector2(0, 0),
            0,
            Color.White
        );

        // Draw Transparent Background
        Raylib.DrawRectangle(0, 0, ScreenWidth, ScreenHeight, new Color(0, 0, 0, 100));

        // Draw event dialog box
        int dialogBoxWidth = 1200;
        int dialogBoxHeight = 600;
        int dialogBoxX = (ScreenWidth - dialogBoxWidth) / 2;
        int dialogBoxY = (ScreenHeight - dialogBoxHeight) / 2;

        // Draw dialog box background with a parchment-like texture
        Raylib.DrawTexturePro(
            scrollTexture,
            new Rectangle(0, 0, scrollTexture.Width, scrollTexture.Height),
            new Rectangle(dialogBoxX, dialogBoxY, dialogBoxWidth, dialogBoxHeight),
            new Vector2(0, 0),
            0,
            new Color(255, 255, 255, 230)
        );

        // Draw dialog text
        string[] words = eventRoom.Dialog.Split(' ');
        string currentLine = "";
        int lineY = dialogBoxY + 50;
        int maxWidth = dialogBoxWidth - 100;
        int lineHeight = 30;

        foreach (string word in words)
        {
            string testLine = currentLine + word + " ";
            int textWidth = (int)Raylib.MeasureTextEx(descriptionFont, testLine, 24, 1).X;
            
            if (textWidth > maxWidth)
            {
                Raylib.DrawTextPro(
                    descriptionFont,
                    currentLine,
                    new Vector2(dialogBoxX + 50, lineY),
                    new Vector2(0, 0),
                    0,
                    24,
                    1,
                    Color.Black
                );
                currentLine = word + " ";
                lineY += lineHeight;
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
            new Vector2(dialogBoxX + 150, lineY + 50),
            new Vector2(0, 0),
            0,
            24,
            1,
            Color.Black
        );

        // Draw choices
        int choiceY = dialogBoxY + 200;
        int choiceHeight = 60;
        int choiceSpacing = 20;
        Vector2 mousePos = Raylib.GetMousePosition() + new Vector2(0, 30);

        for (int i = 0; i < eventRoom.Choices.Count; i++)
        {
            var choice = eventRoom.Choices[i];
            int choiceBoxY = choiceY + (i * (choiceHeight + choiceSpacing));
            
            // Draw choice box
            Rectangle choiceBox = new Rectangle(
                dialogBoxX + 150,
                choiceBoxY,
                dialogBoxWidth - 300,
                choiceHeight
            );

            // Check if mouse is hovering over choice
            bool isHovering = Raylib.CheckCollisionPointRec(mousePos, choiceBox);

            // Draw choice background with hover effect
            Color boxColor = isHovering ? new Color(200, 200, 200, 200) : new Color(180, 180, 180, 200);
            Raylib.DrawRectangleRec(choiceBox, boxColor);
            Raylib.DrawRectangleLinesEx(choiceBox, 2, Color.DarkGray);

            // Draw choice text
            Raylib.DrawTextPro(
                descriptionFont,
                choice.Text,
                new Vector2(choiceBox.X + 20, choiceBox.Y + 20),
                new Vector2(0, 0),
                0,
                20,
                1,
                Color.Black
            );

            // Handle choice click
            if (isHovering && Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                // Only allow click if input buffer has passed
                if ((float)Raylib.GetTime() - lastRoomEntryTime > INPUT_BUFFER_TIME)
                {
                    lastEventChoice = choice;
                    eventRoom.MakeChoice(choice);
                    showEventRewardScreen = true;
                    lastRoomEntryTime = 0; // Reset for next room
                    return; // Exit immediately after making choice
                }
            }
        }
        // Reset input buffer if we leave the event screen
        if (Program.currentScreen != Program.GameScreen.Event)
        {
            lastRoomEntryTime = 0;
        }
    }

    private static void DrawEventRewardScreen()
    {
        if (lastEventChoice == null) return;

        // Draw semi-transparent background
        Raylib.DrawRectangle(0, 0, ScreenWidth, ScreenHeight, new Color(0, 0, 0, 180));

        // Draw reward box
        int boxWidth = 800;
        int boxHeight = 600;
        int boxX = (ScreenWidth - boxWidth) / 2;
        int boxY = (ScreenHeight - boxHeight) / 2;

        // Draw box background
        Raylib.DrawTexturePro(
            scrollTexture,
            new Rectangle(0, 0, scrollTexture.Width, scrollTexture.Height),
            new Rectangle(boxX, boxY, boxWidth, boxHeight),
            new Vector2(0, 0),
            0,
            new Color(255, 255, 255, 230)
        );

        // Draw title
        string titleText = "Event Result";
        int titleWidth = Raylib.MeasureText(titleText, 40);
        Raylib.DrawText(titleText, ScreenWidth/2 - titleWidth/2, boxY + 50, 40, Color.Black);

        // Draw rewards
        int currentY = boxY + 150;
        int spacing = 60;

        // Draw gold reward if any
        if (lastEventChoice.GoldReward != 0)
        {
            string goldText = $"Gold: {(lastEventChoice.GoldReward > 0 ? "+" : "")}{lastEventChoice.GoldReward}";
            Raylib.DrawText(goldText, boxX + 100, currentY, 30, Color.Gold);
            currentY += spacing;
        }

        // Draw health reward if any
        if (lastEventChoice.HealthChange != 0)
        {
            string healthText = $"Health: {(lastEventChoice.HealthChange > 0 ? "+" : "")}{lastEventChoice.HealthChange}";
            Raylib.DrawText(healthText, boxX + 100, currentY, 30, Color.Red);
            currentY += spacing;
        }

        // Draw card reward if any
        if (lastEventChoice.CardReward != null)
        {
            string cardText = $"New Card: {lastEventChoice.CardReward.Name}";
            Raylib.DrawText(cardText, boxX + 100, currentY, 30, Color.Blue);
            currentY += spacing;

            // Draw card description
            string[] words = lastEventChoice.CardReward.Description.Split(' ');
            string currentLine = "";
            int maxWidth = boxWidth - 200;

            foreach (string word in words)
            {
                string testLine = currentLine + word + " ";
                int lineWidth = (int)Raylib.MeasureTextEx(descriptionFont, testLine, 20, 1).X;
                
                if (lineWidth > maxWidth)
                {
                    Raylib.DrawTextPro(
                        descriptionFont,
                        currentLine,
                        new Vector2(boxX + 100, currentY),
                        new Vector2(0, 0),
                        0,
                        20,
                        1,
                        Color.Black
                    );
                    currentLine = word + " ";
                    currentY += 25;
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
                new Vector2(boxX + 100, currentY),
                new Vector2(0, 0),
                0,
                20,
                1,
                Color.Black
            );
            currentY += spacing;
        }

        // Draw charm reward if any
        if (lastEventChoice.CharmReward != null)
        {
            string charmText = $"New Charm: {lastEventChoice.CharmReward.Name}";
            Raylib.DrawText(charmText, boxX + 100, currentY, 30, Color.Purple);
            currentY += spacing;

            // Draw charm description
            string[] words = lastEventChoice.CharmReward.Description.Split(' ');
            string currentLine = "";
            int maxWidth = boxWidth - 200;

            foreach (string word in words)
            {
                string testLine = currentLine + word + " ";
                int lineWidth = (int)Raylib.MeasureTextEx(descriptionFont, testLine, 20, 1).X;
                
                if (lineWidth > maxWidth)
                {
                    Raylib.DrawTextPro(
                        descriptionFont,
                        currentLine,
                        new Vector2(boxX + 100, currentY),
                        new Vector2(0, 0),
                        0,
                        20,
                        1,
                        Color.Black
                    );
                    currentLine = word + " ";
                    currentY += 25;
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
                new Vector2(boxX + 100, currentY),
                new Vector2(0, 0),
                0,
                20,
                1,
                Color.Black
            );
        }

        // Draw continue button
        const int buttonWidth = 200;
        const int buttonHeight = 50;
        Rectangle buttonRect = new Rectangle(
            ScreenWidth/2 - buttonWidth/2,
            boxY + boxHeight - 100,
            buttonWidth,
            buttonHeight
        );

        // Check if mouse is hovering over button
        Vector2 mousePos = Raylib.GetMousePosition();
        bool isHovering = Raylib.CheckCollisionPointRec(mousePos, buttonRect);

        // Draw button
        Color buttonColor = isHovering ? new Color(200, 200, 200, 255) : Color.White;
        Raylib.DrawRectangleRec(buttonRect, buttonColor);
        Raylib.DrawRectangleLinesEx(buttonRect, 2, Color.DarkGray);

        // Draw button text
        string buttonText = "Continue";
        int buttonTextWidth = Raylib.MeasureText(buttonText, 20);
        Raylib.DrawText(
            buttonText,
            (int)(buttonRect.X + (buttonWidth - buttonTextWidth) / 2),
            (int)(buttonRect.Y + (buttonHeight - 20) / 2),
            20,
            Color.Black
        );

        // Handle button click
        if (isHovering && Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            // Update current room and make next rooms available
            var currentRoom = game.Layers[playerLayer][playerIndex];
            currentRoom.SetCurrent(false);
            currentRoom.SetCleared(true);  // Mark the room as cleared
            
            // Make only directly connected rooms available
            foreach (var nextRoom in currentRoom.Connections)
            {
                nextRoom.SetAvailable(true);
            }
            
            showEventRewardScreen = false;
            lastEventChoice = null;
            Program.currentScreen = Program.GameScreen.MapSelection;
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
        const int cardWidth = 168;  // 
        const int cardHeight = 251; // 
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
        const int cardWidth = 168;  // Increased from 140 by 20%
        const int cardHeight = 251; // Increased from 209 by 20%
        const int shadowWidth = 18;
        const int gradientSteps = 6;
        const int hoverElevation = 20;
        const int padding = 10;
        const int costBoxSize = 28;

        // Check if this card was just drawn (moved from draw pile to hand)
        var cardsInHand = game.Player.Cards.Where(c => c.CardLocation == CardLocation.Hand).ToList();
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
                    var handCards = game.Player.Cards.Where(c => c.CardLocation == CardLocation.Hand).ToList();
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
                        game.Player.PlayCard(handCards[cardIndex]);
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
            cardNameFont,
            cardName,
            new Vector2(position.X + costBoxSize + padding * 2 - 30, position.Y + padding + 30),
            new Vector2(0, 0),
            0,
            20,  
            1,
            Color.Black
        );

        // Draw description
        string[] words = description.Split(' ');
        string currentLine = "";
        int lineY = (int)position.Y + 46 + 30;
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
        const int bookWidth = 330;
        const int bookHeight = 300;
        Vector2 enemyPos = new Vector2(ScreenWidth - 600, ScreenHeight / 2 - 100);

        int textureIndex = combatRoom.Enemy.TextureIndex;
        switch (textureIndex)
        {
            case 1:
                enemyTexture = enemyTexture1;
                break;
            case 2:
                enemyTexture = enemyTexture2;
                break;
            case 3:
                enemyTexture = enemyTexture3;
                break;
            case 4:
                enemyTexture = enemyTexture4;
                break;
            case 5:
                enemyTexture = enemyTexture5;
                break;
            case 6:
                enemyTexture = enemyTexture6;
                break;
            case 7:
                enemyTexture = enemyTexture7;
                break;
        }
        // Draw enemy sprite
        Raylib.DrawTexturePro(
            enemyTexture,
            new Rectangle(0, 0, enemyTexture.Width, enemyTexture.Height),
            new Rectangle(enemyPos.X - bookWidth/2, enemyPos.Y - bookHeight/2, bookWidth, bookHeight),
            new Vector2(0, 0),
            0,
            Color.White
        );

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
                float y = anim.Position.Y - (progress * 80); // Move up 50 pixels
                float alpha = 2.0f - progress; // Fade out
                
                // Draw damage number
                string damageText = anim.Damage.ToString();
                Vector2 textSize = Raylib.MeasureTextEx(descriptionFont, damageText, 40, 1);
                Raylib.DrawTextPro(
                    descriptionFont,
                    damageText,
                    new Vector2(anim.Position.X - textSize.X/2, y),
                    new Vector2(0, 0),
                    0,
                    100,
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
                (int)enemyPos.X - textWidth/2 - 10,
                (int)enemyPos.Y - bookHeight/2 - 40,
                textWidth + 20,
                30,
                new Color(0, 0, 0, 150)
            );

            // Draw intent text
            Raylib.DrawText(
                intentText,
                (int)enemyPos.X - textWidth/2,
                (int)enemyPos.Y - bookHeight/2 - 35,
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

        // Draw effect stacks with adjusted position
        if (!isPlayer)
        {
            // For enemy, position effects below HP bar with more space
            DrawEffectStacks(unit, new Vector2(position.X, position.Y + barHeight + 10));
        }
        else
        {
            // For player, keep original position
            DrawEffectStacks(unit, new Vector2(position.X, position.Y + barHeight + 10));
        }
    }

    private static void DrawEffectStacks(Unit unit, Vector2 startPosition)
    {
        const int effectSize = 30;  // Size for effect icons
        const int effectSpacing = 35;
        int currentX = (int)startPosition.X;

        Vector2 mousePos = Raylib.GetMousePosition();
        string hoveredDescription = null;
        int hoveredX = 0, hoveredY = 0;

        // Draw each effect that has stacks
        foreach (var effect in unit.Effects)
        {
            if (effect.Stacks > 0)
            {
                Texture2D effectTexture;
                bool shouldDraw = true;

                switch (effect.EffectType)
                {
                    case EffectType.Vulnerable:
                        effectTexture = vulnerableTexture;
                        break;
                    case EffectType.Buffer:
                        effectTexture = bufferTexture;
                        break;
                    case EffectType.DexterityUp:
                        effectTexture = dexterityTexture;
                        break;
                    case EffectType.Frail:
                        effectTexture = frailTexture;
                        break;
                    case EffectType.StrengthUp:
                        effectTexture = strengthTexture;
                        break;
                    case EffectType.Thorn:
                        effectTexture = thornTexture;
                        break;
                    case EffectType.Weak:
                        effectTexture = weakTexture;
                        break;
                    default:
                        shouldDraw = false;
                        effectTexture = vulnerableTexture; // Dummy value, won't be used
                        break;
                }

                Rectangle iconRect = new Rectangle(currentX - effectSize / 2, (int)startPosition.Y - effectSize / 2, effectSize, effectSize);

                // Check for hover
                if (shouldDraw && Raylib.CheckCollisionPointRec(mousePos, iconRect))
                {
                    hoveredDescription = effect.EffectDescription; // <-- Make sure EffectDescription is public
                    hoveredX = currentX;
                    hoveredY = (int)startPosition.Y - effectSize / 2;
                }

                if (shouldDraw)
                {
                    // Draw effect texture
                    Raylib.DrawTexturePro(
                        effectTexture,
                        new Rectangle(0, 0, effectTexture.Width, effectTexture.Height),
                        iconRect,
                        new Vector2(0, 0),
                        0,
                        Color.White
                    );
                    // Draw stack count
                    string stackText = effect.Stacks.ToString();
                    Vector2 textSize = Raylib.MeasureTextEx(descriptionFont, stackText, 16, 1);
                    Raylib.DrawTextPro(
                        descriptionFont,
                        stackText,
                        new Vector2(currentX - textSize.X / 2, (int)startPosition.Y - textSize.Y / 2),
                        new Vector2(0, 0),
                        0,
                        16,
                        1,
                        Color.White
                    );
                }
                currentX += effectSpacing;
            }
        }

        // Draw overlay if hovering
        if (!string.IsNullOrEmpty(hoveredDescription))
        {
            int overlayWidth = 360;
            int overlayHeight = 60;
            int overlayX = hoveredX - overlayWidth / 2;
            int overlayY = hoveredY - overlayHeight - 10;

            Raylib.DrawRectangle(overlayX, overlayY, overlayWidth, overlayHeight, new Color(30, 30, 30, 220));
            Raylib.DrawRectangleLines(overlayX, overlayY, overlayWidth, overlayHeight, Color.Gold);

            Vector2 descSize = Raylib.MeasureTextEx(descriptionFont, hoveredDescription, 18, 1);
            Raylib.DrawTextPro(
                descriptionFont,
                hoveredDescription,
                new Vector2(overlayX + (overlayWidth - descSize.X) / 2, overlayY + (overlayHeight - descSize.Y) / 2),
                new Vector2(0, 0),
                0,
                18,
                1,
                Color.White
            );
        }
    }

    private static void DrawEnergyCounter(int maxEnergy, int currentEnergy)
    {
        // Sectioned bar settings
        const int sectionWidth = 50;
        const int sectionHeight = 50;
        const int sectionSpacing = 8;
        const int startX = 370;
        const int startY = ScreenHeight - 410;

        // Calculate total number of orbs to display
        int totalOrbs = Math.Max(maxEnergy, currentEnergy);

        for (int i = 0; i < totalOrbs; i++)
        {
            int x = startX + i * (sectionWidth + sectionSpacing);
            Color fill;
            
            if (i < maxEnergy)
            {
                // Normal energy orbs
                fill = i < currentEnergy ? Color.White : new Color(180, 180, 180, 120); // Dimmed for empty
            }
            else
            {
                // Extra energy orb (beyond max)
                fill = Color.Yellow; // Special color for extra energy
            }

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
        if (game?.Player == null || combatRoom == null) return; // Safety check

        const int buttonWidth = 150;
        const int buttonHeight = 50;
        const int buttonX = ScreenWidth - buttonWidth - 200;
        const int buttonY = ScreenHeight - buttonHeight - 400;

        // Check if mouse is hovering over button
        Vector2 mousePos = Raylib.GetMousePosition() + new Vector2(0, -20);
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
                var cardsInHand = game.Player.Cards.Where(c => c.CardLocation == CardLocation.Hand).ToList();
                
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
                game.Player.EndTurn();
                combatRoom.StartEnemyTurn();
                combatRoom.EndEnemyTurn();
                
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error during turn transition: {e.Message}");
            }
        }
    }

    public static void GenerateMapGraph()
    {
        if (game == null) return;  // Safety check

        int totalLayers = 12;
        int[] roomsPerLayer = new int[totalLayers];
        for (int i = 0; i < totalLayers; i++)
        {
            roomsPerLayer[i] = (i % 2 == 0) ? 2 : 3; // alternate 2,3,2,3...
        }
        roomsPerLayer[0] = 1; // Start with 1 room
        roomsPerLayer[totalLayers-1] = 1; // End with 1 room (boss)

        game.Layers = new List<List<Room>>();  // Ensure layers is initialized
        int verticalSpacing = 70;
        int horizontalSpacing = 120;
        int startY = 120;
        int centerX = ScreenWidth / 2;

        // Create rooms
        for (int layer = 0; layer < totalLayers; layer++)
        {
            int rooms = roomsPerLayer[layer];
            int y = startY + layer * verticalSpacing;
            int totalWidth = (rooms - 1) * horizontalSpacing;
            var layerRooms = new List<Room>();
            
            // Add the layer list before adding rooms to it
            game.Layers.Add(layerRooms);

            for (int n = 0; n < rooms; n++)
            {
                int x = centerX - totalWidth / 2 + n * horizontalSpacing;
                string roomType;
                
                // Calculate the room index based on the map graph
                int roomIndex = 0;
                for (int l = 0; l < layer; l++)
                {
                    roomIndex += game.Layers[l].Count;
                }
                roomIndex += n;

                // Check if we have a room at this index
                if (game?.Rooms != null && roomIndex < game.Rooms.Count)
                {
                    if (game.Rooms[roomIndex] is Combat combatRoom)
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
                    else if (game.Rooms[roomIndex] is Event)
                    {
                        roomType = "Event";
                    }
                    else if (game.Rooms[roomIndex] is Rest)
                    {
                        roomType = "Rest";
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
                    else if (layer % 3 == 0 && n == rooms - 1)
                        roomType = "Elite";
                    else if (layer % 4 == 0 && n == rooms - 2)
                        roomType = "Event";
                    else
                        roomType = "Enemy";
                }
                if (roomType == "Enemy")
                {
                    layerRooms.Add(new Combat(false, false, false, null, EnemyType.Basic, TurnPhase.PlayerStart, 3) { Layer = layer, Index = n, RoomType = roomType, X = x, Y = y });
                }
                else if (roomType == "Boss")
                {
                    layerRooms.Add(new Combat(false, false, false, null, EnemyType.Boss, TurnPhase.PlayerStart, 3) { Layer = layer, Index = n, RoomType = roomType, X = x, Y = y });
                }
                else if (roomType == "Elite")
                {
                    layerRooms.Add(new Combat(false, false, false, null, EnemyType.Elite, TurnPhase.PlayerStart, 3) { Layer = layer, Index = n, RoomType = roomType, X = x, Y = y });
                }
                else if (roomType == "Event")
                {
                    layerRooms.Add(new Event(false, false, false, "A mysterious event awaits...", new List<EventChoice>()) { Layer = layer, Index = n, RoomType = roomType, X = x, Y = y });
                }
                else if (roomType == "Rest")
                {
                    layerRooms.Add(new Rest(false, false, false) { Layer = layer, Index = n, RoomType = roomType, X = x, Y = y });
                }
            }
        }
        // Connect rooms with improved branching logic
        for (int layer = 1; layer < totalLayers; layer++)
        {
            var prevLayer = game.Layers[layer-1];
            var currLayer = game.Layers[layer];
            int prevCount = prevLayer.Count;
            int currCount = currLayer.Count;

            // Special handling for 2-to-3 and 3-to-2 transitions
            if (prevCount == 2 && currCount == 3)
            {
                // Left room connects to left and center
                prevLayer[0].Connections.Add(currLayer[0]);
                prevLayer[0].Connections.Add(currLayer[1]);
                // Right room connects to center and right
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
                // For each room in prevLayer, connect to its two nearest neighbors in currLayer
                for (int p = 0; p < prevCount; p++)
                {
                    float proportional = (float)p / (prevCount - 1) * (currCount - 1);
                    int left = (int)Math.Floor(proportional);
                    int right = (int)Math.Ceiling(proportional);
                    prevLayer[p].Connections.Add(currLayer[left]);
                    if (right != left) prevLayer[p].Connections.Add(currLayer[right]);
                }
            }
            // Ensure every room in currLayer has at least one incoming connection
            foreach (var room in currLayer)
            {
                bool hasIncoming = prevLayer.Any(prev => prev.Connections.Contains(room));
                if (!hasIncoming)
                {
                    var closest = prevLayer.OrderBy(prev => Math.Abs(prev.X - room.X)).First();
                    closest.Connections.Add(room);
                }
            }
        }
        // Set starting room as available/current
        game.Layers[0][0].SetAvailable(true);
        game.Layers[0][0].SetCurrent(true);
        playerLayer = 0;
        playerIndex = 0;
    }

    public static int DrawMapSelectionScreen()
    {

        if (game.Layers == null) GenerateMapGraph();
        Raylib.DrawTexture(mapBackground, 0, 0, Color.White);

        // Draw player stats
        if (game?.Player != null)
        {
            // Draw HP
            string hpText = $"HP: {game.Player.Health}/{game.Player.MaxHealth}";
            Raylib.DrawText(hpText, 50, 50, 30, Color.Red);

            // Draw Gold
            string goldText = $"Gold: {game.Player.Gold}";
            Raylib.DrawText(goldText, 50, 90, 30, Color.Gold);
        }

        DrawMapOverlay();
        
        // Check all layers for available rooms
        Vector2 mouse = Raylib.GetMousePosition();
        int roomRadius = 30;
        
        // Check each layer
        for (int layer = 0; layer < game.Layers.Count; layer++)
        {
            var currentLayer = game.Layers[layer];
            for (int i = 0; i < currentLayer.Count; i++)
            {
                var room = currentLayer[i];
                if (room.IsAvailable && !room.IsCleared)  // Only allow selecting available and uncleared rooms
                {
                    float dx = mouse.X - room.X;
                    float dy = mouse.Y - room.Y;
                    if (dx * dx + dy * dy <= roomRadius * roomRadius)
                    {
                        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                        {
                            // Reset availability of all rooms except the current one and its connections
                            foreach (var layerList in game.Layers)
                            {
                                foreach (var r in layerList)
                                {
                                    if (r != room && !room.Connections.Contains(r))
                                    {
                                        r.SetAvailable(false);
                                    }
                                }
                            }

                            // Update current room
                            var oldCurrentRoom = game.Layers[playerLayer][playerIndex];
                            oldCurrentRoom.SetCurrent(false);
                            
                            // Set new current room
                            room.SetCurrent(true);
                            playerLayer = layer;
                            playerIndex = i;
                            
                            // Calculate the actual room index based on the map graph
                            int roomIndex = 0;
                            for (int l = 0; l < layer; l++)
                            {
                                roomIndex += game.Layers[l].Count;
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
        if (game.Layers == null) GenerateMapGraph();
        Raylib.DrawRectangle(0, 0, ScreenWidth, ScreenHeight, new Color(0, 0, 0, 60));
        Raylib.DrawTexturePro(
            mapScrollTexture, 
            new Rectangle(0, 0, mapScrollTexture.Width, mapScrollTexture.Height), 
            new Rectangle(700, 0, 500, 1050), 
            new Vector2(0, 0), 
            0, 
            Color.White);
        int roomRadius = 30;
        // Draw connections
        for (int layer = 0; layer < game.Layers.Count-1; layer++)
        {
            foreach (var room in game.Layers[layer])
            {
                foreach (var next in room.Connections)
                {
                    Raylib.DrawLine(room.X, room.Y, next.X, next.Y, Color.Gray);
                }
            }
        }
        // Draw rooms
        for (int layer = 0; layer < game.Layers.Count; layer++)
        {
            foreach (var room in game.Layers[layer])
            {
                Color fill;
                if (room.IsCurrent)
                    fill = Color.Yellow;
                else if (room.IsCleared)
                    fill = new Color(100, 100, 100, 255);  // Gray out cleared rooms
                else if (room.IsAvailable)
                    fill = Color.LightGray;
                else
                    fill = new Color(80, 80, 80, 255);

                Raylib.DrawCircle(room.X, room.Y, roomRadius, fill);
                Raylib.DrawCircleLines(room.X, room.Y, roomRadius, Color.DarkGray);
                Raylib.DrawText(room.RoomType, room.X-24, room.Y-10, 18, Color.Black);

                // Draw X on cleared rooms
                if (room.IsCleared)
                {
                    int xSize = 20;  // Size of the X
                    int lineThickness = 3;  // Thickness of the X lines
                    
                    // Draw X with two crossed thick lines
                    for (int offset = -lineThickness/2; offset <= lineThickness/2; offset++)
                    {
                        // First diagonal line
                        Raylib.DrawLine(
                            room.X - xSize + offset, room.Y - xSize,
                            room.X + xSize + offset, room.Y + xSize,
                            Color.Red
                        );
                        Raylib.DrawLine(
                            room.X - xSize, room.Y - xSize + offset,
                            room.X + xSize, room.Y + xSize + offset,
                            Color.Red
                        );

                        // Second diagonal line
                        Raylib.DrawLine(
                            room.X + xSize + offset, room.Y - xSize,
                            room.X - xSize + offset, room.Y + xSize,
                            Color.Red
                        );
                        Raylib.DrawLine(
                            room.X + xSize, room.Y - xSize + offset,
                            room.X - xSize, room.Y + xSize + offset,
                            Color.Red
                        );
                    }
                }
            }
        }
        // Draw close instruction
        string closeText = "Click a room to start";
        int textWidth = Raylib.MeasureText(closeText, 24);
        Raylib.DrawText(closeText, ScreenWidth/2 - textWidth/2, ScreenHeight - 60, 24, Color.White);
    }

    public static void DrawRewardScreen()
    {
        // Always reset shop access flag when entering reward screen
        shopVisitedThisReward = false;

        Raylib.DrawRectangle(0, 0, ScreenWidth, ScreenHeight, new Color(0, 0, 0, 180));

        Raylib.DrawTexturePro(
            rewardBackground,
            new Rectangle(0, 0, rewardBackground.Width, rewardBackground.Height),
            new Rectangle(0, 0, ScreenWidth, ScreenHeight),
            new Vector2(0, 0),
            0,
            Color.White
        );

        // Generate reward cards if not already done
        if (!rewardCardsGenerated)
        {
            rewardCards = game.GenerateRewardCards();
            rewardCardsGenerated = true;
        }

        // Draw semi-transparent background
        Raylib.DrawRectangle(0, 0, ScreenWidth, ScreenHeight, new Color(0, 0, 0, 90));

        // Draw title
        string titleText = "Choose a Card";
        int titleWidth = Raylib.MeasureText(titleText, 40);
        Raylib.DrawText(titleText, ScreenWidth/2 - titleWidth/2, 50, 40, Color.White);

        // Draw cards
        const int cardWidth = 235;  // Match the gameplay card width
        const int cardHeight = 351; // Match the gameplay card height
        const int cardSpacing = 100;
        int totalWidth = (cardWidth * 3) + (cardSpacing * 2);
        int startX = (ScreenWidth - totalWidth) / 2;
        int startY = ScreenHeight/2 - cardHeight/2;

        Vector2 mousePos = Raylib.GetMousePosition();

        for (int i = 0; i < rewardCards.Count; i++)
        {
            Card card = rewardCards[i];
            Vector2 position = new Vector2(
                startX + (i * (cardWidth + cardSpacing)),
                startY
            );

            // Check if mouse is hovering over card
            bool isHovering = mousePos.X >= position.X && 
                            mousePos.X <= position.X + cardWidth &&
                            mousePos.Y >= position.Y && 
                            mousePos.Y <= position.Y + cardHeight;

            // Apply hover effect
            if (isHovering)
            {
                position.Y -= 20;
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
            const int padding = 10;
            const int costBoxSize = 28;
            Raylib.DrawRectangle(
                (int)position.X + padding,
                (int)position.Y + padding,
                costBoxSize,
                costBoxSize,
                Color.Red
            );

            // Draw cost number
            string costText = card.CardCost.ToString();
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
                cardNameFont,
                card.Name,
                new Vector2(position.X + costBoxSize + padding * 2 - 30, position.Y + padding + 30),
                new Vector2(0, 0),
                0,
                20,
                1,
                Color.Black
            );

            // Draw description
            string[] words = card.Description.Split(' ');
            string currentLine = "";
            int lineY = (int)position.Y + 46 + 30;
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

            // Handle card selection
            if (isHovering && Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                // Add selected card to player's deck
                if (game?.Player != null)
                {
                    game.Player.AddCard(card);
                    // Reset reward screen state
                    rewardCardsGenerated = false;
                    rewardCards.Clear();
                    // Update room states
                    var currentRoom = game.Layers[playerLayer][playerIndex];
                    currentRoom.SetCleared(true);
                    currentRoom.SetCurrent(false);
                    
                    foreach (var nextRoom in currentRoom.Connections)
                    {
                        nextRoom.SetAvailable(true);
                    }
                    // Return to map selection
                    Program.currentScreen = Program.GameScreen.MapSelection;
                }
            }
        }

        // Draw Gold Reward if current room is Combat
        if (game?.CurrentRoom is Combat combatRoom)
        {
            string goldRewardText = $"Gold Reward: {combatRoom.GoldReward}";
            int goldRewardWidth = Raylib.MeasureText(goldRewardText, 30);
            Raylib.DrawText(goldRewardText, ScreenWidth/2 - goldRewardWidth/2, ScreenHeight - 200, 30, Color.Gold);
        }

        // Draw instruction text
        string instructionText = "Click a card to add it to your deck";
        int instructionWidth = Raylib.MeasureText(instructionText, 20);
        Raylib.DrawText(instructionText, ScreenWidth/2 - instructionWidth/2, ScreenHeight - 50, 20, Color.White);

        // Draw shop button
        const int shopButtonWidth = 150;
        const int shopButtonHeight = 50;
        Rectangle shopButtonRect = new Rectangle(
            50,
            ScreenHeight - shopButtonHeight - 50,
            shopButtonWidth,
            shopButtonHeight
        );

        // Check if mouse is hovering over shop button
        bool isShopHovering = Raylib.CheckCollisionPointRec(mousePos, shopButtonRect);

        // Draw shop button
        Color shopButtonColor = shopVisitedThisReward ? new Color(120, 120, 120, 180) : (isShopHovering ? new Color(200, 200, 200, 255) : Color.White);
        Raylib.DrawRectangleRec(shopButtonRect, shopButtonColor);
        Raylib.DrawRectangleLinesEx(shopButtonRect, 2, Color.Gray);

        // Draw shop button text
        string shopText = "Shop";
        int shopTextWidth = Raylib.MeasureText(shopText, 20);
        Raylib.DrawText(
            shopText,
            (int)(shopButtonRect.X + (shopButtonWidth - shopTextWidth) / 2),
            (int)(shopButtonRect.Y + (shopButtonHeight - 20) / 2),
            20,
            shopVisitedThisReward ? Color.Gray : Color.Black
        );

        // Handle shop button click
        if (!shopVisitedThisReward && isShopHovering && Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            Program.currentScreen = Program.GameScreen.Shop;
            shopVisitedThisReward = true;
        }

        // Draw skip button
        const int skipButtonWidth = 150;
        const int skipButtonHeight = 50;
        Rectangle skipButtonRect = new Rectangle(
            ScreenWidth - skipButtonWidth - 50,
            ScreenHeight - skipButtonHeight - 50,
            skipButtonWidth,
            skipButtonHeight
        );

        // Check if mouse is hovering over skip button
        bool isSkipHovering = Raylib.CheckCollisionPointRec(mousePos, skipButtonRect);

        // Draw skip button
        Color skipButtonColor = isSkipHovering ? new Color(200, 200, 200, 255) : Color.White;
        Raylib.DrawRectangleRec(skipButtonRect, skipButtonColor);
        Raylib.DrawRectangleLinesEx(skipButtonRect, 2, Color.Gray);

        // Draw skip button text
        string skipText = "Skip";
        int skipTextWidth = Raylib.MeasureText(skipText, 20);
        Raylib.DrawText(
            skipText,
            (int)(skipButtonRect.X + (skipButtonWidth - skipTextWidth) / 2),
            (int)(skipButtonRect.Y + (skipButtonHeight - 20) / 2),
            20,
            Color.Black
        );

        // Handle skip button click
        if (isSkipHovering && Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            // Reset reward screen state
            rewardCardsGenerated = false;
            rewardCards.Clear();
            // Update room states
            var currentRoom = game.Layers[playerLayer][playerIndex];
            currentRoom.SetCleared(true);
            currentRoom.SetCurrent(false);
            
            foreach (var nextRoom in currentRoom.Connections)
            {
                nextRoom.SetAvailable(true);
            }
            // Return to map selection
            Program.currentScreen = Program.GameScreen.MapSelection;
        }
    }

    public static void DrawShopScreen()
    {
        // Draw shop background
        Raylib.DrawTexturePro(
            shopBackground,
            new Rectangle(0, 0, shopBackground.Width, shopBackground.Height),
            new Rectangle(0, 0, ScreenWidth, ScreenHeight),
            new Vector2(0, 0),
            0,
            Color.White
        );

        // Draw semi-transparent overlay
        Raylib.DrawRectangle(0, 0, ScreenWidth, ScreenHeight, new Color(0, 0, 0, 120));

        // Draw title
        string titleText = "Shop";
        int titleWidth = Raylib.MeasureText(titleText, 40);
        Raylib.DrawText(titleText, ScreenWidth/2 - titleWidth/2, 50, 40, Color.White);

        // Draw gold amount using player's gold
        string goldText = $"Gold: {game?.Player?.Gold ?? 0}";
        Raylib.DrawText(goldText, 50, 50, 30, Color.Gold);

        // Generate shop items if not already done
        if (!shopCardsGenerated)
        {
            shopCards = game.GenerateRewardCards();
            shopCharms = game.GenerateShopCharms();
            shopCardsGenerated = true;
        }

        // Draw cards section
        string cardsTitle = "Cards";
        Raylib.DrawText(cardsTitle, 50, 120, 30, Color.White);

        // Draw cards
        const int cardWidth = 235;
        const int cardHeight = 351;
        const int cardSpacing = 50;
        int totalCardWidth = (cardWidth * 3) + (cardSpacing * 2);
        int startCardX = (ScreenWidth - totalCardWidth) / 2;
        int startCardY = 180;

        Vector2 mousePos = Raylib.GetMousePosition();

        for (int i = 0; i < shopCards.Count; i++)
        {
            Card card = shopCards[i];
            Vector2 position = new Vector2(
                startCardX + (i * (cardWidth + cardSpacing)),
                startCardY
            );

            // Check if mouse is hovering over card
            bool isHovering = mousePos.X >= position.X && 
                            mousePos.X <= position.X + cardWidth &&
                            mousePos.Y >= position.Y && 
                            mousePos.Y <= position.Y + cardHeight;

            // Apply hover effect
            if (isHovering)
            {
                position.Y -= 20;
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
            const int padding = 10;
            const int costBoxSize = 28;
            Raylib.DrawRectangle(
                (int)position.X + padding,
                (int)position.Y + padding,
                costBoxSize,
                costBoxSize,
                Color.Red
            );

            // Draw cost number
            string costText = card.CardCost.ToString();
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
                cardNameFont,
                card.Name,
                new Vector2(position.X + costBoxSize + padding * 2 - 30, position.Y + padding + 30),
                new Vector2(0, 0),
                0,
                20,
                1,
                Color.Black
            );

            // Draw description
            string[] words = card.Description.Split(' ');
            string currentLine = "";
            int lineY = (int)position.Y + 46 + 30;
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

            // Draw price
            int price = card.Price; 
            string priceText = $"{price} Gold";
            Vector2 priceTextSize = Raylib.MeasureTextEx(descriptionFont, priceText, 24, 1);
            Raylib.DrawTextPro(
                descriptionFont,
                priceText,
                new Vector2(
                    position.X + (cardWidth - priceTextSize.X) / 2,
                    position.Y + cardHeight + 15
                ),
                new Vector2(0, 0),
                0,
                24,
                1,
                Color.Gold
            );

            // Handle card purchase
            if (isHovering && Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                if (game?.Player?.Gold >= price)
                {
                    // Purchase card
                    if (game?.Player != null)
                    {
                        game.Player.AddCard(card);
                        game.Player.RemoveGold(price);
                        shopCards.RemoveAt(i);
                    }
                }
            }
        }

        // Draw charms section
        string charmsTitle = "Charms";
        Raylib.DrawText(charmsTitle, 50, startCardY + cardHeight + 50, 30, Color.White);

        // Draw charms
        const int charmWidth = 300;
        const int charmHeight = 400;
        const int charmSpacing = 100;
        int totalCharmWidth = (charmWidth * 3) + (charmSpacing * 2);
        int startCharmX = (ScreenWidth - totalCharmWidth) / 2;
        int startCharmY = startCardY + cardHeight + 80;

        for (int i = 0; i < shopCharms.Count; i++)
        {
            Charm charm = shopCharms[i];
            Vector2 position = new Vector2(
                startCharmX + (i * (charmWidth + charmSpacing)),
                startCharmY
            );

            // Check if mouse is hovering over charm
            bool isHovering = mousePos.X >= position.X && 
                            mousePos.X <= position.X + charmWidth &&
                            mousePos.Y >= position.Y && 
                            mousePos.Y <= position.Y + charmHeight;

            // Apply hover effect
            if (isHovering)
            {
                position.Y -= 20;
            }

            // Draw charm box
            Color boxColor = isHovering ? new Color(200, 200, 255, 255) : Color.White;
            Raylib.DrawRectangleRec(
                new Rectangle(position.X, position.Y, charmWidth, charmHeight),
                boxColor
            );
            Raylib.DrawRectangleLinesEx(
                new Rectangle(position.X, position.Y, charmWidth, charmHeight),
                2,
                Color.DarkBlue
            );

            // Draw charm icon
            const int iconSize = 100;
            int iconX = (int)position.X + (charmWidth - iconSize) / 2;
            int iconY = (int)position.Y + 50;
            Texture2D charmTexture = GetCharmTexture(charm.CharmType);
            Raylib.DrawTexturePro(
                charmTexture,
                new Rectangle(0, 0, charmTexture.Width, charmTexture.Height),
                new Rectangle(iconX, iconY, iconSize, iconSize),
                new Vector2(0, 0),
                0,
                Color.White
            );

            // Draw charm name
            Raylib.DrawText(
                charm.Name,
                (int)position.X + 20,
                iconY + iconSize + 20,
                30,
                Color.DarkBlue
            );

            // Draw charm description (wrapped)
            string description = charm.Description;
            string[] words = description.Split(' ');
            string currentLine = "";
            int lineY = iconY + iconSize + 60;
            int maxWidth = charmWidth - 40;

            foreach (string word in words)
            {
                string testLine = currentLine + word + " ";
                int textWidth = (int)Raylib.MeasureTextEx(descriptionFont, testLine, 20, 1).X;
                
                if (textWidth > maxWidth)
                {
                    Raylib.DrawTextPro(
                        descriptionFont,
                        currentLine,
                        new Vector2(position.X + 20, lineY),
                        new Vector2(0, 0),
                        0,
                        20,
                        1,
                        Color.Black
                    );
                    currentLine = word + " ";
                    lineY += 25;
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
                new Vector2(position.X + 20, lineY),
                new Vector2(0, 0),
                0,
                20,
                1,
                Color.Black
            );

            // Draw price
            int price = GetCharmPrice(charm.CharmType);
            string priceText = $"{price} Gold";
            Vector2 priceTextSize = Raylib.MeasureTextEx(descriptionFont, priceText, 24, 1);
            Raylib.DrawTextPro(
                descriptionFont,
                priceText,
                new Vector2(
                    position.X + (charmWidth - priceTextSize.X) / 2,
                    position.Y + charmHeight - 40
                ),
                new Vector2(0, 0),
                0,
                24,
                1,
                Color.Gold
            );

            // Handle charm purchase
            if (isHovering && Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                if (game?.Player?.Gold >= price)
                {
                    // Purchase charm
                    if (game?.Player != null)
                    {
                        game.Player.AddCharm(charm);
                        game.Player.RemoveGold(price);
                        shopCharms.RemoveAt(i);
                    }
                }
            }
        }

        // Draw back button
        const int backButtonWidth = 150;
        const int backButtonHeight = 50;
        Rectangle backButtonRect = new Rectangle(
            ScreenWidth - backButtonWidth - 50,
            ScreenHeight - backButtonHeight - 50,
            backButtonWidth,
            backButtonHeight
        );

        // Check if mouse is hovering over back button
        bool isBackHovering = Raylib.CheckCollisionPointRec(mousePos, backButtonRect);

        // Draw back button
        Color backButtonColor = isBackHovering ? new Color(200, 200, 200, 255) : Color.White;
        Raylib.DrawRectangleRec(backButtonRect, backButtonColor);
        Raylib.DrawRectangleLinesEx(backButtonRect, 2, Color.Gray);

        // Draw back button text
        string backText = "Back";
        int backTextWidth = Raylib.MeasureText(backText, 20);
        Raylib.DrawText(
            backText,
            (int)(backButtonRect.X + (backButtonWidth - backTextWidth) / 2),
            (int)(backButtonRect.Y + (backButtonHeight - 20) / 2),
            20,
            Color.Black
        );

        // Handle back button click
        if (isBackHovering && Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            // Reset shop state
            shopCardsGenerated = false;
            shopCards.Clear();
            shopCharms.Clear();
            // Return to reward screen
            Program.currentScreen = Program.GameScreen.Reward;
        }
    }

    private static Texture2D GetCharmTexture(CharmType charmType)
    {
        switch (charmType)
        {
            case CharmType.StudyGuide:
                return studyGuideTexture;
            case CharmType.CoffeeMug:
                return coffeeMugTexture;
            case CharmType.LuckyPen:
                return luckyPenTexture;
            case CharmType.Bookmark:
                return bookmarkTexture;
            case CharmType.Calculator:
                return calculatorTexture;
            case CharmType.Highlighter:
                return highlighterTexture;
            case CharmType.StickyNotes:
                return stickyNotesTexture;
            case CharmType.StudyTimer:
                return studyTimerTexture;
            case CharmType.FlashCards:
                return flashCardsTexture;
            case CharmType.TextBook:
                return textBookTexture;
            case CharmType.Notebook:
                return notebookTexture;
            case CharmType.SmartWatch:
                return smartWatchTexture;
            case CharmType.StudyGroup:
                return studyGroupTexture;
            case CharmType.AllNighter:
                return allNighterTexture;
            case CharmType.GeniusIdea:
                return geniusIdeaTexture;
            default:
                return studyGuideTexture; // Default texture
        }
    }

    private static int GetCharmPrice(CharmType charmType)
    {
        switch (charmType)
        {
            // Common Charms
            case CharmType.Bookmark:
            case CharmType.Calculator:
            case CharmType.Highlighter:
            case CharmType.StickyNotes:
                return 100;

            // Uncommon Charms
            case CharmType.StudyTimer:
            case CharmType.FlashCards:
            case CharmType.TextBook:
            case CharmType.Notebook:
                return 200;

            // Rare Charms
            case CharmType.SmartWatch:
            case CharmType.StudyGroup:
            case CharmType.AllNighter:
            case CharmType.GeniusIdea:
                return 300;

            default:
                return 100;
        }
    }

    private static void DrawPileOverlay(List<Card> cards, string title)
    {
        // Draw semi-transparent background
        Raylib.DrawRectangle(0, 0, ScreenWidth, ScreenHeight, new Color(0, 0, 0, 180));

        // Draw title
        int titleWidth = Raylib.MeasureText(title, 40);
        Raylib.DrawText(title, ScreenWidth/2 - titleWidth/2, 50, 40, Color.White);

        // Draw cards in a grid layout
        const int cardWidth = 168;
        const int cardHeight = 251;
        const int cardsPerRow = 5;
        const int cardSpacing = 20;
        const int rowSpacing = 30;

        int startX = (ScreenWidth - (cardsPerRow * (cardWidth + cardSpacing))) / 2;
        int startY = 120;

        for (int i = 0; i < cards.Count; i++)
        {
            int row = i / cardsPerRow;
            int col = i % cardsPerRow;

            Vector2 position = new Vector2(
                startX + col * (cardWidth + cardSpacing),
                startY + row * (cardHeight + rowSpacing)
            );

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
            const int padding = 10;
            const int costBoxSize = 28;
            Raylib.DrawRectangle(
                (int)position.X + padding,
                (int)position.Y + padding,
                costBoxSize,
                costBoxSize,
                Color.Red
            );

            // Draw cost number
            string costText = cards[i].CardCost.ToString();
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
                cardNameFont,
                cards[i].Name,
                new Vector2(position.X + costBoxSize + padding * 2 - 30, position.Y + padding + 30),
                new Vector2(0, 0),
                0,
                20,
                1,
                Color.Black
            );

            // Draw description
            string[] words = cards[i].Description.Split(' ');
            string currentLine = "";
            int lineY = (int)position.Y + 46 + 30;
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
        }

        // Draw instruction text
        string instructionText = "Click anywhere to close";
        int instructionWidth = Raylib.MeasureText(instructionText, 20);
        Raylib.DrawText(instructionText, ScreenWidth/2 - instructionWidth/2, ScreenHeight - 50, 20, Color.White);

        // Check for click to close overlay
        float currentTime = (float)Raylib.GetTime();
        if ((Raylib.IsMouseButtonPressed(MouseButton.Left) || Raylib.IsKeyPressed(KeyboardKey.D)) && 
            currentTime - lastClickTime > CLICK_DELAY)
        {
            showDrawPileOverlay = false;
            showDiscardPileOverlay = false;
            lastClickTime = currentTime;
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

    public static int DrawCharmSelectionScreen()
    {
        const int charmWidth = 300;
        const int charmHeight = 400;
        const int padding = 50;
        const int titleHeight = 100;
        const int iconSize = 100;  // Size for charm icons
        // Draw background
        Raylib.DrawTexture(charmBackground, 0, 0, Color.White);
        Raylib.DrawRectangle(0, 0, ScreenWidth, ScreenHeight, new Color(0, 0, 0, 50));

        // Draw title
        string title = "Choose Your Starting Charm";
        Raylib.DrawText(
            title,
            ScreenWidth / 2 - Raylib.MeasureText(title, 40) / 2,
            titleHeight,
            40,
            Color.DarkBlue
        );

        // Draw subtitle
        string subtitle = "Select one charm to help you in your journey";
        Raylib.DrawText(
            subtitle,
            ScreenWidth / 2 - Raylib.MeasureText(subtitle, 20) / 2,
            titleHeight + 50,
            20,
            Color.Gray
        );

        // Calculate positions for the three charms
        int totalWidth = (charmWidth * 3) + (padding * 2);
        int startX = (ScreenWidth - totalWidth) / 2;
        int startY = ScreenHeight / 2 - charmHeight / 2;

        // Charm data
        var charms = new[]
        {
            new { Name = "Study Guide", Description = "Start each combat with 1 extra energy", Type = CharmType.StudyGuide, Texture = studyGuideTexture },
            new { Name = "Coffee Mug", Description = "Start each combat with 1 extra card draw", Type = CharmType.CoffeeMug, Texture = coffeeMugTexture },
            new { Name = "Lucky Pen", Description = "3% chance to draw an extra card when you draw cards", Type = CharmType.LuckyPen, Texture = luckyPenTexture }
        };

        // Draw each charm
        for (int i = 0; i < charms.Length; i++)
        {
            int x = startX + (i * (charmWidth + padding));
            Rectangle charmRec = new Rectangle(x, startY, charmWidth, charmHeight);
            
            // Check for hover
            Vector2 mousePoint = Raylib.GetMousePosition();
            bool isHovered = Raylib.CheckCollisionPointRec(mousePoint, charmRec);
            
            // Draw charm box
            Color boxColor = isHovered ? new Color(200, 200, 255, 150) : new Color(255, 255, 255, 150);
            Raylib.DrawRectangleRec(charmRec, boxColor);
            Raylib.DrawRectangleLinesEx(charmRec, 2, Color.DarkBlue);

            // Draw charm icon
            int iconX = x + (charmWidth - iconSize) / 2;
            int iconY = startY + padding;
            Raylib.DrawTexturePro(
                charms[i].Texture,
                new Rectangle(0, 0, charms[i].Texture.Width, charms[i].Texture.Height),
                new Rectangle(iconX, iconY, iconSize, iconSize),
                new Vector2(0, 0),
                0,
                Color.White
            );

            // Draw charm name
            Raylib.DrawText(
                charms[i].Name,
                x + padding,
                iconY + iconSize + 20,
                30,
                Color.DarkBlue
            );

            // Draw charm description (wrapped)
            string description = charms[i].Description;
            string[] words = description.Split(' ');
            string currentLine = "";
            int lineY = iconY + iconSize + 60;
            int maxWidth = charmWidth - (padding * 2);
            int lineHeight = 25;
            
            foreach (string word in words)
            {
                string testLine = currentLine + (currentLine == "" ? "" : " ") + word;
                if (Raylib.MeasureText(testLine, 20) <= maxWidth)
                {
                    currentLine = testLine;
                }
                else
                {
                    Raylib.DrawText(currentLine, x + padding, lineY, 20, Color.Black);
                    lineY += lineHeight;
                    currentLine = word;
                }
            }
            Raylib.DrawText(currentLine, x + padding, lineY, 20, Color.Black);

            // Check for click
            if (isHovered && Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                return i;
            }
        }

        return -1; // No charm selected
    }

    public static void DrawRestScreen()
    {
        if (game?.CurrentRoom is Rest restRoom)
        {
            // Draw rest background
            Raylib.DrawTexturePro(
                restBackground,
                new Rectangle(0, 0, restBackground.Width, restBackground.Height),
                new Rectangle(0, 0, ScreenWidth, ScreenHeight),
                new Vector2(0, 0),
                0,
                Color.White
            );

            // If choice is made, return to map
            if (restRoom.IsChoiceMade)
            {
                Program.currentScreen = Program.GameScreen.MapSelection;
                return;
            }

            // Draw dialog box
            int dialogBoxWidth = 1200;
            int dialogBoxHeight = 400;
            int dialogBoxX = (ScreenWidth - dialogBoxWidth) / 2;
            int dialogBoxY = (ScreenHeight - dialogBoxHeight) / 2;

            // Draw dialog box background
            Raylib.DrawRectangle(dialogBoxX, dialogBoxY, dialogBoxWidth, dialogBoxHeight, new Color(255, 255, 255, 150));
            Raylib.DrawRectangleLinesEx(
                new Rectangle(dialogBoxX, dialogBoxY, dialogBoxWidth, dialogBoxHeight),
                2,
                Color.DarkGray
            );

            // Draw dialog text
            string dialogText = restRoom.Dialog;
            int dialogWidth = Raylib.MeasureText(dialogText, 30);
            Raylib.DrawText(
                dialogText,
                ScreenWidth/2 - dialogWidth/2,
                dialogBoxY + 50,
                30,
                Color.Black
            );

            // Draw choices
            Vector2 mousePos = Raylib.GetMousePosition();
            int choiceY = dialogBoxY + 150;

            foreach (var choice in restRoom.Choices)
            {
                // Calculate button dimensions
                const int buttonWidth = 550;
                const int buttonHeight = 60;
                const int buttonSpacing = 20;
                
                // Create button rectangle
                Rectangle buttonRect = new Rectangle(
                    ScreenWidth/2 - buttonWidth/2,
                    choiceY,
                    buttonWidth,
                    buttonHeight
                );

                // Check if mouse is hovering over button
                bool isHovering = Raylib.CheckCollisionPointRec(mousePos, buttonRect);

                // Draw button background
                Color buttonColor = isHovering ? new Color(200, 200, 200, 255) : Color.White;
                Raylib.DrawRectangleRec(buttonRect, buttonColor);
                Raylib.DrawRectangleLinesEx(buttonRect, 2, Color.DarkGray);

                // Draw choice text
                string choiceText = choice.Text;
                int choiceWidth = Raylib.MeasureText(choiceText, 20);
                Raylib.DrawText(
                    choiceText,
                    (int)(buttonRect.X + (buttonWidth - choiceWidth) / 2),
                    (int)(buttonRect.Y + (buttonHeight - 20) / 2),
                    20,
                    Color.Black
                );

                // Handle button click
                if (isHovering && Raylib.IsMouseButtonPressed(MouseButton.Left))
                {
                    restRoom.MakeChoice(choice);
                    return; // Exit immediately after making choice
                }

                choiceY += buttonHeight + buttonSpacing;
            }
        }
    }

    public static void DrawTitleScreen()
    {
        // Draw main menu background
        Raylib.DrawTexture(mainMenuBackground, 0, 0, Color.White);
        Raylib.DrawRectangle(0, 0, ScreenWidth, ScreenHeight, new Color(0, 0, 0, 50));

        // Draw title text
        string titleText = "      12 Hours Before Final";
        int titleWidth = Raylib.MeasureText(titleText, 80);
        Raylib.DrawTextPro(
            descriptionFont,
            titleText,
            new Vector2(ScreenWidth/2 - titleWidth/2, 150),
            new Vector2(0, 0),
            0,
            80,
            1,
            Color.Black
        );

        // Draw start button
        const int buttonWidth = 800;
        const int buttonHeight = 300;
        Rectangle startButton = new Rectangle(
            ScreenWidth/2 - buttonWidth/2,
            400,
            buttonWidth,
            buttonHeight
        );

        // Check if mouse is hovering over start button
        Vector2 mousePos = Raylib.GetMousePosition();
        bool isHovering = Raylib.CheckCollisionPointRec(mousePos, startButton);

        // Draw button background
        Color buttonColor = isHovering ? new Color(200, 200, 200, 150) : new Color(255, 255, 255, 150);
        Raylib.DrawRectangleRec(startButton, buttonColor);
        Raylib.DrawRectangleLinesEx(startButton, 2, Color.DarkGray);

        // Draw button text
        string buttonText = "Start Study       ";
        int textWidth = Raylib.MeasureText(buttonText, 20);
        Raylib.DrawTextPro(
            descriptionFont,
            buttonText,
            new Vector2(startButton.X + (buttonWidth - textWidth) / 2 - 120, startButton.Y + (buttonHeight - 20) / 2 - 30),
            new Vector2(0, 0),
            0,
            90,
            1,
            Color.Black
        );

        // Handle button click
        if (isHovering && Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            Program.currentScreen = Program.GameScreen.CharmSelection;
        }
    }
}
