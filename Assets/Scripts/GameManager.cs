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

    private const float MapMinY = 0.0f;
    private const float MapMaxY = 25.0f;
    // must match actual tile prefab size
    private const float MapTileRadius = 5.0f;

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
        var mapToWorldMapper = new MapToWorldMapper(
            minWorldY: MapMinY,
            maxWorldY: MapMaxY,
            tileRadius: MapTileRadius
        );
        map.ForEachTile((x, y, tile) => {
            var pos = mapToWorldMapper.GetWorldPosition(
                mapX: x,
                mapY: y,
                tile: tile
            );
            var prefab = GetTilePrefab(tile.Type);
            Instantiate(prefab, pos, Quaternion.identity);
        });
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
}
