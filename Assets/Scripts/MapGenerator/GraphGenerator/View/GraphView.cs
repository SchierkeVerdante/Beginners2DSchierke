using System.Collections.Generic;
using UnityEngine;

public class GraphView 
    : MonoBehaviour {
    [SerializeField] Transform graphParent;
    [SerializeField] NodeView noderViewPrefab;

    [SerializeField] private float nodesXOffset = 2f;
    [SerializeField] private float nodesYOffset = 1f;

    private Dictionary<GraphNode, NodeView> nodeViews = new();

    public void DisplayGraph(Graph graph) {
        ClearGraph();

        SpawnNodes(graph);
    }

    private void SpawnNodes(Graph graph) {
        List<List<GraphNode>> levelNodes = graph.Layers;
        

        for (int level = 0; level < levelNodes.Count; level++) {
            List<GraphNode> nodesAtLevel = levelNodes[level];
            float xPos = level * nodesXOffset;


            Transform levelParent = new GameObject().transform;
            levelParent.SetParent(graphParent);
            levelParent.gameObject.name = $"Level_{level}";
            levelParent.transform.localPosition = new Vector3(xPos, 0f, 0f);

            for (int i = 0; i < nodesAtLevel.Count; i++) {
                GraphNode graphNode = nodesAtLevel[i];

                NodeView nodeView = Instantiate(noderViewPrefab, levelParent);

                string nodeName = $"L{graphNode.level}-I{graphNode.index}";
                nodeView.SetText(nodeName);
                nodeView.gameObject.name = nodeName;

               
                float yPos = i * nodesYOffset - (nodesAtLevel.Count - 1) * nodesYOffset / 2f;
                nodeView.transform.localPosition = new Vector3(0, yPos, 0f);

                nodeViews.Add(graphNode, nodeView);
            }
        }

        RefreshConnections();
    }

    private void RefreshConnections() {
        foreach (var kvp in nodeViews) {
            GraphNode graphNode = kvp.Key;
            NodeView nodeView = kvp.Value;

            foreach (GraphNode connectedNode in graphNode.GetAllConnections()) {
                if (nodeViews.TryGetValue(connectedNode, out NodeView targetNodeView)) {
                    nodeView.DrawConnectionTo(targetNodeView.transform.position);
                }
            }
        }
    }

    private void ClearGraph() {
        foreach (Transform child in graphParent) {
            nodeViews.Clear();
            Destroy(child.gameObject);
        }
    }
}