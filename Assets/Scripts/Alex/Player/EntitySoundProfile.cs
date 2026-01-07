using FMODUnity;
using UnityEngine;

[CreateAssetMenu(fileName = "New Entity Sound Profile", menuName = "Audio/Entity Sound Profile")]
public class EntitySoundProfile : ScriptableObject {
    [Header("=== Common Sounds ===")]
    public EventReference takeDamage;

    public EventReference death;

    public EventReference explosion;

    [Header("=== Movement Sounds ===")]
    public EventReference movementLoop;

    public string speedParameterName = "Speed";

    public EventReference jump;

    public EventReference landing;

    public EventReference dash;

    [Header("=== Combat Sounds ===")]
    public EventReference attackSounds;

    public EventReference hitSounds;

    [Header("=== Special Sounds ===")]
    public EventReference specialSounds;
}
