using UnityEngine;
using System.Collections.Generic;

public class ModuleDatabaseLoader : MonoBehaviour
{
    public static List<ModuleJson> LoadedModules;

    void Awake()
    {
        LoadModules();
    }

    void LoadModules()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Modules");

        if (jsonFile == null)
        {
            Debug.LogError("Modules.json not found!");
            return;
        }

        ModuleDatabaseJson database =
            JsonUtility.FromJson<ModuleDatabaseJson>(jsonFile.text);

        LoadedModules = new List<ModuleJson>(database.modules);

        Debug.Log("Loaded modules: " + LoadedModules.Count);
    }
}