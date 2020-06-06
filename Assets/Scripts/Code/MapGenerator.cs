using System;
using UnityEngine;

public readonly struct HeightMap
{
    public HeightMap(
        Color[] colors,
        uint width,
        uint height
    )
    {
        Colors = colors;
        Width = width;
        Height = height;
    }

    // Color values of the height map pixels in row-column order.
    public Color[] Colors { get; }
    public uint Width { get; }
    public uint Height { get; }
}

// Generates a map from a heightmap.
public interface IMapGenerator
{
    Map GenerateMapFromHeightMap(HeightMap heightMap);
}

// Generates a map from a heightmap.
public class MapGenerator : IMapGenerator
{
    public Map GenerateMapFromHeightMap(HeightMap heightMap)
    {
        MapTile[] tiles = new MapTile[heightMap.Width * heightMap.Height];

        for(uint y = 0; y < heightMap.Height; ++y)
        {
            for(uint x = 0; x < heightMap.Width; ++x)
            {
                uint i = y * heightMap.Width + x;
                Color color = heightMap.Colors[i];
                tiles[i] = GetMapTile(color);
            }
        }

        Map map = new Map(
            tiles: tiles,
            width: heightMap.Width,
            height: heightMap.Height
        );
        return map;
    }

    // Get map tile for color.
    // Color is assumed to be grayscale with R=G=B.
    private static MapTile GetMapTile(Color color)
    {
        // take any color channel as gray value
        float gray = color.r;

        var tile = new MapTile(
            height: gray,
            type: GetTileType(gray)
        );
        return tile;
    }

    // Get tile type for gray value.
    private static MapTileType GetTileType(float gray)
    {
        MapTileType tileType;
        if (gray == 0.0f)
        {
            tileType = MapTileType.Water;
        }
        else if (gray <= 0.2f)
        {
            tileType = MapTileType.Sand;
        }
        else if (gray <= 0.4f)
        {
            tileType = MapTileType.Grass;
        }
        else if (gray <= 0.6f)
        {
            tileType = MapTileType.Forest;
        }
        else if (gray <= 0.8f)
        {
            tileType = MapTileType.Stone;
        }
        else if (gray <= 1.0f)
        {
            tileType = MapTileType.Mountain;
        }
        else {
            throw new ArgumentOutOfRangeException(
                "color", "gray value must be in range 0.0 to 1.0"
            );
        }
        return tileType;
    }
}
