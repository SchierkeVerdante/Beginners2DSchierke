using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Graph {
    private List<List<GraphNode>> levelNodes = new();
    private int nextNodeId = 0;

    public void AddNodeToLevel(int level, GraphNode node) {
        // Перевірка чи рівень існує, якщо ні - створити
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
        int roomId = 0;
        for (int level = 0; level < levelNodes.Count; level++) {
            List<GraphNode> levelNodes = this.levelNodes[level];
            for (int i = 0; i < levelNodes.Count; i++) {
                levelNodes[i].id = roomId++;
                levelNodes[i].level = level;
                levelNodes[i].position = new Vector2(level, i);
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

    public GraphNode GetNodeById(int id) {
        return GetAllNodes().FirstOrDefault(n => n.id == id);
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
}

