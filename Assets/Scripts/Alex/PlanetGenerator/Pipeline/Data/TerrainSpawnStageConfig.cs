using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TerrainSpawnStageConfig", menuName = "PlanetGen/Stages/Terrain Spawn")]
// public class TerrainSpawnStageConfig : GenericInstanceConfig<TerrainSpawnStage> {
public class TerrainSpawnStageConfig : ScriptableObject {

    public string terrainType = "crater";
    public int spawnRate = 1;
    public int obstacleDensity = 1;
    public int oilCount = 6;
    public int moduleCount = 3;
    public List<GameObject> enemyList;
    // public List<string> _modulesList = {"0","0","0","0"};
}
