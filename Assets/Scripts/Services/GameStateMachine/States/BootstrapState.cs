using System;
using Tymski;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapState : State<GameManager> {
    private ISaveLoadService _saveLoadService;
    private ISceneDataService _sceneData;

    public BootstrapState(ISaveLoadService saveLoad, ISceneDataService data) {
        _sceneData = data;
        _saveLoadService = saveLoad;
    }

    public override void Enter() {
        Application.targetFrameRate = 60; 
        _saveLoadService.LoadAll();


        if (_sceneData.IsMainMenu()) {
            Context.StateMachine.ChangeState<MainMenuState>();
            return;
        }

        Context.StateMachine.ChangeState<GameLoopState>();
    }

    public override void Exit() {
    }
}
