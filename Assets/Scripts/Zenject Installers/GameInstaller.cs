using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller {
    [SerializeField] private GameManager _gameBootstrapper;
    [SerializeField] private SceneDataService _sceneDataService;
    [SerializeField] private RandomServiceSettings _randomServiceSettings;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private GameController gameController;

    [SerializeField] AudioStateConfig _audioConfig;
    public override void InstallBindings() {
        Container.Bind<IGameManager>()
        .To<GameManager>()
        .FromComponentInNewPrefab(_gameBootstrapper)
        .AsSingle()
        .NonLazy();

        Container.BindInterfacesAndSelfTo<UIManager>()
            .FromComponentInNewPrefab(_uiManager)
            .AsSingle()
            .NonLazy();

        Container.Bind<ILoadingScreenService>().To<LoadingScreenService>().AsSingle();

        Container.Bind<ISceneDataService>()
       .To<SceneDataService>()
       .FromComponentInNewPrefab(_sceneDataService)
       .AsSingle()
       .NonLazy();

        BindSavings();

        StateMachineInstall();

        Container.Bind<IDataSerializer>().To<JsonSerializer>().AsSingle();

        Container.Bind<GameController>().AsSingle().NonLazy(); ;
        Container.Bind<InputManager>().AsSingle().NonLazy();

        //Services
        Container.Bind<IAudioService>().To<FmodAudioService>().AsSingle().WithArguments(_audioConfig);
        Container.Bind<IRandomService>().To<RandomService>().AsSingle().WithArguments(_randomServiceSettings);

        Container.Bind<ILevelProgressService>().To<LevelProgressService>().AsSingle();
        Container.Bind<ISceneLoader>().To<SceneLoader>().AsSingle();
        

        Container.Bind<ISceneTransitionManager>().To<SceneTransitionManager>().AsSingle();

        //Systems

        Container.Bind<LevelProgressSystem>().AsSingle().NonLazy();
        Container.Bind<AudioSystem>().AsSingle().NonLazy();


        Container.Bind<IDataRuntimeFactory>().To<DataRuntimeFactory>().AsSingle();
    }


    private void BindSavings() {
        Container.Bind<ISaveStorage>().To<PlayerPrefsStorage>().AsSingle();
        Container.Bind<ISaveLoadService>().To<SaveLoadService>().AsSingle();
    }

    private void StateMachineInstall() {

        Container.Bind(typeof(IStateFactory<>))
            .To(typeof(StateFactory<>))
            .AsTransient();

        Container.Bind<IStateFactory<GameManager>>()
            .To<StateFactory<GameManager>>()
            .AsSingle();

        Container.Bind<BootstrapState>().AsSingle();
        Container.Bind<LoadingLevelState>().AsSingle();
        Container.Bind<LoadingMainMenuState>().AsSingle();
        Container.Bind<MainMenuState>().AsSingle();
        Container.Bind<GameLoopState>().AsSingle();
        Container.Bind<PauseState>().AsSingle();
        Container.Bind<ExitState>().AsSingle();
    }
}
