using System.Collections.Generic;
using Raylib_cs;
#nullable disable
namespace MyApp;

public class Map
{
    private List<Room> _rooms;
    private Player _player;
    private Room _currentRoom;

    public Map(List<Room> rooms, Player player)
    {
        _rooms = rooms;
        _player = player;
        _currentRoom = rooms.Count > 0 ? rooms[0] : null;
    }

    public List<Room> Rooms
    {
        get { return _rooms; }
        set { _rooms = value; }
    }

    public Player Player
    {
        get { return _player; }
        set { _player = value; }
    }

    public Room CurrentRoom
    {
        get { return _currentRoom; }
        set { _currentRoom = value; }
    }
}
