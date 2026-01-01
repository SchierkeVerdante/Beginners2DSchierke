using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Zenject;

public interface IStarMapService {

}

public class StarMapService : IStarMapService {
    private StarMap starMap;

    public void LoadStarMap(StarMap map) {
        starMap = map;
    }

    public StarMap SaveStarMap() {
        return starMap;
    }

    public IReadOnlyList<Star> GetAllStars() {
        return starMap.GetAllStars();
    }

    public IReadOnlyList<Star> GetConnectedStars(Star star) {
        return GetStarsByCoords(star.Connections);
    } 

    public bool TryGetStarByCoord(LayerCoord coord, out Star star) {
        return starMap.TryFindStarAt(coord, out star);
    }

    public List<Star> GetStarsByCoords(IEnumerable<LayerCoord> coords) {
        List<Star> stars = new List<Star>();

        foreach (var coord in coords) {
            if (TryGetStarByCoord(coord, out Star found)) {
                stars.Add(found);
            }
        }

        return stars;
    }
}

public class StarNavigation : MonoBehaviour {
    [SerializeField] private Spaceship2D spaceship;
    [Inject] StarMapService starMapService;
    private int travelDistance = 1;
    private Star _currentStar;

    public List<Star> GetAvailableStars() {
        if (_currentStar == null) {
            Debug.LogWarning("Current star is not set!");
            return new List<Star>();
        }

        LayerCoord starRef = _currentStar.StarCoord;

        int maxTravelDistance = _currentStar.StarCoord.Layer + travelDistance;

        IEnumerable<LayerCoord> nextConnections = _currentStar.Connections
            .Where(r => r.Layer > _currentStar.StarCoord.Layer && r.Layer <= maxTravelDistance);

        return starMapService.GetStarsByCoords(nextConnections);
    }

    public Star GetCurrentStar() {
        return _currentStar;
    }

    public void MoveTo(LayerCoord coords) {
        if (!starMapService.TryGetStarByCoord(coords, out Star star)) {

            return;
        }

        _currentStar = star;
    }
}


public class StarNavigationVisual : MonoBehaviour {
    [SerializeField] private StarView _starPrefab;
    [Inject] IStarMapService starMapService;

    [SerializeField] private Transform mapParent;
    [SerializeField] private float nodesXOffset = 2f;
    [SerializeField] private float nodesYOffset = 1f;

    private void CreateStars(StarMap map) {
        foreach (var star in map.GetAllStars()) {
            LayerCoord coord = star.StarCoord;
            Vector3 position = CalculatePosition(coord.Layer, coord.Index);

            StarView view = Instantiate(_starPrefab, position, Quaternion.identity, mapParent);

            view.gameObject.name = $"Star : {coord}";
            view.OnClicked += OnStarClicked;

            // Малювання ліній
            foreach (var conn in star.Connections) {
                if (conn.Layer > star.StarCoord.Layer) {
                    Vector3 targetPos = CalculatePosition(conn.Layer, conn.Index);
                    view.DrawConnectionTo(targetPos);
                }
            }
        }
    }

    private void OnStarClicked(StarView star) {
        Debug.Log($"Received click from star {star}");
    }

    private Vector3 CalculatePosition(int layer, int layerIndex) {
        return new Vector3(layer, layerIndex);
    }

    public void Show() {

    }
}