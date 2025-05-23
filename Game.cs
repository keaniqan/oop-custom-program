using System;
using System.Data;
using System.Text;
using Raylib_cs;
#nullable disable
namespace MyApp;

public class Game
{
    private GameState _gameState;
    private Map _map;
    private int seed;

    public Game(int seed, Player player)
    {
        this.seed = seed;
        var rooms = new List<Room>();  // Initialize with empty room list for now
        _map = new Map(rooms, player);
    }

    public Map Map
    {
        get { return _map; }
        set { _map = value; }
    }
    
    
}


