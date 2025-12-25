using System;
using TMPro;
using UnityEngine;

public class NodeView : MonoBehaviour {
    [SerializeField] TextMeshPro text;

    [Header("Line settings")]
    [SerializeField] Material lineMaterial;
    [SerializeField] float lineWidth = 0.05f;

    public void SetText(string text) {
        if (this.text != null)
            this.text.text = text;
    }

    public void DrawConnectionTo(Vector3 targetPosition) {
        // створюємо новий об’єкт для лінії
        var lineObj = new GameObject("ConnectionLine");
        lineObj.transform.SetParent(transform, false);

        var lr = lineObj.AddComponent<LineRenderer>();
        lr.material = lineMaterial;
        lr.widthMultiplier = lineWidth;
        lr.positionCount = 2;
        lr.useWorldSpace = true;

        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, targetPosition);
    }
}


public class NodePresenter {
    private NodeView nodeView;
    private GraphNode graphNode;
    public NodePresenter(NodeView nodeView, GraphNode graphNode) {
        this.nodeView = nodeView;
        this.graphNode = graphNode;
    }
}