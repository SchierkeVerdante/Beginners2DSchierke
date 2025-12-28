using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Graph {
    private List<List<GraphNode>> levelNodes = new();
    private int nextNodeId = 0;

    public void AddNodeToLevel(int level, GraphNode node) {
        while (levelNodes.Count <= level) {
            levelNodes.Add(new List<GraphNode>());
        }
        levelNodes[level].Add(node);
    }

    public bool RemoveNode(GraphNode node) {
        foreach (var level in levelNodes) {
            if (level.Contains(node)) {
                node.ClearConnections();
                return level.Remove(node);
            }
        }
        return false;
    }

    public void AddLevel(List<GraphNode> level) {
        levelNodes.Add(level);
    }

    public void AddLevel(int levelIndex, List<GraphNode> level) {
        levelNodes.Insert(levelIndex, level);
    }

    public void UpdateNodesData() {
        for (int level = 0; level < levelNodes.Count; level++) {
            List<GraphNode> levelNodes = this.levelNodes[level];
            for (int i = 0; i < levelNodes.Count; i++) {
                levelNodes[i].level = level;
                levelNodes[i].index = i;
            }
        }
    }

    public List<List<GraphNode>> GetLevelNodes() => levelNodes;

    public List<GraphNode> GetAllNodes() {
        List<GraphNode> allNodes = new List<GraphNode>();
        foreach (var level in levelNodes) {
            allNodes.AddRange(level);
        }
        return allNodes;
    }

    public int GetLevelCount() => levelNodes.Count;

    public int GetNodesAtLevel(int level) => level < levelNodes.Count ? levelNodes[level].Count : 0;

    public int GetNextNodeId() {
        return nextNodeId++;
    }

    public void Clear() {
        levelNodes.Clear();
        nextNodeId = 0;
    }

    public GraphNode GetEntranceNode() {
        return levelNodes[0][0];
    }

    public GraphNode GetEndRoom() {
        return levelNodes[levelNodes.Count - 1][0];
    }

    public override string ToString() {
        string result = "";
        for (int level = 0; level < levelNodes.Count; level++) {
            result += $"Level {level}:\n";
            foreach (var node in levelNodes[level]) {
                result += $" {node} - Connections: ";
                var connections = node.GetAllConnections();
                result += string.Join(", ", connections.Select(n => n.index));
                result += "\n";
            }
        }
        return result;
    }
}

