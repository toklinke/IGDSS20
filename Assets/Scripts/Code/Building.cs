using System;

// A building that can be placed on a map.
public class Building
{
    public int UpkeepCost { get; }

    public Building(
        int upkeepCost
    )
    {
        UpkeepCost = upkeepCost;
    }

    public override bool Equals(object other)
    {
        var otherBuilding = other as Building;
        if (otherBuilding == null)
            return false;

        var equals = (
            UpkeepCost == otherBuilding.UpkeepCost
        );
        return equals;
    }

    public override int GetHashCode()
    {
        var properties = (UpkeepCost);
        var hashCode = properties.GetHashCode();
        return hashCode;
    }
}
