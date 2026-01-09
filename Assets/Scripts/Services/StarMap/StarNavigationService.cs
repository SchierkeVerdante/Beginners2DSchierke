using System;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;

public interface IStarNavigationService {
    Star CurrentStar { get; }
    Star SelectedStar { get; }
    bool HasEmptyMap { get; }

    void SetCurrentPosition(LayerCoord startCoord);
    void SelectStar(Star star);
    bool TryTravelTo(Star star);
    bool CanTravelTo(Star star);
    void SetNewMap(StarMap starMap);
    void RefreshMap();
    void ClearMap();

    event Action<Star> OnCurrentStarChanged;
    event Action<Star> OnStarSelected;
    event Action<StarMap> OnNewMapSet;
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
    public event Action<StarMap> OnNewMapSet;

    public StarNavigationService(StarMap starMap = null) {
        _starMap = starMap;
    }

    public void SetNewMap(StarMap newMap) {
        CleanupCurrentStar();

        _starMap = newMap;
        LayerCoord beginningCoords = _starMap.GetBeginningCoords();
        SetCurrentPosition(beginningCoords);

        OnNewMapSet?.Invoke(newMap);
    }

    public void SetCurrentPosition(LayerCoord startCoord) {
        if (_starMap.TryGetStar(startCoord, out var startStar)) {
            SetCurrentStar(startStar);
        }
    }

    public void SelectStar(Star selectedStar) {
        if (selectedStar == null) return;

        if (_selectedStar != null) {
            _selectedStar.IsSelected.Value = false;
        }

        _selectedStar = selectedStar;
        _selectedStar.IsSelected.Value = true;

        OnStarSelected?.Invoke(_selectedStar);
    }

    public bool TryTravelTo(Star star) {
        if (!CanTravelTo(star)) return false;

        SetCurrentStar(star);
        return true;
    }

    public bool CanTravelTo(Star star) {
        if (star == null || star == _currentStar) return false;
        if (_currentStar == null) return true;

        bool currentStarVisited = _currentStar.IsVisited.Value;
        bool isNextLayer = star.Coord.Layer > _currentStar.Coord.Layer;
        bool isConnected = _currentStar.IsConnectedTo(star.Coord);

        return currentStarVisited && isNextLayer && isConnected;
    }

    private void SetCurrentStar(Star star) {
        CleanupCurrentStar();

        _currentStar = star;
        _currentStar.IsCurrent.Value = true;

        _currentStar.IsVisited.OnChanged += OnCurrentStarVisited;

        UpdateStarAvailability();

        OnCurrentStarChanged?.Invoke(_currentStar);
    }

    private void CleanupCurrentStar() {
        if (_currentStar != null) {
            _currentStar.IsCurrent.Value = false;
            _currentStar.IsVisited.OnChanged -= OnCurrentStarVisited;
        }
    }

    private void OnCurrentStarVisited(bool isVisited) {
        if (isVisited) {
            UpdateStarAvailability();
        }
    }

    private void UpdateStarAvailability() {
        if (_starMap == null || _currentStar == null) return;

        ResetAllStarsAvailability();

        if (_currentStar.IsVisited.Value) {
            OpenConnectedStars();
        }
    }

    private void ResetAllStarsAvailability() {
        foreach (var star in _starMap.Stars.Values) {
            star.IsAvailable.Value = false;
        }
    }

    private void OpenConnectedStars() {
        foreach (var coord in _currentStar.GetForwardConnections()) {
            if (_starMap.TryGetStar(coord, out var nextStar)) {
                nextStar.IsAvailable.Value = true;
            }
        }
    }

    public void RefreshMap() {
        OnNewMapSet?.Invoke(_starMap);
    }

    public void ClearMap() {
        CleanupCurrentStar();
        _starMap = null;
        OnNewMapSet?.Invoke(_starMap);
    }
}