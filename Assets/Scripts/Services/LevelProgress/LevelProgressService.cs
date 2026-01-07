using System;

public class LevelProgressService : ILevelProgressService, IDataLoader, IDataSaveable {
    private PlayerProgressData _levelProgress = new() { CurrentLevel = 1, HighestLevelUnlocked = 1 };

    private const string LevelNamePrefix = "Level_";
    private readonly IDataRepository<PlayerProgressData> dataRepository;

    public LevelProgressService(IDataRepository<PlayerProgressData> dataRepository) {
        this.dataRepository = dataRepository;
    }

    public string GetCurrentLevelName() {
        return LevelNamePrefix + _levelProgress.CurrentLevel;
    }

    public PlayerProgressData GetProgress() {
        return _levelProgress;
    }

    public void SetProgress(PlayerProgressData progress) {
        _levelProgress = progress;
    }

    public void Load() {
        PlayerProgressData levelProgressData = dataRepository.Load();
        SetProgress(levelProgressData);
    }

    public void Save() {
        dataRepository.Save(_levelProgress);
    }
}

[Serializable]
[DataSource(DataSourceType.PlayerPrefs, "level_progress")]
public class PlayerProgressData {
    public int CurrentLevel;
    public int HighestLevelUnlocked;
}

