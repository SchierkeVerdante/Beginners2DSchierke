using System;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionStage : IPipelineStage<GraphGenerationContext> {
    private readonly ConnectionStageConfig config;
    private readonly IRandomService randomService;
    public string StageName => "Connection Creation";

    public ConnectionStage(ConnectionStageConfig config, IRandomService randomService) {
        this.config = config ?? throw new ArgumentNullException(nameof(config));
        this.randomService = randomService ?? throw new ArgumentNullException(nameof(randomService));
    }

    public void Execute(GraphGenerationContext context) {
        Debug.Log("Connecting nodes...");
        Graph graph = context.Graph;
        List<List<GraphNode>> levelNodes = graph.Layers;

        for (int level = 0; level < levelNodes.Count - 1; level++) {
            ConnectLevel(levelNodes[level], levelNodes[level + 1]);
        }

        EnsureAllNodesConnected(levelNodes);

        context.SetData("ConnectionStageComplete", true);
    }

    private void ConnectLevel(List<GraphNode> currentLevel, List<GraphNode> nextLevel) {
        foreach (var currentNode in currentLevel) {
            ConnectToNextLevel(currentNode, nextLevel);
        }
    }

    private void ConnectToNextLevel(GraphNode currentNode, List<GraphNode> nextLevel) {
        var nearbyNodes = GetNearbyNodesByDistance(currentNode, nextLevel);

        if (nearbyNodes.Count == 0) {
            ConnectToClosest(currentNode, nextLevel);
            return;
        }

        int connectionsCreated = TryCreateRandomConnections(currentNode, nearbyNodes);

        if (connectionsCreated == 0) {
            ConnectToRandomNearby(currentNode, nearbyNodes);
        }
    }

    private int TryCreateRandomConnections(GraphNode currentNode, List<NodeDistance> nearbyNodes) {
        int connectionsCreated = 0;
        int maxConnections = config.MaxConnectionsPerNode;

        foreach (var nodeDistance in nearbyNodes) {
            if (connectionsCreated >= maxConnections) break;

            float distanceFactor = 1f - (nodeDistance.distance / config.MaxConnectionDistance);
            float connectionChance = config.RandomConnectionChance * distanceFactor;

            if (randomService.NextFloat() <= connectionChance) {
                currentNode.ConnectTo(nodeDistance.node);
                connectionsCreated++;
            }
        }

        return connectionsCreated;
    }

    private void ConnectToRandomNearby(GraphNode currentNode, List<NodeDistance> nearbyNodes) {
        int candidatesCount = Mathf.Min(config.ConnectionCandidatesCount, nearbyNodes.Count);
        int randomIndex = randomService.Next(0, candidatesCount);
        currentNode.ConnectTo(nearbyNodes[randomIndex].node);
    }

    private void ConnectToClosest(GraphNode currentNode, List<GraphNode> targetLevel) {
        GraphNode closest = FindClosestNode(currentNode, targetLevel);
        if (closest != null) {
            currentNode.ConnectTo(closest);
        }
    }

    private List<NodeDistance> GetNearbyNodesByDistance(GraphNode currentNode, List<GraphNode> targetLevel) {
        List<NodeDistance> distances = new List<NodeDistance>();
        float maxDistanceSqr = config.MaxConnectionDistance * config.MaxConnectionDistance;

        foreach (var targetNode in targetLevel) {
            float distanceSqr = (currentNode.Position - targetNode.Position).sqrMagnitude;

            if (distanceSqr <= maxDistanceSqr) {
                distances.Add(new NodeDistance (
                    targetNode,
                    Mathf.Sqrt(distanceSqr)
                ));
            }
        }

        distances.Sort((a, b) => a.distance.CompareTo(b.distance));
        return distances;
    }

    private GraphNode FindClosestNode(GraphNode currentNode, List<GraphNode> targetLevel) {
        if (targetLevel.Count == 0) return null;

        GraphNode closest = null;
        float minDistanceSqr = float.MaxValue;

        foreach (var targetNode in targetLevel) {
            float distanceSqr = (currentNode.Position - targetNode.Position).sqrMagnitude;
            if (distanceSqr < minDistanceSqr) {
                minDistanceSqr = distanceSqr;
                closest = targetNode;
            }
        }

        return closest;
    }

    private void EnsureAllNodesConnected(List<List<GraphNode>> levelNodes) {
        for (int level = 0; level < levelNodes.Count - 1; level++) {
            foreach (var node in levelNodes[level]) {
                if (node.NextConnections.Count == 0) {
                    ConnectToClosest(node, levelNodes[level + 1]);
                    Debug.LogWarning($"Fixed isolated node at level {level} (no forward connections)");
                }
            }
        }

        for (int level = 1; level < levelNodes.Count; level++) {
            foreach (var node in levelNodes[level]) {
                if (!node.HasConnectionsToPrevLevel()) {
                    GraphNode closest = FindClosestNode(node, levelNodes[level - 1]);
                    if (closest != null) {
                        closest.ConnectTo(node);
                        //Debug.LogWarning($"Fixed isolated node at level {level} (no backward connections)");
                    }
                }
            }
        }
    }

    private readonly struct NodeDistance {
        public readonly GraphNode node;
        public readonly float distance;

        public NodeDistance(GraphNode node, float distance) {
            this.node = node;
            this.distance = distance;
        }
    }
}