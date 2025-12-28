using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LoadingLevelState : State<GameManager> {
    private readonly ILevelProgressService _levelProgressService;
    private readonly ISceneTransitionManager _sceneTransitionManager;
    private CancellationTokenSource _cts;

    public LoadingLevelState(
        ILevelProgressService levelProgressService,
        ISceneTransitionManager sceneTransitionManager)  {
        _levelProgressService = levelProgressService;
        _sceneTransitionManager = sceneTransitionManager;
    }

    public override void Enter() {
        _cts = new CancellationTokenSource();

        LoadLevelAsync().Forget();
    }

    private async UniTask LoadLevelAsync(CancellationToken token = default) {
        try {
            string currentLevelName = _levelProgressService.GetCurrentLevelName();
            await _sceneTransitionManager.LoadSceneWithLoadingScreenAsync(currentLevelName, _cts.Token);

            Context.StateMachine.ChangeState<GameLoopState>();
        } catch (OperationCanceledException) {
            Debug.Log("Loading level was canceled.");
        } catch (Exception ex) {
            Debug.LogError($"Error loading level: {ex.Message}");
        }
    }


    public override void Exit() {
        _cts?.Cancel();
        _cts?.Dispose();
    }
}
