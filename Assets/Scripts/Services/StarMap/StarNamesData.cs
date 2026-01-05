using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StarNamesData", menuName = "Game/Star Names Data")]
public class StarNamesData : ScriptableObject {
    [SerializeField] private List<string> _starNames = new List<string>();

    public IReadOnlyList<string> Names => _starNames;

#if UNITY_EDITOR
    private void OnValidate() {
        _starNames.RemoveAll(string.IsNullOrWhiteSpace);

        var uniqueNames = new HashSet<string>();
        for (int i = _starNames.Count - 1; i >= 0; i--) {
            if (!uniqueNames.Add(_starNames[i])) {
                _starNames.RemoveAt(i);
            }
        }
    }
#endif
}