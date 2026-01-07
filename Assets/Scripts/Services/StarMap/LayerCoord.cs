using System;

public readonly struct LayerCoord : IEquatable<LayerCoord> {
    public int Layer { get; }
    public int Index { get; }

    public LayerCoord(int layer, int index) {
        Layer = layer;
        Index = index;
    }

    public bool Equals(LayerCoord other) =>
        Layer == other.Layer && Index == other.Index;

    public override bool Equals(object obj) =>
        obj is LayerCoord other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Layer, Index);

    public override string ToString() => $"L{Layer}:I{Index}";
}

