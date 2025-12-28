using UnityEngine;
using Zenject;

public class GraphGenerator {
    private readonly GraphGenerationConfig graphGenerationData;
    private readonly IDataRuntimeFactory dataRuntimeFactory;

    public GraphGenerator(GraphGenerationConfig graphGenerationData, IDataRuntimeFactory dataRuntimeFactory) {
        this.graphGenerationData = graphGenerationData;
        this.dataRuntimeFactory = dataRuntimeFactory;
    }

   
    private GraphGenerationPipeline pipeline;

    public Graph GenerateGraph() {
        if (graphGenerationData.graphPipelineConfig == null) {
            Debug.LogError("GraphPipelineConfig is not assigned!");
        }

        if (pipeline == null)  pipeline = CreatePipeline();

        var context = new GraphGenerationContext(graphGenerationData);

        pipeline.Execute(context);

        Debug.Log($"Generated graph {context.Graph}");

        return context.Graph;
    }

    private GraphGenerationPipeline CreatePipeline() {
        var pipeline = new GraphGenerationPipeline();
        var stages = graphGenerationData.graphPipelineConfig.stageConfigs;

        foreach (var stageConfig in stages) {
            if (stageConfig == null) continue;
            var stageInstance = dataRuntimeFactory.CreateInstanse(stageConfig) as IPipelineStage<GraphGenerationContext>;
            pipeline.AddStage(stageInstance);
        }
        return pipeline;
    }
}
