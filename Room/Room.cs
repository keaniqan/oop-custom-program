using System.Collections.Generic;
using Raylib_cs;
#nullable disable
namespace MyApp;

public abstract class Room
{
    private int _layer;
    private int _index;
    private string _roomType;
    private List<Room> _connections = new List<Room>();
    private int _x;
    private int _y;
    private bool _isAvailable = false;
    private bool _isCurrent = false;
    private bool _isCleared = false;  // Add IsCleared property

    public Room(bool isCleared, bool isAvailable, bool isCurrent)
    {
        _isCleared = false;
        _isAvailable = false;
        _isCurrent = false;
    }

    public bool IsCleared
    {
        get { return _isCleared; }
        set { _isCleared = value; }
    }

    public bool IsAvailable
    {
        get { return _isAvailable; }
        set { _isAvailable = value; }
    }

    public bool IsCurrent
    {
        get { return _isCurrent; }
        set { _isCurrent = value; }
    }

    public int Layer
    {
        get { return _layer; }
        set { _layer = value; }
    }

    public int Index
    {
        get { return _index; }
        set { _index = value; }
    }

    public string RoomType
    {
        get { return _roomType; }
        set { _roomType = value; }
    }

    public List<Room> Connections
    {
        get { return _connections; }
        set { _connections = value; }
    }

    public int X
    {
        get { return _x; }
        set { _x = value; }
    }

    public int Y
    {
        get { return _y; }
        set { _y = value; }
    }
    
    public virtual void SetAvailable(bool isAvailable)
    {
        _isAvailable = isAvailable;
    }
    public virtual void SetCurrent(bool isCurrent)
    {
        _isCurrent = isCurrent;
    }
    public virtual void SetCleared(bool isCleared)
    {
        _isCleared = isCleared;
    }
    public virtual void EnterRoom()
    {
        _isCurrent = true;
    }
    
    protected Game GetGame()
    {
        return GameRenderer.game;
    }

    public abstract void Reward();
}
