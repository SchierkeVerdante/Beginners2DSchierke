using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class StarNavigationVisual : MonoBehaviour {
    [SerializeField] private NavStarView _starPrefab;
    [Inject] IPresenterFactory<NavStarPresenter> presenterFactory;

    [SerializeField] private Spaceship2D spaceship;
    [SerializeField] private Transform mapParent;
    [SerializeField] private float nodesXOffset = 2f;
    [SerializeField] private float nodesYOffset = 1f;

    Dictionary<NavStar, NavStarPresenter> stars = new();
    public void ReGenerateMap(StarMap map) {
        ClearParent();

        for (int layer = 0; layer < map.LayersCount; layer++) {
            IReadOnlyList<Star> stars = map.GetStarsInLayer(layer);
            SpawnLayer(layer, stars);
        }

        DrawStarConnections(map);
    }

    private void SpawnLayer(int layer, IReadOnlyList<Star> stars) {
        foreach (var star in stars) {
            LayerCoord starCoord = star.StarCoord;

            Vector3 localPosition = CalculateStarPosition(starCoord.Layer, starCoord.Index, stars.Count);

            Vector3 spawnPosition = mapParent.transform.position + localPosition;

            NavStarView view = Instantiate(_starPrefab, spawnPosition, Quaternion.identity, mapParent);

            view.gameObject.name = star.ToString();

            NavStarPresenter starPresenter = presenterFactory.Create(view, star);

        }
    }

    private Vector3 CalculateStarPosition(int layer, int layerIndex, int layerStarsCount) {
        float xOffset = nodesXOffset * layer;
        float yOffset = (layerIndex - (layerStarsCount - 1) / 2f) * nodesYOffset;

        return new Vector3(xOffset, yOffset, 0);
    }

    private void DrawStarConnections(StarMap map) {

        foreach (var presenter in stars.Values) {
            LayerCoord[] layerCoords = presenter.Model.Star.GetNextConnections();

            List<Star> nextStars = map.GetStarsAt(layerCoords);

            foreach (var nextStar in nextStars) {
                
            }
        }
    }

    private Vector3 GetStarGlobalPosition() {
        return new Vector3();
    }

    private void ClearParent() {
        foreach (Transform child in mapParent) {
            Destroy(child.gameObject);
        }
    }
}