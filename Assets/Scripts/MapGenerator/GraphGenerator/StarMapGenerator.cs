using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class StarMapGenerator : MonoBehaviour {
    [SerializeField] private GraphGenerationConfig graphGenerationData;
    [SerializeField] private StarView starPrefab;
    [SerializeField] private Transform mapParent;
    [SerializeField] private float nodesXOffset = 2f;
    [SerializeField] private float nodesYOffset = 1f;

    [Inject] private IDataRuntimeFactory dataRuntimeFactory;

    private GraphGenerator _graphGenerator;
    private StarConnectionDrawer starConnectionDrawer;

    Dictionary<GraphNode, StarPresenter> nodeMap;

    private void Awake() {
        _graphGenerator = new GraphGenerator(graphGenerationData, dataRuntimeFactory);
        starConnectionDrawer = new StarConnectionDrawer();
    }

    public void GenerateMap() {
        if (nodeMap != null) {
            ClearNodeMap();
        }

        Graph graph = _graphGenerator.GenerateGraph();

        var builder = new StarMapViewBuilder(
            starPrefab,
            mapParent ?? transform,
            nodesXOffset,
            nodesYOffset
        );

        nodeMap = builder.Build(graph);

        starConnectionDrawer.Draw(graph, nodeMap);
    }

    private void ClearNodeMap() {
        foreach (Transform child in mapParent) {
            Destroy(child.gameObject);
        }

        nodeMap.Clear();
    }
}

public class StarConnectionDrawer {
    public void Draw(Graph graph, Dictionary<GraphNode, StarPresenter> map) {
        foreach (var node in graph.AllNodes) {
            var from = map[node].View;

            foreach (var neighbor in node.NextConnections) {
                var toPos = map[neighbor].View.transform.position;
                from.DrawConnectionTo(toPos);
            }
        }
    }
}

public class StarMapViewBuilder {
    private readonly StarView _starPrefab;
    private readonly Transform _parent;
    private readonly float _xOffset;
    private readonly float _yOffset;

    public StarMapViewBuilder(
        StarView starPrefab,
        Transform parent,
        float xOffset,
        float yOffset
    ) {
        _starPrefab = starPrefab;
        _parent = parent;
        _xOffset = xOffset;
        _yOffset = yOffset;
    }

    public Dictionary<GraphNode, StarPresenter> Build(Graph graph) {
        var map = new Dictionary<GraphNode, StarPresenter>();

        for (int layer = 0; layer < graph.Layers.Count; layer++) {
            var layerParent = CreateLayer(layer);

            var nodes = graph.Layers[layer];
            for (int i = 0; i < nodes.Count; i++) {
                var node = nodes[i];
                var view = CreateNodeView(node, i, nodes.Count, layerParent);
                var presenter = new StarPresenter(view, new Star());

                map.Add(node, presenter);
            }
        }

        return map;
    }

    private Transform CreateLayer(int layer) {
        var go = new GameObject($"Level_{layer}");
        go.transform.SetParent(_parent);
        go.transform.localPosition = new Vector3(layer * _xOffset, 0f, 0f);
        return go.transform;
    }

    private StarView CreateNodeView(
        GraphNode node,
        int index,
        int count,
        Transform parent
    ) {
        float y = index * _yOffset - (count - 1) * _yOffset / 2f;
        var view = Object.Instantiate(_starPrefab, parent);
        view.transform.localPosition = new Vector3(0, y, 0);

        view.SetText($"L{node.level}-I{node.index}");
        return view;
    }
}
