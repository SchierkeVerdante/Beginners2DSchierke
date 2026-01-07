using System;

public interface IStarNavigationService {
    Star CurrentStar { get; }
    Star SelectedStar { get; }
    bool HasEmptyMap { get; }

    void SetCurrentPosition(LayerCoord startCoord);
    void SelectStar(Star star);
    bool TryTravelTo(Star star);
    bool CanTravelTo(Star star);
    void UpdateMap(StarMap starMap);
    void RefreshMap();

    event Action<Star> OnCurrentStarChanged;
    event Action<Star> OnStarSelected;
    event Action<StarMap> OnMapUpdated;
}

public class StarNavigationService : IStarNavigationService {
    private StarMap _starMap;
    private Star _currentStar;
    private Star _selectedStar;
    public Star CurrentStar => _currentStar;
    public Star SelectedStar => _selectedStar;

    public bool HasEmptyMap => _starMap == null;

    public event Action<Star> OnCurrentStarChanged;
    public event Action<Star> OnStarSelected;
    public event Action<StarMap> OnMapUpdated;

    public StarNavigationService(StarMap starMap) {
        _starMap = starMap ?? throw new ArgumentNullException(nameof(starMap));
    }
    public StarNavigationService() {
    }

    public void UpdateMap(StarMap newMap) {
        _starMap = newMap;
        OnMapUpdated?.Invoke(newMap);
    }

    public void SetCurrentPosition(LayerCoord startCoord) {
        if (_starMap.TryGetStar(startCoord, out var startStar)) {
            SetCurrentStar(startStar);
        }
    }

    public void SelectStar(Star star) {
        if (star == null) return;

        _selectedStar = star;
        OnStarSelected?.Invoke(star);
    }

    public bool TryTravelTo(Star star) {
        if (!CanTravelTo(star)) return false;

        SetCurrentStar(star);
        return true;
    }

    public bool CanTravelTo(Star star) {
        if (star == null || star == _currentStar) return false;

        if (_currentStar == null) return true;

        //If player didnt visited current star dont allow next star
        //if (_currentStar.State.Value == StarState.Visited) return false;

        return star.Coord.Layer > _currentStar.Coord.Layer &&
               _currentStar.IsConnectedTo(star.Coord);
    }

    private void SetCurrentStar(Star star) {
        if (_currentStar != null) {
            _currentStar.State.Value = StarState.Visited;
        }

        _currentStar = star;
        _currentStar.State.Value = StarState.Current;

        UpdateAvailableStars();
        OnCurrentStarChanged?.Invoke(_currentStar);
    }

    private void UpdateAvailableStars() {
        foreach (var star in _starMap.Stars.Values) {
            if (star.State.Value == StarState.Available) {
                star.State.Value = StarState.Locked;
            }
        }

        foreach (var coord in _currentStar.GetForwardConnections()) {
            if (_starMap.TryGetStar(coord, out var nextStar)) {
                nextStar.State.Value = StarState.Available;
            }
        }
    }

    public void RefreshMap() {
        OnMapUpdated?.Invoke(_starMap);
    }
}

