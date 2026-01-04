using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Graph {
    private List<List<GraphNode>> layerNodes = new();
    private int nextNodeId = 0;
    private string seed;
    public string Seed => seed;

    public Graph(string seed) {
        this.seed = seed;
    }

    public List<List<GraphNode>> Layers => layerNodes;

    public List<GraphNode> AllNodes => layerNodes.SelectMany(x => x).ToList();

    public void AddNodeToLevel(int level, GraphNode node) {
        while (layerNodes.Count <= level) {
            layerNodes.Add(new List<GraphNode>());
        }
        layerNodes[level].Add(node);
    }

    public bool RemoveNode(GraphNode node) {
        foreach (var level in layerNodes) {
            if (level.Contains(node)) {
                node.ClearConnections();
                return level.Remove(node);
            }
        }
        return false;
    }

    public void AddLevel(List<GraphNode> level) {
        layerNodes.Add(level);
    }

    public void AddLevel(int levelIndex, List<GraphNode> level) {
        layerNodes.Insert(levelIndex, level);
    }

    public void UpdateNodesData() {
        for (int level = 0; level < layerNodes.Count; level++) {
            List<GraphNode> levelNodes = this.layerNodes[level];
            for (int i = 0; i < levelNodes.Count; i++) {
                levelNodes[i].layer = level;
                levelNodes[i].layerIndex = i;
            }
        }
    }

    public int GetLayersCount() => layerNodes.Count;

    public int GetNodesAtLayer(int level) => level < layerNodes.Count ? layerNodes[level].Count : 0;

    public int GetNextNodeId() {
        return nextNodeId++;
    }

    public void Clear() {
        layerNodes.Clear();
        nextNodeId = 0;
    }

    public GraphNode GetEntranceNode() {
        return layerNodes[0][0];
    }

    public GraphNode GetEndNode() {
        return layerNodes[layerNodes.Count - 1][0];
    }

    public override string ToString() {
        string result = "";
        for (int level = 0; level < layerNodes.Count; level++) {
            result += $"Layer {level}:\n";
            foreach (var node in layerNodes[level]) {
                result += $" {node} - Connections: ";
                var connections = node.GetAllConnections();
                result += string.Join(", ", connections.Select(n => n.layerIndex));
                result += "\n";
            }
        }
        return result;
    }
}

