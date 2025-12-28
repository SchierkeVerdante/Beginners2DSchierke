using System;
using Tymski;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour, IGameManager {
    [Inject] private ILevelProgressService _levelProgress;
    [Inject] IStateFactory<GameManager> stateFactory;

    private ContextStateMachine<GameManager> _gameStateMachine;
    public ContextStateMachine<GameManager> StateMachine => _gameStateMachine;

    private void Start() {
        _gameStateMachine = new ContextStateMachine<GameManager>(this, stateFactory);

        _gameStateMachine.ChangeState<BootstrapState>();
    }

    private void OnApplicationQuit() {
        if (_gameStateMachine != null)
            _gameStateMachine.ChangeState<ExitState>();
    }
    public void StartNewGame() {
        Debug.Log("Game Started!");
        _levelProgress.SetProgress(new LevelProgress { CurrentLevel = 1 });
        _gameStateMachine.ChangeState<LoadingLevelState>();
    }

    public void ContinueGame() {
        _gameStateMachine.ChangeState<LoadingLevelState>();
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
}

