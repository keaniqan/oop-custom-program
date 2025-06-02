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
    private bool _isBuff;

    public Effect(EffectType effectType, string effectDescription, int stack, bool isBuff)
    {
        _effectType = effectType;
        _effectDescription = effectDescription;
        _stack = stack;
        _isBuff = isBuff;
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

    public int Stack
    {
        get { return _stack; }
        set { _stack = value; }
    }

    public string EffectDescription
    {
        get { return _effectDescription; }
        set { _effectDescription = value; }
    }
}
