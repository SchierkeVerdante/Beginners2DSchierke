using UnityEngine;

[CreateAssetMenu(fileName = "StarVisualTheme", menuName = "StarMap/Star Visual Theme")]
public class StarVisualTheme : ScriptableObject {
    [Header("Progress State Colors")]
    public Color LockedColor = Color.gray;
    public Color AvailableColor = Color.white;
    public Color VisitedColor = Color.cyan;
    public Color CurrentColor = Color.yellow;

    [Header("Colors (Override Progress)")]
    public Color SelectedColor = Color.green;
    public Color HighlightedColor = Color.magenta;

    [Header("Line Colors")]
    public Color LockedLineColor = new Color(0.3f, 0.3f, 0.3f);
    public Color AvailableLineColor = Color.white;
}