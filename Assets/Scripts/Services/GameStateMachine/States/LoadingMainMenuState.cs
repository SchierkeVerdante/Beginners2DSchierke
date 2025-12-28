using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingMainMenuState : State<GameManager> {
    private readonly ISceneLoader _sceneLoader;
    private readonly ILoadingScreenService _loadingScreenService;
    private readonly ISceneDataService _sceneData;
    public LoadingMainMenuState( ISceneLoader sceneLoader, ISceneDataService sceneData) { 
        _sceneLoader = sceneLoader;
        _sceneData = sceneData;
    }
    public override void Enter() {
        Scene scene = SceneManager.GetActiveScene();

        if (_sceneData.IsMainMenu()) {
            Context.StateMachine.ChangeState<MainMenuState>();
            return;
        }

        LoadScene().Forget();
    }

    private async UniTaskVoid LoadScene() {
        await _sceneLoader.LoadAsync(_sceneData.GetMenuSceneName());
             Context.StateMachine.ChangeState<MainMenuState>();
    }

    public override void Exit() {
    }
}
