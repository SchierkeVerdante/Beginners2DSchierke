using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "PlanetData", menuName = "PlanetGen/Biome Data")]
public class BiomeData : ScriptableObject {
    public string biomeLabel;
    public string displayName;
    public Tile[] tiles;
    public Color backgroundColor;
    public Color dimmingColor;
    public GameObject[] obstaclePrefabs;
    public GameObject[] enemyPrefabs;

    public int spawnRate = 1;
    public int obstacleDensity = 1;
    public int oilCount = 6;
    public int moduleCount = 3;

    public GameObject parallaxPrefab;
    public Sprite planetSprite;
}

