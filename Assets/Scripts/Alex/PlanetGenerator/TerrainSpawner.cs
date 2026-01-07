using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainSpawner : MonoBehaviour {
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private GameObject _background;
    [SerializeField] private GameObject _parallaxBackground;

    private Dictionary<TileBase, Queue<TileBase>> _tilePool = new Dictionary<TileBase, Queue<TileBase>>();
    private List<Vector3Int> _spawnedTiles = new List<Vector3Int>();

    public void ClearTerrain() {
        foreach (var position in _spawnedTiles) {
            var tile = _tilemap.GetTile(position);
            if (tile != null) {
                ReturnTileToPool(tile, position);
            }
        }

        _tilemap.ClearAllTiles();
        _spawnedTiles.Clear();

        if (_parallaxBackground != null) {
            Destroy(_parallaxBackground);
        }
    }

    public void SpawnTerrain(BiomeData biome, int borderSizeX, int borderSizeY) {
        ClearTerrain();

        // ‘ÓÌ
        if (_background != null) {
            var renderer = _background.GetComponent<SpriteRenderer>();
            if (renderer != null) {
                renderer.color = biome.backgroundColor;
            }
        }

        if (biome.parallaxPrefab != null && _parallaxBackground == null) {
            _parallaxBackground = Instantiate(biome.parallaxPrefab);
        }

        GenerateTiles(biome, borderSizeX, borderSizeY);
    }

    private void GenerateTiles(BiomeData biome, int width, int height) {
        int startX = -width;
        int endX = width;
        int startY = -height;
        int endY = height;

        System.Random random = new System.Random();
        int tileCount = biome.tiles.Length;

        for (int x = startX; x <= endX; x++) {
            for (int y = startY; y <= endY; y++) {
                if (tileCount == 0) continue;

                TileBase tile = GetTileFromPool(biome.tiles[random.Next(0, tileCount)]);
                Vector3Int position = new Vector3Int(x, y, 0);

                _tilemap.SetTile(position, tile);
                _spawnedTiles.Add(position);
            }
        }
    }

    private TileBase GetTileFromPool(TileBase tileType) {
        if (_tilePool.ContainsKey(tileType) && _tilePool[tileType].Count > 0) {
            return _tilePool[tileType].Dequeue();
        }
        return tileType;
    }

    private void ReturnTileToPool(TileBase tile, Vector3Int position) {
        if (!_tilePool.ContainsKey(tile)) {
            _tilePool[tile] = new Queue<TileBase>();
        }
        _tilePool[tile].Enqueue(tile);
        _tilemap.SetTile(position, null);
    }
}


