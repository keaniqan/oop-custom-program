using System;
using System.Numerics;
using System.Linq;
using Raylib_cs;
#nullable disable

namespace MyApp;

public class Player: Unit
{
    private int _gold;
    private List<Charm> _charms;
    private List<Card> _cards;
    private double _shopDiscount;

    public Player(string name, int health, int maxHealth, int block, List<Effect> effects, int gold, List<Charm> charms, List<Card> cards, double shopDiscount) : base(name, health, maxHealth, block, effects)
    {
        _gold = gold;
        _charms = charms;
        _cards = cards;
        _shopDiscount = shopDiscount;
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
        card.DealCard(card);
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
        foreach (var card in _cards){
            if (cardsDrawn >= amount) break;
            if (card.CardLocation == CardLocation.DrawPile){
                card.DrawCard(card);
                cardsDrawn++;
            }
        }
    }
    public void ShuffleDeck(){
        Random random = new Random();
        
        // Fisher-Yates shuffle algorithm on the entire card list
        for (int i = _cards.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            Card temp = _cards[i];
            _cards[i] = _cards[j];
            _cards[j] = temp;
        }
    }
    public void EmptyDiscardPiletoDrawPile(){
        foreach (var card in _cards){
            if (card.CardLocation == CardLocation.DiscardPile){
                card.CardLocation = CardLocation.DrawPile;
            }
        }
    }

    public void EndTurn(){  
        DiscardHand();
    }
}



