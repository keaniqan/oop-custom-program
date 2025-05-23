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
    

    public void EnterRoom()
    {
        _isCurrent = true;
    }
    
    public void ExitRoom()
    {
        _isCurrent = false;
    }
    
    public abstract void Reward();
}