using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class NodeCreationStage : IPipelineStage<GraphGenerationContext> {
    private readonly NodeCreationStageConfig config;

    public string StageName => "Node Creation";

    public NodeCreationStage(NodeCreationStageConfig config) {
        this.config = config;
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

        if (IsFirstOrLastLevel(level, settings)) {
            return 1;
        }

        int prevLevelNodeCount = levelNodes[level - 1].Count;
        var (minPossible, maxPossible) = CalculateNodeRange(prevLevelNodeCount, settings);

        return context.Random.Next(minPossible, maxPossible + 1);
    }

    private bool IsFirstOrLastLevel(int level, GraphGenerationConfig settings) {
        return level == 0 || level == settings.levelCount - 1;
    }

    private (int min, int max) CalculateNodeRange(int prevLevelNodeCount, GraphGenerationConfig settings) {
        int minPossible = CalculateMinPossible(prevLevelNodeCount, settings);
        int maxPossible = CalculateMaxPossible(prevLevelNodeCount, settings);

        return (Math.Min(minPossible, maxPossible), Math.Max(minPossible, maxPossible));
    }

    private int CalculateMinPossible(int prevLevelNodeCount, GraphGenerationConfig settings) {
        int min = Math.Max(settings.minNodesPerLevel, prevLevelNodeCount - settings.maxNodeDeviation);

        if (!settings.allowGradualDecrease) {
            min = Math.Max(min, prevLevelNodeCount);
        }

        return min;
    }

    private int CalculateMaxPossible(int prevLevelNodeCount, GraphGenerationConfig settings) {
        int max = Math.Min(settings.maxNodesPerLevel, prevLevelNodeCount + settings.maxNodeDeviation);

        if (!settings.allowGradualIncrease) {
            max = Math.Min(max, prevLevelNodeCount);
        }

        return max;
    }
}
