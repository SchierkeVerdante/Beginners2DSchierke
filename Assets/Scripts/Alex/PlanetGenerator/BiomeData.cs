using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "PlanetData", menuName = "PlanetGen/Biome Data")]
public class BiomeData : ScriptableObject {
    public string biomeLabel;
    public string displayName;
    public Tile[] tiles;
    public Color backgroundColor;
    public GameObject[] obstaclePrefabs;
    public GameObject[] enemyPrefabs;

    public float movementSpeedMultiplier = 1f;
    public GameObject parallaxPrefab;
}

