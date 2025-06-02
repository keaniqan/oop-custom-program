using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using Raylib_cs;
#nullable disable
namespace MyApp;

public class Game
{
    private List<Room> _rooms;
    private Player _player;
    private Room _currentRoom;

    private List<List<Room>> _layers = new List<List<Room>>();
    public Game(Player player)
    {
        _rooms = new List<Room>();
        _layers = new List<List<Room>>();  // Initialize layers once
        _player = player;
        _currentRoom = _rooms.Count > 0 ? _rooms[0] : null;
        CreateMap();
    }

    public List<Room> Rooms
    {
        get { return _rooms; }
        set { _rooms = value; }
    }

    public Player Player
    {
        get { return _player; }
        set { _player = value; }
    }

    public Room CurrentRoom
    {
        get { return _currentRoom; }
        set { _currentRoom = value; }
    }

    public List<List<Room>> Layers
    {
        get { return _layers; }
        set { _layers = value; }
    }

    public static List<Card> CreateStarterDeck()
    {
        Action strikeAction = new(ActionType.Attack, 30, null, false);
        Action bashAttack = new(ActionType.Attack, 8, null, false);
        Action addVulnerable = new(ActionType.Effect, 1, EffectType.Vulnerable, false);
        Action addWeak = new(ActionType.Effect, 1, EffectType.Weak, false);
        Action drawCards = new(ActionType.Draw, 2, null, true);
        Action gainEnergy = new(ActionType.Energy, 1, null, true);
        Action defendAction = new(ActionType.Block, 5, null, true);
        Action addStrength = new(ActionType.Effect, 1, EffectType.StrengthUp, true);

        return new List<Card>
        {
            new Card("Study", "Deal 6 damage.", 1, new List<ActionCommand> { new AttackCommand(6, EffectTarget.Enemy) }, CardLocation.DrawPile, 1),
            new Card("Study", "Deal 6 damage.", 1, new List<ActionCommand> { new AttackCommand(6, EffectTarget.Enemy) }, CardLocation.DrawPile, 1),
            new Card("Study", "Deal 6 damage.", 1, new List<ActionCommand> { new AttackCommand(6, EffectTarget.Enemy) }, CardLocation.DrawPile, 1),
            new Card("Study", "Deal 6 damage.", 1, new List<ActionCommand> { new AttackCommand(6, EffectTarget.Enemy) }, CardLocation.DrawPile, 1),
            new Card("Memory", "Gain 5 block.", 1, new List<ActionCommand> { new BlockCommand(5) }, CardLocation.DrawPile, 1),
            new Card("Memory", "Gain 5 block.", 1, new List<ActionCommand> { new BlockCommand(5) }, CardLocation.DrawPile, 1),
            new Card("Memory", "Gain 5 block.", 1, new List<ActionCommand> { new BlockCommand(5) }, CardLocation.DrawPile, 1),
            new Card("Memory", "Gain 5 block.", 1, new List<ActionCommand> { new BlockCommand(5) }, CardLocation.DrawPile, 1),
            new Card("Unthinking", "Apply 2 Weak.", 80, new List<ActionCommand> { new ApplyEffectCommand(EffectType.Weak, 2, EffectTarget.Enemy) }, CardLocation.DrawPile, 1),
            new Card("New Lesson", "Gain 1 Strength.", 82, new List<ActionCommand> {new ApplyEffectCommand(EffectType.StrengthUp, 1, EffectTarget.Player)}, CardLocation.DrawPile, 1),
            new Card("Crit Thinking", "Deal 8 damage. Apply 2 Vulnerable.", 2, new List<ActionCommand> {new AttackCommand(8, EffectTarget.Enemy), new ApplyEffectCommand(EffectType.Vulnerable, 2, EffectTarget.Enemy)}, CardLocation.DrawPile, 2),     
        };
    }

