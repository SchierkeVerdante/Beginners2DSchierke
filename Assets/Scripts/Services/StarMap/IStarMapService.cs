using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Zenject;

public interface IStarMapService {
    StarMap StarMap { get; }
    IReadOnlyList<Star> GetStarsInLayer(int layer);
    void LoadMap(StarMap map);
    
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



    public IReadOnlyList<Star> GetStarsInLayer(int layer) {
        return starMap.GetStarsInLayer(layer);
    }

}


