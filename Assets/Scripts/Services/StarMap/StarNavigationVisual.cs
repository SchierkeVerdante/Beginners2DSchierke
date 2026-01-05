using ModestTree;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class StarNavigationVisual : MonoBehaviour {
    [SerializeField] private NavStarView _starPrefab;

    [SerializeField] private Spaceship2D spaceship;
    [SerializeField] private Transform mapParent;
    [SerializeField] private float nodesXOffset = 2f;
    [SerializeField] private float nodesYOffset = 1f;

    private Dictionary<LayerCoord, NavStarView> starViews = new();

    [SerializeField] Button travelButton;
    public Action OnTravelRequest;

    private void Start() {
        if (travelButton != null)
        travelButton.onClick.AddListener(HandleTravelClick);
    }

    private void HandleTravelClick() {
        OnTravelRequest?.Invoke();
    }

    private void OnDestroy() {
        if (travelButton != null)
            travelButton.onClick.RemoveListener(HandleTravelClick);
    }

    public NavStarView SpawnStar(NavStar navStar, int starsInLayer) {
        int layer = navStar.StarCoord.Layer;
        int index = navStar.StarCoord.Index;

        Vector3 localPosition = CalculateStarPosition(layer, index, starsInLayer);

        Vector3 spawnPosition = mapParent.transform.position + localPosition;

        NavStarView view = Instantiate(_starPrefab, spawnPosition, Quaternion.identity, mapParent);

        view.gameObject.name = navStar.ToString();
        starViews.Add(navStar.StarCoord, view);

        return view;
    }

    public void DrawStarConnections(LayerCoord from, LayerCoord to) {
        if (starViews.TryGetValue(from, out NavStarView fromStar)) {
            if (starViews.TryGetValue(to, out NavStarView toStar)) {
                fromStar.DrawConnectionTo(toStar.transform.position);
            }
        }
    }

    public void ClearMap() {
        ClearParent();
        starViews.Clear();
    }

    private Vector3 CalculateStarPosition(int layer, int layerIndex, int layerStarsCount) {
        float xOffset = nodesXOffset * layer;
        float yOffset = (layerIndex - (layerStarsCount - 1) / 2f) * nodesYOffset;

        return new Vector3(xOffset, yOffset, 0);
    }

    private void ClearParent() {
        foreach (Transform child in mapParent) {
            Destroy(child.gameObject);
        }
    }

    public Action OnDestinationReached;

    public void MoveTo(LayerCoord destinationCoord) {
        if (starViews.TryGetValue(destinationCoord, out NavStarView destStar)) {
            Vector3 destPos = destStar.transform.position;

            spaceship.SetTarget(destPos, () => spaceship.StartOrbit());
            
        }
    }

}

public class StarNavigator {

    private readonly IStarMapService _starMap;
    private readonly IPresenterFactory<NavStarPresenter> presenterFactory;

    private readonly Dictionary<LayerCoord, NavStar> _coordsToStars = new();
    private readonly Dictionary<NavStar, NavStarPresenter> _starsToPresenters = new();

    private StarNavigationVisual _starNavigationVisual;

    private NavStar _currentNavStar;
    public NavStar CurrentNavStar => _currentNavStar;

    public event Action<NavStar> OnCurrentStarChanged;

    private NavStar _selectedNavStar;

    public NavStar SelectedNavStar => _selectedNavStar;
    private List<NavStar> _availableStars = new();

    public StarNavigator(StarNavigationVisual visual, IPresenterFactory<NavStarPresenter> presenterFactory, IStarMapService starMap) {
        _starNavigationVisual = visual;
        _starNavigationVisual.OnTravelRequest += HandleTravelRequest;
        this.presenterFactory = presenterFactory;
        _starMap = starMap;
    }

    private void HandleTravelRequest() {
        if (_selectedNavStar == null) return;

        TravelTo(_selectedNavStar);
    }

