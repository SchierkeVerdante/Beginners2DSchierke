public interface ILevelProgressService {
    string GetCurrentLevelName();
    LevelProgressData GetProgress();
    void SetProgress(LevelProgressData progress);
}

