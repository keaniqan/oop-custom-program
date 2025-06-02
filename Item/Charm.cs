#nullable disable
namespace MyApp;

public class Charm: Item
{
    private CharmType _charmType;
    private int _charges;
    private int _maxCharges;

    public Charm(string name, string description, CharmType charmType, List<Action> actions, int maxCharges, int price) : base(name, description, price, actions)
    {
        _charmType = charmType;
        _maxCharges = maxCharges;
        _charges = maxCharges;
    }

    public CharmType CharmType
    {
        get { return _charmType; }
        set { _charmType = value; }
    }

    public int Charges
    {
        get { return _charges; }
        set { _charges = value; }
    }

    public int MaxCharges
    {
        get { return _maxCharges; }
        set { _maxCharges = value; }
    }

    public void OnCardDraw(Player player)
    {
        // Handle card draw triggers based on charm type
        switch (_charmType)
        {
            case CharmType.LuckyPen:
                // 10% chance to draw an extra card
                if (Random.Shared.NextDouble() < 0.10)
                {
                    player.DrawCards(1);
                }
                break;
            case CharmType.FlashCards:
                // This is handled separately in the shuffle event
                break;
            // Add more card draw related charm effects here
        }
    }

    public void OnDeckShuffle(Player player)
    {
        // Handle shuffle triggers based on charm type
        if (_charmType == CharmType.FlashCards)
        {
            player.DrawCards(1);
        }
    }

    public void OnCardPlayed(Player player, Card card)
{
    if (_charmType == CharmType.GeniusIdea)
    {
        if (Random.Shared.NextDouble() < 0.25)
        {
            // Play the card effect without triggering OnCardPlayed again
                player.PlayCard(card);
            }
        }
        if (_charmType == CharmType.StickyNotes)
        {
            player.AddBlock(1); // Actually give the player block
        }
    }

    public void OnTurnStart(Player player)
    {
        if (_charmType == CharmType.AllNighter)
        {
            if (GameRenderer.game?.CurrentRoom is Combat combatRoom)
            {
                combatRoom.AddEnergy(1);
            }
        }
    }

    public void ExecuteActions(Player player)
    {
        foreach (var action in Actions)
        {
            if (action.ActionType == ActionType.Draw)
            {
                player.DrawCards(action.Value);
            }
            else if (action.ActionType == ActionType.Energy)
            {
                if (GameRenderer.game?.CurrentRoom is Combat combatRoom)
                {
                    combatRoom.AddEnergy(action.Value);
                }
            }
            else if (action.ActionType == ActionType.Effect)
            {
                player.AddEffect(new Effect(action.EffectType.Value, action.EffectType.ToString(), action.Value, true));
            }
        }
    }
}

public enum CharmType
{
    // Starter Charms
    StudyGuide,      // Start each combat with 1 extra energy
    CoffeeMug,       // Start each combat with 1 extra card draw
    LuckyPen,        // 10% chance to draw an extra card when you draw cards
    
    // Common Charms
    Bookmark,        // Draw 1 extra card at the start of your turn
    Calculator,      // Gain 1 energy at the start of your turn
    Highlighter,     // Cards that cost 0 deal 2 more damage
    StickyNotes,     // When you play a card, gain 1 block
    
    // Uncommon Charms
    StudyTimer,      // Every 3 turns, gain 1 energy
    FlashCards,      // When you shuffle your draw pile, draw 1 card
    TextBook,        // Start each combat with 2 Strength
    Notebook,        // Start each combat with 2 Dexterity
    
    // Rare Charms
    SmartWatch,      // At the start of your turn, gain 1 energy and draw 1 card
    StudyGroup,      // Your first card each turn costs 0
    AllNighter,      // Start each combat with 2 extra energy
    GeniusIdea       // When you play a card, there's a 25% chance to play it again
}

