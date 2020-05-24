// A single tile in a map.
public struct MapTile
{
    public MapTile(float height, MapTileType type)
    {
        Height = height;
        Type = type;
    }

    public float Height { get; }
    public MapTileType Type { get; }

    public override string ToString()
    {
        string result = string.Format(
            "MapTile(height: {0}, type: {1})", Height, Type
        );
        return result;
    }
}
