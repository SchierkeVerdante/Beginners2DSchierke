using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class StarNavigation : MonoBehaviour {
    [SerializeField] private Spaceship2D spaceship;
    [Inject] IStarMapService starMapService;
    private int travelDistance = 1;
    private Star _currentStar;

    public List<Star> GetAvailableStars() {
        if (_currentStar == null) {
            Debug.LogWarning("Current star is not set!");
            return new List<Star>();
        }

        LayerCoord starRef = _currentStar.StarCoord;

        int maxTravelDistance = _currentStar.StarCoord.Layer + travelDistance;

        IEnumerable<LayerCoord> nextConnections = _currentStar.Connections
            .Where(r => r.Layer > _currentStar.StarCoord.Layer && r.Layer <= maxTravelDistance);

        return starMapService.GetStarsByCoords(nextConnections);
    }

    public Star GetCurrentStar() {
        return _currentStar;
    }

    public void MoveTo(LayerCoord coords) {
        if (!starMapService.TryGetStarByCoord(coords, out Star star)) {

            return;
        }

        _currentStar = star;
    }
}
