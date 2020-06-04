using System;

// A building that can be placed on a map.
public class Building
{
    public override bool Equals(object other)
    {
        var otherBuilding = other as Building;
        if (otherBuilding == null)
            return false;

        var equals = true; // TODO: check members
        return equals;
    }

    public override int GetHashCode()
    {
        var hashCode = 42; // TODO: compute using members
        return hashCode;
    }
}
