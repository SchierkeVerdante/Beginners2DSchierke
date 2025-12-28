using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class NodeView : MonoBehaviour, IPointerDownHandler {
    [SerializeField] TextMeshPro text;

    [Header("Line settings")]
    [SerializeField] Material lineMaterial;
    [SerializeField] float lineWidth = 0.05f;

    public Action<NodeView> OnNodeClicked;

    public void SetText(string text) {
        if (this.text != null)
            this.text.text = text;
    }

    public void DrawConnectionTo(Vector3 targetPosition) {
        // ��������� ����� �ᒺ�� ��� ����
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

    public void OnPointerDown(PointerEventData eventData) {
        Debug.Log("Clicked!");
    }
}
