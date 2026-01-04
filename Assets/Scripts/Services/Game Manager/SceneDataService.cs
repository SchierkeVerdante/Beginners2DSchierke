using System;
using Tymski;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDataService : MonoBehaviour, ISceneDataService {
    [SerializeField] private SceneData sceneData;
    public SceneDataService(SceneData sceneData) {
        this.sceneData = sceneData;
    }

    public string GetMenuSceneName() {
        return sceneData.mainMenuScene.SceneName;
    }

    public bool IsMainMenu() {
        Scene scene = SceneManager.GetActiveScene();

        return GetMenuSceneName() == scene.name;
    }
}


[Serializable]
public class SceneData {
    public SceneReference mainMenuScene;
}