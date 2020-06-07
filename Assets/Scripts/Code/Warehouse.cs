using System;
using System.Collections.Generic;

// stores all available resources.
public class Warehouse
{
    private Dictionary<ResourceType, int> Resources;

    public Warehouse()
    {
        Resources = new Dictionary<ResourceType, int>();
        var resourceTypes = Enum.GetValues(
            typeof(ResourceType)
        );
        foreach(ResourceType type in resourceTypes)
        {
            Resources.Add(type, 0);
        }
    }

    // Get the number of available resources of a certain type.
    public int GetAvailableAmount(ResourceType type)
    {
        return Resources[type];
    }

    // return whether a certain amount of a resource is available.
    public bool IsAvailable(ResourceType type, int amount)
    {
        // TODO: catch negative amount
        var availableAmount = Resources[type];
        var isAvailable = amount <= availableAmount;
        return isAvailable;
    }

    // pick a certain amount of a resource from the warehouse.
    public void Pick(ResourceType type, int amount)
    {
        // TODO: catch negative amount
        // TODO: catch picking more than available
        Resources[type] -= amount;
    }

    // store a certain amount of a resource in the warehouse.
    public void Store(ResourceType type, int amount)
    {
        // TODO: catch negative amount
        Resources[type] += amount;
    }
}
