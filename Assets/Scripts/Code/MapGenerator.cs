using System;
using UnityEngine;

// Generates a map from a heightmap.
public class MapGenerator
{
    public Map GenerateMapFromHeightMap(
        Color[] heightMapColors, uint heightMapWidth, uint heightMapHeight
    )
    {
        MapTile[] tiles = new MapTile[heightMapWidth * heightMapHeight];

        for(uint y = 0; y < heightMapHeight; ++y)
        {
            for(uint x = 0; x < heightMapWidth; ++x)
            {
                uint i = y * heightMapWidth + x;
                Color color = heightMapColors[i];
                tiles[i] = GetMapTile(color);
            }
        }

        Map map = new Map(
            tiles: tiles,
            width: heightMapWidth,
            height: heightMapHeight
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
