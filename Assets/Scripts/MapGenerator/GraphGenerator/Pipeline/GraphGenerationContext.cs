using System.Collections.Generic;

public class GraphGenerationContext {
    public Graph Graph { get; set; }
    public MapGenerationData Config { get; set; }
    public Dictionary<string, object> SharedData { get; private set; }

    public GraphGenerationContext(MapGenerationData config) {
        Config = config;
        Graph = new Graph();
        SharedData = new Dictionary<string, object>();
    }

    public void SetData(string key, object value) => SharedData[key] = value;
    public T GetData<T>(string key) => SharedData.ContainsKey(key) ? (T)SharedData[key] : default;
    public bool HasData(string key) => SharedData.ContainsKey(key);
}
