using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller {
    [SerializeField] private GameManager _gameBootstrapper;
    [SerializeField] private SceneDataService _sceneDataService;
    [SerializeField] private RandomServiceSettings _randomServiceSettings;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private GameController gameController;

    
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
        Container.Bind<ISceneLoader>().To<SceneLoader>().AsSingle();
        Container.Bind<ISceneTransitionManager>().To<SceneTransitionManager>().AsSingle();
        Container.Bind<ISceneDataService>()
       .To<SceneDataService>()
       .FromComponentInNewPrefab(_sceneDataService)
       .AsSingle()
       .NonLazy();

        StateMachineInstall();

        Container.Bind<GameController>().AsSingle().NonLazy(); ;
        Container.Bind<InputManager>().AsSingle().NonLazy();

        Container.Bind<ISaveLoadService>().To<SaveLoadService>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<LevelProgressService>().AsSingle();
        Container.BindInterfacesAndSelfTo<StarNamerService>().AsSingle().NonLazy();

        //Services
        Container.Bind<IRandomService>().To<RandomService>().AsSingle().WithArguments(_randomServiceSettings);
        
        Container.Bind<IStarMapService>().To<StarMapService>().AsSingle();

        //Systems
        
        Container.Bind<IDataRuntimeFactory>().To<DataRuntimeFactory>().AsSingle();
        Container.Bind(typeof(IPresenterFactory<>)).To(typeof(PresenterPlaceholderFactory<>)).AsTransient();

        InstalRepos();

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

    private void InstalRepos() {
        var assembly = Assembly.GetExecutingAssembly();
        var dataTypes = assembly.GetTypes()
            .Where(t => t.GetCustomAttribute<DataSourceAttribute>() != null);

        foreach (var dataType in dataTypes) {
            var attribute = dataType.GetCustomAttribute<DataSourceAttribute>();
            var repositoryInterface = typeof(IDataRepository<>).MakeGenericType(dataType);
            var repositoryImpl = GetRepoType(attribute.SourceType).MakeGenericType(dataType);

            Container.Bind(repositoryInterface)
                .To(repositoryImpl)
                .AsSingle()
                .WithArguments(attribute.Key);
        }
    }
    private Type GetRepoType(DataSourceType source) {
        return source switch {
            DataSourceType.PlayerPrefs => typeof(PlayerPrefsRepository<>),
            DataSourceType.Resources => typeof(ResourcesRepository<>),
            DataSourceType.FileSystem => typeof(JsonDataRepository<>),
            _ => throw new NotSupportedException()
        };
    }
}
