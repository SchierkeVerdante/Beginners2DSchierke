public interface ILevelProgressService {
    string GetCurrentLevelName();
    PlayerProgressData GetProgress();
    void SetProgress(PlayerProgressData progress);
}

