using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class UIModuleSelecter : MonoBehaviour
{
    public GameObject panel;
    public ModuleButton[] moduleButtons;
    public UnityEvent<ModuleJson> onSelectModule;
    Dictionary<string, Sprite> iconCache;

    private void Awake() {
        LoadIcons();
    }

    void LoadIcons() {
        iconCache = new Dictionary<string, Sprite>();

        Sprite[] icons = Resources.LoadAll<Sprite>("Icons/Modules");

        foreach (Sprite sprite in icons) {
            iconCache[sprite.name] = sprite;
        }
    }

    public void Show(ModulePickup modulePickup)
    {
        List<ModuleJson> allModules = LDirectory2D.Instance.LoadedModules;
        panel.SetActive(true);
        Time.timeScale = 0f; // pause game

        List<ModuleJson> choices = GetRandomModules(allModules, Mathf.Min(moduleButtons.Length, allModules.Count));

        for (int i = 0; i < moduleButtons.Length; i++)
        {
            if (i < choices.Count) {
                ModuleJson module = choices[i];
                Sprite icon = null;
                iconCache.TryGetValue(module.icon, out icon);
                moduleButtons[i].gameObject.SetActive(true);
                moduleButtons[i].Setup(choices[i], icon, this);
            } else {
                moduleButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void SelectModule(ModuleJson selected)
    {
        onSelectModule.Invoke(selected);
        panel.SetActive(false);
        Time.timeScale = 1f;
    }

    List<ModuleJson> GetRandomModules(List<ModuleJson> modules, int count)
    {
        List<ModuleJson> pool = new(modules);
        List<ModuleJson> result = new();

        for (int i = 0; i < count; i++)
        {
            if (pool.Count == 0) break;
            int index = Random.Range(0, pool.Count);
            result.Add(pool[index]);
            pool.RemoveAt(index);
        }

        return result;
    }
}
