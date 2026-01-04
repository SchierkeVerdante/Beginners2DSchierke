using System.Collections.Generic;
using UnityEngine;

public class ModuleRewardManager : MonoBehaviour
{
    public static ModuleRewardManager Instance;

    public GameObject moduleUIPanel;
    //public ModuleButton[] moduleButtons;

    public List<ModuleJson> allModules;

    private void Awake()
    {
        Instance = this;
        moduleUIPanel.SetActive(false);
    }

    public void OpenModuleSelection()
    {
       /* Time.timeScale = 0f;
        moduleUIPanel.SetActive(true);

        List<ModuleData> choices = GetRandomModules(3);

        for (int i = 0; i < moduleButtons.Length; i++)
        {
            moduleButtons[i].Setup(choices[i]);
        }*/
    }

    List<ModuleJson> GetRandomModules(int count)
    {
        List<ModuleJson> pool = new List<ModuleJson>(allModules);
        List<ModuleJson> result = new List<ModuleJson>();

        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, pool.Count);
            result.Add(pool[index]);
            pool.RemoveAt(index);
        }

        return result;
    }

    public void OnModuleChosen(ModuleJson module)
    {
        //PlayerModuleController.Instance.AddModule(module);

        moduleUIPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
