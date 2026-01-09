// Клас вузла залишаємо майже без змін
using UnityEngine;

[CreateAssetMenu(fileName = "MapGenerationData", menuName = "StarMap/MapGenerationData")]
public class StarMapGenerationConfig : GenericInstanceConfig<StarMapGenerator> {
    public GraphGenerationConfig graphConfig;
    public PlanetsData planetsData;
}