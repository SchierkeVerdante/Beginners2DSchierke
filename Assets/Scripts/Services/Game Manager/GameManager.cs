using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour, IGameManager {
    [Inject] private ILevelProgressService _levelProgress;
    [Inject] IStateFactory<GameManager> stateFactory;

    private ContextStateMachine<GameManager> _gameStateMachine;
    public ContextStateMachine<GameManager> StateMachine => _gameStateMachine;

    private void Start() {
        _gameStateMachine = new ContextStateMachine<GameManager>(this, stateFactory);

        _gameStateMachine.OnStateEnter += HandleStateChanged;
        _gameStateMachine.ChangeState<BootstrapState>();
    }

    public ILevelProgressService GetLevelProgress() { return _levelProgress; }

    private void HandleStateChanged(IState state) {
        Debug.Log($"State changed to : {state}");
    }

    private void OnApplicationQuit() {
        if (_gameStateMachine != null)
            _gameStateMachine.ChangeState<ExitState>();
    }
    public void StartNewGame() {
        Debug.Log("Game Started!");
        PlayerProgressData playerProgressData = new PlayerProgressData { CurrentLevel = 1 };
        _levelProgress.SetProgress(playerProgressData);
        //_gameStateMachine.ChangeState<LoadingLevelState>();
        _gameStateMachine.ChangeState<LoadingMapState>();
    }

    public void ContinueGame() {
        Time.timeScale = 1.0f;
        _gameStateMachine.ChangeState<LoadingMapState>();
    }

    #region Pause/Resume
    public void TogglePause() {
        bool isPaused = _gameStateMachine.CurrentState is PauseState;
        if (isPaused) {
            ResumeGame();
        } else {
            PauseGame();
        }
    }

    public void PauseGame() {
        _gameStateMachine.ChangeState<PauseState>();
    }

    public void ResumeGame() {
        _gameStateMachine.ChangeState<GameLoopState>();
    }
    #endregion

    public void ExitToMainMenu() {
        _gameStateMachine.ChangeState<LoadingMainMenuState>();
    }

    public void ExitGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnDestroy() {
        if (_gameStateMachine != null)
        _gameStateMachine.OnStateEnter -= HandleStateChanged;
    }

    public void LoadStarExploration(Star selectedStar) {
        Debug.Log("Request to load: " + selectedStar);
        _gameStateMachine.ChangeState<LoadingLevelState>();
        //_gameStateMachine.ChangeState<LoadingTerrainState>(); // for quick test
    }

    public void LoadMapScene() {
        Debug.Log("Request to load map scene");
        _gameStateMachine.ChangeState<LoadingMapState>();
    }

    public void FinishGame() {
        Debug.Log("Request to finish game");
        _gameStateMachine.ChangeState<LoadingMainMenuState>();
        _levelProgress.SetPlayerState(null);
    }
}

