using System;

public class LevelProgressService : ILevelProgressService, IDataLoader, IDataSaveable {
    private LevelProgressData _levelProgress = new() { CurrentLevel = 1, HighestLevelUnlocked = 1 };

    private const string LevelNamePrefix = "Level_";
    private readonly IDataRepository<LevelProgressData> dataRepository;

    public LevelProgressService(IDataRepository<LevelProgressData> dataRepository) {
        this.dataRepository = dataRepository;
    }

    public string GetCurrentLevelName() {
        return LevelNamePrefix + _levelProgress.CurrentLevel;
    }

    public LevelProgressData GetProgress() {
        return _levelProgress;
    }

    public void SetProgress(LevelProgressData progress) {
        _levelProgress = progress;
    }

    public void Load() {
        LevelProgressData levelProgressData = dataRepository.Load();
        SetProgress(levelProgressData);
    }

    public void Save() {
        dataRepository.Save(_levelProgress);
    }
}

[Serializable]
[DataSource(DataSourceType.PlayerPrefs, "level_progress")]
public class LevelProgressData {
    public int CurrentLevel;
    public int HighestLevelUnlocked;
}

