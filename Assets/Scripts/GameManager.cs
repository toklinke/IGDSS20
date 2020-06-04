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
    public float MapMinY;
    public float MapMaxY;
    public float MapTileRadius; // must match actual tile prefab size

    private MapToWorldMapper MapToWorldMapper;
    private Map Map;
    private int SelectedBuildingPrefabIndex;

    // Start is called before the first frame update
    void Start()
    {
        this.MapToWorldMapper = new MapToWorldMapper(
            minWorldY: MapMinY,
            maxWorldY: MapMaxY,
            tileRadius: MapTileRadius
        );
        this.Map = SpawnMap();
    }

    // Update is called once per frame
    void Update()
    {
        HandleKeyboardInput();
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

        var prefab = BuildingPrefabs[SelectedBuildingPrefabIndex];
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

    // Sets the index for the currently selected building prefab
    // by checking key presses on the numbers 1 to 0
    void HandleKeyboardInput()
    {
        var keycodes = new KeyCode[] {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6,
            KeyCode.Alpha7,
            KeyCode.Alpha8,
            KeyCode.Alpha9,
            KeyCode.Alpha0,
        };

        for (int i = 0; i < keycodes.Length; ++i)
        {
            var keycode = keycodes[i];
            if (Input.GetKeyDown(keycode))
            {
                if (i < BuildingPrefabs.Length)
                {
                    SelectedBuildingPrefabIndex = i;
                    Debug.Log(
                        string.Format(
                            "Selected building index {0}",
                            SelectedBuildingPrefabIndex
                        )
                    );
                }
                else
                {
                    Debug.Log(
                        string.Format("There is no building for index {0}", i)
                    );
                }
                break;
            }
        }
    }
}
