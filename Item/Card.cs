using System;
using System.Numerics;
using Raylib_cs;
#nullable disable
namespace MyApp;

public class Card : Item
{
    private const int ScreenWidth = 1920;
    private const int ScreenHeight = 1080;
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
        // foreach (var action in card.Actions)
        // {
        //     if (action.ActionType == ActionType.Draw)
        //     {
        //         GameRenderer.game.Player.DrawCards(action.Value);
        //     }
        //     if (action.ActionType == ActionType.Heal)
        //     {
        //         GameRenderer.game.Player.AddHealth(action.Value);
        //     }
        //     if (action.ActionType == ActionType.Energy)
        //     {
        //         if (GameRenderer.game?.Rooms?.Count > 0 && 
        //             GameRenderer.game.Rooms[0] is Combat combatRoom)
        //         {
        //             combatRoom.AddEnergy(action.Value);
        //         }
        //     }
        //     if (!action.IsTargetPlayer)
        //     {
        //         if (GameRenderer.game?.Rooms?.Count > 0 && 
        //             GameRenderer.game.Rooms[0] is Combat combatRoom)
        //         {
        //             var strengthEffect = GameRenderer.game.Player.Effects.FirstOrDefault(e => e.EffectType == EffectType.StrengthUp);
        //             var weakEffect = GameRenderer.game.Player.Effects.FirstOrDefault(e => e.EffectType == EffectType.Weak);
        //             var thornEffect = GameRenderer.game.Player.Effects.FirstOrDefault(e => e.EffectType == EffectType.Thorn);
        //             var vulnerableEffect = GameRenderer.game.Player.Effects.FirstOrDefault(e => e.EffectType == EffectType.Vulnerable);
        //             var dexterityEffect = GameRenderer.game.Player.Effects.FirstOrDefault(e => e.EffectType == EffectType.DexterityUp);
        //             var frailEffect = GameRenderer.game.Player.Effects.FirstOrDefault(e => e.EffectType == EffectType.Frail);

        //             if (action.ActionType == ActionType.Attack)
        //             {
        //                 int damage = action.Value;
        //                 if (strengthEffect != null)
        //                 {
        //                     damage += strengthEffect.Stack;
        //                 }
        //                 if (weakEffect != null && weakEffect.Stack > 0)
        //                 {
        //                     damage = (int)(damage * 0.75);
        //                 }
        //                 if (thornEffect != null)
        //                 {
        //                     GameRenderer.game.Player.TakeDamage(thornEffect.Stack);
        //                 }
        //                 if (vulnerableEffect != null && vulnerableEffect.Stack > 0)
        //                 {
        //                     damage = (int)(damage * 1.5);
        //                 }
        //                 combatRoom.Enemy.TakeDamage(damage);
        //                 //Check if enemy is dead
        //                 if (combatRoom.Enemy.Health <= 0)
        //                 {
        //                     combatRoom.EndCombat();
        //                 }
        //                 // Create damage number animation at enemy position
        //                 GameRenderer.CreateDamageNumber(damage, new Vector2(ScreenWidth - 600, ScreenHeight / 2 - 100));
        //             }
        //             else if (action.ActionType == ActionType.Block)
        //             {
        //                 int block = action.Value;
        //                 if (dexterityEffect != null)
        //                 {
        //                     block += dexterityEffect.Stack;
        //                 }
        //                 if (frailEffect != null && frailEffect.Stack > 0)
        //                 {
        //                     block = (int)(block * 0.75);
        //                 }
        //                 GameRenderer.game.Player.AddBlock(block);
        //             }
        //             else if (action.ActionType == ActionType.Effect)
        //             {
        //                 combatRoom.Enemy.AddEffectStack(action.EffectType.Value, action.Value);
        //             }
        //         }
        //     }
        //     if (action.IsTargetPlayer)
        //     {
        //         if (action.ActionType == ActionType.Attack)
        //         {
        //             int damage = action.Value;
        //             GameRenderer.game.Player.TakeDamage(damage);
        //         }
        //         else if (action.ActionType == ActionType.Block)
        //         {
        //             int block = action.Value;
                    
        //             // Apply dexterity effect
        //             var dexterityEffect = GameRenderer.game.Player.Effects.FirstOrDefault(e => e.EffectType == EffectType.DexterityUp);
        //             if (dexterityEffect != null)
        //             {
        //                 block += dexterityEffect.Stack;
        //             }

        //             // Apply frail effect
        //             var frailEffect = GameRenderer.game.Player.Effects.FirstOrDefault(e => e.EffectType == EffectType.Frail);
        //             if (frailEffect != null && frailEffect.Stack > 0)
        //             {
        //                 block = (int)(block * 0.75f); // 25% less block
        //             }

        //             GameRenderer.game.Player.AddBlock(block);
        //         }
        //         else if (action.ActionType == ActionType.Effect)
        //         {
        //             GameRenderer.game.Player.AddEffectStack(action.EffectType.Value, action.Value);
        //         }
        //     }
        // }
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
