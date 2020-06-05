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

    public Game(
        HeightMap heightMap,
        MapTileSpawner spawnMapTile,
        IMapToWorldMapper mapToWorldMapper,
        int initialMoney
    )
    {
        this.Map = SpawnMap(
            heightMap: heightMap,
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
            getIncome: null, // TODO: pass correct func
            getUpkeepCosts: null // TODO: pass correct func
        );
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

        var building = new Building(); // TODO: params
        tile.Building = building;
    }

    private Map SpawnMap(
        HeightMap heightMap,
        MapTileSpawner spawnMapTile,
        IMapToWorldMapper mapToWorldMapper
    )
    {
        var mapGenerator = new MapGenerator();
        var map = mapGenerator.GenerateMapFromHeightMap(heightMap);
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
}
