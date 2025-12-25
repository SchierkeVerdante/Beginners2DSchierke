using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GraphPipelineConfig", menuName = "Graph Generation/Pipeline Configuration")]
public class GraphPipelineConfig : ScriptableObject, IRuntimeConfig {
    public List<GraphStageConfig> stageConfigs;

    public Type RuntimeType => typeof(GraphGenerationPipeline);

    protected void OnValidate() {
        if (stageConfigs == null) return;

        foreach(var config in stageConfigs) {
            if (config == null) {
                stageConfigs.Remove(config);
            }
        }
    }
}


