using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Zenject;

public interface IStarMapService {
    StarMap StarMap { get; }
    int LayersCount { get; }

    IReadOnlyList<Star> GetAllStars();
    bool TryGetStarByCoord(LayerCoord coord, out Star star);
    List<Star> GetStarsAt(LayerCoord[] layerCoords);
    IReadOnlyList<Star> GetStarsInLayer(int layer);
    void LoadMap(StarMap map);
    
}

public class StarMapService : IStarMapService {
    private StarMap starMap;

    public StarMap StarMap => starMap;

    public int LayersCount => StarMap.LayersCount;

    public void LoadMap(StarMap map) {
        starMap = map;
    }

    public StarMap SaveMap() {
        return starMap;
    }

    public IReadOnlyList<Star> GetAllStars() {
        return starMap.GetAllStars();
    }

    public bool TryGetStarByCoord(LayerCoord coord, out Star star) {
        return starMap.TryFindStarAt(coord, out star);
    }


    public IReadOnlyList<Star> GetStarsInLayer(int layer) {
        return starMap.GetStarsInLayer(layer);
    }

    public List<Star> GetStarsAt(LayerCoord[] layerCoords) {
        return starMap.GetStarsAt(layerCoords);
    }
}


