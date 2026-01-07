using System;
using Tymski;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface ISceneDataService {
    string GetMenuSceneName();
    string GetMapSceneName();
    string GetTerrainSceneName();
    bool IsMainMenu();
}


public class SceneDataService : MonoBehaviour, ISceneDataService {
    [SerializeField] private SceneData sceneData;
    public SceneDataService(SceneData sceneData) {
        this.sceneData = sceneData;
    }

    public string GetMapSceneName() {
        return sceneData.mapScene.SceneName;
    }

    public string GetMenuSceneName() {
        return sceneData.mainMenuScene.SceneName;
    }

    public string GetTerrainSceneName() {
        return sceneData.terrainScene.SceneName;
    }

    public bool IsMainMenu() {
        Scene scene = SceneManager.GetActiveScene();

        return GetMenuSceneName() == scene.name;
    }
}


[Serializable]
public class SceneData {
    public SceneReference mainMenuScene;
    public SceneReference terrainScene;
    public SceneReference mapScene;
}