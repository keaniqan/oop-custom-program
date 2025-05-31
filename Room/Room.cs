using System.Collections.Generic;
using Raylib_cs;
#nullable disable
namespace MyApp;

public abstract class Room
{
    private bool _isCleared;
    private bool _isAvailable;
    private bool _isCurrent;
    
    public Room(bool isCleared, bool isAvailable, bool isCurrent)
    {
        _isCleared = isCleared;
        _isAvailable = isAvailable;
        _isCurrent = isCurrent;
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
    
    public void SetAvailabileRoom(Game game)
    {
        if (game.Rooms[0].IsCurrent)
        {
            //set all rooms after index 1 to available
            game.Rooms[1].SetAvailable();
            game.Rooms[2].SetAvailable();
            game.Rooms[3].SetAvailable();
            //set all rooms before index 1 and the current room to unavailable
            game.Rooms[0].SetUnavailable();
        }
    }
    public abstract void Reward();
}