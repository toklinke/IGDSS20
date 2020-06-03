// A single tile in a map.
public class MapTile
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

    public override bool Equals(object other)
    {
        var otherTile = other as MapTile;
        if (otherTile == null)
            return false;

        var equals = (
            Height == otherTile.Height &&
            Type == otherTile.Type
        );
        return equals;
    }

    public override int GetHashCode()
    {
        var properties = (Height, Type);
        var hashCode = properties.GetHashCode();
        return hashCode;
    }
}
