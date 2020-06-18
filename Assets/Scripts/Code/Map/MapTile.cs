// A single tile in a map.
public class MapTile
{
    public MapTile(
        float height, MapTileType type, ProductionBuilding building = null
    )
    {
        Height = height;
        Type = type;
        Building = building;
    }

    public float Height { get; }
    public MapTileType Type { get; }

    // The building that is placed on this tile.
    public ProductionBuilding Building { get; set; }

    public override string ToString()
    {
        string result = string.Format(
            "MapTile(height: {0}, type: {1}, building: {2})",
            Height, Type, Building
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
            Type == otherTile.Type &&
            (
                Building == otherTile.Building ||
                Building.Equals(otherTile.Building)
            )
        );
        return equals;
    }

    public override int GetHashCode()
    {
        var properties = (Height, Type, Building);
        var hashCode = properties.GetHashCode();
        return hashCode;
    }

    public MapTile Clone()
    {
        var clone = new MapTile(
            height: Height,
            type: Type,
            building: Building
        );
        return clone;
    }
}
