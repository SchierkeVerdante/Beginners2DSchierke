using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System.Collections.Generic;

public class MapController : MonoBehaviour
{
    [SerializeField] Button startButton;

    [SerializeField] StarMapGenerator generator;
    [SerializeField] GraphGenerationConfig graphGenerationData;
    [SerializeField] StarNavigationVisual starNavigationVisual;

    [Inject] IStarMapService starMapService;
    [Inject] IDataRuntimeFactory dataRuntimeFactory;
    [Inject] IPresenterFactory<NavStarPresenter> presenterFactory;
    

    private GraphGenerator _graphGenerator;
    private StarNavigator starNavigation;

    private void Awake() {
        _graphGenerator = dataRuntimeFactory.CreateInstance<GraphGenerator>(graphGenerationData);
        starNavigation = new StarNavigator(starNavigationVisual, presenterFactory, starMapService);
    }

    private void Start() {
        if (startButton != null)
            startButton.onClick.AddListener(OnGenerateClicked);

    }

    private void OnGenerateClicked() {
        StarMap starMap = CreateStars();
        starMapService.LoadMap(starMap);

        starNavigation.ReloadMap();
    }

    private StarMap CreateStars() {
        Graph graph = _graphGenerator.GenerateGraph();

        StarMap map = new StarMap(graph.Seed);

        foreach (var layerNodes in graph.Layers) {
            foreach (var node in layerNodes) {
                Star star = new(node.layer, node.layerIndex);
                map.AddStar(star);
                InitConnections(star, node);
            }
        }

        return map;
    }

    private void InitConnections(Star star, GraphNode node) {
        List<GraphNode> connectedNodes = node.GetAllConnections();
        foreach (var connectedNode in connectedNodes) {
            star.AddConnection(connectedNode.layer, connectedNode.layerIndex);
        } 
    }

    private void OnDestroy() {
        if (startButton != null)
            startButton.onClick.RemoveListener(OnGenerateClicked);
    }
}


