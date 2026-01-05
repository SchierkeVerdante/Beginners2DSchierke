using UnityEngine;

[CreateAssetMenu(fileName = "StarVisualTheme", menuName = "StarMap/Star Visual Theme")]
public class StarVisualTheme : ScriptableObject {
    [Header("Colors")]
    public Color LockedColor = Color.gray;
    public Color AvailableColor = Color.white;
    public Color VisitedColor = Color.cyan;
    public Color CurrentColor = Color.yellow;
    public Color SelectedColor = Color.green;

    [Header("Scales")]
    public float NormalScale = 1f;
    public float HoverScale = 1.2f;

    public Color GetColor(NavStarState state) {
        return state switch {
            NavStarState.Locked => LockedColor,
            NavStarState.Available => AvailableColor,
            NavStarState.Visited => VisitedColor,
            NavStarState.Current => CurrentColor,
            NavStarState.Selected => SelectedColor,
            _ => Color.white
        };
    }
}
