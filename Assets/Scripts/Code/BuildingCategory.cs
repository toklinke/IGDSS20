using System;
using System.Collections.Generic;
using UnityEngine;


// Defines a category of buildings.
public class BuildingCategory : MonoBehaviour
{
    // Human-readable name of the building of the category.
    public string Name;

    // Upkeep costs for a building for each simulation cycle.
    public int UpkeepCost;
    // initial building costs
    public int BuildCostMoney;
    // number of planks needed for initial building
    public int BuildCostPlanks;

    // All tile types where a building of the category can be placed.
    public MapTileType[] CompatibleTileTypes;

    // Number of seconds for one production cycle
    // (when operating at 100% efficiency.
    public float ResourceGenerationInterval;
    // number of ouput resources per production cycle.
    public int OutputCount;

    // If non-null,
    // the efficiency of a building of the category scales
    // with the number of free tiles of the given type.
    public MapTileType? EfficiencyScaleTileType;
    // Minimum number of neighbors
    // that must be present for reaching efficiency > 0.
    public int EfficiencyScaleMinNeighbors;
    // Maximum number of neighbors
    // that are considered for scaling efficiency.
    public int EfficiencyScaleMaxNeighbors;

    // Type of resource types
    // that are consumed by a building of the category.
    public List<ResourceType> InputResources;
    // Type of resource that is produced by a building of the category.
    public ResourceType OutputResource;

    // Return whether a building of this category can be placed
    // on the given tile type.
    public bool IsCompatibleTileType(MapTileType tileType)
    {
        var index = Array.IndexOf(CompatibleTileTypes, tileType);
        var isCompatible = index > -1 ? true : false;
        return isCompatible;
    }
}
