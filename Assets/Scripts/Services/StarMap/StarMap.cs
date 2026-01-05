using System;
using System.Collections.Generic;
using System.Linq;

public class StarMap {
    private readonly Dictionary<LayerCoord, Star> _stars = new();
    private readonly Dictionary<int, List<Star>> _starsByLayer = new();

    public IReadOnlyDictionary<LayerCoord, Star> Stars => _stars;
    public string Seed { get; }

    public StarMap(string seed) => Seed = seed;

    public bool AddStar(Star star) {
        if (star == null || _stars.ContainsKey(star.Coord))
            return false;

        _stars[star.Coord] = star;

        if (!_starsByLayer.TryGetValue(star.Coord.Layer, out var layerStars)) {
            layerStars = new List<Star>();
            _starsByLayer[star.Coord.Layer] = layerStars;
        }
        layerStars.Add(star);

        return true;
    }

    public bool TryGetStar(LayerCoord coord, out Star star) =>
        _stars.TryGetValue(coord, out star);

    public IReadOnlyList<Star> GetStarsInLayer(int layer) =>
        _starsByLayer.TryGetValue(layer, out var stars)
            ? stars.AsReadOnly()
            : Array.Empty<Star>();

    public IEnumerable<int> GetLayers() => _starsByLayer.Keys.OrderBy(l => l);
}
