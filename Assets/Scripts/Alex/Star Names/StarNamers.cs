using System;
using System.Collections.Generic;
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

    private List<string> availableNames = new();
    private List<string> usedNames = new();

    private StarNamesData data;
    private IDataRepository<StarNamesData> dataRepository;

    public StarNamerService(IDataRepository<StarNamesData> dataRepository) {
        this.dataRepository = dataRepository;
    }

    public string GetUniqueName() {
        if (availableNames.Count == 0) {
            Debug.LogWarning("No unique star names left! Resetting...");
            ResetNameUniq();
        }
           
        var random = new System.Random();
        int index = random.Next(availableNames.Count);

        string name = availableNames[index];

        availableNames.RemoveAt(index);
        usedNames.Add(name);

        return name;
    }

    public void ResetNameUniq() {
        availableNames.Clear();
        usedNames.Clear();

        if (data != null)
        availableNames.AddRange(data.starNames);
    }

    public void Load() {
        data = dataRepository.Load();
        ResetNameUniq();
    }
}
