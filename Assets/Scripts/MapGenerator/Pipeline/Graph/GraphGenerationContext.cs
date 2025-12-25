using System.Collections.Generic;
// Інші класи залишаються незмінними:

public class GraphGenerationContext {
    public Graph Graph { get; set; }
    public MapGenerationData Config { get; set; }
    public System.Random Random { get; set; }
    public Dictionary<string, object> SharedData { get; private set; }

    public GraphGenerationContext(MapGenerationData config, int? seed = null) {
        Config = config;
        Graph = new Graph();
        Random = seed.HasValue ? new System.Random(seed.Value) : new System.Random();
        SharedData = new Dictionary<string, object>();
    }

    public void SetData(string key, object value) => SharedData[key] = value;
    public T GetData<T>(string key) => SharedData.ContainsKey(key) ? (T)SharedData[key] : default;
    public bool HasData(string key) => SharedData.ContainsKey(key);
}
