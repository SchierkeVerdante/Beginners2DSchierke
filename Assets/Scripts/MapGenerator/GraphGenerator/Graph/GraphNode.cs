using ModestTree;
using System.Collections.Generic;
using UnityEngine;

public class GraphNode {
    public HashSet<GraphNode> prevLevelConnections = new HashSet<GraphNode>();
    public HashSet<GraphNode> nextLevelConnections = new HashSet<GraphNode>();
    public GameObject roomInstance;
    public int level;
    public int index;

    public Vector2 Position => new Vector2(level, index);

    public GraphNode(int level, int index) {
        this.level = level;
        this.index = index;
    }

    public void ConnectTo(GraphNode other) {
        if (other.level > level) {
            ConnectToNext(other);
        } else if (other.level < level) {
            ConnectToPrev(other);
        } else {
            Debug.LogWarning($"Wrong connection Try to same level: {level} and {other.level}");
        }
    }

    public void Disconnect(GraphNode unconnectNode) {
        if (unconnectNode.level > level) {
            UnConnectFromNext(unconnectNode);
        } else if (unconnectNode.level < level) {
            UnConnectFromPrev(unconnectNode);
        } else {
            Debug.LogWarning($"Wrong connection Try to same level: {level} and {unconnectNode.level}");
        }
    }

    public void ConnectToNext(GraphNode other) {
        if (!nextLevelConnections.Contains(other)) {
            nextLevelConnections.Add(other);
        }

        if (!other.prevLevelConnections.Contains(this)) {
            other.prevLevelConnections.Add(this);
        }
    }

    // Метод для з'єднання з вузлом попереднього рівня
    public void ConnectToPrev(GraphNode other) {
        if (!prevLevelConnections.Contains(other)) {
            prevLevelConnections.Add(other);
        }

        if (!other.nextLevelConnections.Contains(this)) {
            other.nextLevelConnections.Add(this);
        }
    }

    public void UnConnectFromNext(GraphNode connection) {
        if (nextLevelConnections.Contains(connection)) {
            nextLevelConnections.Remove(connection);
        }

        if (connection.prevLevelConnections.Contains(this)) {
            connection.prevLevelConnections.Remove(this);
        }
    }

    // Метод для від'єднання від вузла попереднього рівня
    public void UnConnectFromPrev(GraphNode connection) {
        if (prevLevelConnections.Contains(connection)) {
            prevLevelConnections.Remove(connection);
        }

        if (connection.nextLevelConnections.Contains(this)) {
            connection.nextLevelConnections.Remove(this);
        }
    }

    // Допоміжний метод для отримання всіх зв'язків (для сумісності)
    public List<GraphNode> GetAllConnections() {
        List<GraphNode> allConnections = new List<GraphNode>();
        allConnections.AddRange(prevLevelConnections);
        allConnections.AddRange(nextLevelConnections);
        return allConnections;
    }

    public void ClearConnections() {
        List<GraphNode> prevConnections = new List<GraphNode>(prevLevelConnections);
        foreach (GraphNode prevNode in prevConnections) {
            prevNode.nextLevelConnections.Remove(this);
            prevLevelConnections.Remove(prevNode);
        }

        // Видаляємо зв'язки з наступним рівнем
        List<GraphNode> nextConnections = new List<GraphNode>(nextLevelConnections);
        foreach (GraphNode nextNode in nextConnections) {
            nextNode.prevLevelConnections.Remove(this);
            nextLevelConnections.Remove(nextNode);
        }
    }

    public bool HasConnectionsToPrevLevel() {
        return !prevLevelConnections.IsEmpty();
    }

    public bool HasConnectionsToNextLevel() {
        return !nextLevelConnections.IsEmpty();
    }

    public bool IsConnectedTo(GraphNode targetNode) {
        return nextLevelConnections.Contains(targetNode) || prevLevelConnections.Contains(targetNode);
    }

    public bool IsLinked() {
        return HasConnectionsToPrevLevel() && HasConnectionsToNextLevel();
    }

    public override string ToString() {
        return $"Node(L: {level}, I: {index})";
    }
}