    public void ReloadMap() {
        _starNavigationVisual.ClearMap();

        _coordsToStars.Clear();
        _starsToPresenters.Clear();
        _currentNavStar = null;

        for (int layer = 0; layer < _starMap.LayersCount; layer++) {
            IReadOnlyList<Star> stars = _starMap.GetStarsInLayer(layer);
            SpawnLayer(layer, stars);
        }

        DrawStarConnections();

        LayerCoord startPosition = new LayerCoord(0, 0);
        _coordsToStars.TryGetValue(startPosition, out var startStar);
        TravelTo(startStar);
    }

    private void SpawnLayer(int layer, IReadOnlyList<Star> stars) {
        foreach (var star in stars) {
            NavStar navStar = new NavStar(star);
            LayerCoord starCoord = star.StarCoord;

            NavStarView view = _starNavigationVisual.SpawnStar(navStar, stars.Count);
            NavStarPresenter starPresenter = presenterFactory.Create(view, navStar, this);


            _coordsToStars.Add(starCoord, navStar);
            _starsToPresenters.Add(navStar, starPresenter);
        }
    }

    private void DrawStarConnections() {
        foreach (var navStar in _starsToPresenters.Keys) {
            LayerCoord currentCoords = navStar.StarCoord;

            LayerCoord[] layerCoords = navStar.GetNextConnections();

            List<Star> nextStars = _starMap.GetStarsAt(layerCoords);
            foreach (Star star in nextStars) {
                LayerCoord nextCoord = star.StarCoord;

                _starNavigationVisual.DrawStarConnections(currentCoords, nextCoord);
            }

        }
    }

    public bool TravelTo(NavStar targetNavStar) {
        if (!CanTravelTo(targetNavStar)) return false;

        NavStar oldStar = _currentNavStar;
        oldStar?.SetAvailability(false);
        oldStar?.SetVisited(true);
        oldStar?.SetCurrent(false);

        _currentNavStar = targetNavStar;

        _currentNavStar.SetCurrent(true);

        UpdateAvailableStars();
        OnCurrentStarChanged?.Invoke(_currentNavStar);
        _starNavigationVisual.MoveTo(_currentNavStar.StarCoord);

        return true;
    }

    public bool CanTravelTo(NavStar targetNavStar) {
        if (_currentNavStar == null && targetNavStar != null) return true;
        if (targetNavStar == null) return false;
        if (targetNavStar == _currentNavStar) return false;

        bool isConnected = _currentNavStar.AreConnectedTo(targetNavStar.StarCoord);
        bool isNext = _currentNavStar.StarCoord.Layer < targetNavStar.StarCoord.Layer;
        return isNext && isConnected;
    }

    public void SelectStar(NavStar navStar) {
        if (navStar == null) return;

        _selectedNavStar?.SetSelected(false);
        _selectedNavStar = navStar;
        _selectedNavStar.SetSelected(true);
        
    }

    private void UpdateAvailableStars() {
        if (!_availableStars.IsEmpty()) {
            foreach (var star in _availableStars) {
                star.SetAvailability(false);
            }
            _availableStars.Clear();
        }

        _starsToPresenters.TryGetValue(_currentNavStar, out NavStarPresenter presenter);

        List<NavStar> availableStars = GetNavStarsAt(_currentNavStar.GetNextConnections());

        foreach (var navStar in availableStars) {
            navStar?.SetAvailability(true);
            _availableStars.Add(navStar);
        }
    }

    private IReadOnlyList<NavStar> GetConnectedNavStars(NavStar star) {
        if (_currentNavStar == null) return Array.Empty<NavStar>();

        return GetNavStarsAt(star.Connections);
    }

    private List<NavStar> GetNavStarsAt(IEnumerable<LayerCoord> coords) {
        List<NavStar> foundStars = new();

        foreach (var coord in coords) {
            if (_coordsToStars.TryGetValue(coord, out var navStar)) {
                foundStars.Add(navStar);
            }
        }
        return foundStars;
    }
}