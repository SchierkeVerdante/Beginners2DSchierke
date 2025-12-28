using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class StarView : MonoBehaviour, IPointerDownHandler {
    public Action OnClicked { get; internal set; }
    [SerializeField] TextMeshPro text;

    [Header("Line settings")]
    [SerializeField] Material lineMaterial;
    [SerializeField] float lineWidth = 0.05f;

    public void OnPointerDown(PointerEventData eventData) {
        Debug.Log("Clicked!");

        OnClicked?.Invoke();
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

    public void SetText(string text) {
        if (this.text != null)
            this.text.text = text;
    }
}

public class StarPresenter {
    public readonly StarView View;
    public readonly Star Model;

    public StarPresenter(StarView view, Star model) {
        View = view;
        Model = model;

        // Підписки
        View.OnClicked += HandleClick;

        // Ініціалізація початкового стану
        SyncView();
    }

    internal void Destroy() {
        if (Model is IDisposable disposable) {
            disposable.Dispose();
        }
        GameObject.Destroy(View);
    }

    private void HandleClick() {
        if (!Model.IsEnabled) return;

        Debug.Log("Star clicked and logic processed in Presenter!");
    }

    private void SyncView() {
        View.SetText(Model.IsEnabled ? "Active" : "Locked");
    }
}

public class Star {
    public bool IsEnabled { get; internal set; }
}
