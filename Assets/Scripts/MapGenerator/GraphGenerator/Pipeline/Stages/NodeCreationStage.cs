using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class NodeCreationStage : IPipelineStage<GraphGenerationContext> {
    private readonly NodeCreationStageConfig config;
    private readonly IRandomService randomService;

    public string StageName => "Node Creation";

    public NodeCreationStage(NodeCreationStageConfig config, IRandomService randomService) {
        this.config = config;
        this.randomService = randomService;
    }

    public void Execute(GraphGenerationContext context) {
        Debug.Log("Creating nodes...");
        GraphGenerationConfig settings = context.Config;
        Graph graph = context.Graph;

        for (int level = 0; level < settings.levelCount; level++) {

            List<GraphNode> currentLevelNodes = CreateNodesLevel(level, context);

            graph.AddLevel(currentLevelNodes);
        }
    }

    private List<GraphNode> CreateNodesLevel(int level, GraphGenerationContext context) {
        GraphGenerationConfig settings = context.Config;

        List<GraphNode> currentLevelNodes = new List<GraphNode>();
        
        int nodesToCreate = CalculateTargetNodeCount(level, context.Graph.Layers, context);

        for (int i = 0; i < nodesToCreate; i++) {
            GraphNode newNode = new GraphNode(level, i);
            currentLevelNodes.Add(newNode);
        }

        return currentLevelNodes;
    }

    private int CalculateTargetNodeCount(int level, List<List<GraphNode>> levelNodes, GraphGenerationContext context) {
        GraphGenerationConfig settings = context.Config;

        if (level == 0 || level == settings.levelCount - 1) {
            return 1;
        }
        int prevLevelNodeCount = levelNodes[level - 1].Count;
        int minPossible = Mathf.Max(settings.minNodesPerLevel, prevLevelNodeCount - settings.maxNodeDeviation);
        int maxPossible = Mathf.Min(settings.maxNodesPerLevel, prevLevelNodeCount + settings.maxNodeDeviation);
        if (!settings.allowGradualIncrease) {
            maxPossible = Mathf.Min(maxPossible, prevLevelNodeCount);
        }
        if (!settings.allowGradualDecrease) {
            minPossible = Mathf.Max(minPossible, prevLevelNodeCount);
        }
        return randomService.Next(minPossible, maxPossible + 1);
    }
}
