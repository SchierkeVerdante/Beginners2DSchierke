public interface ILevelProgressService {
    string GetCurrentLevelName();
    PlayerState GetPlayerState();
    PlayerProgressData GetProgress();
    void SetPlayerState(PlayerState state);
    void SetProgress(PlayerProgressData progress);
}

