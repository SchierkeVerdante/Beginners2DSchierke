using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Zenject;

public interface IStarMapService {
    public StarMap StarMap { get; }

    void AddStarPresenter(StarPresenter starPresenter);
    void Clear();
    Vector3 GetStarGlobalPosition(Star star);
    List<Star> GetStarsByCoords(IEnumerable<LayerCoord> coords);
    IReadOnlyList<Star> GetStarsInLayer(int layer);
    void LoadMap(StarMap map);
    bool TryGetStarByCoord(LayerCoord coord, out Star star);
    bool TryGetStarPresenter(Star star, out StarPresenter presenter);
}

public class StarMapService : IStarMapService {
    private StarMap starMap;

    public StarMap StarMap => starMap;

    public void LoadMap(StarMap map) {
        starMap = map;
    }

    public StarMap SaveMap() {
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

    public IReadOnlyList<Star> GetStarsInLayer(int layer) {
        return starMap.GetStarsInLayer(layer);
    }

    private Dictionary<LayerCoord, StarPresenter> starPresenters = new();
    public void AddStarPresenter(StarPresenter starPresenter) {
        starPresenters.Add(starPresenter.Model.StarCoord, starPresenter);
    }

    public Vector3 GetStarGlobalPosition(Star star) {
        if (TryGetStarPresenter(star, out StarPresenter presenter)) {
            return presenter.View.gameObject.transform.position;
        }

        return Vector3.zero;
    }

    public bool TryGetStarPresenter(Star star, out StarPresenter presenter) {
        return starPresenters.TryGetValue(star.StarCoord, out presenter);
    }

    public void Clear() {
        starPresenters.Clear();
    }
}


