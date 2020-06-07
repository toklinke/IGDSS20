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
    public int AvailableMoney { get { return Economy.AvailableMoney; }}

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

        if(!buildingCategoryParams.IsCompatibleTileType(tile.Type))
            return;

        if(!Economy.CanAfford(buildingCategoryParams.BuildCostMoney))
            return;

        Economy.SpendMoney(buildingCategoryParams.BuildCostMoney);

        var pos = this.MapToWorldMapper.GetWorldPosition(
            mapX: mapX,
            mapY: mapY,
            tile: tile
        );
        spawnBuilding(pos);

        var building = new Building(
            upkeepCost: buildingCategoryParams.UpkeepCost,
            resourceGenerationInterval: (
                // TODO: this assumes one tick == one second
                (int)buildingCategoryParams.ResourceGenerationInterval
            ),
            outputResource: buildingCategoryParams.OutputResource,
            outputCount: buildingCategoryParams.OutputCount
        );
        tile.Building = building;
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
        map.ForEachTile((x, y, tile) => {
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
        Map.ForEachTile((x, y, tile) => {
            var building = tile.Building;
            if (building != null)
            {
                upkeepCosts.Add(building.UpkeepCost);
            }
        });
        return upkeepCosts;
    }
}
