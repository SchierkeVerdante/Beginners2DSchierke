using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingMainMenuState : State<GameManager> {
    private readonly ISceneLoader _sceneLoader;
    private readonly ILoadingScreenService _loadingScreenService;
    private readonly ISceneDataService _sceneData;
    public LoadingMainMenuState(ISceneLoader sceneLoader, ISceneDataService sceneData) { 
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


public class LoadingMapState : State<GameManager> {
    private readonly ISceneLoader _sceneLoader;
    private readonly ILoadingScreenService _loadingScreenService;
    private readonly ISceneDataService _sceneData;
    public LoadingMapState(ISceneLoader sceneLoader, ISceneDataService sceneData) {
        _sceneLoader = sceneLoader;
        _sceneData = sceneData;
    }
    public override void Enter() {
        LoadScene().Forget();
    }

    private async UniTaskVoid LoadScene() {
        await _sceneLoader.LoadAsync(_sceneData.GetMapSceneName());
        Context.StateMachine.ChangeState<GameLoopState>();
    }

    public override void Exit() {
    }
}

public class LoadingTerrainState : State<GameManager> {
    private readonly ISceneLoader _sceneLoader;
    private readonly ILoadingScreenService _loadingScreenService;
    private readonly ISceneDataService _sceneData;
    private readonly IStarNavigationService _starNavigationService;
    private readonly PlanetGenerator planetGenerator;
    public LoadingTerrainState(ISceneLoader sceneLoader, ISceneDataService sceneData, IStarNavigationService starNavigationService, PlanetGenerator planetGenerator) {
        _sceneLoader = sceneLoader;
        _sceneData = sceneData;
        _starNavigationService = starNavigationService;
        this.planetGenerator = planetGenerator;
    }
    public override void Enter() {
        LoadScene().Forget();
    }

    private async UniTaskVoid LoadScene() {
        await _sceneLoader.LoadAsync(_sceneData.GetTerrainSceneName());
        Star currentStar = _starNavigationService.CurrentStar;
        planetGenerator.GeneratePlanet(currentStar.PlanetConfig);
        Context.StateMachine.ChangeState<GameLoopState>();
    }

    public override void Exit() {
    }
}