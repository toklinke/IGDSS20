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
        var heightMap = new HeightMap(
            colors: HeightMap.GetPixels(),
            width: (uint)HeightMap.width,
            height: (uint)HeightMap.height
        );
        this.Map = SpawnMap(heightMap);
        var worldSize = this.MapToWorldMapper.GetWorldSize(
            mapWidth: Map.Width,
            mapHeight: Map.Height
        );
        SetCameraLimits(worldSize);
    }

    // Update is called once per frame
    void Update()
    {
        HandleKeyboardInput();
    }

    private Map SpawnMap(HeightMap heightMap)
    {
        var mapGenerator = new MapGenerator();
        var map = mapGenerator.GenerateMapFromHeightMap(heightMap);
        map.ForEachTile((x, y, tile) => {
            var pos = this.MapToWorldMapper.GetWorldPosition(
                mapX: x,
                mapY: y,
                tile: tile
            );
            SpawnMapTile(
                tile: tile,
                mapX: x,
                mapY: y,
                worldPos: pos
            );
        });

        return map;
    }

    // Spawn a map tile.
    public void SpawnMapTile(
        MapTile tile, uint mapX, uint mapY, Vector3 worldPos
    )
    {
        var prefab = GetTilePrefab(tile.Type);
        var tileObject = Instantiate(prefab, worldPos, Quaternion.identity);

        var tileManager = tileObject.GetComponent<TileManager>();
        tileManager.Tile = tile;
        tileManager.MapX = mapX;
        tileManager.MapY = mapY;
        tileManager.TileClicked += (sender, args) => {
            var tileManagerSender = (TileManager)sender;
            Debug.Log(
                $"Clicked on tile: {tileManagerSender.Tile.Type} " +
                $"at map pos {tileManagerSender.MapX}, " +
                $"{tileManagerSender.MapY}"
            );

            var buildingPrefab = BuildingPrefabs[SelectedBuildingPrefabIndex];
            var buildingCategory = (
                buildingPrefab.GetComponent<BuildingCategory>()
            );
            var buildingCategoryParams = buildingCategory.GetParams();

            PlaceBuildingOnTile(
                tile: tileManagerSender.Tile,
                mapX: tileManagerSender.MapX,
                mapY: tileManagerSender.MapY,
                buildingCategoryParams: buildingCategoryParams,
                spawnBuilding: buildingWorldPos => {
                    SpawnBuilding(
                        buildingPrefabIndex: SelectedBuildingPrefabIndex,
                        worldPos: buildingWorldPos
                    );
                }
            );
        };
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

    // Set camera limits based on world size.
    private void SetCameraLimits(Vector3 worldSize)
    {
        // TODO: is there a better method for communication between scripts?
        var mouseManagerObj = GameObject.Find("/Rendering/Main Camera");
        var mouseManager = mouseManagerObj.GetComponent<MouseManager>();
        mouseManager.CameraMinX = 0.0f;
        mouseManager.CameraMaxX = worldSize.x;
        mouseManager.CameraMinZ = 0.0f;
        mouseManager.CameraMaxZ = worldSize.z;
    }

    // A function that spawns a building at a certain position.
    private delegate void BuildingSpawner(Vector3 worldPos);

    // try to place a building at a certain map position.
    private void PlaceBuildingOnTile(
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

        var pos = this.MapToWorldMapper.GetWorldPosition(
            mapX: mapX,
            mapY: mapY,
            tile: tile
        );
        spawnBuilding(pos);

        var building = new Building(); // TODO: params
        tile.Building = building;
    }

    // Spawn a building using a certain prefab.
    private void SpawnBuilding(int buildingPrefabIndex, Vector3 worldPos)
    {
        var prefab = BuildingPrefabs[buildingPrefabIndex];
        Instantiate(prefab, worldPos, Quaternion.identity);
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
