using UnityEngine;

public class GraphGeneratorController : MonoBehaviour {
    [SerializeField] private GraphPipelineConfig graphPipelineConfig;
    [SerializeField] private MapGenerationData mapGenerationData;
    [SerializeField] private int seed = 12345;

    private void Start() {
        if (graphPipelineConfig == null) {
            Debug.LogError("GraphPipelineConfig is not assigned!");
            return;
        }

        var pipeline = graphPipelineConfig.GeneratePipeline();
        var context = new GraphGenerationContext(mapGenerationData, seed);

        pipeline.Execute(context);

        Debug.Log($"Generated graph with {context.Graph.GetAllNodes().Count} nodes");
    }
}
