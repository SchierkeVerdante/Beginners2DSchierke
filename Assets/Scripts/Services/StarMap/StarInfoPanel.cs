using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


public class StarInfoPanel : UIPanel {
    [SerializeField] private TextMeshProUGUI _coordText;
    [SerializeField] private TextMeshProUGUI _infoText;
    [SerializeField] private Button _travelButton;
    [SerializeField] private TextMeshProUGUI _travelButtonText;

    public event Action OnTravelRequested;

    protected override void Awake() {
        base.Awake();
        _travelButton.onClick.AddListener(HandleTravelClicked);
    }

    public void SetStarInfo(Star star) {
        if (_infoText != null) {
            _infoText.text = $"{star.Name}";
        }

        if (_coordText != null) {
            _coordText.text = $"Star {star.Coord}";
        }
    }

    public void SetTravelAvailable(bool available) {
        if (_travelButton != null) {
            _travelButton.interactable = available;
        }

        if (_travelButtonText != null) {
            _travelButtonText.text = available ? "Travel" : "Can't Travel";
        }
    }

    private void HandleTravelClicked() {
        OnTravelRequested?.Invoke();
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        _travelButton.onClick.RemoveListener(HandleTravelClicked);
    }
}