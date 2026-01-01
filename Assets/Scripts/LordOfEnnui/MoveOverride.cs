using System;

public enum OverrideType {
    TurnSpeed,
    MovementSpeed
}

public enum OverrideDuration {
    Startup,
    Move,
    End
}

[Serializable]
public struct MoveOverride {
    public OverrideType type;
    public OverrideDuration duration;
    public float value;
}
