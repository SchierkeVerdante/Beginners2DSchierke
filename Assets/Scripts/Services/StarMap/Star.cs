using System;
using System.Collections.Generic;
using System.Linq;

public class Star {
    public Guid Id { get; }
    public LayerCoord Coord { get; }

    private readonly HashSet<LayerCoord> _connections = new();
    public string Name { get; set; }

    public PlanetConfig PlanetConfig;

    public IReadOnlyCollection<LayerCoord> Connections => _connections;

    public ReactiveProperty<StarState> State { get; }

    public Star(int layer, int index, string name = "Unknown") {
        Id = Guid.NewGuid();
        Coord = new LayerCoord(layer, index);
        State = new ReactiveProperty<StarState>(StarState.Locked);
        Name = name;
    }

    public void ConnectTo(LayerCoord coord) => _connections.Add(coord);
    public bool IsConnectedTo(LayerCoord coord) => _connections.Contains(coord);

    public IEnumerable<LayerCoord> GetForwardConnections() =>
        _connections.Where(c => c.Layer > Coord.Layer);

    public override string ToString() => $"{Name} {Coord}";

    public void SetPlanetData(PlanetConfig planetConfig) {
        this.PlanetConfig = planetConfig;
    }
}

