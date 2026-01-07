using UnityEngine;
using Zenject;

public class GraphGenerator {
    private readonly IDataRuntimeFactory dataRuntimeFactory;

    public GraphGenerator(IDataRuntimeFactory dataRuntimeFactory) {
        this.dataRuntimeFactory = dataRuntimeFactory;
    }

    private GraphGenerationPipeline pipeline;

    public Graph GenerateGraph(GraphGenerationConfig graphGenerationData) {
        if (graphGenerationData.graphPipelineConfig == null) {
            Debug.LogError("GraphPipelineConfig is not assigned!");
        }

        if (pipeline == null)  pipeline = CreatePipeline(graphGenerationData);

        var context = new GraphGenerationContext(graphGenerationData);

        pipeline.Execute(context);

        //Debug.Log($"Generated graph {context.Graph}");

        return context.Graph;
    }

    private GraphGenerationPipeline CreatePipeline(GraphGenerationConfig graphGenerationData) {
        var pipeline = new GraphGenerationPipeline();
        var stages = graphGenerationData.graphPipelineConfig.stageConfigs;

        foreach (var stageConfig in stages) {
            if (stageConfig == null) continue;
            var stageInstance = dataRuntimeFactory.CreateInstance(stageConfig) as IPipelineStage<GraphGenerationContext>;
            pipeline.AddStage(stageInstance);
        }
        return pipeline;
    }
}
