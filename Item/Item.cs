using System;
using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;
#nullable disable

namespace MyApp;

public abstract class Item
{
    private string _name;
    private string _description;
    private int _price;

    public Item(string name, string description, int price)
    {
        _name = name;
        _description = description;
        _price = price;
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
    public int Price
    {
        get { return _price; }
        set { _price = value; }
    }
}
