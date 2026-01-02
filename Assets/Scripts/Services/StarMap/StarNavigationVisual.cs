using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class StarNavigationVisual : MonoBehaviour {
    [SerializeField] private StarView _starPrefab;
    [Inject] IStarMapService starMapService;
    [Inject] IPresenterFactory<StarPresenter> presenterFactory;

    [SerializeField] private Transform mapParent;
    [SerializeField] private float nodesXOffset = 2f;
    [SerializeField] private float nodesYOffset = 1f;

    public void ReGenerateMap() {
        ClearParent();
        starMapService.Clear();
        StarMap map = starMapService.StarMap;

        for (int layer = 0; layer < map.LayersCount; layer++) {
            IReadOnlyList<Star> stars = map.GetStarsInLayer(layer);
            SpawnLayer(layer, stars);
        }

        DrawStarConnections();
    }

    private void DrawStarConnections() {
        StarMap map = starMapService.StarMap;

        foreach (var currentStar in map.GetAllStars()) {
            LayerCoord[] layerCoords = currentStar.GetNextConnections();

            List<Star> nextStars = starMapService.GetStarsByCoords(layerCoords);

            foreach (var nextStar in nextStars) {
                Vector3 starGlobalPosition = starMapService.GetStarGlobalPosition(nextStar);

                if (starMapService.TryGetStarPresenter(currentStar, out StarPresenter starPresenter)) {
                    starPresenter.View.DrawConnectionTo(starGlobalPosition);
                }
            }

        }
    }

    private void SpawnLayer(int layer, IReadOnlyList<Star> stars) {
        foreach (var star in stars) {
            LayerCoord starCoord = star.StarCoord;

            Vector3 localPosition = CalculateStarPosition(starCoord.Layer, starCoord.Index, stars.Count);

            Vector3 spawnPosition = mapParent.transform.position + localPosition;

            StarView view = Instantiate(_starPrefab, spawnPosition, Quaternion.identity, mapParent);

            view.gameObject.name = star.ToString();

            StarPresenter starPresenter = presenterFactory.Create(view, star);

            starMapService.AddStarPresenter(starPresenter);
        }
    }

    private Vector3 CalculateStarPosition(int layer, int layerIndex, int layerStarsCount) {
        float xOffset = nodesXOffset * layer;
        float yOffset = (layerIndex - (layerStarsCount - 1) / 2f) * nodesYOffset;

        return new Vector3(xOffset, yOffset, 0);
    }

    private void ClearParent() {
        foreach (Transform child in mapParent) {
            Destroy(child.gameObject);
        }
    }
}