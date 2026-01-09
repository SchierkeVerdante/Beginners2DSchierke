using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainSpawnStage : IPipelineStage<PlanetGenContext> {
    [SerializeField]
    TerrainSpawnStageConfig config;

    public TerrainSpawnStage(TerrainSpawnStageConfig config) {
        this.config = config;
    }

    public string StageName => "TerrainSpawnStage";

    public void Execute(PlanetGenContext context) {
        Debug.Log($"Spawning Terrain for biome {context.choosenBiome.biomeLabel}");
        GenerateTerrain(context);
    }

    public void GenerateTerrain(PlanetGenContext context) {
        BiomeData biome = context.choosenBiome;
        Tile[] tiles = biome.tiles;

        Tilemap terrainTilemap = context._terrainTilemap;

        float planetNormalizedSize = context.planetConfig.normalizedVolume;

        int standardPlanetVolume = context.planetsRes.defaultPlanetVolume;

        int planetVolume = (int) (standardPlanetVolume * planetNormalizedSize);

        int maxBoundsY = planetVolume;
        int maxBoundsX = planetVolume;

        for (int x = -maxBoundsX; x <= maxBoundsX; x++) {
            for (int y = -maxBoundsY; y <= maxBoundsY; y++) {
                Tile tile = tiles[
                    context.Random.Next(0, tiles.Length)
                ];
                terrainTilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
    }
}
