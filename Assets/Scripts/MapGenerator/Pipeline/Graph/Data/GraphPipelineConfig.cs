using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GraphPipelineConfig", menuName = "Graph Generation/Pipeline Configuration")]
public class GraphPipelineConfig : ScriptableObject {
    [SerializeReference] public List<GraphStageConfig> stageConfigs;

    public GraphGenerationPipeline GeneratePipeline() {
        var pipeline = new GraphGenerationPipeline();

        foreach (var config in stageConfigs) {
            if (config != null) {
                var stage = config.CreateStage();
                pipeline.AddStage(stage);
            }
        }

        return pipeline;
    }
}

[Serializable]
public abstract class GraphStageConfig : BaseStageConfig<GraphGenerationContext> {

}

public class NodeCreationStageConfig : GraphStageConfig {
    public int minNodes = 10;
    public int maxNodes = 20;

    public override IPipelineStage<GraphGenerationContext> CreateStage() {
        return new NodeCreationStage(this);
    }
}

public class ConnectionStageConfig : GraphStageConfig {
    public float connectionProbability = 0.3f;
    public int maxConnectionsPerNode = 4;

    public override IPipelineStage<GraphGenerationContext> CreateStage() {
        return new ConnectionStage(this);
    }
}


