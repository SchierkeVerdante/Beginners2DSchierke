using System.Collections.Generic;
using UnityEngine;

public interface IObjectSpawner {
    List<GameObject> SpawnObjects(
        BiomeData biome,
        int areaWidth,
        int areaHeight,
        float density
    );
}

public class ObstacleSpawner : MonoBehaviour, IObjectSpawner {
    [SerializeField] private Transform _obstaclesContainer;

    public List<GameObject> SpawnObjects(
        BiomeData biome,
        int areaWidth,
        int areaHeight,
        float density) {
        List<GameObject> obstacles = new List<GameObject>();

        if (biome.obstaclePrefabs == null || biome.obstaclePrefabs.Length == 0)
            return obstacles;

        int totalCells = (areaWidth * 2 + 1) * (areaHeight * 2 + 1);
        int obstaclesToSpawn = Mathf.RoundToInt(totalCells * density);

        System.Random random = new System.Random();

        for (int i = 0; i < obstaclesToSpawn; i++) {
            int x = random.Next(-areaWidth, areaWidth + 1);
            int y = random.Next(-areaHeight, areaHeight + 1);

            GameObject prefab = biome.obstaclePrefabs[
                random.Next(0, biome.obstaclePrefabs.Length)
            ];

            Vector3 position = new Vector3(x, y, 0);
            GameObject obstacle = Instantiate(prefab, position, Quaternion.identity);

            if (_obstaclesContainer != null)
                obstacle.transform.SetParent(_obstaclesContainer);

            obstacles.Add(obstacle);
        }

        return obstacles;
    }
}