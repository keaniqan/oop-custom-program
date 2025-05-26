using System;
using System.Numerics;
using Raylib_cs;
#nullable disable
namespace MyApp;

public class Card : Item
{
    private CardLocation _cardLocation;
    private int _cardCost;
    private AffinityType _cardAffinity;
    private bool _isPower;
    private bool _justDrawn;
    public Card(string name, string description, int price, List<Action> actions, CardLocation cardLocation, int cardCost, AffinityType cardAffinity, bool isPower) : base(name, description, price, actions)
    {
        _cardLocation = cardLocation;
        _cardCost = cardCost;
        _cardAffinity = cardAffinity;
        _isPower = isPower;
        _justDrawn = false;
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
    public AffinityType CardAffinity
    {
        get { return _cardAffinity; }
        set { _cardAffinity = value; }
    }
    public bool IsPower
    {
        get { return _isPower; }
        set { _isPower = value; }
    }
    public bool JustDrawn
    {
        get { return _justDrawn; }
        set { _justDrawn = value; }
    }
    
    public void DealCard(Card card)
    {
        foreach (var action in card.Actions)
        {
            if (action.ActionType == ActionType.Draw)
            {
                GameRenderer.game.Map.Player.DrawCards(action.Value);
            }
            if (action.ActionType == ActionType.Heal)
            {
                GameRenderer.game.Map.Player.AddHealth(action.Value);
            }
            if (action.ActionType == ActionType.Energy)
            {
                if (GameRenderer.game?.Map?.Rooms?.Count > 0 && 
                    GameRenderer.game.Map.Rooms[0] is Combat combatRoom)
                {
                    combatRoom.AddEnergy(action.Value);
                }
            }
            if (!action.IsTargetPlayer)
            {
                if (action.ActionType == ActionType.Attack)
                {
                    // Deal damage to enemy
                if (GameRenderer.game?.Map?.Rooms?.Count > 0 && 
                    GameRenderer.game.Map.Rooms[0] is Combat combatRoom)
                {
                    combatRoom.Enemy.TakeDamage(action.Value);
                }
                }
                else if (action.ActionType == ActionType.Block)
                {
                    // Add block to player
                    if (GameRenderer.game?.Map?.Rooms?.Count > 0 && 
                        GameRenderer.game.Map.Rooms[0] is Combat combatRoom)
                    {
                        combatRoom.Enemy.AddBlock(action.Value);
                    }
                }
                else if (action.ActionType == ActionType.Effect)
                {
                    if (GameRenderer.game?.Map?.Rooms?.Count > 0 && 
                        GameRenderer.game.Map.Rooms[0] is Combat combatRoom)
                    {
                        combatRoom.Enemy.AddEffectStack(action.EffectType.Value, action.Value);
                    }
                }
            }
            if (action.IsTargetPlayer)
            {
                if (action.ActionType == ActionType.Attack)
                {
                    GameRenderer.game.Map.Player.TakeDamage(action.Value);
                }
                else if (action.ActionType == ActionType.Block)
                {
                    GameRenderer.game.Map.Player.AddBlock(action.Value);
                }
                else if (action.ActionType == ActionType.Effect)
                {
                    GameRenderer.game.Map.Player.AddEffectStack(action.EffectType.Value, action.Value);
                }
            }
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
    public void ExhaustCard(Card card)
    {
        _cardLocation = CardLocation.ExhaustPile;
    }
}
