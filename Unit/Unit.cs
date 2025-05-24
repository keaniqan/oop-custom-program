using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Numerics;
using Raylib_cs;
#nullable disable
namespace MyApp;
public class Unit
{
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
        
        // Initialize all possible effects with 0 stacks
        foreach (EffectType effectType in Enum.GetValues(typeof(EffectType)))
        {
            _effects.Add(new Effect(effectType, effectType.ToString(), 0, true));
        }
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
    public void RemoveBlock(int block)
    {
        _block -= block;
    }
    public void AddHealth(int health)
    {
        _health += health;
    }
    public void RemoveHealth(int health)
    {
        _health -= health;
    }
    public void IncreaseEffect(Effect effect)
    {
        effect.IncreaseEffect(this);
    }
    public void DecreaseEffect(Effect effect)
    {
        effect.DecreaseEffect(this);
    }
    public void TakeDamage(int damage)
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
}
