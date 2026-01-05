using UnityEngine;

public class GraphGenerationPipeline : Pipeline<GraphGenerationContext> {
    protected override void OnExecutionStart(GraphGenerationContext context) {
        //Debug.Log($"[Pipeline] Starting graph generation with {stages.Count} stages");
    }

    protected override void OnStageStart(IPipelineStage<GraphGenerationContext> stage, GraphGenerationContext context) {
        //Debug.Log($"[Pipeline] Executing stage: {stage.StageName}");
    }

    protected override void OnStageComplete(IPipelineStage<GraphGenerationContext> stage, GraphGenerationContext context) {
        //Debug.Log($"[Pipeline] Completed stage: {stage.StageName}");
    }

    protected override void OnExecutionComplete(GraphGenerationContext context) {
       Debug.Log($"[Pipeline] Graph generation completed. Total nodes: {context.Graph.AllNodes.Count}");
    }
}
