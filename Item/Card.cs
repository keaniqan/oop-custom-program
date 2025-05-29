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
                        int damage = action.Value;
                        
                        // Apply Vulnerable effect
                        var vulnerableEffect = combatRoom.Enemy.Effects.FirstOrDefault(e => e.EffectType == EffectType.Vulnerable);
                        if (vulnerableEffect != null && vulnerableEffect.Stack > 0)
                        {
                            damage = (int)(damage * 1.5f); // 50% more damage
                        }
                        
                        // Apply strength effect
                        var strengthEffect = GameRenderer.game.Map.Player.Effects.FirstOrDefault(e => e.EffectType == EffectType.StrengthUp);
                        if (strengthEffect != null)
                        {
                            damage += strengthEffect.Stack;
                        }

                        // Apply weak effect
                        var weakEffect = GameRenderer.game.Map.Player.Effects.FirstOrDefault(e => e.EffectType == EffectType.Weak);
                        if (weakEffect != null && weakEffect.Stack > 0)
                        {
                            damage = (int)(damage * 0.75f); // 25% less damage
                        }

                        // Deal damage and check for thorn effect
                        combatRoom.Enemy.TakeDamage(damage);
                        
                        // Create damage number animation
                        Vector2 enemyPos = new Vector2(
                            GameRenderer.ScreenWidth - 600,  // Enemy X position
                            GameRenderer.ScreenHeight / 2 - 100  // Enemy Y position
                        );
                        GameRenderer.CreateDamageNumber(damage, enemyPos);
                        
                        // Check for thorn effect on enemy
                        var thornEffect = combatRoom.Enemy.Effects.FirstOrDefault(e => e.EffectType == EffectType.Thorn);
                        if (thornEffect != null)
                        {
                            GameRenderer.game.Map.Player.TakeDamage(thornEffect.Stack);
                        }
                    }
                }
                else if (action.ActionType == ActionType.Block)
                {
                    // Add block to enemy
                    if (GameRenderer.game?.Map?.Rooms?.Count > 0 && 
                        GameRenderer.game.Map.Rooms[0] is Combat combatRoom)
                    {
                        int block = action.Value;
                        
                        // Apply dexterity effect
                        var dexterityEffect = combatRoom.Enemy.Effects.FirstOrDefault(e => e.EffectType == EffectType.DexterityUp);
                        if (dexterityEffect != null && dexterityEffect.Stack > 0)
                        {
                            block += dexterityEffect.Stack;
                        }

                        // Apply frail effect
                        var frailEffect = combatRoom.Enemy.Effects.FirstOrDefault(e => e.EffectType == EffectType.Frail);
                        if (frailEffect != null && frailEffect.Stack > 0)
                        {
                            block = (int)(block * 0.75f); // 25% less block
                        }

                        combatRoom.Enemy.AddBlock(block);
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
                    int damage = action.Value;
                    
                    // Apply vulnerable effect
                    var vulnerableEffect = GameRenderer.game.Map.Player.Effects.FirstOrDefault(e => e.EffectType == EffectType.Vulnerable);
                    if (vulnerableEffect != null && vulnerableEffect.Stack > 0)
                    {
                        damage = (int)(damage * 1.5f); // 50% more damage
                    }
                    
                    GameRenderer.game.Map.Player.TakeDamage(damage);
                }
                else if (action.ActionType == ActionType.Block)
                {
                    int block = action.Value;
                    
                    // Apply dexterity effect
                    var dexterityEffect = GameRenderer.game.Map.Player.Effects.FirstOrDefault(e => e.EffectType == EffectType.DexterityUp);
                    if (dexterityEffect != null )
                    {
                        block += dexterityEffect.Stack;
                    }

                    // Apply frail effect
                    var frailEffect = GameRenderer.game.Map.Player.Effects.FirstOrDefault(e => e.EffectType == EffectType.Frail);
                    if (frailEffect != null)
                    {
                        block = (int)(block * 0.75f); // 25% less block
                    }

                    GameRenderer.game.Map.Player.AddBlock(block);
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
