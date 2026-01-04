using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class StarNavigation {
    private readonly StarMap _starMap;
    private readonly Dictionary<LayerCoord, NavStar> _navStars = new();

    private NavStar _currentNavStar;
    private NavStar _selectedNavStar;

    public NavStar CurrentNavStar => _currentNavStar;
    public NavStar SelectedNavStar => _selectedNavStar;

    public event Action<NavStar> OnCurrentStarChanged;
    public event Action<NavStar> OnStarSelected;

    public StarNavigation(StarMap starMap) {
        _starMap = starMap;
        InitializeNavStars();
    }

    private void InitializeNavStars() {
        foreach (var star in _starMap.GetAllStars()) {
            var navStar = new NavStar(star);
            _navStars[star.StarCoord] = navStar;
        }
    }

    public NavStar GetNavStar(LayerCoord coord) {
        return _navStars.TryGetValue(coord, out var navStar) ? navStar : null;
    }

    public void SetStartPosition(LayerCoord coord) {
        if (!_navStars.TryGetValue(coord, out var navStar)) return;

        _currentNavStar = navStar;
        _currentNavStar.SetState(StarState.Current);

        UpdateAvailableStars();
        OnCurrentStarChanged?.Invoke(_currentNavStar);
    }

    public void SelectStar(NavStar navStar) {
        _selectedNavStar = navStar;
        OnStarSelected?.Invoke(navStar);
    }

    public bool CanTravelTo(NavStar targetNavStar) {
        if (_currentNavStar == null) return false;
        if (targetNavStar == null) return false;
        if (targetNavStar == _currentNavStar) return false;

        // Перевірка з'єднання
        return _currentNavStar.Star.AreConnectedTo(targetNavStar.StarCoord);
    }

    public bool TravelTo(NavStar targetNavStar) {
        if (!CanTravelTo(targetNavStar)) return false;

        // Попередня зірка стає completed
        _currentNavStar.SetState(StarState.Completed);

        // Переміщення
        _currentNavStar = targetNavStar;
        _currentNavStar.SetState(StarState.Current);

        UpdateAvailableStars();
        OnCurrentStarChanged?.Invoke(_currentNavStar);

        return true;
    }

    private void UpdateAvailableStars() {
        // Оновлюємо доступні зірки на основі поточної позиції
        foreach (var coord in _currentNavStar.Star.Connections) {
            if (_navStars.TryGetValue(coord, out var navStar)) {
                if (navStar.CurrentState == StarState.Locked) {
                    navStar.SetState(StarState.Available);
                }
            }
        }
    }

    public IReadOnlyList<NavStar> GetConnectedNavStars() {
        if (_currentNavStar == null) return Array.Empty<NavStar>();

        var connected = new List<NavStar>();
        foreach (var coord in _currentNavStar.Star.Connections) {
            if (_navStars.TryGetValue(coord, out var navStar)) {
                connected.Add(navStar);
            }
        }
        return connected;
    }
}

public class StarMapController : MonoBehaviour {
    [SerializeField] private Transform _starsContainer;
    [SerializeField] private NavStarView _starViewPrefab;
    [SerializeField] private StarInfoPanel _infoPanel;

    private StarMap _starMap;
    private StarNavigation _starNavigation;
    private List<NavStarPresenter> _presenters = new();

    private void Start() {
        // Ініціалізація StarMap (ваша генерація)
        _starMap = new StarMap("seed123");
        GenerateStarMap(); // Ваша логіка генерації

        // Створюємо навігацію
        _starNavigation = new StarNavigation(_starMap);
        _starNavigation.OnStarSelected += HandleStarSelected;
        _starNavigation.OnCurrentStarChanged += HandleCurrentStarChanged;

        // Створюємо View для всіх зірок
        CreateStarViews();

        // Встановлюємо стартову позицію
        var startCoord = new LayerCoord(0, 0);
        _starNavigation.SetStartPosition(startCoord);

        // UI Panel
        _infoPanel.OnTravelRequested += HandleTravelRequested;
    }

    private void CreateStarViews() {
        foreach (var star in _starMap.GetAllStars()) {
            var navStar = _starNavigation.GetNavStar(star.StarCoord);
            if (navStar == null) continue;

            // Створюємо View
            var viewObj = Instantiate(_starViewPrefab, _starsContainer);
            viewObj.transform.position = CalculateStarPosition(star.StarCoord);

            // Створюємо Presenter
            var presenter = new NavStarPresenter(navStar, viewObj, _starNavigation);
            _presenters.Add(presenter);

            // Малюємо з'єднання
            DrawConnections(viewObj, star);
        }
    }

    private void DrawConnections(NavStarView view, Star star) {
        foreach (var connCoord in star.Connections) {
            if (_starMap.TryFindStarAt(connCoord, out var connStar)) {
                var targetPos = CalculateStarPosition(connCoord);
                view.DrawConnectionTo(targetPos);
            }
        }
    }

    private Vector3 CalculateStarPosition(LayerCoord coord) {
        // Ваша логіка позиціонування зірок
        float x = coord.Layer * 3f;
        float y = coord.Index * 2f;
        return new Vector3(x, y, 0);
    }

    private void HandleStarSelected(NavStar navStar) {
        bool canTravel = _starNavigation.CanTravelTo(navStar);
        _infoPanel.Show(navStar, canTravel);
    }

    private void HandleTravelRequested() {
        if (_starNavigation.SelectedNavStar != null) {
            bool success = _starNavigation.TravelTo(_starNavigation.SelectedNavStar);
            if (success) {
                _infoPanel.Hide();
            }
        }
    }

    private void HandleCurrentStarChanged(NavStar newCurrentStar) {
        Debug.Log($"Player moved to: {newCurrentStar.StarCoord}");
        // Тут можна додати анімацію корабля
    }

    private void GenerateStarMap() {
        // Ваша логіка генерації зірок та з'єднань
        var star0 = new Star(0, 0);
        var star1 = new Star(0, 1);
        var star2 = new Star(1, 0);

        star0.AddConnection(star1.StarCoord);
        star0.AddConnection(star2.StarCoord);

        _starMap.AddStar(star0);
        _starMap.AddStar(star1);
        _starMap.AddStar(star2);
    }
}

public class StarInfoPanel : UIPanel {
    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI _starNameText;
    [SerializeField] private TextMeshProUGUI _starInfoText;
    [SerializeField] private Button _travelButton;
    [SerializeField] private TextMeshProUGUI _travelButtonText;

    public event Action OnTravelRequested;

    private NavStar _currentNavStar;

    protected override void Awake() {
        base.Awake();
        _travelButton.onClick.AddListener(HandleTravelClicked);
        Hide();
    }

    public void Show(NavStar navStar, bool canTravel) {
        _currentNavStar = navStar;
        _panel.SetActive(true);

        _starNameText.text = $"Star {navStar.StarCoord}";
        _starInfoText.text = $"State: {navStar.CurrentState}\nConnections: {navStar.Star.Connections.Count}";

        _travelButton.interactable = canTravel;
        _travelButtonText.text = canTravel ? "Travel" : "Can't Travel";
    }

    public void Hide() {
        _panel.SetActive(false);
        _currentNavStar = null;
    }

    private void HandleTravelClicked() {
        OnTravelRequested?.Invoke();
    }
}