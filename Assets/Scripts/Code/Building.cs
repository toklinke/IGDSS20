using System;

// A building that can be placed on a map.
public class Building
{
    public int UpkeepCost { get; }
    public int ResourceGenerationInterval { get; }
    public ResourceType OutputResource { get; }
    public int OutputCount { get; }

    // some resource have been produced by this building.
    public event EventHandler<EventArgs> ResourcesProduced;

    private float ProductionCycleProgress;
    private bool ProductionCycleActive;

    public Building(
        int upkeepCost,
        int resourceGenerationInterval, // in game time ticks
        ResourceType outputResource,
        int outputCount
    )
    {
        UpkeepCost = upkeepCost;
        ResourceGenerationInterval = resourceGenerationInterval;
        OutputResource = outputResource;
        OutputCount = outputCount;
        ProductionCycleActive = false;
    }

    // Advance game time by one tick.
    public void GameTimeTick()
    {
        float progress = 1.0f;
        if (ProductionCycleActive)
        {
            ProductionCycleProgress += progress;
            if (ProductionCycleProgress >= (float)ResourceGenerationInterval)
            {
                // production cycle finished
                OnResourcesProduced();
                ProductionCycleProgress -= (float)ResourceGenerationInterval;
            }
        }
        else
        {
            // start first production cycle
            ProductionCycleActive = true;
            ProductionCycleProgress = progress;
        }
    }

    private void OnResourcesProduced()
    {
        EventHandler<EventArgs> handler = ResourcesProduced;
        if (handler != null)
        {
            handler(this, EventArgs.Empty);
        }
    }

    public override bool Equals(object other)
    {
        var otherBuilding = other as Building;
        if (otherBuilding == null)
            return false;

        var equals = (
            UpkeepCost == otherBuilding.UpkeepCost &&
            (
                ResourceGenerationInterval ==
                otherBuilding.ResourceGenerationInterval
            ) &&
            OutputResource == otherBuilding.OutputResource &&
            OutputCount == otherBuilding.OutputCount
        );
        return equals;
    }

    public override int GetHashCode()
    {
        var properties = (
            UpkeepCost,
            ResourceGenerationInterval,
            OutputResource,
            OutputCount
        );
        var hashCode = properties.GetHashCode();
        return hashCode;
    }

    public override string ToString()
    {
        var result = (
            $"Building(" +
            $"upkeepCost: {UpkeepCost}, " +
            $"resourceGenerationInterval: {ResourceGenerationInterval}, " +
            $"outputResource: {OutputResource}, " +
            $"outputCount: {OutputCount}" +
            $")"
        );
        return result;
    }
}
