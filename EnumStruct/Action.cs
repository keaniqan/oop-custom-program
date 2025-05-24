namespace MyApp;

public struct Action
{
    private ActionType _actionType;
    private int _value;
    private EffectType? _effectType;
    private bool _isTargetPlayer;

    public Action(ActionType actionType, int value, EffectType? effectType, bool isTargetPlayer)
    {
        _actionType = actionType;
        _value = value;
        _effectType = effectType;
        _isTargetPlayer = isTargetPlayer;
    }

    public ActionType ActionType
    {
        get { return _actionType; }
        set { _actionType = value; }
    }

    public int Value
    {
        get { return _value; }
        set { _value = value; }
    }

    public EffectType? EffectType
    {
        get { return _effectType; }
        set { _effectType = value; }
    }

    public bool IsTargetPlayer
    {
        get { return _isTargetPlayer; }
        set { _isTargetPlayer = value; }
    }
}

