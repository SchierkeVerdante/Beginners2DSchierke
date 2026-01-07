using System;
using UnityEngine.Audio;

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
        StarMap starMap = CreateFromGraph(_config);
        PopulatePlanetData(starMap);
        return starMap;
    }

    public void Load() {
        planetsData = planetsRepo.Load();
    }

    private StarMap CreateFromGraph(StarMapGenerationConfig _config) {
        var graph = graphGenerator.GenerateGraph(_config.graphConfig);
        var map = new StarMap(graph.Seed);

        foreach (var layer in graph.Layers) {
            foreach (var node in layer) {

                string starName = _starNamer.GetUniqueName();

                var star = new Star(node.layer, node.layerIndex, starName);
                map.AddStar(star);

                foreach (var connection in node.GetAllConnections()) {
                    star.ConnectTo(new LayerCoord(connection.layer, connection.layerIndex));
                }
            }
        }

        return map;
    }

    private void PopulatePlanetData(StarMap starMap) {
        int seed = starMap.Seed.GetHashCode();
        Random random = new Random(seed);

        foreach (Star star in starMap.Stars.Values) {
            PlanetConfig planetConfig = new PlanetConfig();
            planetConfig.seed = seed;
            planetConfig.BiomeLabel = "desert";

            star.SetPlanetData(planetConfig);
        }
    }
}
