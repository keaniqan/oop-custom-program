using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Numerics;
using Raylib_cs;
#nullable disable
namespace MyApp;
public abstract class Unit
{
    public const int ScreenWidth = 1920;
    public const int ScreenHeight = 1080;
    private string _name;
    private int _health;
    private int _maxHealth;
    private int _block;
    protected List<Effect> _effects;

    public Unit(string name, int health, int maxHealth, int block, List<Effect> effects)
    {
        _name = name;
        _health = health;
        _maxHealth = maxHealth;
        _block = block;
        _effects = new List<Effect>();
    }

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }
    public int Health
    {
        get { return _health; }
        set { _health = value; }
    }
    public int MaxHealth
    {
        get { return _maxHealth; }
        set { _maxHealth = value; }
    }
    public int Block
    {
        get { return _block; }
        set { _block = value; }
    }
    public List<Effect> Effects
    {
        get { return _effects; }
        set { _effects = value; }
    }
    public void AddBlock(int block)
    {
        _block += block;
    }
    public void AddHealth(int health)
    {
        _health += health;
    }
    public virtual void TakeDamage(int damage)
    {
        // First reduce damage by block
        if (_block > 0)
        {
            if (_block >= damage)
            {
                _block -= damage;
                damage = 0;
            }
            else
            {
                damage -= _block;
                _block = 0;
            }
        }

        // Then apply remaining damage to health
        if (damage > 0)
        {
            _health = Math.Max(0, _health - damage);
        }
    }

    public void AddEffectStack(EffectType effectType, int amount)
    {
        var effect = _effects.FirstOrDefault(e => e.EffectType == effectType);
        if (effect != null)
        {
            effect.Stacks += amount;
        }
    }
    public bool isDead(){
        return Health <= 0;
    }

    public abstract void EndTurn();
}
