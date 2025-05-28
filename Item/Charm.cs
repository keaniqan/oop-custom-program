using System;
using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;
#nullable disable
namespace MyApp;

public class Charm
{
    private string _name;
    private string _description;
    private CharmType _charmType;
    private bool _isActive;
    private int _charges;
    private int _maxCharges;
    private List<Action> _actions;

    public Charm(string name, string description, CharmType charmType, List<Action> actions, int maxCharges = 0)
    {
        _name = name;
        _description = description;
        _charmType = charmType;
        _isActive = true;
        _maxCharges = maxCharges;
        _charges = maxCharges;
        _actions = actions;
    }

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    public string Description
    {
        get { return _description; }
        set { _description = value; }
    }

    public CharmType CharmType
    {
        get { return _charmType; }
        set { _charmType = value; }
    }

    public bool IsActive
    {
        get { return _isActive; }
        set { _isActive = value; }
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

    public List<Action> Actions
    {
        get { return _actions; }
        set { _actions = value; }
    }

    public void ExecuteActions(Player player)
    {
        if (!_isActive) return;

        foreach (var action in _actions)
        {
            if (action.ActionType == ActionType.Draw)
            {
                player.DrawCards(action.Value);
            }
            else if (action.ActionType == ActionType.Energy)
            {
                if (GameRenderer.game?.Map?.CurrentRoom is Combat combatRoom)
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

