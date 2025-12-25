using UnityEngine;

public class ConnectionStage : IPipelineStage<GraphGenerationContext> {
    private readonly ConnectionStageConfig config;

    public string StageName => "Connection Creation";

    public ConnectionStage(ConnectionStageConfig config) {
        this.config = config;
    }

    public void Execute(GraphGenerationContext context) {
        Debug.Log("Connecting nodes...");
    }
}
