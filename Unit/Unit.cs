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
    private List<Effect> _effects;

    public Unit(string name, int health, int maxHealth, int block, List<Effect> effects)
    {
        _name = name;
        _health = health;
        _maxHealth = maxHealth;
        _block = block;
        _effects = effects;
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
        damage -= _block;
        if (damage > 0)
        {
            _health -= damage;
        }
    }
}
