using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


public class StarInfoPanel : UIPanel {
    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI _starNameText;
    [SerializeField] private TextMeshProUGUI _starInfoText;
    [SerializeField] private Button _travelButton;
    [SerializeField] private TextMeshProUGUI _travelButtonText;

    public event Action OnTravelRequested;

    protected override void Awake() {
        base.Awake();
        _travelButton.onClick.AddListener(HandleTravelClicked);
    }

    public void SetStarInfo(NavStar navStar) {
        _starNameText.text = $"Star {navStar.StarCoord}";
        _starInfoText.text = $"\nConnections: {navStar.Connections.Count}";
    }

    public void SetTravelAvailable(bool isEnabled) {
        _travelButton.interactable = isEnabled;
        _travelButtonText.text = isEnabled ? "Travel" : "Can't Travel";
    }

    private void HandleTravelClicked() {
        OnTravelRequested?.Invoke();
    }

    private void OnDestroy() {
        _travelButton.onClick.RemoveListener(HandleTravelClicked);
    }
}