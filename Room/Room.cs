using System.Collections.Generic;
using Raylib_cs;
#nullable disable
namespace MyApp;

public abstract class Room
{
    public int Layer;
    public int Index;
    public string RoomType;
    public List<Room> Connections = new List<Room>();
    public int X, Y;
    public bool _isAvailable = false;
    public bool _isCurrent = false;
    public bool _isCleared = false;  // Add IsCleared property

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

    public void ClearRoom()
    {
        _isCleared = true;
    }

    public void SetAvailable()
    {
        _isAvailable = true;
    }
    public void SetUnavailable()
    {
        _isAvailable = false;
    }
    
    public virtual void EnterRoom()
    {
        _isCurrent = true;
    }
    
    public void ExitRoom()
    {
        _isCurrent = false;
    }

    protected Game GetGame()
    {
        return GameRenderer.game;
    }

    public abstract void Reward();
}
