using System.Collections.Generic;
using UnityEngine;

// Controls the complete state of the game independent from Unity.
public class Game
{
    // A function that spawns a map tile at a certain position.
    public delegate void MapTileSpawner(
        MapTile tile, uint mapX, uint mapY, Vector3 worldPos
    );
    // A function that spawns a building at a certain position.
    public delegate void BuildingSpawner(Vector3 worldPos);

    public Map Map { get; }
    public Vector3 WorldSize { get; }
    public int AvailableMoney { get { return Economy.AvailableMoney; } }

    private IMapToWorldMapper MapToWorldMapper;

    private EconomySimulation Economy;
    private int EconomyTickInterval;
    // The number of ticks left until the economy ticks again
    private int TicksUntilEconomyTick;

    private Warehouse Warehouse;

    // Setup game.
    // initialMoney: The initially available money.
    // economyTickInterval:
    //  The number of game ticks that elapse between economy ticks.
    // incomePerEconomyTick:
    //  Amount of Money that is added for each economy tick.
    public Game(
        IMapGenerator mapGenerator,
        MapTileSpawner spawnMapTile,
        IMapToWorldMapper mapToWorldMapper,
        int initialMoney,
        int incomePerEconomyTick,
        int economyTickInterval
    )
    {
        this.Map = SpawnMap(
            mapGenerator: mapGenerator,
            spawnMapTile: spawnMapTile,
            mapToWorldMapper: mapToWorldMapper
        );

        this.MapToWorldMapper = mapToWorldMapper;
        WorldSize = mapToWorldMapper.GetWorldSize(
            mapWidth: this.Map.Width,
            mapHeight: this.Map.Height
        );

        Economy = new EconomySimulation(
            initialMoney: initialMoney,
            getIncome: () => incomePerEconomyTick,
            getUpkeepCosts: GetUpkeepCosts
        );
        EconomyTickInterval = economyTickInterval;
        TicksUntilEconomyTick = economyTickInterval;

        this.Warehouse = new Warehouse();

        //Debug.Log(Map.getNeighboursOfTile(0, 0)[1]);
    }

    // Advance game time by one tick.
    public void GameTimeTick()
    {
        --TicksUntilEconomyTick;
        if (TicksUntilEconomyTick == 0)
        {
            Economy.Tick();
            TicksUntilEconomyTick = EconomyTickInterval;
        }

        // Ja ich weiß furchtbares Design, aber Gründe ...
        int workersIncome = 0;
        this.Map.ForEachTile((x, y, tile) =>
        {
            if (tile.Building != null)
            {
                HousingBuilding testCast = tile.Building as HousingBuilding;
                if(testCast != null)
                {
                    workersIncome += testCast.getNumberOfWorkers();
                }

                tile.Building.gameTick();
            }
        });

        Economy.addIncome(workersIncome);
    }

