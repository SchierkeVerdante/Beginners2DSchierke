using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Audio;
using static UnityEngine.Rendering.STP;

public interface IStarMapGenerator {
    StarMap GenerateNewMap(StarMapGenerationConfig _config);
}

public class StarMapGenerator : IStarMapGenerator, IDataLoader {
    private readonly IDataRuntimeFactory _dataFactory;
    private readonly StarNamerService _starNamer;
    private readonly GraphGenerator graphGenerator;
    
    private readonly IDataRepository<PlanetsData> planetsRepo;
    private PlanetsData planetsData;

    public StarMapGenerator(
        IDataRuntimeFactory dataFactory,
        StarNamerService starNamer) {
        _dataFactory = dataFactory;
        _starNamer = starNamer;

        graphGenerator = new GraphGenerator(_dataFactory);
    }

    public StarMap GenerateNewMap(StarMapGenerationConfig _config) {

        var map = new StarMap(_config.graphConfig.SeedString);

        StarGenContext context = new StarGenContext(_config, map);

        // Soon can become pipeline pattern
        CreateFromGraph(context);
        PopulatePlanetData(context);

        return context.StarMap;
    }

    public void Load() {
        planetsData = planetsRepo.Load();
    }

    private void CreateFromGraph(StarGenContext context) {
        var map = context.StarMap;
        var graph = graphGenerator.GenerateGraph(context.Config.graphConfig);
        _starNamer.ResetNameUniq();

        foreach (var layer in graph.Layers) {
            foreach (var node in layer) {

                string starName = _starNamer.GetUniqueName(context.Random);

                var star = new Star(node.layer, node.layerIndex, starName);
                map.AddStar(star);

                foreach (var connection in node.GetAllConnections()) {
                    star.ConnectTo(new LayerCoord(connection.layer, connection.layerIndex));
                }
            }
        }
    }

    private void PopulatePlanetData(StarGenContext context) {
        PlanetsData planetsData = context.Config.planetsData;
        List<BiomeData> biomes = planetsData.biomes;

        IEnumerable<Star> stars = context.StarMap.Stars.Values;
        int intSeed = context.StarMap.Seed.GetHashCode();

        foreach (Star star in stars) {
            int biomeIndex = context.Random.Next(0, biomes.Count);
            BiomeData biomeData = biomes[biomeIndex];

            PlanetConfig planetConfig = new PlanetConfig();
            planetConfig.seed = intSeed;

            planetConfig.BiomeLabel = biomeData.biomeLabel;
            planetConfig.biomeData = biomeData;
            planetConfig.planetSprite = biomeData.planetSprite;
            planetConfig.normalizedVolume = GetPlanetSize(context);
            star.SetPlanetData(planetConfig);
        }
    }

    private float GetPlanetSize(StarGenContext context) {
        Random random = context.Random;
        float minVolumeN = context.Config.planetsData.minPlanetVolumeNormalized;

        float value = (float)random.NextDouble();

        return minVolumeN + value * minVolumeN;
    }

}


public class StarGenContext {
    public Random Random { get; }
    public StarMapGenerationConfig Config { get; }
    public StarMap StarMap { get; }

    public StarGenContext(StarMapGenerationConfig config, StarMap starMap) {
        Config = config ?? throw new ArgumentNullException(nameof(config));
        StarMap = starMap ?? throw new ArgumentNullException(nameof(starMap));

        Random = new Random(config.graphConfig.Seed);
    }
}
