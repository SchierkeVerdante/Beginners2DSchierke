using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.STP;

[CreateAssetMenu(fileName = "PlanetData", menuName = "PlanetGen/Planets Data")]
[DataSource(DataSourceType.Resources, "StarMap/PlanetData")]
public class PlanetsData : ScriptableObject {
    [Header("Biomes")]
    public List<BiomeData> biomes;

    [Range (min: 10, max: 100)]
    public int defaultPlanetVolume = 20;

    public BiomeData GetBiome(string biomeId) {
        foreach (var biome in biomes) {
            if (biome.biomeLabel == biomeId) return biome;
        }
        return biomes.Count > 0 ? biomes[0] : null;
    }

    public BiomeData GetRandomBiome() {
        if (biomes.Count == 0) return null;
        return biomes[UnityEngine.Random.Range(0, biomes.Count)];
    }

    public List<InstanñeConfig> pipelineStages = new();
}
