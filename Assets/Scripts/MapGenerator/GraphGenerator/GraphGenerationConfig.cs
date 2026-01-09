using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MapGenerationData", menuName = "Graph Generation/GraphConfig")]
public class GraphGenerationConfig : GenericInstanceConfig<GraphGenerator> {
    [SerializeField] private string _seedString = "simple_seed";
    [SerializeField] private bool _useRandomSeed = false;
    private int? _cachedSeed = null;

    [Header("Level Settings")]
    public int levelCount = 4;

    [Header("Nodes Level Settings")]
    public int initialNodesPerLevel = 4;
    public int minNodesPerLevel = 3;
    public int maxNodesPerLevel = 5;

    [Header("Node Count Deviation Control")]
    public int maxNodeDeviation = 1;
    public bool allowGradualIncrease = true;
    public bool allowGradualDecrease = true;

    public GraphPipelineConfig graphPipelineConfig;

    public string SeedString {
        get => _seedString;
        set {
            if (_seedString != value) {
                _seedString = value;
                _cachedSeed = null;
            }
        }
    }

    public bool UseRandomSeed {
        get => _useRandomSeed;
        set => _useRandomSeed = value;
    }

    public int Seed {
        get {
            if (_useRandomSeed) {
                return UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            }

            if (!_cachedSeed.HasValue) {
                _cachedSeed = _seedString.GetDeterministicSeed();
            }

            return _cachedSeed.Value;
        }
    }

    protected override void OnValidate() {
        base.OnValidate();

        if (initialNodesPerLevel > maxNodesPerLevel) {
            maxNodesPerLevel = initialNodesPerLevel;
        }

        if (maxNodesPerLevel < minNodesPerLevel) {
            maxNodesPerLevel = minNodesPerLevel + 1;
        }

        if (levelCount < 1) {
            levelCount = 1;
        }

        if (minNodesPerLevel < 1) {
            Debug.LogWarning("Minimum nodes per level cannot be less than 1. Resetting to 1.");
            minNodesPerLevel = 1;
        }

        if (string.IsNullOrEmpty(_seedString)) {
            Debug.LogWarning("Seed is empty. Using default value 'default_seed'.");
            _seedString = "default_seed";
        }

        maxNodeDeviation = Mathf.Max(0, maxNodeDeviation);

        _cachedSeed = null;
    }

    public void ResetSeed() {
        _cachedSeed = null;
    }

    public void GenerateNewRandomSeedString() {
        _seedString = $"random_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
        _cachedSeed = null;
    }
}