#nullable disable
namespace MyApp;

public class Charm: Item
{
    private CharmType _charmType;


    public Charm(string name, string description, CharmType charmType, int maxCharges, int price) : base(name, description, price)
    {
        _charmType = charmType;
    }

    public CharmType CharmType
    {
        get { return _charmType; }
        set { _charmType = value; }
    }

    public void OnCardDraw(Player player)
    {
        // Handle card draw triggers based on charm type
        switch (_charmType)
        {
            case CharmType.LuckyPen:
                // 10% chance to draw an extra card
                if (Random.Shared.NextDouble() < 0.03)
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



    public void OnCardPlayed(Player player, Card card)
{
    if (_charmType == CharmType.GeniusIdea)
        {
            if (Random.Shared.NextDouble() < 0.15)
            {
            // Play the card effect without triggering OnCardPlayed again
                player.PlayCard(card);
            }
        }
        if (_charmType == CharmType.StickyNotes)
        {
            player.AddBlock(1); // Actually give the player block
        }
        if (_charmType == CharmType.Highlighter)
        {
            if (Random.Shared.NextDouble() < 0.02)
            {
                if (GameRenderer.game?.CurrentRoom is Combat combatRoom)
                {
                    combatRoom.AddEnergy(1);
                }
            }
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
        if (_charmType == CharmType.Bookmark)
        {
            player.DrawCards(1);
        }
    }

    public void OnStartOfCombat(Player player)
    {
        if (_charmType == CharmType.StudyGuide)
        {
            if (GameRenderer.game?.CurrentRoom is Combat combatRoom)
            {
                combatRoom.AddEnergy(1);
            }
        }
        if (_charmType == CharmType.CoffeeMug)
        {
            player.DrawCards(1);
        }
        if (_charmType == CharmType.StudyTimer)
        {
            if (GameRenderer.game?.CurrentRoom is Combat combatRoom)
            {
                if (combatRoom.TurnCount % 3 == 0)
                {
                    combatRoom.Enemy.AddEffectStack(EffectType.Vulnerable, 2);
                    combatRoom.Enemy.AddEffectStack(EffectType.Weak, 2);
                }
            }
        }
        if (_charmType == CharmType.TextBook)
        {
            player.AddEffectStack(EffectType.StrengthUp, 2);
        }
        if (_charmType == CharmType.Notebook)
        {
            player.AddEffectStack(EffectType.DexterityUp, 2);
        }
        if (_charmType == CharmType.SmartWatch)
        {
            if (GameRenderer.game?.CurrentRoom is Combat combatRoom)
            {
                combatRoom.AddEnergy(1);
            }
            player.DrawCards(1);
        }
        if (_charmType == CharmType.StudyGroup)
        {
            player.AddEffectStack(EffectType.Buffer, 1);
        }
        if (_charmType == CharmType.FlashCards)
        {
            if (GameRenderer.game?.CurrentRoom is Combat combatRoom)
            {
                combatRoom.Enemy.Health -= 15;
            }
        }
    }   
}
