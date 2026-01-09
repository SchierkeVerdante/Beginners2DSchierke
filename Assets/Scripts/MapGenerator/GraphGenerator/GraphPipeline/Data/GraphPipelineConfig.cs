using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GraphPipelineConfig", menuName = "Graph Generation/Pipeline Configuration")]
public class GraphPipelineConfig : GenericInstanceConfig<GraphGenerationPipeline> {
    public List<InstanceConfig> stageConfigs;

    protected override void OnValidate() {
        base.OnValidate();
        if (stageConfigs == null) return;

        foreach(var config in stageConfigs) {
            if (config == null) {
                stageConfigs.Remove(config);
            }
        }
    }
}


