using UnityEngine;
using Zenject;

public class GraphGenerator : MonoBehaviour {
    [SerializeField] private GraphPipelineConfig graphPipelineConfig;
    [SerializeField] private MapGenerationData mapGenerationData;
    [Inject] IDataRuntimeFactory dataRuntimeFactory;

    public GraphView graphView;
   
    private GraphGenerationPipeline pipeline;

    public Graph GenerateGraph() {
        if (graphPipelineConfig == null) {
            Debug.LogError("GraphPipelineConfig is not assigned!");
        }

        if (pipeline == null)  pipeline = CreatePipeline();

        var context = new GraphGenerationContext(mapGenerationData);

        pipeline.Execute(context);

        Debug.Log($"Generated graph {context.Graph}");

        graphView.DisplayGraph(context.Graph);

        return context.Graph;
    }

    private GraphGenerationPipeline CreatePipeline() {
        var pipeline = new GraphGenerationPipeline();
        var stages = graphPipelineConfig.stageConfigs;
        foreach (var stageConfig in stages) {
            if (stageConfig == null) continue;
            var stageInstance = dataRuntimeFactory.CreateInstanse(stageConfig) as IPipelineStage<GraphGenerationContext>;
            pipeline.AddStage(stageInstance);
        }
        return pipeline;
    }
}
