using MyApp;
#nullable disable

public class EventChoice
{
    private string _text;
    private int _goldReward;
    private int _healthChange;
    private Card _cardReward;
    private Charm _charmReward;

    public EventChoice(string text, int goldReward = 0, int healthChange = 0, Card cardReward = null, Charm charmReward = null)
    {
        _text = text;
        _goldReward = goldReward;
        _healthChange = healthChange;
        _cardReward = cardReward;
        _charmReward = charmReward;
    }

    public string Text
    {
        get { return _text; }
        set { _text = value; }
    }

    public int GoldReward
    {
        get { return _goldReward; }
        set { _goldReward = value; }
    }

    public int HealthChange
    {
        get { return _healthChange; }
        set { _healthChange = value; }
    }

    public Card CardReward
    {
        get { return _cardReward; }
        set { _cardReward = value; }
    }

    public Charm CharmReward
    {
        get { return _charmReward; }
        set { _charmReward = value; }
    }
}