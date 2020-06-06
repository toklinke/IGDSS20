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
    public int InitialMoney;

    private Game Game;
    private int SelectedBuildingPrefabIndex;

    // Start is called before the first frame update
    void Start()
    {
        var mapToWorldMapper = new MapToWorldMapper(
            minWorldY: MapMinY,
            maxWorldY: MapMaxY,
            tileRadius: MapTileRadius
        );
        var heightMap = new HeightMap(
            colors: HeightMap.GetPixels(),
            width: (uint)HeightMap.width,
            height: (uint)HeightMap.height
        );
        this.Game = new Game(
            heightMap: heightMap,
            spawnMapTile: SpawnMapTile,
            mapToWorldMapper: mapToWorldMapper,
            initialMoney: InitialMoney
        );
        SetCameraLimits(this.Game.WorldSize);
    }

    // Update is called once per frame
    void Update()
    {
        HandleKeyboardInput();
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
        tileManager.TileClicked += HandleTileClicked;
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

    // Spawn a building using a certain prefab.
    private void SpawnBuilding(int buildingPrefabIndex, Vector3 worldPos)
    {
        var prefab = BuildingPrefabs[buildingPrefabIndex];
        Instantiate(prefab, worldPos, Quaternion.identity);
    }

    // Control placement of buildings when a tile has been clicked.
    private void HandleTileClicked(object sender, EventArgs args)
    {
        var tileManagerSender = (TileManager)sender;
        Debug.Log(
            $"Clicked on tile: {tileManagerSender.Tile.Type} " +
            $"at map pos {tileManagerSender.MapX}, " +
            $"{tileManagerSender.MapY}"
        );

        returnListOfNeighbors(tileManagerSender.MapX, tileManagerSender.MapY, 16);
        uint calculatedIndex =(tileManagerSender.MapY * 16)  + tileManagerSender.MapX;

        Debug.Log($"Calculated Index: {calculatedIndex}");

        var buildingPrefab = BuildingPrefabs[SelectedBuildingPrefabIndex];
        var buildingCategory = (
            buildingPrefab.GetComponent<BuildingCategory>()
        );
        var buildingCategoryParams = buildingCategory.GetParams();

        this.Game.PlaceBuildingOnTile(
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
    }

    private uint CalcArrayIndex(uint xPos, uint yPos, uint xDimension)
    {
        return (yPos * xDimension) + xPos;
    }
    private void returnListOfNeighbors(uint xPos, uint yPos, uint xDimension)
    {
        uint calculatedIndex = CalcArrayIndex(xPos, yPos, xDimension);

        List<uint> neighborList = new List<uint>();

        bool LowestRow = false;
        bool HighestRow = false;
        bool leftmostColum = false;
        bool rightMostColumn = false;

       if (yPos == 0)
        {
            LowestRow = true;
        }
        else if(yPos == (xDimension-1) )
        {
            HighestRow = true;
        }

        if(xPos == 0)
        {
            leftmostColum = true;
        }
        if(xPos == (xDimension -1))
        {
            rightMostColumn = true;
        }


        Debug.Log($"LowestRow Index: {LowestRow} HighestRow {HighestRow} leftmostColum {leftmostColum} rightMostColumn {rightMostColumn} ");



        if ((yPos + 1) % 2 == 0) // Number is even +1 due to Indices starting at 0
        {
            Debug.Log("Even Row");
            if (!leftmostColum)
            {
                neighborList.Add(CalcArrayIndex(xPos - 1, yPos, xDimension));
            }
            if (!rightMostColumn)
            {
                neighborList.Add(CalcArrayIndex(xPos + 1, yPos, xDimension));
            }


            if (!LowestRow)
            {
                neighborList.Add(CalcArrayIndex(xPos, yPos - 1, xDimension));
            }

            if (!rightMostColumn && !LowestRow)
            {
                neighborList.Add(CalcArrayIndex(xPos + 1, yPos - 1, xDimension));
            }

            if (!rightMostColumn && !HighestRow)
            {
                neighborList.Add(CalcArrayIndex(xPos, yPos + 1, xDimension));
            }

            if (!HighestRow)
            {
                neighborList.Add(CalcArrayIndex(xPos + 1, yPos + 1, xDimension));
            }


          



















        }
        else // Number is odd
        {
            Debug.Log("Uneven Row");

            if (!leftmostColum)
            {
                neighborList.Add(CalcArrayIndex(xPos - 1, yPos, xDimension));
            }
            if (!rightMostColumn)
            {
                neighborList.Add(CalcArrayIndex(xPos + 1, yPos, xDimension));
            }

            if (!LowestRow && !leftmostColum)
            {
                neighborList.Add(CalcArrayIndex(xPos - 1, yPos - 1, xDimension));
            }

            if (!LowestRow)
            {
                neighborList.Add(CalcArrayIndex(xPos, yPos - 1 , xDimension));
            }

            if(!HighestRow && !leftmostColum)
            {
                neighborList.Add(CalcArrayIndex(xPos, yPos + 1, xDimension));
            }

            if(!HighestRow)
            {
                neighborList.Add(CalcArrayIndex(xPos + 1, yPos + 1, xDimension));
            }

        }

        Debug.Log($"Calculated Number of neighbors: {neighborList.Count}");



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
