using System;
using System.Collections.Generic;
using System.Linq;

public class StarMap {
    private readonly Dictionary<int, Dictionary<LayerCoord, Star>> _starsByLayer = new();
    private readonly Dictionary<LayerCoord, Star> _starsByCoord = new();

    public string Seed { get; private set; }
    public int StarsCount => _starsByCoord.Count;
    public int LayersCount => _starsByLayer.Count;
    public StarMap(string seed) {
        Seed = seed;
    }

    public bool AddStar(Star star) {
        if (star == null) return false;
        var coord = star.StarCoord;

        if (_starsByCoord.ContainsKey(coord))
            return false;

        _starsByCoord[coord] = star;

        if (!_starsByLayer.TryGetValue(coord.Layer, out var layerStars)) {
            layerStars = new Dictionary<LayerCoord, Star>();
            _starsByLayer[coord.Layer] = layerStars;
        }

        layerStars[coord] = star;
        return true;
    }

    public bool RemoveStarAt(LayerCoord coord) {
        if (!TryFindStarAt(coord, out var star))
            return false;

        return RemoveStar(star);
    }

    public bool RemoveStar(Star toRemove) {
        if (toRemove == null) return false;

        foreach (var coord in toRemove.Connections.ToList())
        {
            if (TryFindStarAt(coord, out var connectedStar)) {
                connectedStar.RemoveConnection(toRemove.StarCoord);
            }
        }

        toRemove.ClearConnections();

        return RemoveStarAtInternal(toRemove.StarCoord);
    }

    private bool RemoveStarAtInternal(LayerCoord coord) {
        if (!_starsByCoord.Remove(coord, out _))
            return false;

        if (_starsByLayer.TryGetValue(coord.Layer, out var layerStars)) {
            layerStars.Remove(coord);
            if (layerStars.Count == 0)
                _starsByLayer.Remove(coord.Layer);
        }

        return true;
    }

    
    public bool TryFindStarAt(LayerCoord coord, out Star star) {
        return _starsByCoord.TryGetValue(coord, out star);
    }

    public bool HasStarAt(LayerCoord coord) {
        return _starsByCoord.ContainsKey(coord);
    }

    public IReadOnlyList<Star> GetStarsInLayer(int layer) {
        if (_starsByLayer.TryGetValue(layer, out var layerStars)) {
            return layerStars.Values.ToList().AsReadOnly();
        }
        return new List<Star>().AsReadOnly();
    }

    public IReadOnlyList<int> GetExistingLayers() {
        return _starsByLayer.Keys.OrderBy(l => l).ToList().AsReadOnly();
    }

    public IReadOnlyList<Star> GetAllStars() {
        return _starsByCoord.Values.ToList().AsReadOnly();
    }

    

    public int GetCountInLayer(int layer) {
        return _starsByLayer.TryGetValue(layer, out var dict) ? dict.Count : 0;
    }

    internal List<Star> GetStarsAt(LayerCoord[] layerCoords) {
        throw new NotImplementedException();
    }
}
