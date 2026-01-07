using UnityEngine.Audio;

public interface IStarMapGenerator {
    StarMap GenerateNewMap(StarMapGenerationConfig _config);
}

public class StarMapGenerator : IStarMapGenerator {
    private readonly IDataRuntimeFactory _dataFactory;
    private readonly StarNamerService _starNamer;
    private readonly GraphGenerator graphGenerator;

    public StarMapGenerator(
        IDataRuntimeFactory dataFactory,
        StarNamerService starNamer) {
        _dataFactory = dataFactory;
        _starNamer = starNamer;

        graphGenerator = new GraphGenerator(_dataFactory);
    }

    public StarMap GenerateNewMap(StarMapGenerationConfig _config) {
        return CreateFromGraph(_config);
    }

    private StarMap CreateFromGraph(StarMapGenerationConfig _config) {
        var graph = graphGenerator.GenerateGraph(_config.graphConfig);
        var map = new StarMap(graph.Seed);

        foreach (var layer in graph.Layers) {
            foreach (var node in layer) {
                var star = new Star(node.layer, node.layerIndex, _starNamer.GetUniqueName());
                map.AddStar(star);

                foreach (var connection in node.GetAllConnections()) {
                    star.ConnectTo(new LayerCoord(connection.layer, connection.layerIndex));
                }
            }
        }

        return map;
    }
}