    // Try to place a building at a certain map tile
    public void PlaceBuildingOnTile(
        MapTile tile,
        uint mapX,
        uint mapY,
        BuildingCategoryParams buildingCategoryParams,
        BuildingSpawner spawnBuilding
    )
    {
        if (tile.Building != null)
            return;

        if (!buildingCategoryParams.IsCompatibleTileType(tile.Type))
            return;

        if (!Economy.CanAfford(buildingCategoryParams.BuildCostMoney))
            return;

        var enoughPlanksAvailable = this.Warehouse.IsAvailable(
            type: ResourceType.Plank,
            amount: buildingCategoryParams.BuildCostPlanks
        );
        if (!enoughPlanksAvailable)
            return;

        Economy.SpendMoney(buildingCategoryParams.BuildCostMoney);
        this.Warehouse.Pick(
            type: ResourceType.Plank,
            amount: buildingCategoryParams.BuildCostPlanks
        );

        var pos = this.MapToWorldMapper.GetWorldPosition(
            mapX: mapX,
            mapY: mapY,
            tile: tile
        );
        spawnBuilding(pos);

        AbstractBuilding abstractBuilding;

        if (buildingCategoryParams.isProductionBuilding)
        {
            abstractBuilding = new ProductionBuilding(
                upkeepCost: buildingCategoryParams.UpkeepCost,
                resourceGenerationInterval: (
                    // TODO: this assumes one tick == one second
                    (int)buildingCategoryParams.ResourceGenerationInterval
                ),
                outputResource: buildingCategoryParams.OutputResource,
                outputCount: buildingCategoryParams.OutputCount,
                inputResources: buildingCategoryParams.InputResources,
                efficiency: GetBuildingEfficiency(
                    mapX: mapX,
                    mapY: mapY,
                    scaleTileType: (
                        buildingCategoryParams.EfficiencyScaleTileType
                    ),
                    scaleMinNeighbors: (
                        buildingCategoryParams.EfficiencyScaleMinNeighbors
                    ),
                    scaleMaxNeighbors: (
                        buildingCategoryParams.EfficiencyScaleMaxNeighbors
                    )
                ),
                areResourcesAvailable: this.Warehouse.IsAvailable,
                pickResources: this.Warehouse.Pick
            );
            abstractBuilding.ResourcesProduced += (sender, args) =>
            {
                var senderBuilding = (ProductionBuilding)sender;
                this.Warehouse.Store(
                    type: senderBuilding.OutputResource,
                    amount: senderBuilding.OutputCount
                );
            };

        }
        else
        {
            abstractBuilding = new HousingBuilding(0);
        }

        tile.Building = abstractBuilding;
    }

    // Get available amount of a certain resource.
    public int GetAvailableResources(ResourceType type)
    {
        int availableAmount = this.Warehouse.GetAvailableAmount(type);
        return availableAmount;
    }

    private Map SpawnMap(
        IMapGenerator mapGenerator,
        MapTileSpawner spawnMapTile,
        IMapToWorldMapper mapToWorldMapper
    )
    {
        var map = mapGenerator.GenerateMap();
        map.ForEachTile((x, y, tile) =>
        {
            var pos = mapToWorldMapper.GetWorldPosition(
                mapX: x,
                mapY: y,
                tile: tile
            );
            spawnMapTile(
                tile: tile,
                mapX: x,
                mapY: y,
                worldPos: pos
            );
        });

        return map;
    }

    // Get upkeep costs for all buildings.
    private List<int> GetUpkeepCosts()
    {
        var upkeepCosts = new List<int>();
        Map.ForEachTile((x, y, tile) =>
        {
            var building = tile.Building;
            if (building != null)
            {
                upkeepCosts.Add(building.UpkeepCost);
            }
        });
        return upkeepCosts;
    }

    // Get efficiency for a building based on sorrounding tiles.
    private float GetBuildingEfficiency(
        uint mapX,
        uint mapY,
        MapTileType? scaleTileType,
        int scaleMinNeighbors,
        int scaleMaxNeighbors
    )
    {
        float efficiency;
        if (scaleTileType == null)
        {
            // do not scale based on neighbors
            efficiency = 1.0f;
        }
        else
        {
            var neighbors = this.Map.getNeighboursOfTile(mapX, mapY);
            int suitableNeighborsCount = 0;
            foreach (var neighbor in neighbors)
            {
                var isEmpty = neighbor.Building == null;
                var isSuitableType = neighbor.Type == scaleTileType;
                if (isEmpty && isSuitableType)
                    ++suitableNeighborsCount;
            }

            if (suitableNeighborsCount < scaleMinNeighbors)
            {
                efficiency = 0.0f;
            }
            else if (suitableNeighborsCount > scaleMaxNeighbors)
            {
                efficiency = 1.0f;
            }
            else
            {
                efficiency = (
                    ((float)suitableNeighborsCount) /
                    ((float)scaleMaxNeighbors)
                );
            }
        }
        return efficiency;
    }
}
