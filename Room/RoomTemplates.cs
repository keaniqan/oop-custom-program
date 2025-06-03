using MyApp;
#nullable disable

public abstract class RoomTemplate
{
    public abstract Room CreateRoom();
}

public class EnemyRoomTemplate : RoomTemplate
{
    private EnemyTemplate _enemyTemplate;

    public EnemyRoomTemplate(EnemyTemplate enemyTemplate)
    {
        _enemyTemplate = enemyTemplate;
    }

    public override Room CreateRoom()
    {
        return new Combat(false, true, true, _enemyTemplate.CreateEnemy(), _enemyTemplate.Type, TurnPhase.PlayerStart, 3);
    }
}

public class EventRoomTemplate : RoomTemplate
{
    private EventTemplate _eventTemplate;

    public EventRoomTemplate(EventTemplate eventTemplate)
    {
        _eventTemplate = eventTemplate;
    }

    public override Room CreateRoom()
    {
        return new Event(false, true, true, _eventTemplate.Dialog, _eventTemplate.Choices);
    }
}

// Rest rooms

public class RestRoomTemplate : RoomTemplate
{
    public override Room CreateRoom()
    {
        return new Rest(false, true, true);
    }
}

public class EventTemplate
{
    public string Title { get; set; }
    public string Dialog { get; set; }
    public List<EventChoice> Choices { get; set; }

    public EventTemplate(string title, string dialog, List<EventChoice> choices)
    {
        Title = title;
        Dialog = dialog;
        Choices = choices;
    }
}

public class EnemyTemplate
{
    public string Name { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public EnemyType Type { get; set; }
    public int TextureIndex { get; set; }
    public EnemyTemplate(string name, int health, int maxHealth, EnemyType type, int textureIndex)
    {
        Name = name;
        Health = health;
        MaxHealth = maxHealth;
        Type = type;
        TextureIndex = textureIndex;
    }

    public Enemy CreateEnemy()
    {
        return new Enemy(Name, Health, MaxHealth, 0, new List<Effect>(), Type, TextureIndex);
    }
}