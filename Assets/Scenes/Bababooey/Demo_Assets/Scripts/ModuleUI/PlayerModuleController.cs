using System.Collections.Generic;
using UnityEngine;

public class PlayerModuleController : MonoBehaviour
{
    public static PlayerModuleController Instance;

    PlayerMovement movement;
    List<ModuleData> installedModules = new List<ModuleData>();

    private void Awake()
    {
        Instance = this;
        movement = GetComponent<PlayerMovement>();
    }

    public void AddModule(ModuleData module)
    {
        installedModules.Add(module);

        // Apply effects
        movement.SetSpeedMultiplier(module.speedMultiplier);

        Debug.Log("Installed module: " + module.moduleName);
    }
}
