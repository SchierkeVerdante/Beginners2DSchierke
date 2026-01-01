// Клас вузла залишаємо майже без змін
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MapGenerationData", menuName = "Dungeon/MapGenerationData")]
public class GraphGenerationConfig : ScriptableObject {
    public string seed = "simple_seed";

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

    public void OnValidate() {
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

        if (string.IsNullOrEmpty(seed)) {
            Debug.LogWarning("Seed is empty. Using default value 'default_seed'.");
            seed = "default_seed";
        }

        maxNodeDeviation = Mathf.Max(0, maxNodeDeviation);
    }
}
