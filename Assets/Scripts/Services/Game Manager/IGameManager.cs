public interface IGameManager {
    void ContinueGame();
    void StartNewGame();
    void ExitToMainMenu();
    void ExitGame();
    void TogglePause();
    void LoadStarExploration(Star selectedStar);
    void LoadMapScene();
    void FinishGame();
}

