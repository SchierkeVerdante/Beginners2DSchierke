// Клас вузла залишаємо майже без змін
using UnityEngine;

[CreateAssetMenu(fileName = "MapGenerationData", menuName = "StarMap/MapGenerationData")]
public class StarMapGenerationConfig : GenericInstanсeConfig<StarMapGenerator> {
    public GraphGenerationConfig graphConfig;
}