using UnityEngine;

public class NodeCreationStage : IPipelineStage<GraphGenerationContext> {
    private readonly NodeCreationStageConfig config;

    public string StageName => "Node Creation";

    public NodeCreationStage(NodeCreationStageConfig config) {
        this.config = config;
    }

    public void Execute(GraphGenerationContext context) {
        Debug.Log("Creating nodes...");
        int nodeCount = context.Random.Next(config.minNodes, config.maxNodes + 1);

        for (int i = 0; i < nodeCount; i++) {
        }
    }
}
