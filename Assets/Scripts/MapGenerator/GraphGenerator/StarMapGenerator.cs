using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class StarMapGenerator : MonoBehaviour {
    [SerializeField] private GraphGenerationConfig graphGenerationData;
    [Inject] private IDataRuntimeFactory dataRuntimeFactory;
    [Inject] private IStarMapService starMapService;

    private GraphGenerator _graphGenerator;

    private void Awake() {
        _graphGenerator = new GraphGenerator(graphGenerationData, dataRuntimeFactory);
    }

    public void GenerateMap() {

        StarMap starMap = CreateStarMap();
    }

    private StarMap CreateStarMap() {
        Graph graph = _graphGenerator.GenerateGraph();

        StarMap map = new StarMap();
        CreateStars(map, graph);


        return map;
    }

    private void CreateStars(StarMap map, Graph graph) {
        foreach (var layerNodes in graph.Layers) {
            foreach (var node in layerNodes) {
                Star star = new(node.layer, node.layerIndex);
            }
        }
    }
}

public class StarMap {
    private readonly List<Star> _stars = new List<Star>();

    public IReadOnlyList<Star> GetAllStars() {
        return _stars.AsReadOnly();
    }

    public bool AddStar(Star star) {
        if (HasStarAt(star.StarCoord)) 
        {
            return false;
        }

        _stars.Add(star);
        return true;
    }

    public bool RemoveStar(Star star) {
        return _stars.Remove(star);
    }

    public bool TryFindStarAt(LayerCoord starRef, out Star star) {
        star = _stars.FirstOrDefault(s => s.StarCoord.Equals(starRef));
        return star != null;
    }

    private bool HasStarAt(LayerCoord starRef) {
        return _stars.Any(s => s.StarCoord.Equals(starRef));
    }
}
