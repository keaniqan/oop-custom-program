using System;
using System.Numerics;
using Raylib_cs;
#nullable disable
namespace MyApp;

public class Card : Item
{
    private CardLocation _cardLocation;
    private int _cardCost;
    private bool _justDrawn;
    private List<ActionCommand> _commands;
    public Card(string name, string description, int price, List<ActionCommand> commands, CardLocation cardLocation, int cardCost) : base(name, description, price)
    {
        _cardLocation = cardLocation;
        _cardCost = cardCost;
        _justDrawn = false;
        _commands = commands;
    }

    public CardLocation CardLocation
    {
        get { return _cardLocation; }
        set { _cardLocation = value; }
    }

    public int CardCost
    {
        get { return _cardCost; }
        set { _cardCost = value; }
    }
    
    public bool JustDrawn
    {
        get { return _justDrawn; }
        set { _justDrawn = value; }
    }
    
    public void DealCard(Card card)
    {
        foreach (var charm in GameRenderer.game.Player.Charms)
        {
            charm.OnCardPlayed(GameRenderer.game.Player, this);
        }
        foreach (var command in _commands)
        {
            command.Execute(GameRenderer.game.Player, GameRenderer.game.CurrentRoom is Combat combatRoom ? combatRoom.Enemy : null, GameRenderer.game);
        }
    }
    public void DrawCard(Card card)
    {
        _cardLocation = CardLocation.Hand;
        _justDrawn = true;
    }
    public void DiscardCard(Card card)
    {
        _cardLocation = CardLocation.DiscardPile;
    }
}
