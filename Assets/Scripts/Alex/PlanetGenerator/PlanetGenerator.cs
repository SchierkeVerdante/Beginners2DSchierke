using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using Zenject;

public class PlanetGenerator : MonoBehaviour {
    [Header("Data")]
    [SerializeField] private PlanetsData _planetsData;
    private Pipeline<PlanetGenContext> planetGenPipeline;
    [Inject] IDataRuntimeFactory _runtimeFactory;

    [SerializeField] private WorldTilemapGrid _terrainGridPrefab;

    private readonly IStarNavigationService _starNavigationService;

    private void Start() {
        PlanetConfig planetGenData = new();
        planetGenData.BiomeLabel = "ice";

        planetGenPipeline = new();
        foreach (var stageConfig in _planetsData.pipelineStages) {
            var instance = _runtimeFactory.CreateInstance(stageConfig) as IPipelineStage<PlanetGenContext>;
            planetGenPipeline.AddStage(instance);
        }

        //GeneratePlanet(planetGenData);
    }

    public void GeneratePlanet(PlanetConfig planetGenConfig) {
        Debug.Log($"Generating {planetGenConfig.BiomeLabel} biome label");
        if (_terrainGridPrefab == null) {
            Debug.LogWarning("TilemapPrefab not assigned");
            return;
        }

        WorldTilemapGrid worldTilemapGrid = Instantiate(_terrainGridPrefab, Vector3.zero, Quaternion.identity);
        Tilemap tilemap = worldTilemapGrid.tilemap;

        BiomeData choosendBiome = DefineBiomeData(planetGenConfig);
        PlanetGenContext planetGen = new(_planetsData, planetGenConfig, choosendBiome, tilemap);
        planetGenPipeline.Execute(planetGen);
    }

    private BiomeData DefineBiomeData(PlanetConfig planetGenConfig) {
        string requestedBiome = planetGenConfig.BiomeLabel.ToLower();

        for (int i = 0; i < _planetsData.biomes.Count; i++) {
            BiomeData biomeData = _planetsData.biomes[i];
            string currentBiomeLabel = biomeData.biomeLabel.ToLower();

            if (currentBiomeLabel.Equals(requestedBiome)) {
                return biomeData;
            }
        }

        Debug.LogWarning("Cant find biome for label: " + requestedBiome);
        if (_planetsData.biomes.Count > 0) {
            return _planetsData.biomes[0];
        }

        return null;
    }
}

public class PlanetGenContext {
    public PlanetsData planetsRes;
    public PlanetConfig planetConfig;
    public BiomeData choosenBiome;
    public Tilemap _terrainTilemap;

    public System.Random Random { get; }

    public PlanetGenContext(PlanetsData planetsRes, PlanetConfig planetConfig, BiomeData choosenBiome, Tilemap terrainTilemap) {
        this.planetsRes = planetsRes;
        this.planetConfig = planetConfig;
        Random = new System.Random(planetConfig.seed);
        this.choosenBiome = choosenBiome;
        _terrainTilemap = terrainTilemap;
    }
}

public class PlanetConfig {
    public string BiomeLabel = "Unknown";
    public BiomeData biomeData;
    public int seed = 12345;

    public float obstacleDensity = 0.05f;
    public float enemyDensity = 0.03f;

    public List<string> modulesList = new();
    public Sprite planetSprite;
    public float normalizedVolume;
}