    public static List<Card> CreateRewardCardPool()
    {

        return new List<Card>
        {
            // Attack Cards
            new Card("Quick Study", "Deal 6 damage. Draw 1 card.", 70, new List<ActionCommand> { new AttackCommand(6, EffectTarget.Enemy), new DrawCommand(1) }, CardLocation.DrawPile, 1),
            new Card("Deep Analysis", "Deal 8 damage. Apply 2 Vulnerable.", 70, new List<ActionCommand> { new AttackCommand(8, EffectTarget.Enemy), new ApplyEffectCommand(EffectType.Vulnerable, 2, EffectTarget.Enemy), new ApplyEffectCommand(EffectType.Vulnerable, 2, EffectTarget.Enemy) }, CardLocation.DrawPile, 2),
            new Card("Critical Review", "Deal 12 damage. Apply 1 Weak.", 85, new List<ActionCommand> { new AttackCommand(12, EffectTarget.Enemy), new ApplyEffectCommand(EffectType.Weak, 1, EffectTarget.Enemy) }, CardLocation.DrawPile, 2),
            new Card("Unthinking", "Apply 2 Weak.", 80, new List<ActionCommand> {new ApplyEffectCommand(EffectType.Weak, 2, EffectTarget.Enemy), new ApplyEffectCommand(EffectType.Weak, 2, EffectTarget.Enemy)}, CardLocation.DrawPile, 1),
            new Card("Free Knowledge", "Deal 5 Damage", 71, new List<ActionCommand> { new AttackCommand(5, EffectTarget.Enemy)}, CardLocation.DrawPile, 0),
            new Card("Memory of Study", "Deal 6 Damage. Gain 6 Block.", 91, new List<ActionCommand> { new AttackCommand(6, EffectTarget.Enemy), new BlockCommand(6)}, CardLocation.DrawPile, 1),
            new Card("Speed Reading", "Deal 8 Damage. Draw 1 Card.", 81, new List<ActionCommand> { new AttackCommand(8, EffectTarget.Enemy), new DrawCommand(1)}, CardLocation.DrawPile, 1),
            new Card("Spaced Repitition", "Deal 3 Damage 3 Times. Draw 1 Card.", 71, new List<ActionCommand> { new AttackCommand(3, EffectTarget.Enemy), new AttackCommand(3, EffectTarget.Enemy), new AttackCommand(3, EffectTarget.Enemy), new DrawCommand(1)}, CardLocation.DrawPile, 1),
            new Card("Double Major", "Deal 5 Damage twice", 81, new List<ActionCommand> { new AttackCommand(5, EffectTarget.Enemy), new AttackCommand(5, EffectTarget.Enemy)}, CardLocation.DrawPile, 1),
            new Card("Sleep Dept", "Take 2 Damage, Deal 15 Damage", 81, new List<ActionCommand> { new SelfDamageCommand(2), new AttackCommand(15, EffectTarget.Enemy)}, CardLocation.DrawPile, 1),
            new Card("Topic Synergy", "Heal 4 hp, Deal 4 Damage", 111, new List<ActionCommand> { new HealCommand(4), new AttackCommand(4, EffectTarget.Enemy)}, CardLocation.DrawPile, 2),
            new Card("Eureka!", "Deal 28 Damage", 131, new List<ActionCommand> { new AttackCommand(28, EffectTarget.Enemy)}, CardLocation.DrawPile, 3),
            new Card("Passtask 13", "Deal 13 Damage 1 times", 111, new List<ActionCommand> { new AttackCommand(1, EffectTarget.Enemy), new AttackCommand(1, EffectTarget.Enemy), new AttackCommand(1, EffectTarget.Enemy), new AttackCommand(1, EffectTarget.Enemy), new AttackCommand(1, EffectTarget.Enemy), new AttackCommand(1, EffectTarget.Enemy), new AttackCommand(1, EffectTarget.Enemy), new AttackCommand(1, EffectTarget.Enemy), new AttackCommand(1, EffectTarget.Enemy), new AttackCommand(1, EffectTarget.Enemy), new AttackCommand(1, EffectTarget.Enemy), new AttackCommand(1, EffectTarget.Enemy), new AttackCommand(1, EffectTarget.Enemy)}, CardLocation.DrawPile, 3),
            new Card("Sleepless", "Deal 15 Damage, lost 1 strength", 111, new List<ActionCommand> { new AttackCommand(15, EffectTarget.Enemy), new ApplyEffectCommand(EffectType.StrengthUp, -1, EffectTarget.Player)}, CardLocation.DrawPile, 2),
            new Card("Mental Block", "Gain 4 block. Gain 1 Dexterity.", 90, new List<ActionCommand> { new BlockCommand(4), new ApplyEffectCommand(EffectType.DexterityUp, 1, EffectTarget.Player) }, CardLocation.DrawPile, 1),
            new Card("Study Break", "Gain 8 block. Draw 1 card.", 90, new List<ActionCommand> { new BlockCommand(8), new DrawCommand(1) }, CardLocation.DrawPile, 1),
            new Card("Last Minute Cram", "Gain 9 block. Gain 1 Frail.", 77, new List<ActionCommand> { new BlockCommand(12), new ApplyEffectCommand(EffectType.Frail, 1, EffectTarget.Player) }, CardLocation.DrawPile, 1),
            new Card("Project Betrayal", "Gain 16 block. Gain 2 Thorns.", 81, new List<ActionCommand> { new BlockCommand(16), new ApplyEffectCommand(EffectType.Thorn, 2, EffectTarget.Player) }, CardLocation.DrawPile, 2),
            new Card("Study Group", "Gain 1 Strength. Draw 1 card.", 71, new List<ActionCommand> { new ApplyEffectCommand(EffectType.StrengthUp, 1, EffectTarget.Player), new DrawCommand(1) }, CardLocation.DrawPile, 1),
            new Card("Coffee Break", "Gain 1 Energy. Draw 1 card.", 100, new List<ActionCommand> { new GainEnergyCommand(1), new DrawCommand(1) }, CardLocation.DrawPile, 0),
            new Card("All-Nighter", "Gain 2 Strength. Gain 2 Dexterity.", 82, new List<ActionCommand> { new ApplyEffectCommand(EffectType.StrengthUp, 2, EffectTarget.Player), new ApplyEffectCommand(EffectType.StrengthUp, 2, EffectTarget.Player), new ApplyEffectCommand(EffectType.DexterityUp, 2, EffectTarget.Player), new ApplyEffectCommand(EffectType.DexterityUp, 2, EffectTarget.Player) }, CardLocation.DrawPile, 2),
            new Card("New Lesson", "Gain 1 Strength.", 82, new List<ActionCommand> {new ApplyEffectCommand(EffectType.StrengthUp, 1, EffectTarget.Player)}, CardLocation.DrawPile, 1),
            new Card("Power Nap", "Heal 4 HP. Gain 1 Energy.", 104, new List<ActionCommand> { new HealCommand(4), new GainEnergyCommand(1) }, CardLocation.DrawPile, 1),
            new Card("Study Guide", "Draw 2 cards. Gain 1 Energy.", 80, new List<ActionCommand> { new DrawCommand(2), new GainEnergyCommand(1) }, CardLocation.DrawPile, 1),
            new Card("Defend Notes", "Gain 5 block. Gain 1 Thorn.", 91, new List<ActionCommand> { new BlockCommand(5), new ApplyEffectCommand(EffectType.Thorn, 1, EffectTarget.Player) }, CardLocation.DrawPile, 1),
            new Card("Ask ChatGPT", "Gain 2 Buffer", 151, new List<ActionCommand> { new ApplyEffectCommand(EffectType.Buffer, 2, EffectTarget.Player)}, CardLocation.DrawPile, 3),
            new Card("Distraction", "Apply 3 Vulnerable and 3 Weak", 71, new List<ActionCommand> { new ApplyEffectCommand(EffectType.Vulnerable, 3, EffectTarget.Enemy), new ApplyEffectCommand(EffectType.Weak, 3, EffectTarget.Enemy)}, CardLocation.DrawPile, 2),
            new Card("Aim for Pass", "Deal 3 damage, gain 2 block, draw 1 card", 111, new List<ActionCommand> { new AttackCommand(3, EffectTarget.Enemy), new BlockCommand(2), new DrawCommand(1)}, CardLocation.DrawPile, 0),
            new Card("Eduroam Lag", "Deal 4 damage. Reduce Enemy Strength by 2", 90, new List<ActionCommand> { new AttackCommand(4, EffectTarget.Enemy), new ApplyEffectCommand(EffectType.StrengthUp, -2, EffectTarget.Enemy) }, CardLocation.DrawPile, 1),
            new Card("Class Cancelled!", "Lose 5 hp. Gain 10 vulnerable. Deal 15 Damage, Apply 20 Vulnerable", 90, new List<ActionCommand> { new SelfDamageCommand(5), new ApplyEffectCommand(EffectType.Vulnerable, 10, EffectTarget.Player), new AttackCommand(15, EffectTarget.Enemy), new ApplyEffectCommand(EffectType.Vulnerable, 20, EffectTarget.Enemy) }, CardLocation.DrawPile, 3),
            new Card("Coffee Break", "Deal 2 Damage. Gain 1 Thorn", 71, new List<ActionCommand> { new AttackCommand(2, EffectTarget.Enemy), new ApplyEffectCommand(EffectType.Thorn, 1, EffectTarget.Player) }, CardLocation.DrawPile, 0),
            new Card("Burntout Wave", "Gain 20 Block and 5 Thorn", 90, new List<ActionCommand> { new BlockCommand(20), new ApplyEffectCommand(EffectType.Thorn, 5, EffectTarget.Player) }, CardLocation.DrawPile, 3),
            new Card("Submitted 11:59", "Deal 36 damage. Lose 3 Strength    ", 71, new List<ActionCommand> { new AttackCommand(36, EffectTarget.Enemy), new ApplyEffectCommand(EffectType.StrengthUp, -3, EffectTarget.Player) }, CardLocation.DrawPile, 1),
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

    public static Charm CreateStarterCharm(int index)
    {
        switch (index)
        {
            case 0:
                return new Charm(
                    "Study Guide",
                    "Start each combat with 1 extra energy",
                    CharmType.StudyGuide,
                    new List<ActionCommand> { new GainEnergyCommand(1) },
                    0,
                    100
                );
            case 1:
                return new Charm(
                    "Coffee Mug",
                    "Start each combat with 1 extra card draw",
                    CharmType.CoffeeMug,
                    new List<ActionCommand> { new DrawCommand(1) },
                    0,
                    100
                );
            case 2:
                return new Charm(
                    "Lucky Pen",
                    "3% chance to draw an extra card when you draw cards",
                    CharmType.LuckyPen,
                    new List<ActionCommand> { new DrawCommand(1) },
                    0,
                    100
                );
            default:
                return new Charm(
                    "Study Guide",
                    "Start each combat with 1 extra energy",
                    CharmType.StudyGuide,
                    new List<ActionCommand> { new GainEnergyCommand(1) },
                    0,
                    100
                );
        }
    }

    public static List<Charm> CreateShopCharmPool()
    {
        var charmPool = new List<Charm>();

        // Common Charms
        charmPool.Add(new Charm(
            "Bookmark",
            "Draw 1 extra card at the start of your turn",
            CharmType.Bookmark,
            new List<ActionCommand> { new DrawCommand(1) },
            0,
            100
        ));

        charmPool.Add(new Charm(
            "Highlighter",
            "Cards that cost 0 deal 2 more damage",
            CharmType.Highlighter,
            new List<ActionCommand> { new ApplyEffectCommand(EffectType.StrengthUp, 2, EffectTarget.Enemy) },
            0,
            100
        ));

        charmPool.Add(new Charm(
            "Sticky Notes",
            "When you play a card, gain 1 block",
            CharmType.StickyNotes,
            new List<ActionCommand> { new BlockCommand(1) },
            0,
            100
        ));

        // Uncommon Charms
        charmPool.Add(new Charm(
            "Study Timer",
            "Start each combat with enemy 2 weak 2 vulnerable",
            CharmType.StudyTimer,
            new List<ActionCommand> { new ApplyEffectCommand(EffectType.Weak, 2, EffectTarget.Enemy), new ApplyEffectCommand(EffectType.Vulnerable, 2, EffectTarget.Enemy) },
            0,
            100
        ));

        charmPool.Add(new Charm(
            "Flash Cards",
            "When you shuffle your draw pile, draw 1 card",
            CharmType.FlashCards,
            new List<ActionCommand> { new DrawCommand(1) },
            0,
            100
        ));

        charmPool.Add(new Charm(
            "Text Book",
            "Start each combat with 2 Strength",
            CharmType.TextBook,
            new List<ActionCommand> { new ApplyEffectCommand(EffectType.StrengthUp, 2, EffectTarget.Player) },
            0,
            100
        ));

        charmPool.Add(new Charm(
            "Notebook",
            "Start each combat with 2 Dexterity",
            CharmType.Notebook,
            new List<ActionCommand> { new ApplyEffectCommand(EffectType.DexterityUp, 2, EffectTarget.Player) },
            0,
            100
        ));

        // Rare Charms
        charmPool.Add(new Charm(
            "Smart Watch",
            "At the start of your turn, gain 1 energy and draw 1 card",
            CharmType.SmartWatch,
            new List<ActionCommand> { 
                new GainEnergyCommand(1),
                new DrawCommand(1)
            },
            0,
            100
        ));

        charmPool.Add(new Charm(
            "Study Group",
            "Start Combat with 1 buffer",
            CharmType.StudyGroup,
            new List<ActionCommand> { new ApplyEffectCommand(EffectType.Buffer, 1, EffectTarget.Player) },
            0,
            100
        ));

        charmPool.Add(new Charm(
            "All-Nighter",
            "Gain 1 energy each turn",
            CharmType.AllNighter,
            new List<ActionCommand> { new GainEnergyCommand(1) },
            0,
            100
        ));

        charmPool.Add(new Charm(
            "Genius Idea",
            "When you play a card, there's a 25% chance to play it again",
            CharmType.GeniusIdea,
            new List<ActionCommand> { new ApplyEffectCommand(EffectType.Buffer, 0, EffectTarget.Player) },
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

    public void CreateMap()
    {
        // Create enemy templates
        var basicEnemyTemplates = new List<EnemyTemplate>
        {
            new EnemyTemplate("Pre-Algebra", 35, 35, EnemyType.Basic, 4),
            new EnemyTemplate("English Notes", 40, 40, EnemyType.Basic, 4),
            new EnemyTemplate("Biology Lab", 45, 45, EnemyType.Basic, 2),
            new EnemyTemplate("Chemistry Test", 40, 40, EnemyType.Basic, 2)
        };
        var eliteEnemyTemplates = new List<EnemyTemplate>
        {
            new EnemyTemplate("Intro Calculus", 50, 50, EnemyType.Elite, 1),
            new EnemyTemplate("1000 Words Essay", 50, 50, EnemyType.Elite, 1),
            new EnemyTemplate("Plant Anatomy", 50, 50, EnemyType.Elite, 6),
            new EnemyTemplate("World History", 50, 50, EnemyType.Elite, 7),
        };
        var eliteEnemyTemplate = new EnemyTemplate("Elite Enemy", 50, 50, EnemyType.Elite, 5);
        var bossTemplate = new EnemyTemplate("Boss Enemy", 75, 75, EnemyType.Boss, 3);
        var firstRoomTemplate = new EnemyTemplate("Intro Math", 30, 30, EnemyType.Basic, 4);

        // Create event templates
        var eventTemplates = new List<EventTemplate>
        {
            new EventTemplate(
                "Study Group",
                "A group of students are studying together. They invite you to join.",
                new List<EventChoice>
                {
                    new EventChoice("Join them", 0, 10, null, null),
                    new EventChoice("Decline politely", 50, 0, null, null)
                }
            ),
            new EventTemplate(
                "Coffee Shop",
                "You find a coffee shop. The barista offers you a free coffee.",
                new List<EventChoice>
                {
                    new EventChoice("Accept the coffee", 0, 15, null, null),
                    new EventChoice("Buy a coffee for everyone", -50, 20, null, null)
                }
            ),
            new EventTemplate(
                "Library",
                "The library has a special collection of study materials.",
                new List<EventChoice>
                {
                    new EventChoice("Study intensively", 0, -10, Game.CreateStarterDeck()[0], null),
                    new EventChoice("Take a quick look", 25, 0, null, null)
                }
            ),
            new EventTemplate(
                "Office Hours",
                "Your professor is holding office hours.",
                new List<EventChoice>
                {
                    new EventChoice("Ask for help", 0, 0, null, Game.CreateStarterCharm(0)),
                    new EventChoice("Skip it", 25, 0, null, null)
                }
            )
        };

        // Create a pool of room templates
        var shufflePool = new List<RoomTemplate>();
        var rng = new Random();
        
        // Add rest rooms
        for (int i = 0; i < 2; i++)
        {
            shufflePool.Add(new RestRoomTemplate());
        }
        
        // Add basic enemies
        for (int i = 0; i < 13; i++)
        {
            // Randomly select a basic enemy template
            int templateIndex = rng.Next(basicEnemyTemplates.Count);
            shufflePool.Add(new EnemyRoomTemplate(basicEnemyTemplates[templateIndex]));
        }
        
        // Add elite enemies
        for (int i = 0; i < 4; i++)
        {       
            int templateIndex = rng.Next(eliteEnemyTemplates.Count);
            shufflePool.Add(new EnemyRoomTemplate(eliteEnemyTemplates[templateIndex]));
        }

        // Add events
        for (int i = 0; i < 4; i++)
        {
            int templateIndex = rng.Next(eventTemplates.Count);
            shufflePool.Add(new EventRoomTemplate(eventTemplates[templateIndex]));
        }

        // Shuffle the pool
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
        shufflePool.Add(new EnemyRoomTemplate(bossTemplate));
        // Insert the first room at the start
        shufflePool.Insert(0, new EnemyRoomTemplate(firstRoomTemplate));

        // Create rooms from templates
        var rooms = new List<Room>();
        foreach (var template in shufflePool)
        {
            var newRoom = template.CreateRoom();
            rooms.Add(newRoom);
        }

        // Set the rooms in the game's map
        _rooms = rooms;
    }

    private abstract class RoomTemplate
    {
        public abstract Room CreateRoom();
    }

    private class EnemyRoomTemplate : RoomTemplate
    {
        private EnemyTemplate _enemyTemplate;

        public EnemyRoomTemplate(EnemyTemplate enemyTemplate)
        {
            _enemyTemplate = enemyTemplate;
        }

        public override Room CreateRoom()
        {
            return new Combat(false, true, true, _enemyTemplate.CreateEnemy(), _enemyTemplate.Type, TurnPhase.PlayerStart, 3);
        }
    }

    private class EventRoomTemplate : RoomTemplate
    {
        private EventTemplate _eventTemplate;

        public EventRoomTemplate(EventTemplate eventTemplate)
        {
            _eventTemplate = eventTemplate;
        }

        public override Room CreateRoom()
        {
            return new Event(false, true, true, _eventTemplate.Dialog, _eventTemplate.Choices);
        }
    }

    // Rest rooms
    
    private class RestRoomTemplate : RoomTemplate
    {
        public override Room CreateRoom()
        {
            return new Rest(false, true, true);
        }
    }

    private class EventTemplate
    {
        public string Title { get; set; }
        public string Dialog { get; set; }
        public List<EventChoice> Choices { get; set; }

        public EventTemplate(string title, string dialog, List<EventChoice> choices)
        {
            Title = title;
            Dialog = dialog;
            Choices = choices;
        }
    }

    private class EnemyTemplate
    {
        public string Name { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public EnemyType Type { get; set; }
        public int TextureIndex { get; set; }
        public EnemyTemplate(string name, int health, int maxHealth, EnemyType type, int textureIndex)
        {
            Name = name;
            Health = health;
            MaxHealth = maxHealth;
            Type = type;
            TextureIndex = textureIndex;
        }

        public Enemy CreateEnemy()
        {
            return new Enemy(Name, Health, MaxHealth, 0, new List<Effect>(), Type, TextureIndex);
        }
    }
}


