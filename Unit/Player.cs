using System;
using System.Numerics;
using System.Linq;
using Raylib_cs;
#nullable disable

namespace MyApp;

public class Player: Unit
{
    private int _maxEnergy;
    private int _gold;
    private List<Charm> _charms;
    private List<Card> _cards;
    private double _shopDiscount;


    public Player(string name, int health, int maxHealth, int block, int maxEnergy, List<Effect> effects, int gold, List<Charm> charms, List<Card> cards, double shopDiscount) : base(name, health, maxHealth, block, effects)
    {
        _maxEnergy = maxEnergy;
        _gold = gold;
        _charms = charms;
        _cards = cards;
        _shopDiscount = shopDiscount;

        // Initialize all possible effects with 0 stacks
        foreach (EffectType effectType in Enum.GetValues(typeof(EffectType)))
        {
            _effects.Add(new Effect(effectType, effectType.ToString(), 0, true));
        }

        // Print player information
        Console.WriteLine("\nPlayer Information:");
        Console.WriteLine("------------------");
        Console.WriteLine($"Name: {Name}");
        Console.WriteLine($"Health: {Health}/{MaxHealth}");
        Console.WriteLine($"Block: {Block}");
        Console.WriteLine($"Max Energy: {_maxEnergy}");
        Console.WriteLine($"Gold: {_gold}");
        Console.WriteLine($"Shop Discount: {_shopDiscount:P}");
        
        Console.WriteLine("\nEffects:");
        Console.WriteLine("--------");
        foreach (var effect in Effects)
        {
            Console.WriteLine($"- {effect.EffectType}: {effect.Stack} stacks");
        }
        
        Console.WriteLine("\nCharms:");
        Console.WriteLine("-------");
        foreach (var charm in _charms)
        {
            Console.WriteLine($"- {charm.Name}: {charm.Description}");
        }
        
        Console.WriteLine("\nCards in Deck:");
        Console.WriteLine("--------------");
        foreach (var card in _cards)
        {
            Console.WriteLine($"- {card.Name} (Cost: {card.CardCost})");
        }
        Console.WriteLine();
    }

    public List<Card> Cards
    {
        get { return _cards; }
        set { _cards = value; }
    }
    
    public List<Charm> Charms
    {
        get { return _charms; }
        set { _charms = value; }
    }

    public int Gold
    {
        get { return _gold; }
        set { _gold = value; }
    }

    public void AddCharm(Charm charm){
        _charms.Add(charm);
    }
    public void AddCard(Card card){
        _cards.Add(card);
    }
    public void RemoveCard(Card card){
        _cards.Remove(card);
    }
    public void AddGold(int gold){
        _gold += gold;
    }
    public void RemoveGold(int gold){
        _gold -= gold;
    }
    public void AddShopDiscount(double shopDiscount){
        _shopDiscount += shopDiscount;
    }
    public void PlayCard(Card card){
        if (GameRenderer.game?.Map?.Rooms?.Count > 0 && 
            GameRenderer.game.Map.Rooms[0] is Combat combatRoom)
        {
            if (combatRoom.CurrentEnergy >= card.CardCost)
            {
                combatRoom.CurrentEnergy -= card.CardCost;
                card.DealCard(card);
                card.DiscardCard(card);
            }
        }
    }
    public void DiscardHand(){
        foreach (var card in _cards){
            if (card.CardLocation == CardLocation.Hand){
                card.DiscardCard(card);
            }
        }
    }
    public void DrawCards(int amount){
        int cardsDrawn = 0;
        while (cardsDrawn < amount)
        {
            // If draw pile is empty, refill from discard and shuffle
            if (!_cards.Any(c => c.CardLocation == CardLocation.DrawPile))
            {
                EmptyDiscardPiletoDrawPile();
                ShuffleDeck();
            }
            // Try to draw a card
            var card = _cards.FirstOrDefault(c => c.CardLocation == CardLocation.DrawPile);
            if (card != null)
            {
                card.DrawCard(card);
                cardsDrawn++;
            }
            else
            {
                // No more cards to draw
                break;
            }
        }
    }
    public void ShuffleDeck(){
        Random random = new Random();
        
        // Get only cards in draw pile
        var drawPile = _cards.Where(c => c.CardLocation == CardLocation.DrawPile).ToList();
        
        // Fisher-Yates shuffle algorithm on the draw pile
        for (int i = drawPile.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            Card temp = drawPile[i];
            drawPile[i] = drawPile[j];
            drawPile[j] = temp;
        }
        
        // Update the original list with shuffled draw pile
        int drawPileIndex = 0;
        for (int i = 0; i < _cards.Count; i++)
        {
            if (_cards[i].CardLocation == CardLocation.DrawPile)
            {
                _cards[i] = drawPile[drawPileIndex++];
            }
        }
    }
    public void EmptyDiscardPiletoDrawPile(){
        foreach (var card in _cards){
            if (card.CardLocation == CardLocation.DiscardPile){
                card.CardLocation = CardLocation.DrawPile;
            }
        }
    }

    public void RemoveCharm(Charm charm)
    {
        _charms.Remove(charm);
    }

    public void TriggerStartOfCombat()
    {
        foreach (var charm in _charms)
        {
            charm.ExecuteActions(this);
        }
    }

    public void TriggerEndOfTurn()
    {
        foreach (var charm in _charms)
        {
            charm.ExecuteActions(this);
        }
    }

    public void TriggerCardPlayed(Card card)
    {
        foreach (var charm in _charms)
        {
            charm.ExecuteActions(this);
        }
    }

    public void TriggerDamageTaken(int damage)
    {
        foreach (var charm in _charms)
        {
            charm.ExecuteActions(this);
        }
    }

    public void TriggerEnemyDamaged(Enemy enemy, int damage)
    {
        foreach (var charm in _charms)
        {
            charm.ExecuteActions(this);
        }
    }

    public void TriggerBlockGained(int block)
    {
        foreach (var charm in _charms)
        {
            charm.ExecuteActions(this);
        }
    }

    public override void EndTurn(){  
        DiscardHand();
        
        // Reduce stacks of vulnerable, frail, and weak effects by one
        var vulnerableEffect = _effects.FirstOrDefault(e => e.EffectType == EffectType.Vulnerable);
        if (vulnerableEffect != null && vulnerableEffect.Stack > 0)
        {
            vulnerableEffect.Stack--;
        }

        var frailEffect = _effects.FirstOrDefault(e => e.EffectType == EffectType.Frail);
        if (frailEffect != null && frailEffect.Stack > 0)
        {
            frailEffect.Stack--;
        }

        var weakEffect = _effects.FirstOrDefault(e => e.EffectType == EffectType.Weak);
        if (weakEffect != null && weakEffect.Stack > 0)
        {
            weakEffect.Stack--;
        }

        // Trigger end of turn effects for charms
        TriggerEndOfTurn();
    }

    public int MaxEnergy
    {
        get { return _maxEnergy; }
        set { _maxEnergy = value; }
    }

    public void AddEffect(Effect effect)
    {
        _effects.Add(effect);
    }
    
    
}



