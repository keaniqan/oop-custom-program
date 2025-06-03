using System;
using System.Numerics;
using Raylib_cs;
#nullable disable
namespace MyApp;

public class Effect
{
    private EffectType _effectType;
    private string _effectDescription;
    private int _stack;


    public Effect(EffectType effectType, string effectDescription, int stack, bool isBuff)
    {
        _effectType = effectType;
        _effectDescription = effectDescription;
        _stack = stack;
    }

    public void IncreaseEffect(Unit unit)
    {
        _stack += 1;
    }
    public void DecreaseEffect(Unit unit)
    {
        _stack -= 1;
    }

    public EffectType EffectType
    {
        get { return _effectType; }
        set { _effectType = value; }
    }

    public int Stacks
    {
        get { return _stack; }
        set { _stack = value; }
    }

    public string EffectDescription
    {
        get { return _effectDescription; }
        set { _effectDescription = value; }
    }
        
    public int ApplyStrength(int damage)
    {
        return damage + _stack;
    }
    public int ApplyVulnerable(int damage)
    {
        return (int)(damage * 1.5);
    }
    public int ApplyWeak(int damage)
    {
        return (int)(damage * 0.75);
    }
}
