using System;
using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;
#nullable disable
namespace MyApp;

public class Charm : Item
{
    private bool _isUsed;  

    public Charm(string name, string description, int price, List<Action> actions, bool isUsed) : base(name, description, price, actions)
    {
        _isUsed = false;
    }
    void ApplyEffect(Action action)
    {
        
    }

    void UseCharm()
    {
        _isUsed = true;
    }
}
