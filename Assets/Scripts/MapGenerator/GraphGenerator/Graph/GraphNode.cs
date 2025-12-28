using ModestTree;
using System.Collections.Generic;
using UnityEngine;

public class GraphNode {
    public HashSet<GraphNode> PrevConnections = new HashSet<GraphNode>();
    public HashSet<GraphNode> NextConnections = new HashSet<GraphNode>();
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
        if (!NextConnections.Contains(other)) {
            NextConnections.Add(other);
        }

        if (!other.PrevConnections.Contains(this)) {
            other.PrevConnections.Add(this);
        }
    }

    public void ConnectToPrev(GraphNode other) {
        if (!PrevConnections.Contains(other)) {
            PrevConnections.Add(other);
        }

        if (!other.NextConnections.Contains(this)) {
            other.NextConnections.Add(this);
        }
    }

    public void UnConnectFromNext(GraphNode connection) {
        if (NextConnections.Contains(connection)) {
            NextConnections.Remove(connection);
        }

        if (connection.PrevConnections.Contains(this)) {
            connection.PrevConnections.Remove(this);
        }
    }

    public void UnConnectFromPrev(GraphNode connection) {
        if (PrevConnections.Contains(connection)) {
            PrevConnections.Remove(connection);
        }

        if (connection.NextConnections.Contains(this)) {
            connection.NextConnections.Remove(this);
        }
    }

    public List<GraphNode> GetAllConnections() {
        List<GraphNode> allConnections = new List<GraphNode>();
        allConnections.AddRange(PrevConnections);
        allConnections.AddRange(NextConnections);
        return allConnections;
    }

    public void ClearConnections() {
        List<GraphNode> prevConnections = new List<GraphNode>(PrevConnections);
        foreach (GraphNode prevNode in prevConnections) {
            prevNode.NextConnections.Remove(this);
            PrevConnections.Remove(prevNode);
        }

        List<GraphNode> nextConnections = new List<GraphNode>(NextConnections);
        foreach (GraphNode nextNode in nextConnections) {
            nextNode.PrevConnections.Remove(this);
            NextConnections.Remove(nextNode);
        }
    }

    public bool HasConnectionsToPrevLevel() {
        return !PrevConnections.IsEmpty();
    }

    public bool HasConnectionsToNextLevel() {
        return !NextConnections.IsEmpty();
    }

    public bool IsConnectedTo(GraphNode targetNode) {
        return NextConnections.Contains(targetNode) || PrevConnections.Contains(targetNode);
    }

    public bool IsLinked() {
        return HasConnectionsToPrevLevel() && HasConnectionsToNextLevel();
    }

    public override string ToString() {
        return $"Node(L: {level}, I: {index})";
    }
}
