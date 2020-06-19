using System;
using System.Collections.Generic;
using System.Linq;

// A building that can be placed on a map.
public class ProductionBuilding : Building
{
    public int ResourceGenerationInterval { get; }
    public ResourceType OutputResource { get; }
    public int OutputCount { get; }
    public List<ResourceType> InputResources { get; }
    public float Efficiency { get; }

    // some resource have been produced by this building.
    public event EventHandler<EventArgs> ResourcesProduced;

    private Func<ResourceType, int, bool> AreResourcesAvailable;
    private Action<ResourceType, int> PickResources;

    private float ProductionCycleProgress;
    private bool ProductionCycleActive;

    public ProductionBuilding(
        int upkeepCost,
        int resourceGenerationInterval, // in game time ticks
        ResourceType outputResource,
        int outputCount,
        List<ResourceType> inputResources,
        float efficiency,
        Func<ResourceType, int, bool> areResourcesAvailable,
        Action<ResourceType, int> pickResources
    ) : base(upkeepCost)
    {
        ResourceGenerationInterval = resourceGenerationInterval;
        OutputResource = outputResource;
        OutputCount = outputCount;
        InputResources = (inputResources ?? new List<ResourceType>());
        Efficiency = efficiency;
        AreResourcesAvailable = areResourcesAvailable;
        PickResources = pickResources;
        ProductionCycleActive = false;
    }

    // Advance game time by one tick.
    public override void gameTick()
    {
        float progress = Efficiency;
        if (ProductionCycleActive)
        {
            ProductionCycleProgress += progress;
            if (ProductionCycleProgress >= (float)ResourceGenerationInterval)
            {
                // production cycle finished
                OnResourcesProduced();
                ProductionCycleProgress -= (float)ResourceGenerationInterval;
                ProductionCycleActive = false;
            }
        }
        else
        {
            // start production cycle, if all input resources available
            bool allResourcesAvailable = true;
            foreach (var inputResource in InputResources)
            {
                allResourcesAvailable = (
                    allResourcesAvailable &&
                    AreResourcesAvailable(inputResource, 1)
                );
            }
            if (allResourcesAvailable)
            {
                foreach (var inputResource in InputResources)
                {
                    PickResources(inputResource, 1);
                }
                ProductionCycleActive = true;
                ProductionCycleProgress = progress;
            }
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
        var otherBuilding = other as ProductionBuilding;
        if (otherBuilding == null)
            return false;

        var equals = (
            UpkeepCost == otherBuilding.UpkeepCost &&
            (
                ResourceGenerationInterval ==
                otherBuilding.ResourceGenerationInterval
            ) &&
            OutputResource == otherBuilding.OutputResource &&
            OutputCount == otherBuilding.OutputCount &&
            InputResources.SequenceEqual(otherBuilding.InputResources) &&
            Efficiency == otherBuilding.Efficiency
        );
        return equals;
    }

    public override int GetHashCode()
    {
        var properties = (
            UpkeepCost,
            ResourceGenerationInterval,
            OutputResource,
            OutputCount,
            InputResources,
            Efficiency
        );
        var hashCode = properties.GetHashCode();
        return hashCode;
    }

    public override string ToString()
    {
        var inputResources = String.Join(", ", InputResources);
        var result = (
            $"ProductionBuilding(" +
            $"upkeepCost: {UpkeepCost}, " +
            $"resourceGenerationInterval: {ResourceGenerationInterval}, " +
            $"outputResource: {OutputResource}, " +
            $"outputCount: {OutputCount}, " +
            $"inputResources: {inputResources}" +
            $")"
        );
        return result;
    }
}
