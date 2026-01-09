using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Zenject;

public class StarNamer : MonoBehaviour
{
    [Inject] StarNamerService starDataService;
    private void Start() {

    }

}

public class StarNamerService : IDataLoader {
    private readonly List<string> _availableNames = new List<string>();
    private readonly HashSet<string> _usedNames = new HashSet<string>();
    private readonly Dictionary<string, int> _nameCounters = new Dictionary<string, int>();

    private StarNamesData _data;
    private readonly IDataRepository<StarNamesData> _dataRepository;
    private readonly System.Random _defaultRandom;

    public StarNamerService(IDataRepository<StarNamesData> dataRepository, IRandomService randomService) {
        _dataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));
        _defaultRandom = new System.Random(randomService?.Seed ?? DateTime.Now.Millisecond);
    }

    public string GetUniqueName(System.Random customRandom = null) {
        var rnd = customRandom ?? _defaultRandom;

        if (_availableNames.Count > 0) {
            return GetAndRemoveRandomName(rnd);
        }

        return GeneratePatternName(rnd);
    }

    private string GetAndRemoveRandomName(System.Random rnd) {
        int index = rnd.Next(_availableNames.Count);
        string name = _availableNames[index];

        _availableNames[index] = _availableNames[_availableNames.Count - 1];
        _availableNames.RemoveAt(_availableNames.Count - 1);

        _usedNames.Add(name);
        return name;
    }

    private string GeneratePatternName(System.Random rnd) {
        if (_data?.starNames == null || _data.starNames.Length == 0) {
            Debug.LogError("StarNamerService: No base names available for generation!");
            return $"Sector-{rnd.Next(1000, 9999)}";
        }

        string baseName = _data.starNames[rnd.Next(_data.starNames.Length)];

        if (!_nameCounters.TryGetValue(baseName, out int currentNumber)) {
            currentNumber = 2;
        }

        string newName = $"{baseName} {currentNumber}";
        _nameCounters[baseName] = currentNumber + 1;
        _usedNames.Add(newName);

        return newName;
    }

    public void ResetNameUniq() {
        _availableNames.Clear();
        _usedNames.Clear();
        _nameCounters.Clear();

        if (_data?.starNames != null) {
            var uniqueInput = new HashSet<string>(_data.starNames);
            _availableNames.AddRange(uniqueInput.Where(n => !string.IsNullOrWhiteSpace(n)));

            foreach (var name in _availableNames) {
                _nameCounters[name] = 2;
            }
        }
    }

    public void Load() {
        try {
            _data = _dataRepository.Load();
            ResetNameUniq();
        } catch (Exception ex) {
            Debug.LogError($"StarNamerService: Load failed: {ex.Message}");
            _data = new StarNamesData { starNames = new string[] { "Alpha", "Beta", "Gamma" } };
            ResetNameUniq();
        }
    }
}
