using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // To be set from Unity Editor
    public Texture2D HeightMap;
    public GameObject PrefabWaterTile;
    public GameObject PrefabSandTile;
    public GameObject PrefabGrassTile;
    public GameObject PrefabForestTile;
    public GameObject PrefabStoneTile;
    public GameObject PrefabMountainTile;
    public GameObject[] BuildingPrefabs;

    private const float MapMinY = 0.0f;
    private const float MapMaxY = 25.0f;
    // must match actual tile prefab size
    private const float MapTileRadius = 5.0f;

    private MapToWorldMapper MapToWorldMapper = new MapToWorldMapper(
        minWorldY: MapMinY,
        maxWorldY: MapMaxY,
        tileRadius: MapTileRadius
    );

    private Map Map;

    // Start is called before the first frame update
    void Start()
    {
        this.Map = SpawnMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Map SpawnMap()
    {
        var mapGenerator = new MapGenerator();
        var map = mapGenerator.GenerateMapFromHeightMap(
            heightMapColors: HeightMap.GetPixels(),
            heightMapWidth: (uint)HeightMap.width,
            heightMapHeight: (uint)HeightMap.height
        );
        map.ForEachTile((x, y, tile) => {
            var pos = this.MapToWorldMapper.GetWorldPosition(
                mapX: x,
                mapY: y,
                tile: tile
            );
            var prefab = GetTilePrefab(tile.Type);
            var tileObject = Instantiate(prefab, pos, Quaternion.identity);

            var tileManager = tileObject.GetComponent<TileManager>();
            tileManager.Tile = tile;
            tileManager.MapX = x;
            tileManager.MapY = y;
            tileManager.TileClicked += (sender, args) => {
                var tileManagerSender = (TileManager)sender;
                Debug.Log(
                    $"Clicked on tile: {tileManagerSender.Tile.Type} " +
                    $"at map pos {tileManagerSender.MapX}, " +
                    $"{tileManagerSender.MapY}"
                );
                PlaceBuildingOnTile(
                    tile: tileManagerSender.Tile,
                    mapX: tileManagerSender.MapX,
                    mapY: tileManagerSender.MapY
                );
            };
        });

        var worldSize = this.MapToWorldMapper.GetWorldSize(
            mapWidth: map.Width,
            mapHeight: map.Height
        );

        // TODO: is there a better method for communication between scripts?
        var mouseManagerObj = GameObject.Find("/Rendering/Main Camera");
        var mouseManager = mouseManagerObj.GetComponent<MouseManager>();
        mouseManager.CameraMinX = 0.0f;
        mouseManager.CameraMaxX = worldSize.x;
        mouseManager.CameraMinZ = 0.0f;
        mouseManager.CameraMaxZ = worldSize.z;

        return map;
    }

    private GameObject GetTilePrefab(MapTileType type)
    {
        GameObject prefab;
        switch(type)
        {
            case MapTileType.Water:
                prefab = PrefabWaterTile;
            break;
            case MapTileType.Grass:
                prefab = PrefabGrassTile;
            break;
            case MapTileType.Forest:
                prefab = PrefabForestTile;
            break;
            case MapTileType.Sand:
                prefab = PrefabSandTile;
            break;
            case MapTileType.Stone:
                prefab = PrefabStoneTile;
            break;
            case MapTileType.Mountain:
                prefab = PrefabMountainTile;
            break;
            default:
                throw new ArgumentOutOfRangeException(
                    "type", "unknown tile type"
                );
        }
        return prefab;
    }

    // try to place a building at a certain map position.
    private void PlaceBuildingOnTile(MapTile tile, uint mapX, uint mapY)
    {
        if (tile.Building != null)
            return;

        // TODO: Use selected building type prefab
        var prefab = BuildingPrefabs[0];

        var buildingCategory = prefab.GetComponent<BuildingCategory>();

        if(!buildingCategory.IsCompatibleTileType(tile.Type))
            return;

        var pos = this.MapToWorldMapper.GetWorldPosition(
            mapX: mapX,
            mapY: mapY,
            tile: tile
        );
        Instantiate(prefab, pos, Quaternion.identity);

        var building = new Building(); // TODO: params
        tile.Building = building;
    }
}
