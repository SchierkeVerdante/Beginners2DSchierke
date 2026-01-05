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

    public Color GetColor(StarState state) {
        return state switch {
            StarState.Locked => LockedColor,
            StarState.Available => AvailableColor,
            StarState.Visited => VisitedColor,
            StarState.Current => CurrentColor,
            StarState.Selected => SelectedColor,
            _ => Color.white
        };
    }
}
